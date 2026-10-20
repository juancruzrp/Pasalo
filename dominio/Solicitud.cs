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
}
