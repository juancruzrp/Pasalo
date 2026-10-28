-- PÁSALO - Script 06: verificación de correo y recuperación de contraseña.
-- Ejecutar una sola vez, después de los scripts anteriores. Si el número 06 ya lo usó alguien, renumerar.
USE Pasalo;
GO

-- Correo verificado: los usuarios nuevos arrancan en 0 y se verifican con el enlace del correo.
ALTER TABLE Usuario
    ADD EmailVerificado BIT NOT NULL CONSTRAINT DF_Usuario_EmailVerificado DEFAULT 0;
GO

-- Los usuarios que ya existían se registraron antes de esta función: se consideran verificados
-- (si no, quedarían sin poder ingresar).
UPDATE Usuario SET EmailVerificado = 1;
GO

-- Una sola tabla de enlaces para las dos funciones.
--   Tipo 1 = verificar correo (vence en 24 h)    Tipo 2 = recuperar contraseña (vence en 1 h)
-- Se guarda el HASH del código, no el código: quien lea la base no puede armar enlaces válidos.
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
GO


SELECT IdUsuario, Email, EmailVerificado FROM Usuario;