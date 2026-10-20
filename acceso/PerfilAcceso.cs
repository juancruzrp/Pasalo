using System;
using System.Collections.Generic;
using dominio;

namespace acceso
{
    // Consultas de la pantalla Perfil. Solo lee: no modifica nada en la base.
    public class PerfilAcceso
    {
        // Cabecera + totales de impacto + reputacion. null si el usuario no existe o esta inactivo.
        public PerfilUsuario ObtenerPerfil(int idUsuario)
        {
            PerfilUsuario perfil = null;

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT u.IdUsuario, u.Nombre, u.FechaRegistro, ca.Nombre AS Carrera, f.IdFacultad, f.Nombre AS Facultad " +
                    "FROM Usuario u " +
                    "LEFT JOIN Carrera ca ON ca.IdCarrera = u.IdCarrera " +
                    "LEFT JOIN Facultad f ON f.IdFacultad = ca.IdFacultad " +
                    "WHERE u.IdUsuario = @id AND u.Activo = 1");
                datos.SetearParametro("@id", idUsuario);
                datos.EjecutarLectura();

                if (datos.Lector.Read())
                {
                    perfil = new PerfilUsuario();
                    perfil.IdUsuario = (int)datos.Lector["IdUsuario"];
                    perfil.Nombre = (string)datos.Lector["Nombre"];
                    perfil.FechaRegistro = (DateTime)datos.Lector["FechaRegistro"];
                    perfil.Carrera = datos.Lector["Carrera"] is DBNull ? null : (string)datos.Lector["Carrera"];
                    perfil.IdFacultad = datos.Lector["IdFacultad"] is DBNull ? (int?)null : (int)datos.Lector["IdFacultad"];
                    perfil.Facultad = datos.Lector["Facultad"] is DBNull ? null : (string)datos.Lector["Facultad"];
                }
            }
            finally
            {
                datos.CerrarConexion();
            }

            if (perfil == null)
                return null;

            // Impacto: objetos entregados y kg estimados (peso por categoria)
            datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT COUNT(*) AS Objetos, ISNULL(SUM(c.PesoEstimadoKg), 0) AS Kg " +
                    "FROM Publicacion p INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria " +
                    "WHERE p.IdUsuario = @id AND p.Estado = 2");
                datos.SetearParametro("@id", idUsuario);
                datos.EjecutarLectura();
                if (datos.Lector.Read())
                {
                    perfil.ObjetosEntregados = (int)datos.Lector["Objetos"];
                    perfil.KgEvitados = Convert.ToDecimal(datos.Lector["Kg"]);
                }
            }
            finally
            {
                datos.CerrarConexion();
            }

            // Reputacion: promedio de las valoraciones recibidas
            datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT COUNT(*) AS Cantidad, AVG(CAST(Puntaje AS DECIMAL(4,2))) AS Promedio " +
                    "FROM Valoracion WHERE IdEvaluado = @id");
                datos.SetearParametro("@id", idUsuario);
                datos.EjecutarLectura();
                if (datos.Lector.Read())
                {
                    perfil.CantidadValoraciones = (int)datos.Lector["Cantidad"];
                    perfil.PromedioValoracion = datos.Lector["Promedio"] is DBNull ? (decimal?)null : Convert.ToDecimal(datos.Lector["Promedio"]);
                }
            }
            finally
            {
                datos.CerrarConexion();
            }

            return perfil;
        }

        // Publicaciones del usuario. propio = true muestra todas; si no, solo las Disponibles.
        public List<PublicacionPerfil> ListarPublicaciones(int idUsuario, bool propio)
        {
            List<PublicacionPerfil> lista = new List<PublicacionPerfil>();

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT p.IdPublicacion, p.Titulo, p.Modalidad, p.Estado, p.FechaPublicacion, c.Nombre AS Categoria, " +
                    "(SELECT COUNT(*) FROM Solicitud s WHERE s.IdPublicacion = p.IdPublicacion AND s.Estado = 0) AS Pendientes, " +
                    "(SELECT TOP 1 u2.Nombre FROM Solicitud s2 INNER JOIN Usuario u2 ON u2.IdUsuario = s2.IdSolicitante " +
                    "   WHERE s2.IdPublicacion = p.IdPublicacion AND s2.Estado = 1) AS Receptor " +
                    "FROM Publicacion p INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria " +
                    "WHERE p.IdUsuario = @id AND (@propio = 1 OR p.Estado = 0) " +
                    "ORDER BY p.FechaPublicacion DESC, p.IdPublicacion DESC");
                datos.SetearParametro("@id", idUsuario);
                datos.SetearParametro("@propio", propio ? 1 : 0);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    PublicacionPerfil p = new PublicacionPerfil();
                    p.Id = (int)datos.Lector["IdPublicacion"];
                    p.Titulo = (string)datos.Lector["Titulo"];
                    p.Categoria = (string)datos.Lector["Categoria"];
                    p.Modalidad = (ModalidadPublicacion)(byte)datos.Lector["Modalidad"];
                    p.Estado = (EstadoPublicacion)(byte)datos.Lector["Estado"];
                    p.FechaPublicacion = (DateTime)datos.Lector["FechaPublicacion"];
                    p.SolicitudesPendientes = (int)datos.Lector["Pendientes"];
                    p.Receptor = datos.Lector["Receptor"] is DBNull ? null : (string)datos.Lector["Receptor"];
                    lista.Add(p);
                }
                return lista;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Entregas concretadas en las que participo el usuario (como dueño o como quien recibio)
        public List<MovimientoHistorial> ListarHistorial(int idUsuario)
        {
            List<MovimientoHistorial> lista = new List<MovimientoHistorial>();

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT p.IdPublicacion, p.Titulo, p.Modalidad, s.FechaEntrega, " +
                    "CASE WHEN p.IdUsuario = @id THEN us.Nombre ELSE ud.Nombre END AS Otra, v.Puntaje " +
                    "FROM Solicitud s " +
                    "INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion " +
                    "INNER JOIN Usuario ud ON ud.IdUsuario = p.IdUsuario " +
                    "INNER JOIN Usuario us ON us.IdUsuario = s.IdSolicitante " +
                    "LEFT JOIN Valoracion v ON v.IdSolicitud = s.IdSolicitud AND v.IdEvaluado = @id " +
                    "WHERE s.Estado = 1 AND p.Estado = 2 AND (p.IdUsuario = @id OR s.IdSolicitante = @id) " +
                    "ORDER BY s.FechaEntrega DESC, s.IdSolicitud DESC");
                datos.SetearParametro("@id", idUsuario);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    MovimientoHistorial m = new MovimientoHistorial();
                    m.IdPublicacion = (int)datos.Lector["IdPublicacion"];
                    m.Titulo = (string)datos.Lector["Titulo"];
                    m.Modalidad = (ModalidadPublicacion)(byte)datos.Lector["Modalidad"];
                    m.FechaEntrega = datos.Lector["FechaEntrega"] is DBNull ? (DateTime?)null : (DateTime)datos.Lector["FechaEntrega"];
                    m.OtraPersona = (string)datos.Lector["Otra"];
                    m.PuntajeRecibido = datos.Lector["Puntaje"] is DBNull ? (int?)null : Convert.ToInt32(datos.Lector["Puntaje"]);
                    lista.Add(m);
                }
                return lista;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Kg evitados por facultad (suma de lo entregado por los usuarios de cada facultad).
        // Incluye las facultades sin entregas (0 kg) para que el grafico las muestre.
        public List<ImpactoFacultad> ImpactoPorFacultad()
        {
            List<ImpactoFacultad> lista = new List<ImpactoFacultad>();

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT f.IdFacultad, f.Nombre, ISNULL(SUM(x.Kg), 0) AS Kg " +
                    "FROM Facultad f " +
                    "LEFT JOIN (" +
                    "   SELECT ca.IdFacultad, c.PesoEstimadoKg AS Kg " +
                    "   FROM Publicacion p " +
                    "   INNER JOIN Categoria c ON c.IdCategoria = p.IdCategoria " +
                    "   INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario " +
                    "   INNER JOIN Carrera ca ON ca.IdCarrera = u.IdCarrera " +
                    "   WHERE p.Estado = 2" +
                    ") x ON x.IdFacultad = f.IdFacultad " +
                    "GROUP BY f.IdFacultad, f.Nombre " +
                    "ORDER BY Kg DESC, f.Nombre");
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    ImpactoFacultad i = new ImpactoFacultad();
                    i.IdFacultad = (int)datos.Lector["IdFacultad"];
                    i.Nombre = (string)datos.Lector["Nombre"];
                    i.Kg = Convert.ToDecimal(datos.Lector["Kg"]);
                    lista.Add(i);
                }
                return lista;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}
