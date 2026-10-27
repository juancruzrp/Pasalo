// Proyecto: dominio
using System;

namespace dominio
{
    // Un mensaje de una conversación (tabla Mensaje).
    public class MensajeChat
    {
        public int IdMensaje { get; set; }
        public int IdEmisor { get; set; }
        public string Texto { get; set; }

        // Se guarda en UTC (SYSUTCDATETIME en la base); la página lo convierte a hora local al mostrarlo
        public DateTime FechaEnvio { get; set; }
    }

    // Una conversación = una solicitud ACEPTADA, vista desde el usuario que mira.
    // "OtraPersona" es siempre la otra parte (si soy el dueño, el solicitante; si soy el solicitante, el dueño).
    public class ConversacionChat
    {
        public int IdSolicitud { get; set; }
        public int IdPublicacion { get; set; }
        public string TituloPublicacion { get; set; }
        public int IdOtraPersona { get; set; }
        public string NombreOtraPersona { get; set; }
        public string UltimoTexto { get; set; }        // null si todavía no hay mensajes
        public DateTime? UltimaFecha { get; set; }     // UTC; null si todavía no hay mensajes
        public int NoLeidos { get; set; }              // mensajes de la otra persona que todavía no abrí
    }

    // Una novedad para la campanita del menú.
    //   Tipo: "aceptada" (aceptaron mi solicitud), "rechazada" (no la aceptaron) o "mensaje" (mensajes sin leer)
    public class Notificacion
    {
        public string Tipo { get; set; }
        public int IdSolicitud { get; set; }
        public string Titulo { get; set; }             // artículo de la solicitud
        public string Persona { get; set; }            // la otra persona (quien aceptó / quien escribió)
        public string Texto { get; set; }              // frase lista para mostrar
        public string Url { get; set; }                // a dónde lleva al tocarla
        public int Cantidad { get; set; }              // solo "mensaje": cuántos mensajes nuevos
        public DateTime Fecha { get; set; }            // UTC
    }
}
