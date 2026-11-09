using System;
using dominio;
using negocio;

namespace Pasalo1
{
    // Cualquier usuario con sesión puede reportar una publicación (?pub=id) o a otra persona (?usu=id).
    public partial class Reportar : System.Web.UI.Page
    {
        private int? idPub, idUsu;

        protected void Page_Load(object sender, EventArgs e)
        {
            int n;
            idPub = int.TryParse(Request.QueryString["pub"], out n) && n > 0 ? n : (int?)null;
            idUsu = int.TryParse(Request.QueryString["usu"], out n) && n > 0 ? n : (int?)null;

            if (idPub.HasValue == idUsu.HasValue)
            {
                pnlForm.Visible = false;
                pnlNo.Visible = true;
                return;
            }
            lblQue.Text = idPub.HasValue ? "Vas a reportar una publicación." : "Vas a reportar a una persona.";
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (idPub.HasValue == idUsu.HasValue)
                return;
            try
            {
                new ModeracionNegocio().Reportar(Session["Usuario"] as Usuario, idPub, idUsu,
                    ddlMotivo.SelectedValue, txtDetalle.Text);
                pnlForm.Visible = false;
                pnlOk.Visible = true;
            }
            catch (NegocioException ex)
            {
                lblError.Text = Server.HtmlEncode(ex.Message);
                pnlError.Visible = true;
            }
        }
    }
}
