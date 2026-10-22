using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dominio;
using negocio;


namespace Pasalo1
{
    public partial class MiActividad : Page
    {
        protected string TabActiva = "recibidas";

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }

        protected void Page_Init(object sender, EventArgs e)
        {
            CargarSolicitudes();
        }





        private void CargarSolicitudes()
        {

            dominio.Usuario usuario =
                Session["Usuario"] as dominio.Usuario;

            if (usuario == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            string tab = Request.QueryString["tab"];

            if (tab == "enviadas")
                TabActiva = "enviadas";

            try
            {
                SolicitudNegocio negocio =
                    new SolicitudNegocio();

                if (TabActiva == "recibidas")
                {
                    pnlRecibidas.Visible = true;
                    pnlEnviadas.Visible = false;

                    List<SolicitudListado> solicitudes =
                        negocio.ListarRecibidas(
                            usuario.IdUsuario);

                    rptRecibidas.DataSource = solicitudes;
                    rptRecibidas.DataBind();

                    pnlSinRecibidas.Visible =
                        solicitudes.Count == 0;
                }
                else
                {
                    pnlRecibidas.Visible = false;
                    pnlEnviadas.Visible = true;

                    List<SolicitudListado> solicitudes =
                        negocio.ListarEnviadas(
                            usuario.IdUsuario);

                    rptEnviadas.DataSource = solicitudes;
                    rptEnviadas.DataBind();

                    pnlSinEnviadas.Visible =
                        solicitudes.Count == 0;
                }
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.ToString();

                Response.Redirect("Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }

        }










        protected string EstadoCss(object estado)
        {
            EstadoSolicitud valor =
                (EstadoSolicitud)estado;

            switch (valor)
            {
                case EstadoSolicitud.Aceptada:
                    return "aceptada";

                case EstadoSolicitud.Rechazada:
                    return "rechazada";

                default:
                    return "pendiente";
            }
        }

        protected bool TieneOferta(object objeto)
        {
            return objeto != null &&
                   !string.IsNullOrWhiteSpace(
                       Convert.ToString(objeto));
        }






        protected void btnAceptar_Click(object sender, EventArgs e)
        {
            ProcesarSolicitud(sender, true);
        }

        protected void btnRechazar_Click(object sender, EventArgs e)
        {
            ProcesarSolicitud(sender, false);
        }

        private void ProcesarSolicitud(object sender, bool aceptar)
        {
            dominio.Usuario usuario =
                Session["Usuario"] as dominio.Usuario;

            if (usuario == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Button boton = sender as Button;

            int idSolicitud;

            if (boton == null ||
                !int.TryParse(boton.CommandArgument, out idSolicitud))
                return;

            try
            {
                SolicitudNegocio negocio = new SolicitudNegocio();

                if (aceptar)
                    negocio.Aceptar(idSolicitud, usuario.IdUsuario);
                else
                    negocio.Rechazar(idSolicitud, usuario.IdUsuario);

                Response.Redirect(
                    "MiActividad.aspx?tab=recibidas", false);

                Context.ApplicationInstance.CompleteRequest();
            }
            catch (NegocioException ex)
            {
                MostrarError(ex.Message);
            }
            catch (Exception)
            {
                MostrarError(
                    "No se pudo procesar la solicitud. Intentá nuevamente.");
            }
        }

        private void MostrarError(string mensaje)
        {
            lblErrorSolicitud.Text = mensaje;
            pnlErrorSolicitud.Visible = true;
        }













    }
}
