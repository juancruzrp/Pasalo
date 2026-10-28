using System;
using negocio;

namespace Pasalo1
{
    public partial class VerificarEmail : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            // Con token (viene del correo): botón para confirmar. Sin token: solo el formulario de reenvío.
            if (string.IsNullOrEmpty(Request.QueryString["token"]))
                pnlReenviar.Visible = true;
            else
                pnlConfirmar.Visible = true;
        }

        protected void btnConfirmar_Click(object sender, EventArgs e)
        {
            bool ok;
            try { ok = new VerificacionNegocio().ConfirmarCorreo(Request.QueryString["token"]); }
            catch (Exception) { ok = false; }

            pnlConfirmar.Visible = false;
            pnlOk.Visible = ok;
            pnlError.Visible = !ok;
            pnlReenviar.Visible = !ok;
        }

        protected void btnReenviar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try { new VerificacionNegocio().ReenviarVerificacion(txtEmail.Text); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.ToString()); }

            // Mensaje siempre igual: no revela si el correo existe, ya estaba verificado o falló el envío.
            lblMensaje.Text = "Si el correo corresponde a una cuenta pendiente de confirmar, te enviamos un enlace nuevo. Revisá también el spam.";
            pnlMensaje.Visible = true;
        }
    }
}
