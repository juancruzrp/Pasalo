using dominio;
using System;
using System.Web.UI;

namespace Pasalo1
{
    public partial class Principal : System.Web.UI.MasterPage
    {
        // Toda página que use esta master exige sesión iniciada; si no hay, va al login.
        protected void Page_Init(object sender, EventArgs e)
        {
            if (Session["Usuario"] == null)
                Response.Redirect("Login.aspx", true);
        }

        // Los usa el encabezado (Principal.Master) con <%: NombreUsuario %>
        protected string NombreUsuario
        {
            get
            {
                Usuario u = Session["Usuario"] as Usuario;
                return u == null ? "" : u.Nombre;
            }
        }

        protected string InicialUsuario
        {
            get
            {
                string n = NombreUsuario;
                return n.Length == 0 ? "" : n.Substring(0, 1).ToUpper();
            }
        }
    }
}
