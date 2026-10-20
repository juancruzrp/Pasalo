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

            // Publicaciones disponibles para el catálogo.
            // idCarrera / idCategoria = 0 significa "sin filtrar". Las publicaciones sin carrera
            // ("Todas las carreras") se muestran siempre, sea cual sea la carrera elegida.
            public List<Publicacion> ListarDisponibles(int idCarrera, int idCategoria, bool masRecientes)
            {
                List<Publicacion> lista = new List<Publicacion>();
                string orden = masRecientes ? "DESC" : "ASC";

                AccesoDatos datos = new AccesoDatos();
                try
                {
                    datos.SetearConsulta(
                        "SELECT p.IdPublicacion, p.Titulo, p.Modalidad, p.Estado, p.FechaPublicacion, " +
                        "c.IdCategoria, c.Nombre AS Categoria, ca.IdCarrera, ca.Nombre AS Carrera, " +
                        "(SELECT TOP 1 f.Ruta FROM FotoPublicacion f WHERE f.IdPublicacion = p.IdPublicacion ORDER BY f.Orden, f.IdFoto) AS Foto " +
                        "FROM Publicacion p " +
                        "INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria " +
                        "LEFT JOIN Carrera ca ON ca.IdCarrera = p.IdCarrera " +
                        "WHERE p.Estado = 0 " +
                        "AND (@idCarrera = 0 OR p.IdCarrera IS NULL OR p.IdCarrera = @idCarrera) " +
                        "AND (@idCategoria = 0 OR p.IdCategoria = @idCategoria) " +
                        "ORDER BY p.FechaPublicacion " + orden + ", p.IdPublicacion " + orden);
                    datos.SetearParametro("@idCarrera", idCarrera);
                    datos.SetearParametro("@idCategoria", idCategoria);
                    datos.EjecutarLectura();

                    while (datos.Lector.Read())
                    {
                        Publicacion p = new Publicacion();
                        p.Id = (int)datos.Lector["IdPublicacion"];
                        p.Titulo = (string)datos.Lector["Titulo"];
                        p.Modalidad = (ModalidadPublicacion)(byte)datos.Lector["Modalidad"];
                        p.Estado = (EstadoPublicacion)(byte)datos.Lector["Estado"];
                        p.FechaPublicacion = (DateTime)datos.Lector["FechaPublicacion"];
                        p.Categoria = new Categoria
                        {
                            Id = (int)datos.Lector["IdCategoria"],
                            Nombre = (string)datos.Lector["Categoria"]
                        };

                        if (!(datos.Lector["IdCarrera"] is DBNull))
                        {
                            p.Carrera = new Carrera
                            {
                                Id = (int)datos.Lector["IdCarrera"],
                                Nombre = (string)datos.Lector["Carrera"]
                            };
                        }

                        if (!(datos.Lector["Foto"] is DBNull))
                            p.Fotos.Add(new FotoPublicacion { Ruta = (string)datos.Lector["Foto"], Orden = 0 });

                        lista.Add(p);
                    }
                    return lista;
                }
                finally
                {
                    datos.CerrarConexion();
                }
            }

            // Una publicación por Id (cualquier estado) con categoría, carrera y fotos. null si no existe.
            public Publicacion ObtenerPorId(int id)
            {
                AccesoDatos datos = new AccesoDatos();
                try
                {
                    datos.SetearConsulta(
                        "SELECT p.IdPublicacion, p.Titulo, p.Descripcion, p.Modalidad, p.Estado, p.FechaPublicacion, " +
                        "c.IdCategoria, c.Nombre AS Categoria, ca.IdCarrera, ca.Nombre AS Carrera, " +
                        "u.IdUsuario, u.Nombre AS DuenoNombre, u.FechaRegistro AS DuenoDesde, uc.Nombre AS DuenoCarrera " +
                        "FROM Publicacion p " +
                        "INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria " +
                        "LEFT JOIN Carrera ca ON ca.IdCarrera = p.IdCarrera " +
                        "INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario " +
                        "LEFT JOIN Carrera uc ON uc.IdCarrera = u.IdCarrera " +
                        "WHERE p.IdPublicacion = @id");
                    datos.SetearParametro("@id", id);
                    datos.EjecutarLectura();

                    if (!datos.Lector.Read())
                        return null;

                    Publicacion p = new Publicacion();
                    p.Id = (int)datos.Lector["IdPublicacion"];
                    p.Titulo = (string)datos.Lector["Titulo"];
                    p.Descripcion = datos.Lector["Descripcion"] is DBNull ? "" : (string)datos.Lector["Descripcion"];
                    p.Modalidad = (ModalidadPublicacion)(byte)datos.Lector["Modalidad"];
                    p.Estado = (EstadoPublicacion)(byte)datos.Lector["Estado"];
                    p.FechaPublicacion = (DateTime)datos.Lector["FechaPublicacion"];
                    p.Categoria = new Categoria
                    {
                        Id = (int)datos.Lector["IdCategoria"],
                        Nombre = (string)datos.Lector["Categoria"]
                    };
                    if (!(datos.Lector["IdCarrera"] is DBNull))
                        p.Carrera = new Carrera { Id = (int)datos.Lector["IdCarrera"], Nombre = (string)datos.Lector["Carrera"] };
                    p.Dueno = new Usuario
                    {
                        IdUsuario = (int)datos.Lector["IdUsuario"],
                        Nombre = (string)datos.Lector["DuenoNombre"],
                        FechaRegistro = (DateTime)datos.Lector["DuenoDesde"],
                        CarreraNombre = datos.Lector["DuenoCarrera"] is DBNull ? null : (string)datos.Lector["DuenoCarrera"]
                    };
                    datos.CerrarConexion();

                    // Fotos (segunda consulta, con el lector anterior ya cerrado)
                    datos = new AccesoDatos();
                    datos.SetearConsulta("SELECT IdFoto, Ruta, Orden FROM FotoPublicacion WHERE IdPublicacion = @id ORDER BY Orden, IdFoto");
                    datos.SetearParametro("@id", id);
                    datos.EjecutarLectura();
                    while (datos.Lector.Read())
                        p.Fotos.Add(new FotoPublicacion
                        {
                            Id = (int)datos.Lector["IdFoto"],
                            Ruta = (string)datos.Lector["Ruta"],
                            Orden = Convert.ToInt32(datos.Lector["Orden"])
                        });
                    return p;
                }
                finally
                {
                    datos.CerrarConexion();
                }
            }

            // Cuántas publicaciones disponibles hay por categoría (Id de categoría -> cantidad),
            // respetando el filtro de carrera, para los contadores del catálogo.
            public Dictionary<int, int> ContarDisponiblesPorCategoria(int idCarrera)
            {
                Dictionary<int, int> cuentas = new Dictionary<int, int>();

                AccesoDatos datos = new AccesoDatos();
                try
                {
                    datos.SetearConsulta(
                        "SELECT IdCategoria, COUNT(*) AS Cantidad FROM Publicacion " +
                        "WHERE Estado = 0 AND (@idCarrera = 0 OR IdCarrera IS NULL OR IdCarrera = @idCarrera) " +
                        "GROUP BY IdCategoria");
                    datos.SetearParametro("@idCarrera", idCarrera);
                    datos.EjecutarLectura();

                    while (datos.Lector.Read())
                        cuentas[(int)datos.Lector["IdCategoria"]] = (int)datos.Lector["Cantidad"];

                    return cuentas;
                }
                finally
                {
                    datos.CerrarConexion();
                }
            }
        }
    }

