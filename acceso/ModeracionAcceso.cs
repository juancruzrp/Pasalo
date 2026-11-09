using System;
using System.Collections.Generic;
using dominio;

namespace acceso
{
    // Todo el SQL del panel de moderación. Las reglas (quién puede qué) viven en negocio/ModeracionNegocio.
    public class ModeracionAcceso
    {
        // ---------- ayudas ----------
        private static void Accion(string sql, params object[] pares)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(sql);
                for (int i = 0; i < pares.Length; i += 2)
                    datos.SetearParametro((string)pares[i], pares[i + 1]);
                datos.EjecutarAccion();
            }
            finally { datos.CerrarConexion(); }
        }

        private static object Escalar(string sql, params object[] pares)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(sql);
                for (int i = 0; i < pares.Length; i += 2)
                    datos.SetearParametro((string)pares[i], pares[i + 1]);
                return datos.EjecutarScalar();
            }
            finally { datos.CerrarConexion(); }
        }

        private static int? IntNulo(object valor)
        {
            return Convert.IsDBNull(valor) ? (int?)null : Convert.ToInt32(valor);
        }

        private static string TextoNulo(object valor)
        {
            return Convert.IsDBNull(valor) ? "" : (string)valor;
        }

        // ---------- sesión / permisos ----------
        // Devuelve false si el usuario no existe o está suspendido; si existe, deja su rol actual.
        public bool LeerEstado(int idUsuario, out bool activo, out int rol)
        {
            activo = false;
            rol = 0;
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT Activo, Rol FROM Usuario WHERE IdUsuario = @id");
                datos.SetearParametro("@id", idUsuario);
                datos.EjecutarLectura();
                if (!datos.Lector.Read())
                    return false;
                activo = Convert.ToBoolean(datos.Lector["Activo"]);
                rol = Convert.ToInt32(datos.Lector["Rol"]);
                return true;
            }
            finally { datos.CerrarConexion(); }
        }

        public bool EsModeradorActivo(int idUsuario)
        {
            return Convert.ToInt32(Escalar(
                "SELECT COUNT(*) FROM Usuario WHERE IdUsuario = @id AND Rol = 1 AND Activo = 1",
                "@id", idUsuario)) > 0;
        }

        public bool PublicacionSuspendida(int idPublicacion)
        {
            return Convert.ToInt32(Escalar(
                "SELECT COUNT(*) FROM Publicacion WHERE IdPublicacion = @id AND Suspendida = 1",
                "@id", idPublicacion)) > 0;
        }

        // ---------- usuarios ----------
        private const string SqlUsuario =
            "SELECT u.IdUsuario, u.Email, u.Nombre, u.IdCarrera, ca.Nombre AS Carrera, u.Descripcion, " +
            "       u.Rol, u.Activo, u.EmailVerificado, u.FechaRegistro " +
            "FROM Usuario u LEFT JOIN Carrera ca ON ca.IdCarrera = u.IdCarrera ";

        private static ModUsuario LeerUsuario(System.Data.SqlClient.SqlDataReader r)
        {
            return new ModUsuario
            {
                IdUsuario = Convert.ToInt32(r["IdUsuario"]),
                Email = (string)r["Email"],
                Nombre = (string)r["Nombre"],
                IdCarrera = IntNulo(r["IdCarrera"]),
                Carrera = TextoNulo(r["Carrera"]),
                Descripcion = TextoNulo(r["Descripcion"]),
                Rol = Convert.ToInt32(r["Rol"]),
                Activo = Convert.ToBoolean(r["Activo"]),
                EmailVerificado = Convert.ToBoolean(r["EmailVerificado"]),
                FechaRegistro = (DateTime)r["FechaRegistro"]
            };
        }

        public List<ModUsuario> ListarUsuarios(string busqueda)
        {
            List<ModUsuario> lista = new List<ModUsuario>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT TOP 200 * FROM (" + SqlUsuario + ") x " +
                    "WHERE (@b = '' OR x.Nombre LIKE '%' + @b + '%' OR x.Email LIKE '%' + @b + '%') " +
                    "ORDER BY x.Rol DESC, x.Nombre");
                datos.SetearParametro("@b", busqueda ?? "");
                datos.EjecutarLectura();
                while (datos.Lector.Read())
                    lista.Add(LeerUsuario(datos.Lector));
                return lista;
            }
            finally { datos.CerrarConexion(); }
        }

        public ModUsuario ObtenerUsuario(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(SqlUsuario + "WHERE u.IdUsuario = @id");
                datos.SetearParametro("@id", id);
                datos.EjecutarLectura();
                return datos.Lector.Read() ? LeerUsuario(datos.Lector) : null;
            }
            finally { datos.CerrarConexion(); }
        }

        public void EditarUsuario(int id, string nombre, int? idCarrera, string descripcion)
        {
            Accion("UPDATE Usuario SET Nombre = @n, IdCarrera = @c, Descripcion = @d WHERE IdUsuario = @id",
                "@n", nombre, "@c", idCarrera, "@d", string.IsNullOrWhiteSpace(descripcion) ? null : descripcion, "@id", id);
        }

        public void CambiarActivo(int id, bool activo)
        {
            Accion("UPDATE Usuario SET Activo = @a WHERE IdUsuario = @id", "@a", activo, "@id", id);
        }

        public void CambiarRol(int id, int rol)
        {
            Accion("UPDATE Usuario SET Rol = @r WHERE IdUsuario = @id", "@r", (byte)rol, "@id", id);
        }

        public void VerificarCorreo(int id)
        {
            Accion("UPDATE Usuario SET EmailVerificado = 1 WHERE IdUsuario = @id", "@id", id);
        }

        // ---------- publicaciones ----------
        private const string SqlPublicacion =
            "SELECT p.IdPublicacion, p.Titulo, p.Descripcion, p.IdCategoria, c.Nombre AS Categoria, " +
            "       p.IdCarrera, ca.Nombre AS Carrera, p.Modalidad, p.Estado, p.Suspendida, " +
            "       p.IdUsuario, u.Nombre AS Dueno, p.FechaPublicacion, " +
            "       (SELECT COUNT(*) FROM Solicitud s WHERE s.IdPublicacion = p.IdPublicacion) AS Solicitudes " +
            "FROM Publicacion p " +
            "INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria " +
            "INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario " +
            "LEFT JOIN Carrera ca ON ca.IdCarrera = p.IdCarrera ";

        private static ModPublicacion LeerPublicacion(System.Data.SqlClient.SqlDataReader r)
        {
            return new ModPublicacion
            {
                IdPublicacion = Convert.ToInt32(r["IdPublicacion"]),
                Titulo = (string)r["Titulo"],
                Descripcion = (string)r["Descripcion"],
                IdCategoria = Convert.ToInt32(r["IdCategoria"]),
                Categoria = (string)r["Categoria"],
                IdCarrera = IntNulo(r["IdCarrera"]),
                Carrera = TextoNulo(r["Carrera"]),
                Modalidad = Convert.ToInt32(r["Modalidad"]),
                Estado = Convert.ToInt32(r["Estado"]),
                Suspendida = Convert.ToBoolean(r["Suspendida"]),
                IdDueno = Convert.ToInt32(r["IdUsuario"]),
                Dueno = (string)r["Dueno"],
                Fecha = (DateTime)r["FechaPublicacion"],
                Solicitudes = Convert.ToInt32(r["Solicitudes"])
            };
        }

        public List<ModPublicacion> ListarPublicaciones(string busqueda)
        {
            List<ModPublicacion> lista = new List<ModPublicacion>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(SqlPublicacion +
                    "WHERE (@b = '' OR p.Titulo LIKE '%' + @b + '%' OR u.Nombre LIKE '%' + @b + '%') " +
                    "ORDER BY p.FechaPublicacion DESC, p.IdPublicacion DESC");
                datos.SetearParametro("@b", busqueda ?? "");
                datos.EjecutarLectura();
                while (datos.Lector.Read() && lista.Count < 200)
                    lista.Add(LeerPublicacion(datos.Lector));
                return lista;
            }
            finally { datos.CerrarConexion(); }
        }

        public ModPublicacion ObtenerPublicacion(int id)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(SqlPublicacion + "WHERE p.IdPublicacion = @id");
                datos.SetearParametro("@id", id);
                datos.EjecutarLectura();
                return datos.Lector.Read() ? LeerPublicacion(datos.Lector) : null;
            }
            finally { datos.CerrarConexion(); }
        }

        public void EditarPublicacion(int id, string titulo, string descripcion, int idCategoria, int? idCarrera)
        {
            Accion("UPDATE Publicacion SET Titulo = @t, Descripcion = @d, IdCategoria = @c, IdCarrera = @ca WHERE IdPublicacion = @id",
                "@t", titulo, "@d", descripcion, "@c", idCategoria, "@ca", idCarrera, "@id", id);
        }

        public void CambiarSuspension(int id, bool suspendida)
        {
            Accion("UPDATE Publicacion SET Suspendida = @s WHERE IdPublicacion = @id", "@s", suspendida, "@id", id);
        }

        // Solo para publicaciones sin solicitudes (las fotos se borran en cascada).
        public void EliminarPublicacion(int id)
        {
            Accion("DELETE FROM Reporte WHERE IdPublicacion = @id; DELETE FROM Publicacion WHERE IdPublicacion = @id;", "@id", id);
        }

        // ---------- categorías ----------
        public List<ModCategoria> ListarCategorias()
        {
            List<ModCategoria> lista = new List<ModCategoria>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT c.IdCategoria, c.Nombre, c.PesoEstimadoKg, " +
                    "(SELECT COUNT(*) FROM Publicacion p WHERE p.IdCategoria = c.IdCategoria) AS EnUso " +
                    "FROM Categoria c ORDER BY c.Nombre");
                datos.EjecutarLectura();
                while (datos.Lector.Read())
                    lista.Add(new ModCategoria
                    {
                        Id = Convert.ToInt32(datos.Lector["IdCategoria"]),
                        Nombre = (string)datos.Lector["Nombre"],
                        PesoKg = Convert.ToDecimal(datos.Lector["PesoEstimadoKg"]),
                        EnUso = Convert.ToInt32(datos.Lector["EnUso"])
                    });
                return lista;
            }
            finally { datos.CerrarConexion(); }
        }

        public bool ExisteCategoria(string nombre, int exceptoId)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Categoria WHERE Nombre = @n AND IdCategoria <> @id",
                "@n", nombre, "@id", exceptoId)) > 0;
        }

        public void AgregarCategoria(string nombre, decimal peso)
        {
            Accion("INSERT INTO Categoria (Nombre, PesoEstimadoKg) VALUES (@n, @p)", "@n", nombre, "@p", peso);
        }

        public void EditarCategoria(int id, string nombre, decimal peso)
        {
            Accion("UPDATE Categoria SET Nombre = @n, PesoEstimadoKg = @p WHERE IdCategoria = @id", "@n", nombre, "@p", peso, "@id", id);
        }

        public int UsosCategoria(int id)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Publicacion WHERE IdCategoria = @id", "@id", id));
        }

        public void EliminarCategoria(int id)
        {
            Accion("DELETE FROM Categoria WHERE IdCategoria = @id", "@id", id);
        }

        // ---------- facultades y carreras ----------
        public List<ModFacultad> ListarFacultades()
        {
            List<ModFacultad> lista = new List<ModFacultad>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT f.IdFacultad, f.Nombre, " +
                    "(SELECT COUNT(*) FROM Carrera c WHERE c.IdFacultad = f.IdFacultad) AS Carreras " +
                    "FROM Facultad f ORDER BY f.Nombre");
                datos.EjecutarLectura();
                while (datos.Lector.Read())
                    lista.Add(new ModFacultad
                    {
                        Id = Convert.ToInt32(datos.Lector["IdFacultad"]),
                        Nombre = (string)datos.Lector["Nombre"],
                        Carreras = Convert.ToInt32(datos.Lector["Carreras"])
                    });
                return lista;
            }
            finally { datos.CerrarConexion(); }
        }

        public bool ExisteFacultad(string nombre, int exceptoId)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Facultad WHERE Nombre = @n AND IdFacultad <> @id",
                "@n", nombre, "@id", exceptoId)) > 0;
        }

        public bool ExisteIdFacultad(int id)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Facultad WHERE IdFacultad = @id", "@id", id)) > 0;
        }

        public void AgregarFacultad(string nombre)
        {
            Accion("INSERT INTO Facultad (Nombre) VALUES (@n)", "@n", nombre);
        }

        public void EditarFacultad(int id, string nombre)
        {
            Accion("UPDATE Facultad SET Nombre = @n WHERE IdFacultad = @id", "@n", nombre, "@id", id);
        }

        public int CarrerasDeFacultad(int id)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Carrera WHERE IdFacultad = @id", "@id", id));
        }

        public void EliminarFacultad(int id)
        {
            Accion("DELETE FROM Facultad WHERE IdFacultad = @id", "@id", id);
        }

        public List<ModCarrera> ListarCarreras()
        {
            List<ModCarrera> lista = new List<ModCarrera>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT c.IdCarrera, c.Nombre, c.IdFacultad, f.Nombre AS Facultad, " +
                    "((SELECT COUNT(*) FROM Usuario u WHERE u.IdCarrera = c.IdCarrera) + " +
                    " (SELECT COUNT(*) FROM Publicacion p WHERE p.IdCarrera = c.IdCarrera)) AS EnUso " +
                    "FROM Carrera c INNER JOIN Facultad f ON f.IdFacultad = c.IdFacultad ORDER BY f.Nombre, c.Nombre");
                datos.EjecutarLectura();
                while (datos.Lector.Read())
                    lista.Add(new ModCarrera
                    {
                        Id = Convert.ToInt32(datos.Lector["IdCarrera"]),
                        Nombre = (string)datos.Lector["Nombre"],
                        IdFacultad = Convert.ToInt32(datos.Lector["IdFacultad"]),
                        Facultad = (string)datos.Lector["Facultad"],
                        EnUso = Convert.ToInt32(datos.Lector["EnUso"])
                    });
                return lista;
            }
            finally { datos.CerrarConexion(); }
        }

        public bool ExisteCarrera(string nombre, int exceptoId)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Carrera WHERE Nombre = @n AND IdCarrera <> @id",
                "@n", nombre, "@id", exceptoId)) > 0;
        }

        public bool ExisteIdCarrera(int id)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Carrera WHERE IdCarrera = @id", "@id", id)) > 0;
        }

        public bool ExisteIdCategoria(int id)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Categoria WHERE IdCategoria = @id", "@id", id)) > 0;
        }

        public void AgregarCarrera(string nombre, int idFacultad)
        {
            Accion("INSERT INTO Carrera (Nombre, IdFacultad) VALUES (@n, @f)", "@n", nombre, "@f", idFacultad);
        }

        public void EditarCarrera(int id, string nombre, int idFacultad)
        {
            Accion("UPDATE Carrera SET Nombre = @n, IdFacultad = @f WHERE IdCarrera = @id", "@n", nombre, "@f", idFacultad, "@id", id);
        }

        public int UsosCarrera(int id)
        {
            return Convert.ToInt32(Escalar(
                "SELECT (SELECT COUNT(*) FROM Usuario WHERE IdCarrera = @id) + (SELECT COUNT(*) FROM Publicacion WHERE IdCarrera = @id)",
                "@id", id));
        }

        public void EliminarCarrera(int id)
        {
            Accion("DELETE FROM Carrera WHERE IdCarrera = @id", "@id", id);
        }

        // ---------- reportes ----------
        public List<ModReporte> ListarReportes(bool incluirResueltos)
        {
            List<ModReporte> lista = new List<ModReporte>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT TOP 200 r.IdReporte, r.Motivo, r.Detalle, r.Estado, r.Fecha, ur.Nombre AS Reportante, " +
                    "       r.IdPublicacion, p.Titulo, r.IdUsuarioReportado, uu.Nombre AS UsuarioReportado " +
                    "FROM Reporte r " +
                    "INNER JOIN Usuario ur ON ur.IdUsuario = r.IdReportante " +
                    "LEFT JOIN Publicacion p ON p.IdPublicacion = r.IdPublicacion " +
                    "LEFT JOIN Usuario uu ON uu.IdUsuario = r.IdUsuarioReportado " +
                    "WHERE (@todos = 1 OR r.Estado = 0) " +
                    "ORDER BY r.Estado, r.Fecha DESC");
                datos.SetearParametro("@todos", incluirResueltos ? 1 : 0);
                datos.EjecutarLectura();
                while (datos.Lector.Read())
                    lista.Add(new ModReporte
                    {
                        IdReporte = Convert.ToInt32(datos.Lector["IdReporte"]),
                        Motivo = (string)datos.Lector["Motivo"],
                        Detalle = TextoNulo(datos.Lector["Detalle"]),
                        Estado = Convert.ToInt32(datos.Lector["Estado"]),
                        Fecha = (DateTime)datos.Lector["Fecha"],
                        Reportante = (string)datos.Lector["Reportante"],
                        IdPublicacion = IntNulo(datos.Lector["IdPublicacion"]),
                        Publicacion = TextoNulo(datos.Lector["Titulo"]),
                        IdUsuarioReportado = IntNulo(datos.Lector["IdUsuarioReportado"]),
                        UsuarioReportado = TextoNulo(datos.Lector["UsuarioReportado"])
                    });
                return lista;
            }
            finally { datos.CerrarConexion(); }
        }

        public int CantidadReportesPendientes()
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Reporte WHERE Estado = 0"));
        }

        // estado: 1 = desestimado, 2 = resuelto. Solo cambia reportes todavía pendientes.
        public void ResolverReporte(int idReporte, int estado, int idModerador)
        {
            Accion("UPDATE Reporte SET Estado = @e, IdModerador = @m, FechaResolucion = SYSUTCDATETIME() " +
                   "WHERE IdReporte = @id AND Estado = 0",
                "@e", (byte)estado, "@m", idModerador, "@id", idReporte);
        }

        // Devuelve false si ya había un reporte pendiente de la misma persona sobre lo mismo.
        public bool Reportar(int idReportante, int? idPublicacion, int? idUsuario, string motivo, string detalle)
        {
            object filas = Escalar(
                "INSERT INTO Reporte (IdReportante, IdPublicacion, IdUsuarioReportado, Motivo, Detalle) " +
                "SELECT @r, @p, @u, @m, @d " +
                "WHERE NOT EXISTS (SELECT 1 FROM Reporte WHERE IdReportante = @r AND Estado = 0 " +
                "                  AND ISNULL(IdPublicacion, 0) = ISNULL(@p, 0) AND ISNULL(IdUsuarioReportado, 0) = ISNULL(@u, 0)); " +
                "SELECT @@ROWCOUNT;",
                "@r", idReportante, "@p", idPublicacion, "@u", idUsuario, "@m", motivo,
                "@d", string.IsNullOrWhiteSpace(detalle) ? null : detalle);
            return Convert.ToInt32(filas) > 0;
        }

        public bool ExistePublicacion(int id)
        {
            return Convert.ToInt32(Escalar("SELECT COUNT(*) FROM Publicacion WHERE IdPublicacion = @id", "@id", id)) > 0;
        }

        public string NombreUsuario(int id)
        {
            object n = Escalar("SELECT Nombre FROM Usuario WHERE IdUsuario = @id", "@id", id);
            return n == null || Convert.IsDBNull(n) ? null : (string)n;
        }

        public string DuenoPublicacion(int idPublicacion, out int idDueno, out string titulo)
        {
            idDueno = 0;
            titulo = null;
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT p.IdUsuario, p.Titulo, u.Nombre FROM Publicacion p INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario WHERE p.IdPublicacion = @id");
                datos.SetearParametro("@id", idPublicacion);
                datos.EjecutarLectura();
                if (!datos.Lector.Read())
                    return null;
                idDueno = Convert.ToInt32(datos.Lector["IdUsuario"]);
                titulo = (string)datos.Lector["Titulo"];
                return (string)datos.Lector["Nombre"];
            }
            finally { datos.CerrarConexion(); }
        }
    }
}
