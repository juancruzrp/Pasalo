using System;

namespace Pasalo1
{
    public partial class ErrorPagina : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // El detalle técnico se guarda en sesión solo para depurar; nunca se muestra al usuario.
            Session.Remove("Error");
            Response.StatusCode = 500;
            Response.TrySkipIisCustomErrors = true;
        }
    }
}
