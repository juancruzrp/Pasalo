-- =====================================================================
-- PASALO - Script 02: ampliacion para cubrir RF4, RF5, chat, valoraciones,
-- moderacion e impacto ambiental. NO modifica las tablas de Ivana salvo
-- agregar una columna a Categoria. Ejecutar despues del 01, UNA sola vez.
-- =====================================================================
USE Pasalo;
GO

-- ---------------------------------------------------------------------
-- Categoria: peso estimado por objeto (kg), para calcular el impacto ambiental.
-- Los valores de abajo son una ESTIMACION inicial: ajustarlos entre todos.
-- ---------------------------------------------------------------------
ALTER TABLE Categoria ADD PesoEstimadoKg DECIMAL(6,2) NOT NULL CONSTRAINT DF_Categoria_Peso DEFAULT 0;
GO

INSERT INTO Categoria (Nombre, PesoEstimadoKg) VALUES
    (N'Ropa',         0.50),
    (N'Libros',       0.80),
    (N'Apuntes',      0.30),
    (N'Electrónicos', 1.20),
    (N'Otros',        0.50);
GO

-- ---------------------------------------------------------------------
-- Solicitud (RF4 y RF5): un usuario pide una publicacion.
--   Estado: 0 = Pendiente, 1 = Aceptada, 2 = Rechazada
--   ObjetoOfrecido: solo si la publicacion es de Intercambio (lo valida Negocio)
--   RF5: cada parte confirma por separado; cuando las dos confirmaron,
--   Negocio pone FechaEntrega y la publicacion pasa a Entregado.
-- ---------------------------------------------------------------------
CREATE TABLE Solicitud (
    IdSolicitud          INT IDENTITY(1,1) PRIMARY KEY,
    IdPublicacion        INT NOT NULL REFERENCES Publicacion(IdPublicacion),
    IdSolicitante        INT NOT NULL REFERENCES Usuario(IdUsuario),
    ObjetoOfrecido       NVARCHAR(200) NULL,
    Estado               TINYINT NOT NULL DEFAULT 0 CHECK (Estado IN (0,1,2)),
    FechaSolicitud       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    ConfirmoDuenio       BIT NOT NULL DEFAULT 0,
    ConfirmoSolicitante  BIT NOT NULL DEFAULT 0,
    FechaEntrega         DATETIME2 NULL,
    CONSTRAINT UQ_Solicitud_Publicacion_Solicitante UNIQUE (IdPublicacion, IdSolicitante)   -- no pedir dos veces lo mismo
);
GO

-- Garantiza a nivel base que una publicacion tenga como maximo UNA solicitud aceptada
CREATE UNIQUE INDEX UX_Solicitud_UnaAceptada ON Solicitud (IdPublicacion) WHERE Estado = 1;
CREATE INDEX IX_Solicitud_Solicitante ON Solicitud (IdSolicitante);
GO

-- ---------------------------------------------------------------------
-- Mensaje: chat vinculado a una solicitud aceptada
-- ---------------------------------------------------------------------
CREATE TABLE Mensaje (
    IdMensaje   INT IDENTITY(1,1) PRIMARY KEY,
    IdSolicitud INT NOT NULL REFERENCES Solicitud(IdSolicitud),
    IdEmisor    INT NOT NULL REFERENCES Usuario(IdUsuario),
    Texto       NVARCHAR(1000) NOT NULL,
    FechaEnvio  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);
CREATE INDEX IX_Mensaje_Solicitud ON Mensaje (IdSolicitud, FechaEnvio);
GO

-- ---------------------------------------------------------------------
-- Valoracion: calificacion entre usuarios despues de una entrega
-- ---------------------------------------------------------------------
CREATE TABLE Valoracion (
    IdValoracion INT IDENTITY(1,1) PRIMARY KEY,
    IdSolicitud  INT NOT NULL REFERENCES Solicitud(IdSolicitud),
    IdEvaluador  INT NOT NULL REFERENCES Usuario(IdUsuario),
    IdEvaluado   INT NOT NULL REFERENCES Usuario(IdUsuario),
    Puntaje      TINYINT NOT NULL CHECK (Puntaje BETWEEN 1 AND 5),
    Comentario   NVARCHAR(500) NULL,
    Fecha        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    CONSTRAINT UQ_Valoracion_Solicitud_Evaluador UNIQUE (IdSolicitud, IdEvaluador),   -- una valoracion por persona y operacion
    CONSTRAINT CK_Valoracion_DistintasPersonas CHECK (IdEvaluador <> IdEvaluado)
);
CREATE INDEX IX_Valoracion_Evaluado ON Valoracion (IdEvaluado);
GO

-- ---------------------------------------------------------------------
-- Reporte (moderacion): se reporta una publicacion O un usuario
--   Estado: 0 = Pendiente, 1 = Desestimado, 2 = Resuelto (se tomo accion)
-- ---------------------------------------------------------------------
CREATE TABLE Reporte (
    IdReporte          INT IDENTITY(1,1) PRIMARY KEY,
    IdReportante       INT NOT NULL REFERENCES Usuario(IdUsuario),
    IdPublicacion      INT NULL REFERENCES Publicacion(IdPublicacion),
    IdUsuarioReportado INT NULL REFERENCES Usuario(IdUsuario),
    Motivo             NVARCHAR(100) NOT NULL,
    Detalle            NVARCHAR(500) NULL,
    Estado             TINYINT NOT NULL DEFAULT 0 CHECK (Estado IN (0,1,2)),
    Fecha              DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    IdModerador        INT NULL REFERENCES Usuario(IdUsuario),
    FechaResolucion    DATETIME2 NULL,
    CONSTRAINT CK_Reporte_UnSoloObjetivo CHECK (
        (IdPublicacion IS NOT NULL AND IdUsuarioReportado IS NULL) OR
        (IdPublicacion IS NULL AND IdUsuarioReportado IS NOT NULL))
);
CREATE INDEX IX_Reporte_Estado ON Reporte (Estado, Fecha);
GO
