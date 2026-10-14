using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using dominio;



namespace acceso
{
    public class PublicacionAcceso
    {
   
            // Guarda la publicación y sus fotos en UNA sola transacción:
            // si algo falla, no queda una publicación sin fotos ni fotos sueltas.
            // Devuelve el Id generado. Estado inicial: Disponible (0).
            public int Agregar(Publicacion p)
            {
                StringBuilder sql = new StringBuilder();
                sql.Append("SET XACT_ABORT ON; BEGIN TRANSACTION; ");
                sql.Append("INSERT INTO Publicacion (IdUsuario, Titulo, Descripcion, IdCategoria, IdCarrera, Modalidad, Estado) ");
                sql.Append("VALUES (@idUsuario, @titulo, @descripcion, @idCategoria, @idCarrera, @modalidad, 0); ");
                sql.Append("DECLARE @id INT = SCOPE_IDENTITY(); ");
                for (int i = 0; i < p.Fotos.Count; i++)
                    sql.AppendFormat("INSERT INTO FotoPublicacion (IdPublicacion, Ruta, Orden) VALUES (@id, @ruta{0}, {0}); ", i);
                sql.Append("COMMIT TRANSACTION; SELECT @id;");

                AccesoDatos datos = new AccesoDatos();
                try
                {
                    datos.SetearConsulta(sql.ToString());
                    datos.SetearParametro("@idUsuario", p.Dueno.IdUsuario);
                    datos.SetearParametro("@titulo", p.Titulo);
                    datos.SetearParametro("@descripcion", p.Descripcion);
                    datos.SetearParametro("@idCategoria", p.Categoria.Id);
                    datos.SetearParametro("@idCarrera", p.Carrera == null ? (int?)null : p.Carrera.Id);
                    datos.SetearParametro("@modalidad", (byte)p.Modalidad.Value);
                    for (int i = 0; i < p.Fotos.Count; i++)
                        datos.SetearParametro("@ruta" + i, p.Fotos[i].Ruta);

                    return Convert.ToInt32(datos.EjecutarScalar());
                }
                finally
                {
                    datos.CerrarConexion();
                }
            }
        }
    }

