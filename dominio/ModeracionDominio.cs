using System;

namespace dominio
{
    // Filas que se muestran en el panel de moderación (solo datos, sin lógica).
    public class ModUsuario
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public int? IdCarrera { get; set; }
        public string Carrera { get; set; }
        public string Descripcion { get; set; }
        public int Rol { get; set; }                 // 0 = usuario, 1 = moderador
        public bool Activo { get; set; }             // false = suspendido
        public bool EmailVerificado { get; set; }
        public DateTime FechaRegistro { get; set; }  // UTC
    }

    public class ModPublicacion
    {
        public int IdPublicacion { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public int IdCategoria { get; set; }
        public string Categoria { get; set; }
        public int? IdCarrera { get; set; }
        public string Carrera { get; set; }
        public int Modalidad { get; set; }           // 0 = donación, 1 = intercambio
        public int Estado { get; set; }              // 0 disponible, 1 reservado, 2 entregado
        public bool Suspendida { get; set; }
        public int IdDueno { get; set; }
        public string Dueno { get; set; }
        public DateTime Fecha { get; set; }          // UTC
        public int Solicitudes { get; set; }
    }

    public class ModCategoria
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public decimal PesoKg { get; set; }
        public int EnUso { get; set; }               // publicaciones que la usan
    }

    public class ModFacultad
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int Carreras { get; set; }
    }

    public class ModCarrera
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public int IdFacultad { get; set; }
        public string Facultad { get; set; }
        public int EnUso { get; set; }               // usuarios + publicaciones que la usan
    }

    public class ModReporte
    {
        public int IdReporte { get; set; }
        public string Motivo { get; set; }
        public string Detalle { get; set; }
        public int Estado { get; set; }              // 0 pendiente, 1 desestimado, 2 resuelto
        public DateTime Fecha { get; set; }          // UTC
        public string Reportante { get; set; }
        public int? IdPublicacion { get; set; }
        public string Publicacion { get; set; }
        public int? IdUsuarioReportado { get; set; }
        public string UsuarioReportado { get; set; }
    }
}
