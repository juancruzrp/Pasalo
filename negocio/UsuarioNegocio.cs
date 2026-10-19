using System;
using System.Net.Mail;
using acceso;
using dominio;

namespace negocio
{
    public class UsuarioNegocio
    {

        private const string DominioInstitucional = "@alumnos.frgp.utn.edu.ar";
        private const int LargoMinimoPassword = 8;   

        // RF1 - Registrar usuario. Devuelve el IdUsuario creado.
        // Lanza NegocioException con el mensaje a mostrar si no se cumple alguna regla.
        public int Registrar(Usuario nuevo, string password)
        {
            string email = (nuevo.Email ?? "").Trim().ToLower();
            string nombre = (nuevo.Nombre ?? "").Trim();

            if (email == "" || nombre == "" || string.IsNullOrEmpty(password))
                throw new NegocioException("Completá correo, nombre y contraseña.");

            if (nombre.Length > 100)
                throw new NegocioException("El nombre no puede superar los 100 caracteres.");

            if (!EsEmailInstitucional(email))
                throw new NegocioException("El correo ingresado no corresponde a la institución.");

            // La carrera es obligatoria ("Otra carrera" cubre a quien no figure en la lista)
            if (nuevo.IdCarrera == null)
                throw new NegocioException("Seleccioná tu carrera.");

            if (password.Length < LargoMinimoPassword)
                throw new NegocioException("La contraseña debe tener al menos " + LargoMinimoPassword + " caracteres.");

            UsuarioAcceso usuarioAcceso = new UsuarioAcceso();

            if (usuarioAcceso.ExisteEmail(email))
                throw new NegocioException("El correo ya se encuentra en uso.");

            nuevo.Email = email;
            nuevo.Nombre = nombre;
            nuevo.PasswordHash = PasswordHelper.Hash(password);   
            return usuarioAcceso.Agregar(nuevo);
        }

        // Inicio de sesion. Devuelve el usuario o lanza NegocioException.
       
        public Usuario Loguear(string email, string password)
        {
            email = (email ?? "").Trim().ToLower();

            if (email == "" || string.IsNullOrEmpty(password))
                throw new NegocioException("Completá correo y contraseña.");

            Usuario usuario = new UsuarioAcceso().ObtenerPorEmail(email);

            if (usuario == null || !PasswordHelper.Verificar(password, usuario.PasswordHash))
                throw new NegocioException("Correo o contraseña incorrectos.");

            if (!usuario.Activo)
                throw new NegocioException("Tu cuenta está inactiva. Contactá a un administrador.");

            return usuario;
        }

        private bool EsEmailInstitucional(string email)
        {
            if (!email.EndsWith(DominioInstitucional))
                return false;

            // Que tenga algo antes del @ y sea un correo bien formado
            try
            {
                MailAddress direccion = new MailAddress(email);
                return direccion.Address == email;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}
