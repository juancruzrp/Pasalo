using System;
using System.Web.UI.WebControls;
using dominio;
using negocio;

namespace Pasalo1
{
    // Edición del perfil propio: nombre, carrera, descripción y contraseña.
    // Solo se edita el usuario de la sesión: el id nunca viene de la dirección ni de un campo del formulario.
    public partial class EditarPerfil : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Usuario usuario = Session["Usuario"] as Usuario;
            if (usuario == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (IsPostBack)
                return;

            try
            {
                // Mismas carreras que en el registro
                ddlCarrera.DataSource = new PublicacionNegocio().ListarCarreras();
                ddlCarrera.DataTextField = "Nombre";
                ddlCarrera.DataValueField = "Id";
                ddlCarrera.DataBind();
                ddlCarrera.Items.Insert(0, new ListItem("Seleccioná tu carrera", "0"));

                PerfilUsuario datos = new PerfilNegocio().ObtenerPerfil(usuario.IdUsuario);
                if (datos == null)
                {
                    Response.Redirect("Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                txtNombre.Text = datos.Nombre;
                txtDescripcion.Text = datos.Descripcion;
                lblCorreo.Text = usuario.Email;

                if (datos.IdCarrera.HasValue && ddlCarrera.Items.FindByValue(datos.IdCarrera.Value.ToString()) != null)
                    ddlCarrera.SelectedValue = datos.IdCarrera.Value.ToString();

                // Los avisos de "guardado" llegan por la dirección: se vuelve a la página después de guardar
                // para que la barra de arriba muestre el nombre nuevo y F5 no reenvíe el formulario.
                pnlOkDatos.Visible = Request.QueryString["guardado"] == "ok";
                pnlOkClave.Visible = Request.QueryString["clave"] == "ok";
            }
            catch (Exception ex)
            {
                IrAError(ex);
            }
        }

        protected void btnGuardarDatos_Click(object sender, EventArgs e)
        {
            Usuario usuario = Session["Usuario"] as Usuario;
            if (usuario == null) { IrALogin(); return; }

            pnlErrorDatos.Visible = false;

            int idCarrera;
            int.TryParse(ddlCarrera.SelectedValue, out idCarrera);

            try
            {
                string nombreLimpio = new PerfilNegocio().ActualizarPerfil(
                    usuario.IdUsuario, txtNombre.Text, idCarrera, txtDescripcion.Text);

                // La sesión guarda el nombre y la carrera: se actualizan para que el resto del sitio los muestre
                usuario.Nombre = nombreLimpio;
                usuario.IdCarrera = idCarrera;

                Response.Redirect("EditarPerfil.aspx?guardado=ok", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (NegocioException ex)
            {
                lblErrorDatos.Text = ex.Message;
                pnlErrorDatos.Visible = true;
            }
            catch (Exception ex)
            {
                IrAError(ex);
            }
        }

        protected void btnCambiarClave_Click(object sender, EventArgs e)
        {
            Usuario usuario = Session["Usuario"] as Usuario;
            if (usuario == null) { IrALogin(); return; }

            pnlErrorClave.Visible = false;

            try
            {
                new PerfilNegocio().CambiarPassword(
                    usuario.IdUsuario, txtClaveActual.Text, txtClaveNueva.Text, txtClaveRepetida.Text);

                Response.Redirect("EditarPerfil.aspx?clave=ok", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (NegocioException ex)
            {
                lblErrorClave.Text = ex.Message;
                pnlErrorClave.Visible = true;
            }
            catch (Exception ex)
            {
                IrAError(ex);
            }
        }

        private void IrALogin()
        {
            Response.Redirect("Login.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }

        private void IrAError(Exception ex)
        {
            Session.Add("Error", ex.ToString());
            Response.Redirect("Error.aspx", false);
            Context.ApplicationInstance.CompleteRequest();
        }
    }
}
