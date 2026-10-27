// Proyecto: negocio
using System;
using System.Collections.Generic;
using acceso;
using dominio;

namespace negocio
{
    public class NotificacionNegocio
    {
        private readonly NotificacionAcceso acceso = new NotificacionAcceso();

        // Novedades del usuario, ya con la frase y el link listos para mostrar en la campanita
        public List<Notificacion> ListarNuevas(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new NegocioException("El usuario no es válido.");

            List<Notificacion> lista = acceso.ListarNuevas(idUsuario);

            foreach (Notificacion n in lista)
            {
                string titulo = n.Titulo;
                string otro = n.Persona;

                if (n.Tipo == "aceptada")
                {
                    n.Texto = otro + " aceptó tu solicitud por «" + titulo + "».";
                    n.Url = "Chat.aspx?id=" + n.IdSolicitud;               // el chat la marca como vista
                }
                else if (n.Tipo == "rechazada")
                {
                    n.Texto = "Tu solicitud por «" + titulo + "» no fue aceptada.";
                    n.Url = "Notificaciones.ashx?visto=" + n.IdSolicitud;  // la marca como vista y lleva a Mi actividad
                }
                else
                {
                    n.Texto = n.Cantidad == 1
                        ? otro + " te escribió sobre «" + titulo + "»."
                        : n.Cantidad + " mensajes nuevos de " + otro + " sobre «" + titulo + "».";
                    n.Url = "Chat.aspx?id=" + n.IdSolicitud;
                }
            }
            return lista;
        }

        public void MarcarRespuestaVista(int idSolicitud, int idUsuario)
        {
            if (idSolicitud > 0 && idUsuario > 0)
                acceso.MarcarRespuestaVista(idSolicitud, idUsuario);
        }
    }
}
