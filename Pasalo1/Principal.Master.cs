using dominio;
using negocio;
using System;
using System.Web;
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

            // Se vuelve a leer de la base si la cuenta sigue activa y qué rol tiene: así una suspensión
            // o un cambio de rol hecho por un moderador vale en el siguiente clic, sin esperar a otro login.
            Usuario actual = (Usuario)Session["Usuario"];
            bool activo;
            int rol;
            if (!new ModeracionNegocio().LeerEstado(actual.IdUsuario, out activo, out rol) || !activo)
            {
                Session.Clear();
                Session.Abandon();
                Response.Redirect("Login.aspx?suspendida=1", true);
            }
            actual.Rol = rol;

            // Páginas privadas: que el navegador no las guarde. Así, después de "Salir", el botón Atrás
            // no muestra datos de la sesión anterior.
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();
            Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
        }

        // Los usa el encabezado (Principal.Master) con <%: PrimerNombre %> e <%: InicialUsuario %>
        protected string NombreUsuario
        {
            get
            {
                Usuario u = Session["Usuario"] as Usuario;
                return u == null ? "" : u.Nombre;
            }
        }

        // Los usa el menú (Principal.Master) para mostrar "Moderación" solo a moderadores
        protected bool EsModerador
        {
            get
            {
                Usuario u = Session["Usuario"] as Usuario;
                return u != null && u.Rol == 1;
            }
        }

        // Solo el primer nombre, para el saludo ("Hola, Ivana")
        protected string PrimerNombre
        {
            get
            {
                string n = NombreUsuario.Trim();
                int i = n.IndexOf(' ');
                return i < 0 ? n : n.Substring(0, i);
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
