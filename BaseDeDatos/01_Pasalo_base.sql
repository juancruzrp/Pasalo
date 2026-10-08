-- =====================================================================
-- PASALO - Script 01: base de datos original (Ivana)
-- Ejecutar UNA sola vez, de arriba hacia abajo (F5 con nada seleccionado).
-- Unico cambio respecto al original: CREATE DATABASE solo si no existe.
-- =====================================================================
IF DB_ID(N'Pasalo') IS NULL
    CREATE DATABASE Pasalo;
GO
USE Pasalo;
GO

-- Catalogos 
CREATE TABLE Facultad (
    IdFacultad INT IDENTITY(1,1) PRIMARY KEY,
    Nombre     NVARCHAR(100) NOT NULL UNIQUE
);

CREATE TABLE Carrera (
    IdCarrera  INT IDENTITY(1,1) PRIMARY KEY,
    Nombre     NVARCHAR(100) NOT NULL,
    IdFacultad INT NOT NULL REFERENCES Facultad(IdFacultad)
);

CREATE TABLE Categoria (
    IdCategoria INT IDENTITY(1,1) PRIMARY KEY,
    Nombre      NVARCHAR(50) NOT NULL UNIQUE
);

-- Usuario 
CREATE TABLE Usuario (
    IdUsuario     INT IDENTITY(1,1) PRIMARY KEY,
    Email         NVARCHAR(150) NOT NULL UNIQUE,   -- UNIQUE evita duplicados 
    Nombre        NVARCHAR(100) NOT NULL,
    PasswordHash  NVARCHAR(200) NOT NULL,          
    IdCarrera     INT NULL REFERENCES Carrera(IdCarrera),
    Rol           TINYINT NOT NULL DEFAULT 0,      -- 0 = Usuario, 1 = Moderador
    Activo        BIT NOT NULL DEFAULT 1,
    FechaRegistro DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME()
);

-- Publicacion
CREATE TABLE Publicacion (
    IdPublicacion    INT IDENTITY(1,1) PRIMARY KEY,
    IdUsuario        INT NOT NULL REFERENCES Usuario(IdUsuario),   -- dueño
    Titulo           NVARCHAR(100) NOT NULL,
    Descripcion      NVARCHAR(1000) NOT NULL,
    IdCategoria      INT NOT NULL REFERENCES Categoria(IdCategoria),
    IdCarrera        INT NULL REFERENCES Carrera(IdCarrera),
    Modalidad        TINYINT NOT NULL CHECK (Modalidad IN (0,1)),  -- 0 = Donación, 1 = Intercambio
    Estado           TINYINT NOT NULL DEFAULT 0 CHECK (Estado IN (0,1,2)), -- 0 = Disponible, 1 = Reservado, 2 = Entregado
    FechaPublicacion DATETIME2 NOT NULL DEFAULT SYSUTCDATETIME(),
    RowVer           ROWVERSION                                    -- para detectar concurrencia evita que se sobre escriban cambios de otros usuarios
);

CREATE TABLE FotoPublicacion (
    IdFoto        INT IDENTITY(1,1) PRIMARY KEY,
    IdPublicacion INT NOT NULL REFERENCES Publicacion(IdPublicacion) ON DELETE CASCADE,
    Ruta          NVARCHAR(260) NOT NULL,    -- solo la ruta; el archivo va en una carpeta del servidor
    Orden         TINYINT NOT NULL DEFAULT 0
);

-- indice para el filtro 
CREATE INDEX IX_Publicacion_Catalogo ON Publicacion (Estado, IdCategoria, IdCarrera);
GO

--Datos iniciales 
INSERT INTO Facultad (Nombre) VALUES (N'Ingeniería'), (N'Ciencias Económicas');   -- ajustar
INSERT INTO Carrera (Nombre, IdFacultad) VALUES
    (N'Ingeniería en Sistemas', 1),
    (N'Ingeniería Industrial', 1),
    (N'Contador Público', 2);
GO
