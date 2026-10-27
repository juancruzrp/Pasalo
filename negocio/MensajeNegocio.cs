// Proyecto: negocio
using System;
using System.Collections.Generic;
using acceso;
using dominio;

namespace negocio
{
    public class MensajeNegocio
    {
        public const int LargoMaximo = 1000;   // coincide con Mensaje.Texto NVARCHAR(1000)

        private readonly MensajeAcceso acceso = new MensajeAcceso();

        public List<ConversacionChat> ListarConversaciones(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new NegocioException("El usuario no es válido.");
            return acceso.ListarConversaciones(idUsuario);
        }

        // Solo los dos participantes pueden leer una conversación
        public List<MensajeChat> ListarMensajes(int idSolicitud, int idUsuario)
        {
            if (idSolicitud <= 0 || idUsuario <= 0 || !acceso.EsParticipante(idSolicitud, idUsuario))
                throw new NegocioException("No tenés acceso a esta conversación.");
            return acceso.ListarMensajes(idSolicitud);
        }

        public void Enviar(int idSolicitud, int idEmisor, string texto)
        {
            texto = (texto ?? "").Trim();

            if (texto.Length == 0)
                throw new NegocioException("Escribí un mensaje antes de enviar.");

            if (texto.Length > LargoMaximo)
                throw new NegocioException("El mensaje no puede superar los " + LargoMaximo + " caracteres.");

            if (idSolicitud <= 0 || idEmisor <= 0 || !acceso.Enviar(idSolicitud, idEmisor, texto))
                throw new NegocioException("No se pudo enviar: el chat solo está disponible para solicitudes aceptadas.");
        }

        // Llamar al abrir la conversación: borra el "sin leer" (solo si participa)
        public void MarcarComoVista(int idSolicitud, int idUsuario)
        {
            if (idSolicitud > 0 && idUsuario > 0 && acceso.EsParticipante(idSolicitud, idUsuario))
                acceso.MarcarComoVista(idSolicitud, idUsuario);
        }
    }
}
