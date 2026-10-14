using dominio;
using negocio;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pasalo1
{
    public partial class Publicacion : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack)
                return;

            try
            {
                CargarListas();
            }
            catch (Exception ex)
            {
                Session.Add("Error", ex.ToString());
                Response.Redirect("Error.aspx", false);
            }
        }

        // Los desplegables salen de la base (así siempre coinciden con los Id reales)
        private void CargarListas()
        {
            PublicacionNegocio negocio = new PublicacionNegocio();

            ddlCategoria.DataSource = negocio.ListarCategorias();
            ddlCategoria.DataValueField = "Id";
            ddlCategoria.DataTextField = "Nombre";
            ddlCategoria.DataBind();
            ddlCategoria.Items.Insert(0, new ListItem("Seleccionar categoría", "0"));

            ddlCarrera.DataSource = negocio.ListarCarreras();
            ddlCarrera.DataValueField = "Id";
            ddlCarrera.DataTextField = "Nombre";
            ddlCarrera.DataBind();
            ddlCarrera.Items.Insert(0, new ListItem("Todas las carreras", "0"));
        }

        protected void btnPublicar_Click(object sender, EventArgs e)
        {
            try
            {
                // OJO:  se llama Publicacion, por eso la clase del dominio va con su namespace
                dominio.Publicacion nueva = new dominio.Publicacion();
                nueva.Dueno = (Usuario)Session["Usuario"];
                nueva.Titulo = txtTitulo.Text;
                nueva.Descripcion = txtDescripcion.Text;

                int idCategoria;
                int.TryParse(ddlCategoria.SelectedValue, out idCategoria);
                nueva.Categoria = new Categoria { Id = idCategoria };

                int idCarrera;
                int.TryParse(ddlCarrera.SelectedValue, out idCarrera);
                nueva.Carrera = idCarrera > 0 ? new Carrera { Id = idCarrera } : null;

                if (rbDonacion.Checked)
                    nueva.Modalidad = ModalidadPublicacion.Donacion;
                else if (rbIntercambio.Checked)
                    nueva.Modalidad = ModalidadPublicacion.Intercambio;

                new PublicacionNegocio().Publicar(nueva, LeerFotos(), Server.MapPath("~/Uploads/publicaciones"));

                Response.Redirect("Default.aspx?publicado=ok", false);
            }
            catch (NegocioException ex)
            {
                lblError.Text = ex.Message;
                pnlError.Visible = true;
            }
            catch (Exception ex)
            {
                Session.Add("Error", ex.ToString());
                Response.Redirect("Error.aspx", false);
            }
        }

        // Toma solo los campos donde el usuario eligió un archivo, en orden (el primero es la imagen principal)
        private List<FotoSubida> LeerFotos()
        {
            List<FotoSubida> fotos = new List<FotoSubida>();
            FileUpload[] campos = { fuFoto1, fuFoto2, fuFoto3, fuFoto4 };

            foreach (FileUpload campo in campos)
            {
                if (campo.HasFile)
                    fotos.Add(new FotoSubida { NombreOriginal = campo.FileName, Contenido = campo.FileBytes });
            }
            return fotos;
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            Response.Redirect("Default.aspx");
        }
    }
}


