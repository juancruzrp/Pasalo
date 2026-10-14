using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pasalo1
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            // Cerrar sesión: el menú manda a Login.aspx?salir=1
            if (Request.QueryString["salir"] == "1")
            {
                Session.Clear();
                Session.Abandon();
                Response.Redirect("Login.aspx", false);
                return;
            }

            // Si ya hay sesión iniciada, no tiene sentido mostrar el login
            if (Session["Usuario"] != null)
            {
                Response.Redirect("Default.aspx", false);
                return;
            }

            // Viene de Registro.aspx
            if (Request.QueryString["registro"] == "ok")
                pnlExito.Visible = true;
        }

        protected void btnIngresar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario usuario = new UsuarioNegocio().Loguear(txtEmail.Text, txtPassword.Text);

                usuario.PasswordHash = null;      // el hash no tiene por qué viajar en la sesión
                Session["Usuario"] = usuario;

                Response.Redirect("Default.aspx", false);
            }
            catch (NegocioException ex)
            {
                lblError.Text = ex.Message;
                pnlError.Visible = true;
            }
            catch (Exception ex)
            {
                Session.Add("Error", ex.ToString());
                Response.Redirect("Error.aspx", false);
            }
        }
    }
}