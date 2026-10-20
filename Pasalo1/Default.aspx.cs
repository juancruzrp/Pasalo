using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pasalo1
{
    public partial class _Default : Page
    {
        // Una fila de la lista de categorías (la primera es "Todas")
        public class ItemCategoria
        {
            public int Id { get; set; }
            public string Nombre { get; set; }
            public string Icono { get; set; }
            public int Cantidad { get; set; }
            public bool Actual { get; set; }
            public string Url { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                PublicacionNegocio negocio = new PublicacionNegocio();

                if (!IsPostBack)
                    CargarCarreras(negocio);

                MostrarCatalogo(negocio);
            }
            catch (Exception ex)
            {
                Session.Add("Error", ex.ToString());
                Response.Redirect("Error.aspx", false);
            }
        }

        // El filtro de carreras sale de la base (así coincide con el registro y con "Publicar")
        private void CargarCarreras(PublicacionNegocio negocio)
        {
            ddlCarrera.DataSource = negocio.ListarCarreras();
            ddlCarrera.DataValueField = "Id";
            ddlCarrera.DataTextField = "Nombre";
            ddlCarrera.DataBind();
            ddlCarrera.Items.Insert(0, new ListItem("Todas las carreras", "0"));

            // Si venimos desde un link de categoría, se conserva la carrera elegida
            int idCarrera = LeerEntero(Request.QueryString["carrera"]);
            if (ddlCarrera.Items.FindByValue(idCarrera.ToString()) != null)
                ddlCarrera.SelectedValue = idCarrera.ToString();
        }

        // Dibuja categorías (con sus contadores) y la grilla según los filtros elegidos.
        // La categoría viaja por la dirección (?cat=2); carrera y orden por los desplegables.
        private void MostrarCatalogo(PublicacionNegocio negocio)
        {
            int idCarrera = LeerEntero(ddlCarrera.SelectedValue);
            int idCategoria = LeerEntero(Request.QueryString["cat"]);
            bool masRecientes = ddlOrden.SelectedValue != "antiguos";

            // Categorías con la cantidad de objetos disponibles
            Dictionary<int, int> cuentas = negocio.ContarDisponiblesPorCategoria(idCarrera);
            int total = 0;
            foreach (int cantidad in cuentas.Values)
                total += cantidad;

            List<ItemCategoria> categorias = new List<ItemCategoria>();
            categorias.Add(new ItemCategoria
            {
                Id = 0,
                Nombre = "Todas",
                Icono = "ti-layout-grid",
                Cantidad = total,
                Actual = idCategoria == 0,
                Url = UrlCatalogo(0, idCarrera)
            });

            foreach (Categoria c in negocio.ListarCategorias())
            {
                int cant;
                cuentas.TryGetValue(c.Id, out cant);
                categorias.Add(new ItemCategoria
                {
                    Id = c.Id,
                    Nombre = c.Nombre,
                    Icono = IconoCategoria(c.Nombre),
                    Cantidad = cant,
                    Actual = idCategoria == c.Id,
                    Url = UrlCatalogo(c.Id, idCarrera)
                });
            }

            rptCategorias.DataSource = categorias;
            rptCategorias.DataBind();

            // Publicaciones
            List<dominio.Publicacion> lista = negocio.ListarDisponibles(idCarrera, idCategoria, masRecientes);
            bool hayObjetos = lista.Count > 0;

            pnlGrilla.Visible = hayObjetos;
            pnlVacio.Visible = !hayObjetos;

            if (hayObjetos)
            {
                rptPublicaciones.DataSource = lista;
                rptPublicaciones.DataBind();
                lblResumen.Text = "Mostrando " + lista.Count + (lista.Count == 1 ? " objeto" : " objetos") + " para donar o intercambiar";
            }
            else
            {
                bool conFiltros = idCarrera != 0 || idCategoria != 0;
                lblResumen.Text = "No hay objetos para mostrar";
                lblVacio.Text = conFiltros
                    ? "No hay objetos disponibles con estos filtros."
                    : "Todavía no hay objetos publicados. ¡Publicá el primero!";
            }
        }

        private static int LeerEntero(string texto)
        {
            int valor;
            return int.TryParse(texto, out valor) && valor > 0 ? valor : 0;
        }

        private static string UrlCatalogo(int idCategoria, int idCarrera)
        {
            string url = "Default.aspx";
            List<string> partes = new List<string>();
            if (idCategoria > 0) partes.Add("cat=" + idCategoria);
            if (idCarrera > 0) partes.Add("carrera=" + idCarrera);
            return partes.Count == 0 ? url : url + "?" + string.Join("&", partes);
        }

        // Ícono por nombre de categoría; las nuevas usan uno genérico
        private static string IconoCategoria(string nombre)
        {
            switch ((nombre ?? "").Trim().ToLowerInvariant())
            {
                case "ropa": return "ti-shirt";
                case "libros": return "ti-book";
                case "apuntes": return "ti-pencil";
                case "electrónicos": return "ti-device-laptop";
                default: return "ti-box";
            }
        }

        // ---- Ayudas para el marcado de cada tarjeta (Container.DataItem es la publicación) ----

        protected string EstiloFoto(object item)
        {
            dominio.Publicacion p = item as dominio.Publicacion;
            if (p == null || p.Fotos == null || p.Fotos.Count == 0)
                return "";

            string url = HttpUtility.HtmlAttributeEncode(ResolveUrl("~/" + p.Fotos[0].Ruta));
            return " style=\"background: url('" + url + "') center / cover no-repeat;\"";
        }

        protected string IconoFoto(object item)
        {
            dominio.Publicacion p = item as dominio.Publicacion;
            if (p == null)
                return "";
            if (p.Fotos != null && p.Fotos.Count > 0)
                return "";

            return "<i class=\"ti " + IconoCategoria(p.Categoria.Nombre) + "\" aria-hidden=\"true\"></i>";
        }

        protected string TextoCarrera(object item)
        {
            dominio.Publicacion p = item as dominio.Publicacion;
            if (p == null)
                return "";

            string carrera = p.Carrera == null ? "Todas las carreras" : p.Carrera.Nombre;
            string modalidad = p.Modalidad == ModalidadPublicacion.Intercambio ? "Intercambio" : "Donación";
            return carrera + " · " + modalidad;
        }
    }
}
