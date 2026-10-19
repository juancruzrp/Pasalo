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
    
        public partial class Registro : System.Web.UI.Page
        {
            protected void Page_Load(object sender, EventArgs e)
            {
                if (IsPostBack)
                    return;

                try
                {
                    CargarCarreras();
                }
                catch (Exception ex)
                {
                    Session.Add("Error", ex.ToString());
                    Response.Redirect("Error.aspx", false);
                }
            }

            // La lista sale de la tabla Carrera (así coincide con la de Publicar y se guarda el Id real)
            private void CargarCarreras()
            {
                ddlCarrera.DataSource = new PublicacionNegocio().ListarCarreras();
                ddlCarrera.DataValueField = "Id";
                ddlCarrera.DataTextField = "Nombre";
                ddlCarrera.DataBind();
                ddlCarrera.Items.Insert(0, new ListItem("Seleccioná tu carrera", "0"));
            }

            protected void btnRegistrarse_Click(object sender, EventArgs e)
            {
                try
                {
                    // Las dos contraseñas tienen que ser iguales
                    if (txtPassword.Text != txtConfirmar.Text)
                    {
                        lblError.Text = "Las contraseñas no coinciden.";
                        return;
                    }

                    Usuario usuario = new Usuario();
                    usuario.Email = txtEmail.Text;
                    usuario.Nombre = txtNombre.Text;

                    int idCarrera;
                    if (int.TryParse(ddlCarrera.SelectedValue, out idCarrera) && idCarrera > 0)
                        usuario.IdCarrera = idCarrera;

                    UsuarioNegocio negocio = new UsuarioNegocio();
                    negocio.Registrar(usuario, txtPassword.Text);
                    Response.Redirect("Login.aspx?registro=ok", false);
                }
                catch (NegocioException ex)
                {
                    lblError.Text = ex.Message;
                }
                catch (Exception ex)
                {
                    Session.Add("Error", ex.ToString());
                    Response.Redirect("Error.aspx", false);
                }
            }
        }
    }