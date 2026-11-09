using System;

namespace Pasalo1
{
    public partial class ErrorPagina : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // El detalle técnico se guarda en sesión solo para depurar; nunca se muestra al usuario.
            Session.Remove("Error");

            // ?c= solo elige entre textos fijos (lista cerrada): lo que venga en la URL nunca se imprime.
            switch (Request.QueryString["c"])
            {
                case "404":
                    Response.StatusCode = 404;
                    lblTitulo.Text = "No encontramos esa página";
                    lblMensaje.Text = "El enlace no existe o ya no está disponible. Probá desde el catálogo.";
                    break;

                case "texto":
                    Response.StatusCode = 400;
                    lblTitulo.Text = "Hay caracteres que no se pueden enviar";
                    lblMensaje.Text = "Tu texto tiene símbolos como < o > seguidos de letras, que no se aceptan por seguridad. " +
                                      "Volvé atrás y sacalos.";
                    break;

                default:
                    Response.StatusCode = 500;
                    break;
            }
            Response.TrySkipIisCustomErrors = true;
        }
    }
}
