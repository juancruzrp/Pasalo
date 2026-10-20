using System;

namespace dominio
{
    // Datos que muestra la pantalla Perfil (cabecera + impacto ambiental)
    public class PerfilUsuario
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Carrera { get; set; }
        public int? IdFacultad { get; set; }
        public string Facultad { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Impacto: solo cuentan las publicaciones que ya figuran como Entregado
        public int ObjetosEntregados { get; set; }
        public decimal KgEvitados { get; set; }

        // Reputacion: sale de la tabla Valoracion
        public int CantidadValoraciones { get; set; }
        public decimal? PromedioValoracion { get; set; }
    }

    // Una fila de la pestaña "Mis publicaciones"
    public class PublicacionPerfil
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Categoria { get; set; }
        public ModalidadPublicacion Modalidad { get; set; }
        public EstadoPublicacion Estado { get; set; }
        public DateTime FechaPublicacion { get; set; }
        public int SolicitudesPendientes { get; set; }
        public string Receptor { get; set; }          // a quien se le acepto la solicitud (si hay)
    }

    // Una fila de la pestaña "Historial": un intercambio/donacion ya concretado
    public class MovimientoHistorial
    {
        public int IdPublicacion { get; set; }
        public string Titulo { get; set; }
        public ModalidadPublicacion Modalidad { get; set; }
        public DateTime? FechaEntrega { get; set; }
        public string OtraPersona { get; set; }
        public int? PuntajeRecibido { get; set; }
    }

    // Una barra del grafico "Impacto por facultad"
    public class ImpactoFacultad
    {
        public int IdFacultad { get; set; }
        public string Nombre { get; set; }
        public decimal Kg { get; set; }
    }
}
