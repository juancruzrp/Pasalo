using System.Collections.Generic;
using acceso;
using dominio;

namespace negocio
{
    public class PerfilNegocio
    {
        private readonly PerfilAcceso _acceso = new PerfilAcceso();

        // null si el usuario no existe
        public PerfilUsuario ObtenerPerfil(int idUsuario)
        {
            return _acceso.ObtenerPerfil(idUsuario);
        }

        // El dueño ve todas sus publicaciones; el resto de la gente solo las disponibles.
        public List<PublicacionPerfil> ListarPublicaciones(int idUsuario, int idUsuarioQueMira)
        {
            return _acceso.ListarPublicaciones(idUsuario, idUsuario == idUsuarioQueMira);
        }

        // El historial es privado: solo lo ve el propio usuario.
        public List<MovimientoHistorial> ListarHistorial(int idUsuario, int idUsuarioQueMira)
        {
            if (idUsuario != idUsuarioQueMira)
                return new List<MovimientoHistorial>();

            return _acceso.ListarHistorial(idUsuario);
        }

        public List<ImpactoFacultad> ImpactoPorFacultad()
        {
            return _acceso.ImpactoPorFacultad();
        }
    }
}
