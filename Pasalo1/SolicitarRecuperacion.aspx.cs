using System;
using negocio;

namespace Pasalo1
{
    public partial class SolicitarRecuperacion : System.Web.UI.Page
    {
        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            try { new VerificacionNegocio().SolicitarRecuperacion(txtEmail.Text); }
            catch (Exception ex) { System.Diagnostics.Debug.WriteLine(ex.ToString()); }

            // Mismo mensaje exista o no el correo: no se puede usar para averiguar quién está registrado.
            pnlForm.Visible = false;
            pnlListo.Visible = true;
        }
    }
}
