-- =====================================================================
-- PÁSALO - Consultas del Chat y de las Notificaciones (+ escenario de prueba)
-- NO es parte de la instalación. Requiere haber corrido antes los scripts 04 y 05.
-- Las consultas de la sección 1 son las que usa la aplicación
-- (acceso/MensajeAcceso.cs y acceso/NotificacionAcceso.cs).
-- =====================================================================
USE Pasalo;
GO

-- ---------------------------------------------------------------------
-- 1) CONSULTAS QUE USA LA APLICACIÓN (cambiar @idUsuario)
-- ---------------------------------------------------------------------
DECLARE @idUsuario INT = 2;

-- 1.a) Conversaciones del usuario = solicitudes ACEPTADAS donde es dueño o solicitante,
--      con la otra persona, el último mensaje y los no leídos
SELECT s.IdSolicitud, p.IdPublicacion, p.Titulo,
       u.IdUsuario AS IdOtra, u.Nombre AS NombreOtra,
       m.Texto AS UltimoTexto, m.FechaEnvio AS UltimaFecha,
       (SELECT COUNT(*) FROM Mensaje n WHERE n.IdSolicitud = s.IdSolicitud
           AND n.Leido = 0 AND n.IdEmisor <> @idUsuario) AS NoLeidos
FROM Solicitud s
INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion
INNER JOIN Usuario u ON u.IdUsuario =
    CASE WHEN p.IdUsuario = @idUsuario THEN s.IdSolicitante ELSE p.IdUsuario END
OUTER APPLY (SELECT TOP 1 Texto, FechaEnvio FROM Mensaje
             WHERE IdSolicitud = s.IdSolicitud
             ORDER BY FechaEnvio DESC, IdMensaje DESC) m
WHERE s.Estado = 1 AND (p.IdUsuario = @idUsuario OR s.IdSolicitante = @idUsuario)
ORDER BY ISNULL(m.FechaEnvio, s.FechaSolicitud) DESC;

-- 1.b) Notificaciones nuevas de la campanita
--      (respuestas a MIS solicitudes que todavía no vi + mensajes sin leer por conversación)
SELECT CASE s.Estado WHEN 1 THEN 'aceptada' ELSE 'rechazada' END AS Tipo,
       s.IdSolicitud, p.Titulo, u.Nombre AS Otro, 0 AS Cantidad,
       ISNULL(s.FechaRespuesta, s.FechaSolicitud) AS Fecha
FROM Solicitud s
INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion
INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario
WHERE s.IdSolicitante = @idUsuario AND s.Estado IN (1, 2) AND s.RespuestaVista = 0
UNION ALL
SELECT 'mensaje', s.IdSolicitud, p.Titulo, u.Nombre, COUNT(*), MAX(m.FechaEnvio)
FROM Mensaje m
INNER JOIN Solicitud s ON s.IdSolicitud = m.IdSolicitud
INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion
INNER JOIN Usuario u ON u.IdUsuario = m.IdEmisor
WHERE m.Leido = 0 AND m.IdEmisor <> @idUsuario AND s.Estado = 1
  AND (p.IdUsuario = @idUsuario OR s.IdSolicitante = @idUsuario)
GROUP BY s.IdSolicitud, p.Titulo, u.Nombre
ORDER BY Fecha DESC;
GO

-- ---------------------------------------------------------------------
-- 2) VER LOS DATOS CRUDOS
-- ---------------------------------------------------------------------
SELECT IdSolicitud, IdPublicacion, IdSolicitante, Estado, MotivoRespuesta, FechaRespuesta, RespuestaVista FROM Solicitud;
SELECT IdMensaje, IdSolicitud, IdEmisor, Texto, FechaEnvio, Leido FROM Mensaje ORDER BY IdSolicitud, FechaEnvio;
GO

-- ---------------------------------------------------------------------
-- 3) ESCENARIO DE PRUEBA: "al usuario 2 le aceptan una solicitud y le escriben"
--    Se arma con la publicación 1 prestada al usuario 1. Ejecutar de a una sección.
-- ---------------------------------------------------------------------
-- 3.a) ARMAR (usuario 1 = dueño del artículo, usuario 2 = quien lo pidió)
-- UPDATE Publicacion SET IdUsuario = 1, Estado = 1 WHERE IdPublicacion = 1;
-- INSERT INTO Solicitud (IdPublicacion, IdSolicitante, Estado, FechaRespuesta, RespuestaVista) VALUES (1, 2, 1, SYSUTCDATETIME(), 0);
-- INSERT INTO Mensaje (IdSolicitud, IdEmisor, Texto) VALUES (SCOPE_IDENTITY(), 1, N'Hola! Te acepté la solicitud, coordinamos por acá.');

-- 3.b) PROBAR EL "RECHAZADA" (en vez de aceptada)
-- UPDATE Solicitud SET Estado = 2, MotivoRespuesta = 1, RespuestaVista = 0 WHERE IdPublicacion = 1 AND IdSolicitante = 2;

-- 3.c) DEJAR TODO COMO ESTABA
-- DELETE FROM Mensaje WHERE IdSolicitud IN (SELECT IdSolicitud FROM Solicitud WHERE IdPublicacion = 1 AND IdSolicitante = 2);
-- DELETE FROM Solicitud WHERE IdPublicacion = 1 AND IdSolicitante = 2;
-- UPDATE Publicacion SET IdUsuario = 2, Estado = 0 WHERE IdPublicacion = 1;
GO
