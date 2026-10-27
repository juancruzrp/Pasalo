-- =====================================================================
-- PÁSALO - Consultas del Perfil y del impacto ambiental (+ pruebas manuales)
-- NO es parte de la instalación: es material de consulta y de prueba.
-- Ejecutar de a una sección: seleccionar el bloque y F5.
-- Las consultas de la sección 1 son las mismas que usa la aplicación (acceso/PerfilAcceso.cs).
-- =====================================================================
USE Pasalo;
GO

-- ---------------------------------------------------------------------
-- 1) CONSULTAS QUE USA EL PERFIL (cambiar @id por el IdUsuario a mirar)
-- ---------------------------------------------------------------------
DECLARE @id INT = 2;

-- 1.a) Cabecera: nombre, carrera y facultad
SELECT u.IdUsuario, u.Nombre, u.FechaRegistro, ca.Nombre AS Carrera, f.IdFacultad, f.Nombre AS Facultad
FROM Usuario u
LEFT JOIN Carrera ca ON ca.IdCarrera = u.IdCarrera
LEFT JOIN Facultad f ON f.IdFacultad = ca.IdFacultad
WHERE u.IdUsuario = @id AND u.Activo = 1;

-- 1.b) Impacto: solo cuentan las publicaciones ENTREGADAS (Estado = 2). Kg = peso estimado de la categoría
SELECT COUNT(*) AS Objetos, ISNULL(SUM(c.PesoEstimadoKg), 0) AS Kg
FROM Publicacion p INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
WHERE p.IdUsuario = @id AND p.Estado = 2;

-- 1.c) Reputación: promedio de las valoraciones recibidas
SELECT COUNT(*) AS Cantidad, AVG(CAST(Puntaje AS DECIMAL(4,2))) AS Promedio
FROM Valoracion WHERE IdEvaluado = @id;

-- 1.d) Impacto por facultad (incluye las facultades con 0 kg)
SELECT f.IdFacultad, f.Nombre, ISNULL(SUM(x.Kg), 0) AS Kg
FROM Facultad f
LEFT JOIN (
    SELECT ca.IdFacultad, c.PesoEstimadoKg AS Kg
    FROM Publicacion p
    INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria
    INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario
    INNER JOIN Carrera ca ON ca.IdCarrera = u.IdCarrera
    WHERE p.Estado = 2
) x ON x.IdFacultad = f.IdFacultad
GROUP BY f.IdFacultad, f.Nombre
ORDER BY Kg DESC, f.Nombre;
GO

-- ---------------------------------------------------------------------
-- 2) PRUEBA: simular entregas para ver el impacto en el perfil
--    (hoy la app todavía no tiene el paso "confirmar entrega", por eso se hace a mano)
-- ---------------------------------------------------------------------
-- 2.a) Ver qué hay
SELECT IdPublicacion, IdUsuario, Titulo, Estado FROM Publicacion;   -- Estado: 0 Disponible, 1 Reservado, 2 Entregado

-- 2.b) Pasar publicaciones a Entregado (cambiar los ids)
-- UPDATE Publicacion SET Estado = 2 WHERE IdPublicacion IN (1, 2);

-- 2.c) Cargar una carrera a un usuario (para que aparezca "Carrera · Facultad" y el % de aporte)
-- UPDATE Usuario SET IdCarrera = (SELECT TOP 1 IdCarrera FROM Carrera WHERE Nombre LIKE 'Ingenier%a en Sistemas%') WHERE IdUsuario = 2;

-- 2.d) DEJAR TODO COMO ESTABA
-- UPDATE Publicacion SET Estado = 0 WHERE IdPublicacion IN (1, 2);
-- UPDATE Usuario SET IdCarrera = NULL WHERE IdUsuario = 2;
GO
