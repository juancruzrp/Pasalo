using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace dominio
    {
        // Los valores coinciden con los TINYINT de la base de datos
        public enum EstadoPublicacion : byte { Disponible = 0, Reservado = 1, Entregado = 2 }

        public enum ModalidadPublicacion : byte { Donacion = 0, Intercambio = 1 }

        public class FotoPublicacion
        {
            public int Id { get; set; }
            public string Ruta { get; set; }
            public int Orden { get; set; }
        }

        public class Publicacion
        {
            public int Id { get; set; }
            public Usuario Dueno { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public Categoria Categoria { get; set; }
            public Carrera Carrera { get; set; }                  // null = "Todas las carreras"
            public ModalidadPublicacion? Modalidad { get; set; }  // null = todavía no eligió
            public EstadoPublicacion Estado { get; set; }
            public DateTime FechaPublicacion { get; set; }
            public List<FotoPublicacion> Fotos { get; set; } = new List<FotoPublicacion>();
        }
    }

