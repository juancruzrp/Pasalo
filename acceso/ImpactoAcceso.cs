// Proyecto: acceso
using System;
using dominio;

namespace acceso
{
    public class ImpactoAcceso
    {
        // Una sola consulta con los tres totales. Solo cuentan las publicaciones ya Entregadas (Estado = 2).
        public ImpactoComunidad ObtenerComunidad()
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT " +
                    "(SELECT COUNT(*) FROM Publicacion WHERE Estado = 2) AS Objetos, " +
                    "(SELECT ISNULL(SUM(c.PesoEstimadoKg), 0) FROM Publicacion p " +
                    "   INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria WHERE p.Estado = 2) AS Kg, " +
                    "(SELECT COUNT(*) FROM Usuario WHERE Activo = 1) AS Estudiantes");
                datos.EjecutarLectura();

                if (!datos.Lector.Read())
                    return null;

                ImpactoComunidad c = new ImpactoComunidad();
                c.ObjetosEntregados = (int)datos.Lector["Objetos"];
                c.KgEvitados = Convert.ToDecimal(datos.Lector["Kg"]);
                c.Estudiantes = (int)datos.Lector["Estudiantes"];
                return c;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}