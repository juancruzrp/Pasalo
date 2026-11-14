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
            // Libera los intercambios que llevan más de 10 días sin entregarse
            try { new SolicitudNegocio().VencerInactivas(); }
            catch (Exception) { /* si falla, la página sigue funcionando */ }

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

                case EstadoSolicitud.Cancelada:
                    return "cancelada";

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


           




        protected void btnConfirmarCodigo_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario usuario = Session["Usuario"] as Usuario;

                if (usuario == null)
                {
                    Response.Redirect("Login.aspx");
                    return;
                }

                Button boton = (Button)sender;

                int idSolicitud = Convert.ToInt32(boton.CommandArgument);

                RepeaterItem item = (RepeaterItem)boton.NamingContainer;

                TextBox txtCodigo = (TextBox)item.FindControl("txtCodigoEntrega");

                string codigo = txtCodigo.Text.Trim().ToUpperInvariant();

                if (codigo.Length != 6)
                {
                    pnlErrorSolicitud.Visible = true;
                    lblErrorSolicitud.Text = "Ingresá un código de 6 caracteres.";
                    return;
                }

                SolicitudNegocio negocio = new SolicitudNegocio();

                negocio.ConfirmarEntregaConCodigo(
                    idSolicitud,
                    usuario.IdUsuario,
                    codigo
                );

                Response.Redirect("MiActividad.aspx?tab=recibidas");
            }
            catch (NegocioException ex)
            {
                pnlErrorSolicitud.Visible = true;
                lblErrorSolicitud.Text = ex.Message;
            }
        }



        // Lo usa la pantalla para decir "Cancelaste..." o "Fulano canceló..."
        protected int UsuarioActualId
        {
            get
            {
                dominio.Usuario u = Session["Usuario"] as dominio.Usuario;
                return u == null ? 0 : u.IdUsuario;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            dominio.Usuario usuario = Session["Usuario"] as dominio.Usuario;

            if (usuario == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            Button boton = sender as Button;
            int idSolicitud;

            if (boton == null || !int.TryParse(boton.CommandArgument, out idSolicitud))
                return;

            try
            {
                new SolicitudNegocio().Cancelar(idSolicitud, usuario.IdUsuario);

                Response.Redirect("MiActividad.aspx?tab=" + TabActiva, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (NegocioException ex)
            {
                MostrarError(ex.Message);
            }
            catch (Exception)
            {
                MostrarError("No se pudo cancelar el intercambio. Intentá nuevamente.");
            }
        }












    }
}
