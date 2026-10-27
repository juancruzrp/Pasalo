using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.SessionState;
using dominio;
using negocio;

namespace Pasalo1
{
    // Dos usos:
    //   Notificaciones.ashx            -> devuelve en JSON las novedades del usuario (la campanita lo consulta cada 30 s)
    //   Notificaciones.ashx?visto=<id> -> marca como vista una solicitud rechazada y lleva a Mi actividad > Enviadas
    public class Notificaciones : IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable { get { return false; } }

        public void ProcessRequest(HttpContext context)
        {
            Usuario usuario = context.Session["Usuario"] as Usuario;

            int idVisto;
            if (int.TryParse(context.Request.QueryString["visto"], out idVisto))
            {
                if (usuario != null)
                    new NotificacionNegocio().MarcarRespuestaVista(idVisto, usuario.IdUsuario);
                context.Response.Redirect(usuario == null ? "Login.aspx" : "MiActividad.aspx?tab=enviadas", false);
                context.ApplicationInstance.CompleteRequest();
                return;
            }

            context.Response.ContentType = "application/json";
            context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            List<Notificacion> nuevas = new List<Notificacion>();
            if (usuario != null)
            {
                try { nuevas = new NotificacionNegocio().ListarNuevas(usuario.IdUsuario); }
                catch (Exception) { /* la campanita nunca debe romper la página: si falla, queda vacía */ }
            }

            int mensajes = 0;
            StringBuilder json = new StringBuilder();
            json.Append("{\"items\":[");
            for (int i = 0; i < nuevas.Count; i++)
            {
                Notificacion n = nuevas[i];
                if (n.Tipo == "mensaje") mensajes += n.Cantidad;
                if (i > 0) json.Append(',');
                json.Append("{\"tipo\":\"").Append(Esc(n.Tipo))
                    .Append("\",\"texto\":\"").Append(Esc(n.Texto))
                    .Append("\",\"url\":\"").Append(Esc(n.Url))
                    .Append("\",\"fecha\":\"").Append(n.Fecha.ToString("yyyy-MM-ddTHH:mm:ss")).Append("Z\"}");
            }
            json.Append("],\"total\":").Append(nuevas.Count)
                .Append(",\"mensajes\":").Append(mensajes).Append('}');
            context.Response.Write(json.ToString());
        }

        // Escapa lo mínimo para meter texto dentro de un string JSON
        private static string Esc(string s)
        {
            if (s == null) return "";
            StringBuilder b = new StringBuilder();
            foreach (char c in s)
            {
                if (c == '\\') b.Append("\\\\");
                else if (c == '"') b.Append("\\\"");
                else if (c == '\n') b.Append("\\n");
                else if (c == '\r') b.Append("\\r");
                else if (c == '\t') b.Append("\\t");
                else if (c < ' ') b.Append("\\u").Append(((int)c).ToString("x4"));
                else b.Append(c);
            }
            return b.ToString();
        }
    }
}
