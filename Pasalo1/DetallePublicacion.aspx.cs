using System;
using System.Web;
using System.Web.UI;
using dominio;
using negocio;

namespace Pasalo1
{
    public partial class DetallePublicacion : System.Web.UI.Page
    {
        protected dominio.Publicacion Pub;

        protected void Page_Load(object sender, EventArgs e)
        {
            int id;
            if (!int.TryParse(Request.QueryString["id"], out id) || id <= 0)
            {
                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            try
            {
                Pub = new PublicacionNegocio().ObtenerPorId(id);
            }
            catch (Exception ex)
            {
                Session["Error"] = ex.ToString();
                Response.Redirect("Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            if (Pub == null)
            {
                Response.Redirect("Default.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        protected string Estado
        {
            get
            {
                if (Pub.Estado == EstadoPublicacion.Reservado) return "reservado";
                if (Pub.Estado == EstadoPublicacion.Entregado) return "entregado";
                return "disponible";
            }
        }

        protected string EstadoTexto
        {
            get
            {
                if (Pub.Estado == EstadoPublicacion.Reservado) return "Reservado";
                if (Pub.Estado == EstadoPublicacion.Entregado) return "Entregado";
                return "Disponible";
            }
        }

        protected bool EsIntercambio
        {
            get { return Pub.Modalidad == ModalidadPublicacion.Intercambio; }
        }

        protected string ModalidadTexto
        {
            get { return EsIntercambio ? "Intercambio" : "Donación"; }
        }

        protected string CarreraTexto
        {
            get { return Pub.Carrera == null ? "Todas las carreras" : Pub.Carrera.Nombre; }
        }

        protected string IconoCategoria
        {
            get
            {
                string n = (Pub.Categoria.Nombre ?? "").ToLowerInvariant();
                if (n.StartsWith("ropa")) return "ti-shirt";
                if (n.StartsWith("libro")) return "ti-book";
                if (n.StartsWith("apunte")) return "ti-pencil";
                if (n.StartsWith("electr")) return "ti-device-laptop";
                return "ti-box";
            }
        }

        protected string FotoHtml
        {
            get
            {
                if (Pub.Fotos != null && Pub.Fotos.Count > 0)
                {
                    string url = HttpUtility.HtmlAttributeEncode(ResolveUrl("~/" + Pub.Fotos[0].Ruta));
                    return "<img src=\"" + url + "\" alt=\"Foto de " + HttpUtility.HtmlAttributeEncode(Pub.Titulo) + "\" style=\"width:100%;height:100%;object-fit:cover;border-radius:inherit\" />";
                }
                return "<i class=\"ti " + IconoCategoria + "\" aria-hidden=\"true\"></i>";
            }
        }

        protected string DuenoIniciales
        {
            get
            {
                string[] partes = (Pub.Dueno.Nombre ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length == 0) return "?";
                if (partes.Length == 1) return partes[0].Substring(0, 1).ToUpperInvariant();
                return (partes[0].Substring(0, 1) + partes[partes.Length - 1].Substring(0, 1)).ToUpperInvariant();
            }
        }

        protected string DuenoDetalle
        {
            get
            {
                string carrera = string.IsNullOrEmpty(Pub.Dueno.CarreraNombre) ? "Estudiante" : "Estudiante de " + Pub.Dueno.CarreraNombre;
                return carrera + " · Miembro desde " + Pub.Dueno.FechaRegistro.Year;
            }
        }

        protected string PublicadoHace
        {
            get
            {
                TimeSpan t = DateTime.UtcNow - Pub.FechaPublicacion;
                if (t.TotalMinutes < 1) return "Publicado recién";
                if (t.TotalHours < 1) return "Publicado hace " + (int)t.TotalMinutes + " min";
                if (t.TotalDays < 1) return "Publicado hace " + (int)t.TotalHours + " h";
                int d = (int)t.TotalDays;
                return "Publicado hace " + d + (d == 1 ? " día" : " días");
            }
        }
    }
}
