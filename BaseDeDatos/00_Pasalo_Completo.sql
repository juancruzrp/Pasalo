-- =====================================================================
-- PÁSALO - Script único (reemplaza a los scripts 01, 02, 03/04, 05, 06/07, 13 y 14)
-- Se puede ejecutar TODAS las veces que haga falta: crea lo que falta y no toca lo que ya existe.
-- Funciona tanto en una PC sin base como en una que ya tiene datos.
-- Ejecutar con F5 y nada seleccionado.
-- =====================================================================
IF DB_ID(N'Pasalo') IS NULL
    CREATE DATABASE Pasalo;
GO
USE Pasalo;
GO

-- ---------------------------------------------------------------------
-- 1) Tablas base
-- ---------------------------------------------------------------------
IF OBJECT_ID(N'Facultad', N'U') IS NULL
    CREATE TABLE Facultad (
        IdFacultad INT IDENTITY(1,1) PRIMARY KEY,
        Nombre     NVARCHAR(100) NOT NULL UNIQUE
    );

IF OBJECT_ID(N'Carrera', N'U') IS NULL
    CREATE TABLE Carrera (
        IdCarrera  INT IDENTITY(1,1) PRIMARY KEY,
        Nombre     NVARCHAR(100) NOT NULL,
        IdFacultad INT NOT NULL REFERENCES Facultad(IdFacultad)
    );

IF OBJECT_ID(N'Categoria', N'U') IS NULL
    CREATE TABLE Categoria (
        IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
        Nombre      NVARCHAR(50) NOT NULL UNIQUE
    );

IF OBJECT_ID(N'Usuario', N'U') IS NULL
    CREATE TABLE Usuario (
        IdUsuario     INT IDENTITY(1,1) PRIMARY KEY,
        Email         NVARCHAR(150) NOT NULL UNIQUE,
        Nombre        NVARCHAR(100) NOT NULL,
        PasswordHash  NVARCHAR(200) NOT NULL,
        IdCarrera     INT NULL REFERENCES Carrera(IdCarrera),
        Rol           TINYINT NOT NULL DEFAULT 0,      -- 0 = Usuario, 1 = Moderador
        Activo        BIT NOT NULL DEFAULT 1,          -- 0 = suspendido
        FechaRegistro DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

IF OBJECT_ID(N'Publicacion', N'U') IS NULL
    CREATE TABLE Publicacion (
        IdPublicacion    INT IDENTITY(1,1) PRIMARY KEY,
        IdUsuario        INT NOT NULL REFERENCES Usuario(IdUsuario),
        Titulo           NVARCHAR(100) NOT NULL,
        Descripcion      NVARCHAR(1000) NOT NULL,
        IdCategoria      INT NOT NULL REFERENCES Categoria(IdCategoria),
        IdCarrera        INT NULL REFERENCES Carrera(IdCarrera),
        Modalidad        TINYINT NOT NULL CHECK (Modalidad IN (0,1)),            -- 0 = Donación, 1 = Intercambio
        Estado TINYINT NOT NULL DEFAULT 0 CONSTRAINT CK_Publicacion_Estado CHECK (Estado IN (0,1,2,3)),
        FechaPublicacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        RowVer           ROWVERSION
    );

-- Actualizar estados permitidos de Publicacion
-- 0 = Disponible, 1 = Reservado, 2 = Entregado, 3 = Retirada

DECLARE @NombreRestriccion SYSNAME;
DECLARE @SQL NVARCHAR(MAX);

-- Eliminar restricciones antiguas de Estado,
-- conservando las que pertenecen a otras columnas.
DECLARE restricciones CURSOR LOCAL FAST_FORWARD FOR
SELECT cc.name
FROM sys.check_constraints cc
INNER JOIN sys.columns c
    ON c.object_id = cc.parent_object_id
   AND c.column_id = cc.parent_column_id
WHERE cc.parent_object_id = OBJECT_ID(N'dbo.Publicacion')
  AND c.name = N'Estado'
  AND cc.name <> N'CK_Publicacion_Estado';

OPEN restricciones;

FETCH NEXT FROM restricciones INTO @NombreRestriccion;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQL =
        N'ALTER TABLE dbo.Publicacion DROP CONSTRAINT '
        + QUOTENAME(@NombreRestriccion);

    EXEC sp_executesql @SQL;

    FETCH NEXT FROM restricciones INTO @NombreRestriccion;
END;

CLOSE restricciones;
DEALLOCATE restricciones;

-- Crear la restricción si todavía no existe.
IF NOT EXISTS (
    SELECT 1
    FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.Publicacion')
      AND name = N'CK_Publicacion_Estado'
)
BEGIN
    ALTER TABLE dbo.Publicacion
    ADD CONSTRAINT CK_Publicacion_Estado
    CHECK (Estado IN (0,1,2,3));
END;


IF OBJECT_ID(N'FotoPublicacion', N'U') IS NULL
    CREATE TABLE FotoPublicacion (
        IdFoto        INT IDENTITY(1,1) PRIMARY KEY,
        IdPublicacion INT NOT NULL REFERENCES Publicacion(IdPublicacion) ON DELETE CASCADE,
        Ruta          NVARCHAR(260) NOT NULL,
        Orden         TINYINT NOT NULL DEFAULT 0
    );

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Publicacion_Catalogo' AND object_id = OBJECT_ID(N'Publicacion'))
    CREATE INDEX IX_Publicacion_Catalogo ON Publicacion (Estado, IdCategoria, IdCarrera);
GO

-- ---------------------------------------------------------------------
-- 2) Columnas agregadas con el tiempo (cada una solo si falta)
-- ---------------------------------------------------------------------
IF COL_LENGTH(N'Categoria', N'PesoEstimadoKg') IS NULL
    ALTER TABLE Categoria ADD PesoEstimadoKg DECIMAL(6,2) NOT NULL CONSTRAINT DF_Categoria_Peso DEFAULT 0;

IF COL_LENGTH(N'Usuario', N'EmailVerificado') IS NULL
    ALTER TABLE Usuario ADD EmailVerificado BIT NOT NULL CONSTRAINT DF_Usuario_EmailVerificado DEFAULT 0;

IF COL_LENGTH(N'Usuario', N'Descripcion') IS NULL
    ALTER TABLE Usuario ADD Descripcion NVARCHAR(200) NULL;

-- Moderación: una publicación suspendida deja de verse en el catálogo y no recibe solicitudes
IF COL_LENGTH(N'Publicacion', N'Suspendida') IS NULL
    ALTER TABLE Publicacion ADD Suspendida BIT NOT NULL CONSTRAINT DF_Publicacion_Suspendida DEFAULT 0;
GO

-- ---------------------------------------------------------------------
-- 3) Solicitudes, chat, valoraciones y reportes
-- ---------------------------------------------------------------------
IF OBJECT_ID(N'Solicitud', N'U') IS NULL
    CREATE TABLE Solicitud (
        IdSolicitud          INT IDENTITY(1,1) PRIMARY KEY,
        IdPublicacion        INT NOT NULL REFERENCES Publicacion(IdPublicacion),
        IdSolicitante        INT NOT NULL REFERENCES Usuario(IdUsuario),
        ObjetoOfrecido       NVARCHAR(200) NULL,
        Estado               TINYINT NOT NULL DEFAULT 0 CHECK (Estado IN (0,1,2)),   -- 0 Pendiente, 1 Aceptada, 2 Rechazada
        FechaSolicitud       DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        ConfirmoDuenio       BIT NOT NULL DEFAULT 0,
        ConfirmoSolicitante  BIT NOT NULL DEFAULT 0,
        FechaEntrega         DATETIME2 NULL,
        CONSTRAINT UQ_Solicitud_Publicacion_Solicitante UNIQUE (IdPublicacion, IdSolicitante)
    );

-- Código para confirmar la entrega
IF COL_LENGTH(N'Solicitud', N'CodigoEntrega') IS NULL
    ALTER TABLE Solicitud ADD CodigoEntrega VARCHAR(6) NULL;
IF COL_LENGTH(N'Solicitud', N'FechaRespuesta') IS NULL
    ALTER TABLE Solicitud ADD FechaRespuesta DATETIME2 NULL;      -- cuándo se respondió
IF COL_LENGTH(N'Solicitud', N'MotivoRespuesta') IS NULL
    ALTER TABLE Solicitud ADD MotivoRespuesta TINYINT NULL;       -- 1 = rechazó el dueño, 2 = eligió a otra persona
IF COL_LENGTH(N'Solicitud', N'RespuestaVista') IS NULL
BEGIN
    ALTER TABLE Solicitud ADD RespuestaVista BIT NOT NULL CONSTRAINT DF_Solicitud_RespuestaVista DEFAULT 0;
    EXEC (N'UPDATE Solicitud SET RespuestaVista = 1 WHERE Estado <> 0;');   -- lo anterior no cuenta como novedad
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'UX_Solicitud_UnaAceptada' AND object_id = OBJECT_ID(N'Solicitud'))
    CREATE UNIQUE INDEX UX_Solicitud_UnaAceptada ON Solicitud (IdPublicacion) WHERE Estado = 1;
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Solicitud_Solicitante' AND object_id = OBJECT_ID(N'Solicitud'))
    CREATE INDEX IX_Solicitud_Solicitante ON Solicitud (IdSolicitante);
GO

IF OBJECT_ID(N'Mensaje', N'U') IS NULL
    CREATE TABLE Mensaje (
        IdMensaje   INT IDENTITY(1,1) PRIMARY KEY,
        IdSolicitud INT NOT NULL REFERENCES Solicitud(IdSolicitud),
        IdEmisor    INT NOT NULL REFERENCES Usuario(IdUsuario),
        Texto       NVARCHAR(1000) NOT NULL,
        FechaEnvio  DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
    );

IF COL_LENGTH(N'Mensaje', N'Leido') IS NULL
BEGIN
    ALTER TABLE Mensaje ADD Leido BIT NOT NULL CONSTRAINT DF_Mensaje_Leido DEFAULT 0;
    EXEC (N'UPDATE Mensaje SET Leido = 1;');
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Mensaje_Solicitud' AND object_id = OBJECT_ID(N'Mensaje'))
    CREATE INDEX IX_Mensaje_Solicitud ON Mensaje (IdSolicitud, FechaEnvio);
GO

IF OBJECT_ID(N'Valoracion', N'U') IS NULL
    CREATE TABLE Valoracion (
        IdValoracion INT IDENTITY(1,1) PRIMARY KEY,
        IdSolicitud  INT NOT NULL REFERENCES Solicitud(IdSolicitud),
        IdEvaluador  INT NOT NULL REFERENCES Usuario(IdUsuario),
        IdEvaluado   INT NOT NULL REFERENCES Usuario(IdUsuario),
        Puntaje      TINYINT NOT NULL CHECK (Puntaje BETWEEN 1 AND 5),
        Comentario   NVARCHAR(500) NULL,
        Fecha        DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        CONSTRAINT UQ_Valoracion_Solicitud_Evaluador UNIQUE (IdSolicitud, IdEvaluador),
        CONSTRAINT CK_Valoracion_DistintasPersonas CHECK (IdEvaluador <> IdEvaluado)
    );
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Valoracion_Evaluado' AND object_id = OBJECT_ID(N'Valoracion'))
    CREATE INDEX IX_Valoracion_Evaluado ON Valoracion (IdEvaluado);
GO

-- Reporte: se reporta una publicación O un usuario. Estado: 0 Pendiente, 1 Desestimado, 2 Resuelto
IF OBJECT_ID(N'Reporte', N'U') IS NULL
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
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = N'IX_Reporte_Estado' AND object_id = OBJECT_ID(N'Reporte'))
    CREATE INDEX IX_Reporte_Estado ON Reporte (Estado, Fecha);
GO

-- ---------------------------------------------------------------------
-- 4) Verificación de correo y recuperación de contraseña
--    Tipo 1 = verificar correo (24 h), Tipo 2 = recuperar contraseña (1 h). Se guarda el HASH.
-- ---------------------------------------------------------------------
IF OBJECT_ID(N'TokenUsuario', N'U') IS NULL
BEGIN
    CREATE TABLE TokenUsuario (
        IdToken          INT IDENTITY(1,1) PRIMARY KEY,
        IdUsuario        INT NOT NULL REFERENCES Usuario(IdUsuario) ON DELETE CASCADE,
        Tipo             TINYINT NOT NULL CHECK (Tipo IN (1, 2)),
        TokenHash        CHAR(64) NOT NULL,
        FechaCreacion    DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
        FechaVencimiento DATETIME2 NOT NULL,
        Usado            BIT NOT NULL DEFAULT 0
    );
    CREATE UNIQUE INDEX UX_TokenUsuario_Hash ON TokenUsuario (TokenHash);
    CREATE INDEX IX_TokenUsuario_Usuario ON TokenUsuario (IdUsuario, Tipo, Usado);
END
GO

-- ---------------------------------------------------------------------
-- 5) Datos iniciales (solo lo que falta)
-- ---------------------------------------------------------------------
INSERT INTO Facultad (Nombre)
SELECT v.Nombre FROM (VALUES (N'Ingeniería'), (N'Ciencias Económicas')) AS v(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Facultad f WHERE f.Nombre = v.Nombre);

-- La carrera de ejemplo del script original se renombra (conserva su Id)
UPDATE Carrera SET Nombre = N'Ingeniería en Sistemas de Información'
WHERE Nombre = N'Ingeniería en Sistemas'
  AND NOT EXISTS (SELECT 1 FROM Carrera WHERE Nombre = N'Ingeniería en Sistemas de Información');

-- Carreras de la UTN FRGP (las del formulario de registro)
INSERT INTO Carrera (Nombre, IdFacultad)
SELECT v.Nombre, (SELECT IdFacultad FROM Facultad WHERE Nombre = N'Ingeniería')
FROM (VALUES
        (N'Tecnicatura Universitaria en Programación'),
        (N'Ingeniería en Sistemas de Información'),
        (N'Ingeniería Mecánica'),
        (N'Ingeniería Eléctrica'),
        (N'Ingeniería Industrial'),
        (N'Otra carrera')
     ) AS v(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Carrera c WHERE c.Nombre = v.Nombre);

-- Carreras de ejemplo que no existen en la UTN FRGP: se borran solo si nadie las usa
DELETE c FROM Carrera c
WHERE c.Nombre IN (N'Contador Público', N'Ingeniería Química')
  AND NOT EXISTS (SELECT 1 FROM Usuario u WHERE u.IdCarrera = c.IdCarrera)
  AND NOT EXISTS (SELECT 1 FROM Publicacion p WHERE p.IdCarrera = c.IdCarrera);

-- Categorías con su peso estimado (kg) para el impacto ambiental
INSERT INTO Categoria (Nombre, PesoEstimadoKg)
SELECT v.Nombre, v.Peso
FROM (VALUES
        (N'Ropa', 0.50), (N'Libros', 0.80), (N'Apuntes', 0.30),
        (N'Electrónicos', 1.20), (N'Otros', 0.50)
     ) AS v(Nombre, Peso)
WHERE NOT EXISTS (SELECT 1 FROM Categoria c WHERE c.Nombre = v.Nombre);
GO

-- ---------------------------------------------------------------------
-- 6) Moderadores del equipo (Rol = 1). Los usuarios que todavía no se registraron
--    se promueven cuando vuelvas a correr este script después de que se registren.
--    Para sumar a alguien: agregar su correo a la lista.
-- ---------------------------------------------------------------------
UPDATE Usuario SET Rol = 1
WHERE Email IN (
    N'gonzalo.iglesias@alumnos.frgp.utn.edu.ar'
    -- , N'correo.de.ivana@alumnos.frgp.utn.edu.ar'
    -- , N'correo.de.juancruz@alumnos.frgp.utn.edu.ar'
);
GO

-- Resumen para comprobar que quedó todo
SELECT
    (SELECT COUNT(*) FROM Usuario)                      AS Usuarios,
    (SELECT COUNT(*) FROM Usuario WHERE Rol = 1)        AS Moderadores,
    (SELECT COUNT(*) FROM Categoria)                    AS Categorias,
    (SELECT COUNT(*) FROM Carrera)                      AS Carreras,
    (SELECT COUNT(*) FROM Facultad)                     AS Facultades,
    COL_LENGTH(N'Publicacion', N'Suspendida')           AS ColSuspendida;
GO
-- ---------------------------------------------------------------------
-- 7) Cancelación de intercambios (manual o por vencimiento de 10 días)
--     Solicitud.Estado = 3 (Cancelada)
-- ---------------------------------------------------------------------
IF COL_LENGTH(N'Solicitud', N'FechaCancelacion') IS NULL
    ALTER TABLE Solicitud ADD FechaCancelacion DATETIME2 NULL;

-- 1 = canceló una de las partes, 2 = venció el plazo
IF COL_LENGTH(N'Solicitud', N'MotivoCancelacion') IS NULL
    ALTER TABLE Solicitud ADD MotivoCancelacion TINYINT NULL;

-- Quién canceló (NULL si fue automática)
IF COL_LENGTH(N'Solicitud', N'IdCancelo') IS NULL
    ALTER TABLE Solicitud ADD IdCancelo INT NULL
        CONSTRAINT FK_Solicitud_Cancelo REFERENCES Usuario(IdUsuario);

-- 0 = la otra persona todavía no vio el aviso
IF COL_LENGTH(N'Solicitud', N'CancelacionVista') IS NULL
    ALTER TABLE Solicitud ADD CancelacionVista BIT NOT NULL
        CONSTRAINT DF_Solicitud_CancelacionVista DEFAULT 1;
GO

-- El estado 3 pasa a ser válido (se reemplaza el CHECK anterior)
DECLARE @RestriccionSolicitud SYSNAME;
DECLARE @SQLSolicitud NVARCHAR(MAX);

DECLARE restriccionesSolicitud CURSOR LOCAL FAST_FORWARD FOR
SELECT cc.name
FROM sys.check_constraints cc
INNER JOIN sys.columns c
    ON c.object_id = cc.parent_object_id
   AND c.column_id = cc.parent_column_id
WHERE cc.parent_object_id = OBJECT_ID(N'dbo.Solicitud')
  AND c.name = N'Estado'
  AND cc.name <> N'CK_Solicitud_Estado';

OPEN restriccionesSolicitud;
FETCH NEXT FROM restriccionesSolicitud INTO @RestriccionSolicitud;
WHILE @@FETCH_STATUS = 0
BEGIN
    SET @SQLSolicitud = N'ALTER TABLE dbo.Solicitud DROP CONSTRAINT ' + QUOTENAME(@RestriccionSolicitud);
    EXEC sp_executesql @SQLSolicitud;
    FETCH NEXT FROM restriccionesSolicitud INTO @RestriccionSolicitud;
END;
CLOSE restriccionesSolicitud;
DEALLOCATE restriccionesSolicitud;

IF NOT EXISTS (
    SELECT 1 FROM sys.check_constraints
    WHERE parent_object_id = OBJECT_ID(N'dbo.Solicitud')
      AND name = N'CK_Solicitud_Estado'
)
    ALTER TABLE dbo.Solicitud
    ADD CONSTRAINT CK_Solicitud_Estado CHECK (Estado IN (0,1,2,3));
GO

-- Se puede volver a pedir un objeto cuyo intercambio se canceló
IF EXISTS (SELECT 1 FROM sys.key_constraints
           WHERE name = N'UQ_Solicitud_Publicacion_Solicitante'
             AND parent_object_id = OBJECT_ID(N'dbo.Solicitud'))
    ALTER TABLE dbo.Solicitud DROP CONSTRAINT UQ_Solicitud_Publicacion_Solicitante;

IF NOT EXISTS (SELECT 1 FROM sys.indexes
               WHERE name = N'UX_Solicitud_PedidoUnico'
                 AND object_id = OBJECT_ID(N'dbo.Solicitud'))
    CREATE UNIQUE INDEX UX_Solicitud_PedidoUnico
        ON dbo.Solicitud (IdPublicacion, IdSolicitante) WHERE Estado <> 3;
GO