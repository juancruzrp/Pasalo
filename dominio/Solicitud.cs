using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace dominio
{
    public enum EstadoSolicitud : byte
    {
        Pendiente = 0,
        Aceptada = 1,
        Rechazada = 2
    }

    public class Solicitud
    {
        public int Id { get; set; }
        public int IdPublicacion { get; set; }
        public int IdSolicitante { get; set; }
        public string ObjetoOfrecido { get; set; }
        public EstadoSolicitud Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }
    }


    public class SolicitudListado
    {
        public int IdSolicitud { get; set; }
        public int IdPublicacion { get; set; }

        public string TituloPublicacion { get; set; }
        public string Modalidad { get; set; }
        public string ObjetoOfrecido { get; set; }

        public string NombreUsuario { get; set; }

        public EstadoSolicitud Estado { get; set; }
        public DateTime FechaSolicitud { get; set; }

        public string EstadoTexto
        {
            get
            {
                switch (Estado)
                {
                    case EstadoSolicitud.Aceptada:
                        return "Aceptada";

                    case EstadoSolicitud.Rechazada:
                        return "Rechazada";

                    default:
                        return "Pendiente";
                }
            }
        }

        public string FechaTexto
        {
            get
            {
                DateTime fecha = DateTime.SpecifyKind(
                    FechaSolicitud, DateTimeKind.Utc);

                return fecha.ToLocalTime()
                    .ToString("dd/MM/yyyy HH:mm");
            }
      }



        public int? MotivoRespuesta { get; set; }

        // Lo que ve quien hizo el pedido (pestaña Enviadas)
        public string MensajeSolicitante
        {
            get
            {
                if (Estado != EstadoSolicitud.Rechazada) return null;

                return MotivoRespuesta == 2
                    ? "El dueño eligió a otra persona para este artículo."
                    : "El dueño rechazó tu solicitud.";
            }
        }

        // Lo que ve el dueño (pestaña Recibidas)
        public string MensajeDueno
        {
            get
            {
                if (Estado != EstadoSolicitud.Rechazada) return null;

                return MotivoRespuesta == 2
                    ? "Se rechazó automáticamente al aceptar a otra persona."
                    : "Rechazaste esta solicitud.";
            }
        }



        public bool ConfirmoDuenio { get; set; }
        public bool ConfirmoSolicitante { get; set; }
        public DateTime? FechaEntrega { get; set; }

        public bool EntregaFinalizada
        {
            get { return FechaEntrega.HasValue; }
        }

        public string EstadoEntregaTexto
        {
            get
            {
                if (EntregaFinalizada)
                    return "Entrega completada";

                if (ConfirmoDuenio && ConfirmoSolicitante)
                    return "Entrega confirmada";

                if (ConfirmoDuenio || ConfirmoSolicitante)
                    return "Esperando la confirmación de la otra persona";

                return "Pendiente de entrega";
            }
        }



    }



}
