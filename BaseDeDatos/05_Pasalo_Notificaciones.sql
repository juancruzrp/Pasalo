-- PÁSALO - Script 05: lo que necesita la campanita de notificaciones y los mensajes sin leer.
-- Ejecutar UNA sola vez, después del 04.
--   Mensaje.Leido           : 0 = la otra persona todavía no abrió la conversación con este mensaje
--   Solicitud.RespuestaVista: 0 = el solicitante todavía no vio que su solicitud fue aceptada/rechazada
USE Pasalo;
GO

ALTER TABLE Mensaje   ADD Leido          BIT NOT NULL CONSTRAINT DF_Mensaje_Leido DEFAULT 0;
ALTER TABLE Solicitud ADD RespuestaVista BIT NOT NULL CONSTRAINT DF_Solicitud_RespuestaVista DEFAULT 0;
GO

-- Lo que ya existía antes de este script no cuenta como novedad
UPDATE Mensaje   SET Leido = 1;
UPDATE Solicitud SET RespuestaVista = 1 WHERE Estado <> 0;
GO
