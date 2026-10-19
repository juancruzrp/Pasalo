-- PÁSALO - Script 05: carreras reales de la UTN FRGP (las mismas del formulario de registro).
-- Reemplaza a los datos de ejemplo del script 01 y al script 04 (no hace falta ejecutar el 04).
-- Se puede ejecutar más de una vez sin duplicar nada.
USE Pasalo;
GO
 
-- 1) Si existe la carrera de ejemplo "Ingeniería en Sistemas", se renombra (conserva su Id y lo que ya la use)
UPDATE Carrera
SET Nombre = N'Ingeniería en Sistemas de Información'
WHERE Nombre = N'Ingeniería en Sistemas'
  AND NOT EXISTS (SELECT 1 FROM Carrera WHERE Nombre = N'Ingeniería en Sistemas de Información');
 
-- 2) Se agregan las que falten
INSERT INTO Carrera (Nombre, IdFacultad)
SELECT v.Nombre, 1
FROM (VALUES
        (N'Tecnicatura Universitaria en Programación'),
        (N'Ingeniería en Sistemas de Información'),
        (N'Ingeniería Mecánica'),
        (N'Ingeniería Eléctrica'),
        (N'Ingeniería Industrial'),
        (N'Otra carrera')
     ) AS v(Nombre)
WHERE NOT EXISTS (SELECT 1 FROM Carrera c WHERE c.Nombre = v.Nombre);
 
-- 3) Se borran las carreras de ejemplo que no existen en la UTN FRGP, solo si nadie las está usando
DELETE c
FROM Carrera c
WHERE c.Nombre IN (N'Contador Público', N'Ingeniería Química')
  AND NOT EXISTS (SELECT 1 FROM Usuario u WHERE u.IdCarrera = c.IdCarrera)
  AND NOT EXISTS (SELECT 1 FROM Publicacion p WHERE p.IdCarrera = c.IdCarrera);
GO
 

