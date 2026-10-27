-- PÁSALO - Script 06: guardar cuándo y por qué se respondió una solicitud.
-- Ejecutar una sola vez, después del 02.
--   MotivoRespuesta: 1 = el dueño rechazó esta solicitud
--                    2 = el dueño eligió a otra persona (rechazo automático)
--   NULL = no aplica (pendiente o aceptada) o es una solicitud anterior a este script.
USE Pasalo;
GO
 
ALTER TABLE Solicitud ADD
    FechaRespuesta  DATETIME2 NULL,
    MotivoRespuesta TINYINT   NULL;
GO
 