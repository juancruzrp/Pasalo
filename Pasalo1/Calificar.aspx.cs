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
    public partial class Calificar : Page
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

            int idSolicitud;

            if (!int.TryParse(Request.QueryString["id"], out idSolicitud)
                || idSolicitud <= 0)
            {
                MostrarError("La solicitud no es válida.");
                pnlFormulario.Visible = false;
                return;
            }

            try
            {
                // Comprobación previa para mostrar la página.
                // La validación definitiva se hace al guardar en SQL.
                SolicitudNegocio solicitudes = new SolicitudNegocio();

                bool recibida = solicitudes.ListarRecibidas(usuario.IdUsuario)
                    .Any(s => s.IdSolicitud == idSolicitud
                           && s.EntregaFinalizada);

                bool enviada = solicitudes.ListarEnviadas(usuario.IdUsuario)
                    .Any(s => s.IdSolicitud == idSolicitud
                           && s.EntregaFinalizada);

                if (!recibida && !enviada)
                {
                    MostrarError("No podés calificar esta operación.");
                    pnlFormulario.Visible = false;
                    return;
                }

                ValoracionNegocio valoraciones = new ValoracionNegocio();

                if (valoraciones.YaValoro(idSolicitud, usuario.IdUsuario))
                {
                    MostrarError("Ya calificaste a este usuario.");
                    pnlFormulario.Visible = false;
                }
            }
            catch (Exception)
            {
                MostrarError("No se pudo cargar la operación.");
                pnlFormulario.Visible = false;
            }
        }

        protected void btnGuardar_Click(object sender, EventArgs e)
        {
            Usuario usuario = Session["Usuario"] as Usuario;

            if (usuario == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            int idSolicitud;
            int puntaje;

            if (!int.TryParse(Request.QueryString["id"], out idSolicitud)
                || !int.TryParse(hfPuntaje.Value, out puntaje))
            {
                MostrarError("Seleccioná una calificación de 1 a 5 estrellas.");
                return;
            }

            try
            {
                ValoracionNegocio negocio = new ValoracionNegocio();

                negocio.Registrar(
                    idSolicitud,
                    usuario.IdUsuario,
                    puntaje,
                    txtComentario.Text);

                Response.Redirect("MiActividad.aspx?valoracion=ok", false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (NegocioException ex)
            {
                MostrarError(ex.Message);
            }
            catch (Exception)
            {
                MostrarError("No se pudo guardar la calificación.");
            }
        }

        private void MostrarError(string mensaje)
        {
            pnlError.Visible = true;
            lblError.Text = mensaje;
        }
    }
}
