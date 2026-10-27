using System;
using System.Collections.Generic;
using dominio;
using negocio;

namespace Pasalo1
{
    // Chat entre las dos personas de una solicitud ACEPTADA.
    // Chat.aspx            -> abre la primera conversación
    // Chat.aspx?id=<IdSolicitud> -> abre esa conversación (solo si participás de ella)
    public partial class Chat : System.Web.UI.Page
    {
        protected List<ConversacionChat> Conversaciones = new List<ConversacionChat>();
        protected ConversacionChat Actual;                       // conversación abierta (null = ninguna)
        protected List<MensajeChat> Mensajes = new List<MensajeChat>();
        protected int IdYo;                                      // usuario logueado: sirve para saber qué burbujas son mías

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                Usuario logueado = Session["Usuario"] as Usuario;
                if (logueado == null)
                {
                    Response.Redirect("Login.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }
                IdYo = logueado.IdUsuario;

                MensajeNegocio negocio = new MensajeNegocio();
                Conversaciones = negocio.ListarConversaciones(IdYo);

                pnlVacio.Visible = Conversaciones.Count == 0;
                pnlGrid.Visible = Conversaciones.Count > 0;
                if (Conversaciones.Count == 0)
                    return;

                // Si no piden una en particular se abre la más reciente.
                // Como solo se busca entre MIS conversaciones, no se puede abrir la de otra persona cambiando el id.
                int idPedido;
                int.TryParse(Request.QueryString["id"], out idPedido);

                if (idPedido == 0)
                    Actual = Conversaciones.Find(c => c.NoLeidos > 0) ?? Conversaciones[0];   // primero la que tiene mensajes nuevos
                else
                    Actual = Conversaciones.Find(c => c.IdSolicitud == idPedido);

                if (Actual == null)
                {
                    MostrarError("No tenés acceso a esa conversación.");
                    return;
                }

                pnlChat.Visible = true;
                Mensajes = negocio.ListarMensajes(Actual.IdSolicitud, IdYo);

                // Al abrirla, lo que me escribieron pasa a "leído" (y se apaga el aviso de la campanita)
                negocio.MarcarComoVista(Actual.IdSolicitud, IdYo);
                Actual.NoLeidos = 0;
            }
            catch (NegocioException ex)
            {
                MostrarError(ex.Message);
            }
            catch (Exception ex)
            {
                Session.Add("Error", ex.ToString());
                Response.Redirect("Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected void btnEnviar_Click(object sender, EventArgs e)
        {
            if (Actual == null)
                return;

            try
            {
                new MensajeNegocio().Enviar(Actual.IdSolicitud, IdYo, txtMensaje.Text);

                // Redirigimos a la misma página: así F5 no reenvía el mensaje y se ve ya guardado
                Response.Redirect("Chat.aspx?id=" + Actual.IdSolicitud, false);
                Context.ApplicationInstance.CompleteRequest();
            }
            catch (NegocioException ex)
            {
                MostrarError(ex.Message);
            }
            catch (Exception ex)
            {
                Session.Add("Error", ex.ToString());
                Response.Redirect("Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        private void MostrarError(string mensaje)
        {
            lblError.Text = mensaje;
            pnlError.Visible = true;
        }

        // ---- Ayudas para la vista ----

        protected string Inicial(string nombre)
        {
            return string.IsNullOrWhiteSpace(nombre) ? "?" : nombre.Trim().Substring(0, 1).ToUpper();
        }

        // Vista previa del último mensaje en la lista
        protected string Resumen(ConversacionChat c)
        {
            if (c.UltimoTexto == null)
                return "Sin mensajes todavía";
            return c.UltimoTexto.Length <= 40 ? c.UltimoTexto : c.UltimoTexto.Substring(0, 40) + "…";
        }

        // Las fechas están guardadas en UTC; acá se pasan a la hora de la PC (Argentina)
        protected string Hora(DateTime utc)
        {
            DateTime local = utc.ToLocalTime();
            return local.Date == DateTime.Today ? local.ToString("HH:mm") : local.ToString("dd/MM HH:mm");
        }
    }
}
