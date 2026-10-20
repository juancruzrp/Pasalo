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



        public List<Publicacion> ListarDisponibles(
            int idCategoria = 0,
            int idCarrera = 0,
            string busqueda = "",
            bool masRecientes = true)
        {
            List<Publicacion> publicaciones = new List<Publicacion>();
            AccesoDatos datos = new AccesoDatos();

            try
            {
                string consulta = @"
            SELECT
                p.IdPublicacion,
                p.Titulo,
                p.Descripcion,
                p.Modalidad,
                p.Estado,
                p.FechaPublicacion,
                c.IdCategoria,
                c.Nombre AS CategoriaNombre,
                ca.IdCarrera,
                ca.Nombre AS CarreraNombre,
                f.Ruta AS FotoPrincipal
            FROM Publicacion p
            INNER JOIN Categoria c
                ON c.IdCategoria = p.IdCategoria
            LEFT JOIN Carrera ca
                ON ca.IdCarrera = p.IdCarrera
            OUTER APPLY (
                SELECT TOP 1 fp.Ruta
                FROM FotoPublicacion fp
                WHERE fp.IdPublicacion = p.IdPublicacion
                ORDER BY fp.Orden, fp.IdFoto
            ) f
            WHERE p.Estado = 0
              AND (@categoria = 0 OR p.IdCategoria = @categoria)
              AND (@carrera = 0 OR p.IdCarrera = @carrera)
              AND (
                  @busqueda = ''
                  OR p.Titulo LIKE '%' + @busqueda + '%'
                  OR p.Descripcion LIKE '%' + @busqueda + '%'
              )
            ORDER BY p.FechaPublicacion " +
                    (masRecientes ? "DESC" : "ASC") +
                    ", p.IdPublicacion " +
                    (masRecientes ? "DESC" : "ASC");

                datos.SetearConsulta(consulta);
                datos.SetearParametro("@categoria", idCategoria);
                datos.SetearParametro("@carrera", idCarrera);
                datos.SetearParametro("@busqueda", busqueda ?? "");

                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    Publicacion p = new Publicacion();

                    p.Id = Convert.ToInt32(
                        datos.Lector["IdPublicacion"]);

                    p.Titulo = datos.Lector["Titulo"].ToString();
                    p.Descripcion = datos.Lector["Descripcion"].ToString();

                    p.Modalidad = (ModalidadPublicacion)
                        Convert.ToByte(datos.Lector["Modalidad"]);

                    p.Estado = (EstadoPublicacion)
                        Convert.ToByte(datos.Lector["Estado"]);

                    p.FechaPublicacion = Convert.ToDateTime(
                        datos.Lector["FechaPublicacion"]);

                    p.Categoria = new Categoria
                    {
                        Id = Convert.ToInt32(
                            datos.Lector["IdCategoria"]),
                        Nombre = datos.Lector["CategoriaNombre"].ToString()
                    };

                    if (datos.Lector["IdCarrera"] != DBNull.Value)
                    {
                        p.Carrera = new Carrera
                        {
                            Id = Convert.ToInt32(
                                datos.Lector["IdCarrera"]),
                            Nombre = datos.Lector["CarreraNombre"].ToString()
                        };
                    }

                    if (datos.Lector["FotoPrincipal"] != DBNull.Value)
                    {
                        p.Fotos.Add(new FotoPublicacion
                        {
                            Ruta = datos.Lector["FotoPrincipal"].ToString(),
                            Orden = 0
                        });
                    }

                    publicaciones.Add(p);
                }

                return publicaciones;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }



        public Publicacion ObtenerPorId(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            Publicacion publicacion = null;

            try
            {
                datos.SetearConsulta(@"
            SELECT
                p.IdPublicacion,
                p.Titulo,
                p.Descripcion,
                p.Modalidad,
                p.Estado,
                p.FechaPublicacion,
                c.IdCategoria,
                c.Nombre AS CategoriaNombre,
                ca.IdCarrera,
                ca.Nombre AS CarreraNombre,
                u.IdUsuario,
                u.Nombre AS UsuarioNombre,
                u.FechaRegistro AS UsuarioFechaRegistro
            FROM Publicacion p
            INNER JOIN Categoria c
                ON p.IdCategoria = c.IdCategoria
            LEFT JOIN Carrera ca
                ON p.IdCarrera = ca.IdCarrera
            INNER JOIN Usuario u
                ON p.IdUsuario = u.IdUsuario
            WHERE p.IdPublicacion = @id");

                datos.SetearParametro("@id", id);
                datos.EjecutarLectura();

                if (datos.Lector.Read())
                {
                    publicacion = new Publicacion
                    {
                        Id = Convert.ToInt32(
                            datos.Lector["IdPublicacion"]),

                        Titulo = datos.Lector["Titulo"].ToString(),

                        Descripcion = datos.Lector["Descripcion"].ToString(),

                        Modalidad = (ModalidadPublicacion)
                            Convert.ToByte(datos.Lector["Modalidad"]),

                        Estado = (EstadoPublicacion)
                            Convert.ToByte(datos.Lector["Estado"]),

                        FechaPublicacion = Convert.ToDateTime(
                            datos.Lector["FechaPublicacion"]),

                        Categoria = new Categoria
                        {
                            Id = Convert.ToInt32(
                                datos.Lector["IdCategoria"]),
                            Nombre = datos.Lector["CategoriaNombre"].ToString()
                        },

                        Dueno = new Usuario
                        {
                            IdUsuario = Convert.ToInt32(
                                datos.Lector["IdUsuario"]),
                            Nombre = datos.Lector["UsuarioNombre"].ToString(),
                            FechaRegistro = Convert.ToDateTime(
                                datos.Lector["UsuarioFechaRegistro"])
                        }
                    };

                    if (datos.Lector["IdCarrera"] != DBNull.Value)
                    {
                        publicacion.Carrera = new Carrera
                        {
                            Id = Convert.ToInt32(
                                datos.Lector["IdCarrera"]),
                            Nombre = datos.Lector["CarreraNombre"].ToString()
                        };
                    }
                }
            }
            finally
            {
                datos.CerrarConexion();
            }

            if (publicacion == null)
                return null;

            AccesoDatos datosFotos = new AccesoDatos();

            try
            {
                datosFotos.SetearConsulta(@"
            SELECT IdFoto, Ruta, Orden
            FROM FotoPublicacion
            WHERE IdPublicacion = @id
            ORDER BY Orden, IdFoto");

                datosFotos.SetearParametro("@id", id);
                datosFotos.EjecutarLectura();

                while (datosFotos.Lector.Read())
                {
                    publicacion.Fotos.Add(new FotoPublicacion
                    {
                        Id = Convert.ToInt32(
                            datosFotos.Lector["IdFoto"]),

                        Ruta = datosFotos.Lector["Ruta"].ToString(),

                        Orden = Convert.ToInt32(
                            datosFotos.Lector["Orden"])
                    });
                }
            }
            finally
            {
                datosFotos.CerrarConexion();
            }

            return publicacion;
        }













    }
}

