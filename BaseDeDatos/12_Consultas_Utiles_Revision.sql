-- =====================================================================
-- PÁSALO - Consultas útiles para revisar la base (solo lectura)
-- =====================================================================
USE Pasalo;
GO

-- Columnas de cada tabla (sirve para comprobar que se corrieron los scripts 02, 04 y 05)
SELECT t.name AS Tabla, c.column_id AS Nro, c.name AS Columna, ty.name AS Tipo, c.is_nullable AS AceptaNull
FROM sys.tables t
INNER JOIN sys.columns c ON c.object_id = t.object_id
INNER JOIN sys.types ty ON ty.user_type_id = c.user_type_id
ORDER BY t.name, c.column_id;

-- ¿Están las columnas agregadas por los scripts 04 y 05? (deben dar 1 cada una)
SELECT
    (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Solicitud') AND name = 'FechaRespuesta')  AS Script04_FechaRespuesta,
    (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Solicitud') AND name = 'MotivoRespuesta') AS Script04_MotivoRespuesta,
    (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Mensaje')   AND name = 'Leido')           AS Script05_Leido,
    (SELECT COUNT(*) FROM sys.columns WHERE object_id = OBJECT_ID('Solicitud') AND name = 'RespuestaVista')  AS Script05_RespuestaVista;

-- Usuarios (sin mostrar la contraseña)
SELECT u.IdUsuario, u.Email, u.Nombre, ca.Nombre AS Carrera, u.Rol, u.Activo, u.FechaRegistro
FROM Usuario u LEFT JOIN Carrera ca ON ca.IdCarrera = u.IdCarrera;

-- Publicaciones con dueño, categoría y estado en texto
SELECT p.IdPublicacion, p.Titulo, u.Nombre AS Dueno, c.Nombre AS Categoria,
       CASE p.Modalidad WHEN 0 THEN 'Donación' ELSE 'Intercambio' END AS Modalidad,
       CASE p.Estado WHEN 0 THEN 'Disponible' WHEN 1 THEN 'Reservado' ELSE 'Entregado' END AS Estado,
       p.FechaPublicacion
FROM Publicacion p
INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario
INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
ORDER BY p.IdPublicacion;

-- Solicitudes con quién pide qué
SELECT s.IdSolicitud, p.Titulo, ud.Nombre AS Dueno, us.Nombre AS Solicitante,
       CASE s.Estado WHEN 0 THEN 'Pendiente' WHEN 1 THEN 'Aceptada' ELSE 'Rechazada' END AS Estado,
       s.FechaSolicitud, s.FechaRespuesta
FROM Solicitud s
INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion
INNER JOIN Usuario ud ON ud.IdUsuario = p.IdUsuario
INNER JOIN Usuario us ON us.IdUsuario = s.IdSolicitante
ORDER BY s.IdSolicitud;

-- Categorías y su peso estimado (base del cálculo de kg evitados)
SELECT IdCategoria, Nombre, PesoEstimadoKg FROM Categoria ORDER BY IdCategoria;

-- Carreras y facultades
SELECT ca.IdCarrera, ca.Nombre AS Carrera, f.Nombre AS Facultad
FROM Carrera ca INNER JOIN Facultad f ON f.IdFacultad = ca.IdFacultad ORDER BY f.Nombre, ca.Nombre;
GO
