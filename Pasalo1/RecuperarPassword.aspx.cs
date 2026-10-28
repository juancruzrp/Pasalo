using System;
using negocio;

namespace Pasalo1
{
    public partial class RecuperarPassword : System.Web.UI.Page
    {
        private string Token { get { return Request.QueryString["token"]; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;

            bool valido = false;
            try { valido = new VerificacionNegocio().TokenRecuperacionValido(Token); }
            catch (Exception) { }

            pnlForm.Visible = valido;
            pnlInvalido.Visible = !valido;
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            pnlError.Visible = false;

            if (txtPassword.Text != txtConfirmar.Text)
            {
                Mostrar("Las contraseñas no coinciden.");
                return;
            }

            try
            {
                new VerificacionNegocio().RestablecerPassword(Token, txtPassword.Text);
                pnlForm.Visible = false;
                pnlOk.Visible = true;
            }
            catch (NegocioException ex)
            {
                // Si el enlace venció mientras escribía, se muestra el cartel de enlace inválido
                if (!new VerificacionNegocio().TokenRecuperacionValido(Token))
                {
                    pnlForm.Visible = false;
                    pnlInvalido.Visible = true;
                }
                else
                    Mostrar(ex.Message);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Mostrar("No pudimos guardar la contraseña. Probá de nuevo.");
            }
        }

        private void Mostrar(string mensaje)
        {
            lblError.Text = mensaje;
            pnlError.Visible = true;
        }
    }
}
