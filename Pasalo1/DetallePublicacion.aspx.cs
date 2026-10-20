
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using dominio;
using negocio;

namespace Pasalo1
{
    public partial class DetallePublicacion : Page
    {
        protected dominio.Publicacion PublicacionActual { get; private set; }

        protected string TituloSeguro
        {
            get
            {
                return HttpUtility.HtmlEncode(
                    PublicacionActual.Titulo);
            }
        }

        protected string DescripcionSegura
        {
            get
            {
                return HttpUtility.HtmlEncode(
                    PublicacionActual.Descripcion)
                    .Replace("\r\n", "\n")
                    .Replace("\n", "<br />");
            }
        }

        protected string CategoriaTexto
        {
            get
            {
                return HttpUtility.HtmlEncode(
                    PublicacionActual.Categoria.Nombre);
            }
        }

        protected string CarreraTexto
        {
            get
            {
                return PublicacionActual.Carrera == null
                    ? "Todas las carreras"
                    : HttpUtility.HtmlEncode(
                        PublicacionActual.Carrera.Nombre);
            }
        }

        protected string ModalidadTexto
        {
            get
            {
                return PublicacionActual.Modalidad ==
                    ModalidadPublicacion.Donacion
                    ? "Donación"
                    : "Intercambio";
            }
        }

        protected string EstadoTexto
        {
            get
            {
                switch (PublicacionActual.Estado)
                {
                    case EstadoPublicacion.Reservado:
                        return "Reservado";

                    case EstadoPublicacion.Entregado:
                        return "Entregado";

                    default:
                        return "Disponible";
                }
            }
        }

        protected string EstadoCss
        {
            get
            {
                return EstadoTexto.ToLowerInvariant();
            }
        }

        protected string NombreDueno
        {
            get
            {
                return HttpUtility.HtmlEncode(
                    PublicacionActual.Dueno.Nombre);
            }
        }

        protected string InicialesDueno
        {
            get
            {
                string nombre = PublicacionActual.Dueno.Nombre ?? "";
                string[] partes = nombre.Split(
                    new[] { ' ' },
                    StringSplitOptions.RemoveEmptyEntries);

                if (partes.Length == 0)
                    return "?";

                string iniciales = partes[0].Substring(0, 1);

                if (partes.Length > 1)
                    iniciales += partes[partes.Length - 1].Substring(0, 1);

                return HttpUtility.HtmlEncode(
                    iniciales.ToUpperInvariant());
            }
        }

        protected string FechaPublicacionTexto
        {
            get
            {
                // SQL Server guarda la fecha en UTC.
                DateTime fecha = DateTime.SpecifyKind(
                    PublicacionActual.FechaPublicacion,
                    DateTimeKind.Utc);

                return fecha.ToLocalTime().ToString("dd/MM/yyyy");
            }
        }

        protected string FechaRegistroTexto
        {
            get
            {
                DateTime fecha = DateTime.SpecifyKind(
                    PublicacionActual.Dueno.FechaRegistro,
                    DateTimeKind.Utc);

                return fecha.ToLocalTime().ToString("MM/yyyy");
            }
        }

        protected bool EsPropietario
        {
            get
            {
                dominio.Usuario usuario =
                    Session["Usuario"] as dominio.Usuario;

                return usuario != null &&
                       usuario.IdUsuario ==
                       PublicacionActual.Dueno.IdUsuario;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            int id;

            if (!int.TryParse(Request.QueryString["id"], out id)
                || id <= 0)
            {
                MostrarNoEncontrada();
                return;
            }

            try
            {
                PublicacionNegocio negocio =
                    new PublicacionNegocio();

                PublicacionActual = negocio.ObtenerPorId(id);

                if (PublicacionActual == null)
                {
                    MostrarNoEncontrada();
                    return;
                }

                pnlDetalle.Visible = true;
                pnlNoEncontrada.Visible = false;

                rptFotos.DataSource = PublicacionActual.Fotos;
                rptFotos.DataBind();

                pnlSinFotos.Visible =
                    PublicacionActual.Fotos.Count == 0;

                pnlDisponible.Visible =
                    PublicacionActual.Estado ==
                    EstadoPublicacion.Disponible;

                pnlReservado.Visible =
                    PublicacionActual.Estado ==
                    EstadoPublicacion.Reservado;

                pnlEntregado.Visible =
                    PublicacionActual.Estado ==
                    EstadoPublicacion.Entregado;
            }
            catch (Exception)
            {
                pnlDetalle.Visible = false;
                pnlNoEncontrada.Visible = true;

                lblNoEncontrada.Text =
                    "No se pudo cargar la publicación. " +
                    "Intentá nuevamente más tarde.";
            }
        }

        private void MostrarNoEncontrada()
        {
            Response.StatusCode = 404;
            pnlDetalle.Visible = false;
            pnlNoEncontrada.Visible = true;
            lblNoEncontrada.Text =
                "La publicación no existe o ya no está disponible.";
        }

        protected string ObtenerRutaFoto(object ruta)
        {
            string valor = Convert.ToString(ruta);

            if (string.IsNullOrWhiteSpace(valor))
                return "";

            return ResolveUrl("~/" + valor.TrimStart('/'));
        }
    }
}
