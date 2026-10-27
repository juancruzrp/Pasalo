// Proyecto: acceso
using System;
using System.Collections.Generic;
using dominio;

namespace acceso
{
    public class MensajeAcceso
    {
        // Conversaciones del usuario: solicitudes ACEPTADAS (Estado = 1) donde es el dueño o el solicitante.
        // Para cada una trae a la otra persona, el último mensaje (si hay) y cuántos mensajes tiene sin leer.
        public List<ConversacionChat> ListarConversaciones(int idUsuario)
        {
            List<ConversacionChat> lista = new List<ConversacionChat>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT s.IdSolicitud, p.IdPublicacion, p.Titulo, " +
                    "       u.IdUsuario AS IdOtra, u.Nombre AS NombreOtra, " +
                    "       m.Texto AS UltimoTexto, m.FechaEnvio AS UltimaFecha, " +
                    "       (SELECT COUNT(*) FROM Mensaje n WHERE n.IdSolicitud = s.IdSolicitud " +
                    "           AND n.Leido = 0 AND n.IdEmisor <> @idUsuario) AS NoLeidos " +
                    "FROM Solicitud s " +
                    "INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion " +
                    "INNER JOIN Usuario u ON u.IdUsuario = " +
                    "    CASE WHEN p.IdUsuario = @idUsuario THEN s.IdSolicitante ELSE p.IdUsuario END " +
                    "OUTER APPLY (SELECT TOP 1 Texto, FechaEnvio FROM Mensaje " +
                    "             WHERE IdSolicitud = s.IdSolicitud " +
                    "             ORDER BY FechaEnvio DESC, IdMensaje DESC) m " +
                    "WHERE s.Estado = 1 AND (p.IdUsuario = @idUsuario OR s.IdSolicitante = @idUsuario) " +
                    "ORDER BY ISNULL(m.FechaEnvio, s.FechaSolicitud) DESC");
                datos.SetearParametro("@idUsuario", idUsuario);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    ConversacionChat c = new ConversacionChat();
                    c.IdSolicitud = (int)datos.Lector["IdSolicitud"];
                    c.IdPublicacion = (int)datos.Lector["IdPublicacion"];
                    c.TituloPublicacion = Convert.ToString(datos.Lector["Titulo"]);
                    c.IdOtraPersona = (int)datos.Lector["IdOtra"];
                    c.NombreOtraPersona = Convert.ToString(datos.Lector["NombreOtra"]);
                    c.NoLeidos = (int)datos.Lector["NoLeidos"];
                    if (datos.Lector["UltimoTexto"] != DBNull.Value)
                    {
                        c.UltimoTexto = Convert.ToString(datos.Lector["UltimoTexto"]);
                        c.UltimaFecha = DateTime.SpecifyKind(Convert.ToDateTime(datos.Lector["UltimaFecha"]), DateTimeKind.Utc);
                    }
                    lista.Add(c);
                }
            }
            finally
            {
                datos.CerrarConexion();
            }
            return lista;
        }

        // ¿El usuario participa (como dueño o solicitante) de esa solicitud, y está aceptada?
        public bool EsParticipante(int idSolicitud, int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT COUNT(*) FROM Solicitud s " +
                    "INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion " +
                    "WHERE s.IdSolicitud = @idSolicitud AND s.Estado = 1 " +
                    "  AND (p.IdUsuario = @idUsuario OR s.IdSolicitante = @idUsuario)");
                datos.SetearParametro("@idSolicitud", idSolicitud);
                datos.SetearParametro("@idUsuario", idUsuario);
                return Convert.ToInt32(datos.EjecutarScalar()) > 0;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public List<MensajeChat> ListarMensajes(int idSolicitud)
        {
            List<MensajeChat> lista = new List<MensajeChat>();
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT IdMensaje, IdEmisor, Texto, FechaEnvio FROM Mensaje " +
                    "WHERE IdSolicitud = @idSolicitud ORDER BY FechaEnvio, IdMensaje");
                datos.SetearParametro("@idSolicitud", idSolicitud);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    MensajeChat m = new MensajeChat();
                    m.IdMensaje = (int)datos.Lector["IdMensaje"];
                    m.IdEmisor = (int)datos.Lector["IdEmisor"];
                    m.Texto = Convert.ToString(datos.Lector["Texto"]);
                    m.FechaEnvio = DateTime.SpecifyKind(Convert.ToDateTime(datos.Lector["FechaEnvio"]), DateTimeKind.Utc);
                    lista.Add(m);
                }
            }
            finally
            {
                datos.CerrarConexion();
            }
            return lista;
        }

        // Inserta el mensaje SOLO si la solicitud está aceptada y el emisor participa de ella
        // (la regla viaja dentro del mismo INSERT, así no hay forma de saltearla).
        // Devuelve true si se guardó. Nace con Leido = 0: para la otra persona es una novedad.
        public bool Enviar(int idSolicitud, int idEmisor, string texto)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "INSERT INTO Mensaje (IdSolicitud, IdEmisor, Texto) " +
                    "SELECT s.IdSolicitud, @idEmisor, @texto " +
                    "FROM Solicitud s " +
                    "INNER JOIN Publicacion p ON p.IdPublicacion = s.IdPublicacion " +
                    "WHERE s.IdSolicitud = @idSolicitud AND s.Estado = 1 " +
                    "  AND (p.IdUsuario = @idEmisor OR s.IdSolicitante = @idEmisor); " +
                    "SELECT @@ROWCOUNT;");
                datos.SetearParametro("@idSolicitud", idSolicitud);
                datos.SetearParametro("@idEmisor", idEmisor);
                datos.SetearParametro("@texto", texto);
                return Convert.ToInt32(datos.EjecutarScalar()) > 0;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Al abrir una conversación: lo que me escribió la otra persona pasa a "leído"
        // y, si yo soy el solicitante, también queda vista la notificación de "aceptaron tu solicitud".
        public void MarcarComoVista(int idSolicitud, int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "UPDATE Mensaje SET Leido = 1 " +
                    "WHERE IdSolicitud = @idSolicitud AND IdEmisor <> @idUsuario AND Leido = 0; " +
                    "UPDATE Solicitud SET RespuestaVista = 1 " +
                    "WHERE IdSolicitud = @idSolicitud AND IdSolicitante = @idUsuario AND RespuestaVista = 0;");
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
