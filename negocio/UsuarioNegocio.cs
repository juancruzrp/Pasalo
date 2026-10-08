using System;
using System.Net.Mail;
using acceso;
using dominio;

namespace negocio
{
    public class UsuarioNegocio
    {
       
        private const string DominioInstitucional = "@alumnos.frgp.utn.edu.ar";
        private const int LargoMinimoPassword = 6;

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

            if (password.Length < LargoMinimoPassword)
                throw new NegocioException("La contraseña debe tener al menos " + LargoMinimoPassword + " caracteres.");

            UsuarioAcceso usuarioAcceso = new UsuarioAcceso();

            if (usuarioAcceso.ExisteEmail(email))
                throw new NegocioException("El correo ya se encuentra en uso.");

            nuevo.Email = email;
            nuevo.Nombre = nombre;
            nuevo.PasswordHash = PasswordHelper.Hash(password);   // nunca se guarda la contraseña en texto plano

            return usuarioAcceso.Agregar(nuevo);
        }

        // Inicio de sesion. Devuelve el usuario o lanza NegocioException.
        // Mismo mensaje si falla el correo o la contraseña, para no revelar que correos estan registrados.
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
