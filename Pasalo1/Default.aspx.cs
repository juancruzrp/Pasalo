
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using dominio;
using negocio;

namespace Pasalo1
{
    public partial class _Default : Page
    {
        private readonly PublicacionNegocio negocio =
            new PublicacionNegocio();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                try
                {
                    CargarFiltros();
                    CargarPublicaciones();
                }
                catch (Exception)
                {
                    MostrarError();
                }
            }
        }

        private void CargarFiltros()
        {
            ddlCarrera.DataSource = negocio.ListarCarreras();
            ddlCarrera.DataTextField = "Nombre";
            ddlCarrera.DataValueField = "Id";
            ddlCarrera.DataBind();
            ddlCarrera.Items.Insert(
                0, new ListItem("Todas las carreras", "0"));

            ddlCategoria.DataSource = negocio.ListarCategorias();
            ddlCategoria.DataTextField = "Nombre";
            ddlCategoria.DataValueField = "Id";
            ddlCategoria.DataBind();
            ddlCategoria.Items.Insert(
                0, new ListItem("Todas las categorías", "0"));
        }

        private void CargarPublicaciones()
        {
            int categoria = Convert.ToInt32(
                ddlCategoria.SelectedValue);

            int carrera = Convert.ToInt32(
                ddlCarrera.SelectedValue);

            bool masRecientes = ddlOrden.SelectedValue == "DESC";

            List<dominio.Publicacion> lista = negocio.ListarDisponibles(
                categoria,
                carrera,
                txtBuscar.Text,
                masRecientes);

            rptPublicaciones.DataSource = lista;
            rptPublicaciones.DataBind();

            lblCantidad.Text = lista.Count == 1
                ? "Mostrando 1 objeto disponible"
                : "Mostrando " + lista.Count +
                  " objetos disponibles";

            pnlSinResultados.Visible = lista.Count == 0;
            lblError.Visible = false;
        }

        protected void Filtros_Changed(
            object sender, EventArgs e)
        {
            try
            {
                CargarPublicaciones();
            }
            catch (Exception)
            {
                MostrarError();
            }
        }

        private void MostrarError()
        {
            lblError.Text =
                "No se pudieron cargar las publicaciones. " +
                "Intentá nuevamente.";

            lblError.Visible = true;
            pnlSinResultados.Visible = false;

            rptPublicaciones.DataSource =
                new List<dominio.Publicacion>();

            rptPublicaciones.DataBind();
        }

        protected string ObtenerFoto(object dataItem)
        {
            dominio.Publicacion p = (dominio.Publicacion)dataItem;

            if (p.Fotos != null && p.Fotos.Count > 0)
            {
                string ruta = p.Fotos[0].Ruta;

                if (!string.IsNullOrWhiteSpace(ruta))
                    return ResolveUrl("~/" + ruta.TrimStart('/'));
            }

            return "";
        }

        protected string ObtenerCarrera(object dataItem)
        {
            dominio.Publicacion p = (dominio.Publicacion)dataItem;

            return p.Carrera == null
                ? "Todas las carreras"
                : p.Carrera.Nombre;
        }

        protected string ObtenerModalidad(object dataItem)
        {
            dominio.Publicacion p = (dominio.Publicacion)dataItem;

            return p.Modalidad == ModalidadPublicacion.Donacion
                ? "Donación"
                : "Intercambio";
        }

        protected string ObtenerDetalle(object dataItem)
        {
            dominio.Publicacion p = (dominio.Publicacion)dataItem;

            return ResolveUrl(
                "~/DetallePublicacion.aspx?id=" + p.Id);
        }

        protected string TextoSeguro(object valor)
        {
            return HttpUtility.HtmlEncode(
                Convert.ToString(valor) ?? "");
        }
    }
}
