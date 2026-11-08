using acceso;
using dominio;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace negocio
{
    public class PerfilNegocio
    {
        private const int LargoMaximoNombre = 100;
        private const int LargoMaximoDescripcion = 200;

        private readonly PerfilAcceso _acceso = new PerfilAcceso();
        private readonly UsuarioEdicionAcceso _edicion = new UsuarioEdicionAcceso();

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

        // ---------- Edición del perfil propio ----------

        // Guarda nombre, carrera y descripción. Devuelve el nombre ya limpio (para actualizar la sesión).
        public string ActualizarPerfil(int idUsuario, string nombre, int idCarrera, string descripcion)
        {
            nombre = Limpiar(nombre);
            descripcion = Limpiar(descripcion);

            if (nombre == "")
                throw new NegocioException("El nombre no puede quedar vacío.");
            if (nombre.Length > LargoMaximoNombre)
                throw new NegocioException("El nombre no puede superar los " + LargoMaximoNombre + " caracteres.");
            if (idCarrera <= 0)
                throw new NegocioException("Seleccioná tu carrera.");
            if (descripcion.Length > LargoMaximoDescripcion)
                throw new NegocioException("La descripción no puede superar los " + LargoMaximoDescripcion + " caracteres.");

            try
            {
                _edicion.ActualizarPerfil(idUsuario, nombre, idCarrera, descripcion == "" ? null : descripcion);
            }
            catch (SqlException ex) when (ex.Number == 547)   // clave foránea: la carrera no existe
            {
                throw new NegocioException("La carrera elegida no es válida.");
            }

            return nombre;
        }

        // Cambio de contraseña estando con la sesión iniciada: exige la actual.
        public void CambiarPassword(int idUsuario, string actual, string nueva, string repetida)
        {
            if (string.IsNullOrEmpty(actual))
                throw new NegocioException("Ingresá tu contraseña actual.");
            if (nueva != repetida)
                throw new NegocioException("Las contraseñas nuevas no coinciden.");

            UsuarioNegocio.ValidarPassword(nueva);

            string hashActual = _edicion.ObtenerPasswordHash(idUsuario);
            if (hashActual == null || !PasswordHelper.Verificar(actual, hashActual))
                throw new NegocioException("La contraseña actual no es correcta.");
            if (actual == nueva)
                throw new NegocioException("La contraseña nueva tiene que ser distinta de la actual.");

            _edicion.CambiarPasswordHash(idUsuario, PasswordHelper.Hash(nueva));
        }

        // Quita espacios de más y saltos de línea (la descripción se muestra como un solo párrafo)
        private static string Limpiar(string texto)
        {
            return Regex.Replace(texto ?? "", @"\s+", " ").Trim();
        }
    }
}