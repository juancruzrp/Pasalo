using System;
using System.Configuration;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using acceso;
using dominio;

namespace negocio
{
    // Verificación del correo (al registrarse) y recuperación de contraseña.
    // Las dos usan el mismo mecanismo: un enlace con un código aleatorio, de un solo uso y con vencimiento.
    public class VerificacionNegocio
    {
        private const byte TipoVerificacion = 1;
        private const byte TipoRecuperacion = 2;
        private const int HorasVerificacion = 24;
        private const int MinutosRecuperacion = 60;
        private const int MinutosEntreEnvios = 2;

        private readonly UsuarioAcceso _usuarios = new UsuarioAcceso();
        private readonly TokenAcceso _tokens = new TokenAcceso();

        // ================= Verificación del correo =================

        public void EnviarVerificacion(Usuario u)
        {
            string token = NuevoToken(u.IdUsuario, TipoVerificacion, DateTime.UtcNow.AddHours(HorasVerificacion));
            string enlace = Enlace("VerificarEmail.aspx", token);

            string cuerpo =
                "<p>Hola " + WebUtility.HtmlEncode(u.Nombre) + ",</p>" +
                "<p>Para activar tu cuenta en PÁSALO confirmá tu correo con este enlace:</p>" +
                "<p><a href=\"" + enlace + "\">Confirmar mi cuenta</a></p>" +
                "<p>El enlace vence en " + HorasVerificacion + " horas. Si no creaste esta cuenta, ignorá este mensaje.</p>";

            new EmailServicio().Enviar(u.Email, "Confirmá tu cuenta en PÁSALO", cuerpo, enlace);
        }

        // true si el enlace era valido
        public bool ConfirmarCorreo(string token)
        {
            return FormatoValido(token) && _tokens.ConfirmarVerificacion(Hash(token));
        }

        // Si el correo no existe, ya está verificado o se pidió hace instantes, no hace nada ni lo dice
  
        public void ReenviarVerificacion(string email)
        {
            Usuario u = BuscarUsuario(email);
            if (u == null || !u.Activo || u.EmailVerificado)
                return;
            if (_tokens.CreadoHaceMenosDe(u.IdUsuario, TipoVerificacion, MinutosEntreEnvios))
                return;

            EnviarVerificacion(u);
        }

        // ================= Recuperación de contraseña =================

        public void SolicitarRecuperacion(string email)
        {
            Usuario u = BuscarUsuario(email);
            if (u == null || !u.Activo)
                return;
            if (_tokens.CreadoHaceMenosDe(u.IdUsuario, TipoRecuperacion, MinutosEntreEnvios))
                return;

            string token = NuevoToken(u.IdUsuario, TipoRecuperacion, DateTime.UtcNow.AddMinutes(MinutosRecuperacion));
            string enlace = Enlace("RecuperarPassword.aspx", token);

            string cuerpo =
                "<p>Hola " + WebUtility.HtmlEncode(u.Nombre) + ",</p>" +
                "<p>Pediste crear una contraseña nueva para tu cuenta de PÁSALO. Hacé clic acá:</p>" +
                "<p><a href=\"" + enlace + "\">Crear una contraseña nueva</a></p>" +
                "<p>El enlace vence en " + MinutosRecuperacion + " minutos. Si no lo pediste vos, ignorá este mensaje: tu contraseña no cambia.</p>";

            new EmailServicio().Enviar(u.Email, "Recuperá tu contraseña de PÁSALO", cuerpo, enlace);
        }

        // ¿El enlace sirve todavía? (para decidir si se muestra el formulario)
        public bool TokenRecuperacionValido(string token)
        {
            return FormatoValido(token) && _tokens.ExisteVigente(Hash(token), TipoRecuperacion);
        }

        public void RestablecerPassword(string token, string passwordNueva)
        {
            UsuarioNegocio.ValidarPassword(passwordNueva);

            if (!FormatoValido(token) || !_tokens.RestablecerPassword(Hash(token), PasswordHelper.Hash(passwordNueva)))
                throw new NegocioException("El enlace no es válido o ya venció. Pedí uno nuevo.");
        }

        // ================= Auxiliares =================

        private Usuario BuscarUsuario(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;
            return _usuarios.ObtenerPorEmail(email.Trim().ToLower());
        }

        // Anula los enlaces anteriores, genera uno nuevo y guarda solo su hash. Devuelve el código real.
        private string NuevoToken(int idUsuario, byte tipo, DateTime vencimientoUtc)
        {
            _tokens.InvalidarPendientes(idUsuario, tipo);
            string token = GenerarToken();
            _tokens.Insertar(idUsuario, tipo, Hash(token), vencimientoUtc);
            return token;
        }

        private static string Enlace(string pagina, string token)
        {
            string urlBase = (ConfigurationManager.AppSettings["UrlBase"] ?? "").TrimEnd('/');
            if (urlBase == "")
                throw new InvalidOperationException("Falta la clave UrlBase en el Web.config (por ejemplo http://localhost:44387).");

            return urlBase + "/" + pagina + "?token=" + token;
        }

        private static bool FormatoValido(string token)
        {
            return !string.IsNullOrEmpty(token) && Regex.IsMatch(token, "^[0-9a-f]{64}$");
        }

        private static string GenerarToken()
        {
            byte[] bytes = new byte[32];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                rng.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLowerInvariant();   // 64 caracteres hexadecimales
        }

        
        private static string Hash(string token)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] h = sha.ComputeHash(Encoding.UTF8.GetBytes(token));
                return BitConverter.ToString(h).Replace("-", "").ToLowerInvariant();
            }
        }
    }
}