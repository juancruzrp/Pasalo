using System;

namespace negocio
{
    // Error de regla de negocio con un mensaje listo para mostrarle al usuario.
    // La pagina la atrapa y muestra el mensaje; cualquier otra excepcion es un error "real" (va a Error.aspx).
    public class NegocioException : Exception
    {
        public NegocioException(string mensaje) : base(mensaje)
        {
        }
    }
}
