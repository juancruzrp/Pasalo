-- PÁSALO - Script 07: descripción corta en el perfil.
-- Ejecutar una sola vez, con la base Pasalo seleccionada. Si el número 07 ya lo usó alguien, renumerar.
USE Pasalo;
GO

ALTER TABLE Usuario ADD Descripcion NVARCHAR(200) NULL;
GO


USE Pasalo;
GO

IF COL_LENGTH('Usuario', 'Descripcion') IS NULL
    ALTER TABLE Usuario ADD Descripcion NVARCHAR(200) NULL;
GO

SELECT COUNT(*) AS Script07_Descripcion
FROM sys.columns WHERE object_id = OBJECT_ID('Usuario') AND name = 'Descripcion';