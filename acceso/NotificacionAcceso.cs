// Proyecto: acceso
using System;
using System.Collections.Generic;
using dominio;

namespace acceso
{
    // Consultas de la campanita. No hay una tabla de notificaciones: se calculan con lo que ya existe
    // (Solicitud.RespuestaVista y Mensaje.Leido dicen qué todavía no se vio).
    public class NotificacionAcceso
    {
        public List<Notificacion> ListarNuevas(int idUsuario)
        {
            List<Notificacion> lista = new List<Notificacion>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    // 1) Respondieron una solicitud MIA y todavía no lo vi
                    "SELECT CASE s.Estado WHEN 1 THEN 'aceptada' ELSE 'rechazada' END AS Tipo, " +
                    "       s.IdSolicitud, p.Titulo, u.Nombre AS Otro, 0 AS Cantidad, " +
                    "       ISNULL(s.FechaRespuesta, s.FechaSolicitud) AS Fecha " +
                    "FROM Solicitud s " +
                    "INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion " +
                    "INNER JOIN Usuario u ON u.IdUsuario = p.IdUsuario " +
                    "WHERE s.IdSolicitante = @idUsuario AND s.Estado IN (1, 2) AND s.RespuestaVista = 0 " +
                    "UNION ALL " +
                    // 2) Mensajes sin leer, agrupados por conversación
                    "SELECT 'mensaje', s.IdSolicitud, p.Titulo, u.Nombre, COUNT(*), MAX(m.FechaEnvio) " +
                    "FROM Mensaje m " +
                    "INNER JOIN Solicitud s ON s.IdSolicitud = m.IdSolicitud " +
                    "INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion " +
                    "INNER JOIN Usuario u ON u.IdUsuario = m.IdEmisor " +
                    "WHERE m.Leido = 0 AND m.IdEmisor <> @idUsuario AND s.Estado = 1 " +
                    "  AND (p.IdUsuario = @idUsuario OR s.IdSolicitante = @idUsuario) " +
                    "GROUP BY s.IdSolicitud, p.Titulo, u.Nombre " +
                    "ORDER BY Fecha DESC");
                datos.SetearParametro("@idUsuario", idUsuario);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    Notificacion n = new Notificacion();
                    n.Tipo = Convert.ToString(datos.Lector["Tipo"]);
                    n.IdSolicitud = (int)datos.Lector["IdSolicitud"];
                    n.Cantidad = (int)datos.Lector["Cantidad"];
                    n.Fecha = DateTime.SpecifyKind(Convert.ToDateTime(datos.Lector["Fecha"]), DateTimeKind.Utc);

                    n.Titulo = Convert.ToString(datos.Lector["Titulo"]);
                    n.Persona = Convert.ToString(datos.Lector["Otro"]);
                    lista.Add(n);
                }
            }
            finally
            {
                datos.CerrarConexion();
            }
            return lista;
        }

        // El solicitante ya vio que no aceptaron su solicitud
        public void MarcarRespuestaVista(int idSolicitud, int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "UPDATE Solicitud SET RespuestaVista = 1 " +
                    "WHERE IdSolicitud = @idSolicitud AND IdSolicitante = @idUsuario");
                datos.SetearParametro("@idSolicitud", idSolicitud);
                datos.SetearParametro("@idUsuario", idUsuario);
                datos.EjecutarAccion();
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}
