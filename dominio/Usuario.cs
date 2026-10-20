using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace dominio
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }
        public string PasswordHash { get; set; }
        public int? IdCarrera { get; set; }
        public int Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string CarreraNombre { get; set; }   // solo para mostrar (no se guarda en la tabla Usuario)
    }
}
