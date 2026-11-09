using System;
using System.Collections.Generic;
using System.Web.UI.WebControls;
using dominio;
using negocio;

namespace Pasalo1
{
    // Panel de moderación. Solo entra quien es moderador ACTIVO según la base (no según lo que diga la sesión).
    // Los ids de las filas llegan por CommandArgument/HiddenField, pero cada acción se revalida en ModeracionNegocio.
    public partial class Moderacion : System.Web.UI.Page
    {
        private readonly ModeracionNegocio neg = new ModeracionNegocio();
        private Usuario actor;
        private string tab;

        protected void Page_Load(object sender, EventArgs e)
        {
            actor = Session["Usuario"] as Usuario;
            if (actor == null)
            {
                Response.Redirect("Login.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
                return;
            }

            try
            {
                if (!neg.EsModerador(actor))
                {
                    Response.Redirect("Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                tab = (Request.QueryString["t"] ?? "usuarios").ToLowerInvariant();
                if (tab != "publicaciones" && tab != "filtros" && tab != "reportes")
                    tab = "usuarios";

                pnlUsuarios.Visible = tab == "usuarios";
                pnlPublicaciones.Visible = tab == "publicaciones";
                pnlFiltros.Visible = tab == "filtros";
                pnlReportes.Visible = tab == "reportes";

                Marcar(tabUsuarios, tab == "usuarios");
                Marcar(tabPublicaciones, tab == "publicaciones");
                Marcar(tabFiltros, tab == "filtros");
                Marcar(tabReportes, tab == "reportes");

                if (!IsPostBack)
                {
                    if (tab == "usuarios") txtBuscarU.Text = Request.QueryString["q"] ?? "";
                    if (tab == "publicaciones") txtBuscarP.Text = Request.QueryString["q"] ?? "";
                    CargarListas();
                    Refrescar();
                }
            }
            catch (NegocioException ex)
            {
                Error(ex.Message);
            }
        }

        private static void Marcar(System.Web.UI.HtmlControls.HtmlAnchor a, bool activa)
        {
            if (activa) a.Attributes["aria-current"] = "page";
        }

        // ---------- ayudas ----------
        private void Ok(string texto)
        {
            lblOk.Text = Server.HtmlEncode(texto);
            pnlOk.Visible = true;
            pnlError.Visible = false;
        }

        private void Error(string texto)
        {
            lblError.Text = Server.HtmlEncode(texto);
            pnlError.Visible = true;
            pnlOk.Visible = false;
        }

        // Ejecuta una acción; los errores de regla se muestran, los demás van a Error.aspx.
        private void Hacer(Action accion, string okTexto)
        {
            try
            {
                accion();
                Ok(okTexto);
            }
            catch (NegocioException ex)
            {
                Error(ex.Message);
            }
            Refrescar();
        }

        private static int Entero(object valor)
        {
            int n;
            return int.TryParse(Convert.ToString(valor), out n) ? n : 0;
        }

        private static int? EnteroNulo(string valor)
        {
            int n;
            return int.TryParse(valor, out n) && n > 0 ? n : (int?)null;
        }

        private void CargarListas()
        {
            string selCarU = ddlCarU.SelectedValue, selCatP = ddlCatP.SelectedValue, selCarP = ddlCarP.SelectedValue;
            string selFacEd = ddlEdCarrFac.SelectedValue, selFacNew = ddlNuevaCarrFac.SelectedValue;

            List<ModCarrera> carreras = neg.ListarCarreras(actor);
            ddlCarU.Items.Clear();
            ddlCarP.Items.Clear();
            ddlCarU.Items.Add(new ListItem("Sin carrera", "0"));
            ddlCarP.Items.Add(new ListItem("Sin carrera", "0"));
            foreach (ModCarrera c in carreras)
            {
                ddlCarU.Items.Add(new ListItem(c.Nombre, c.Id.ToString()));
                ddlCarP.Items.Add(new ListItem(c.Nombre, c.Id.ToString()));
            }

            ddlCatP.Items.Clear();
            foreach (ModCategoria c in neg.ListarCategorias(actor))
                ddlCatP.Items.Add(new ListItem(c.Nombre, c.Id.ToString()));

            ddlEdCarrFac.Items.Clear();
            ddlNuevaCarrFac.Items.Clear();
            foreach (ModFacultad f in neg.ListarFacultades(actor))
            {
                ddlEdCarrFac.Items.Add(new ListItem(f.Nombre, f.Id.ToString()));
                ddlNuevaCarrFac.Items.Add(new ListItem(f.Nombre, f.Id.ToString()));
            }

            Restaurar(ddlCarU, selCarU); Restaurar(ddlCatP, selCatP); Restaurar(ddlCarP, selCarP);
            Restaurar(ddlEdCarrFac, selFacEd); Restaurar(ddlNuevaCarrFac, selFacNew);
        }

        private static void Restaurar(DropDownList ddl, string valor)
        {
            if (!string.IsNullOrEmpty(valor) && ddl.Items.FindByValue(valor) != null)
                ddl.SelectedValue = valor;
        }

        private void Refrescar()
        {
            int pendientes = neg.ReportesPendientes(actor);
            lblPendientes.Text = pendientes.ToString();
            lblPendientes.Visible = pendientes > 0;

            if (tab == "usuarios")
            {
                List<ModUsuario> l = neg.ListarUsuarios(actor, txtBuscarU.Text);
                rptUsuarios.DataSource = l;
                rptUsuarios.DataBind();
                pnlVacioU.Visible = l.Count == 0;
            }
            else if (tab == "publicaciones")
            {
                List<ModPublicacion> l = neg.ListarPublicaciones(actor, txtBuscarP.Text);
                rptPublicaciones.DataSource = l;
                rptPublicaciones.DataBind();
                pnlVacioP.Visible = l.Count == 0;
            }
            else if (tab == "filtros")
            {
                rptCategorias.DataSource = neg.ListarCategorias(actor);
                rptCategorias.DataBind();
                rptFacultades.DataSource = neg.ListarFacultades(actor);
                rptFacultades.DataBind();
                rptCarreras.DataSource = neg.ListarCarreras(actor);
                rptCarreras.DataBind();
            }
            else
            {
                List<ModReporte> l = neg.ListarReportes(actor, chkTodosRep.Checked);
                rptReportes.DataSource = l;
                rptReportes.DataBind();
                pnlVacioR.Visible = l.Count == 0;
            }
        }

        protected void btnCancelar_Click(object sender, EventArgs e)
        {
            pnlEditU.Visible = false;
            pnlEditP.Visible = false;
            pnlEditCat.Visible = false;
            pnlEditFac.Visible = false;
            pnlEditCarr.Visible = false;
        }

        // ---------- usuarios ----------
        protected void btnBuscarU_Click(object sender, EventArgs e)
        {
            Refrescar();
        }

        protected void rptUsuarios_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Entero(e.CommandArgument);
            try
            {
                switch (e.CommandName)
                {
                    case "editar":
                        ModUsuario u = neg.ObtenerUsuario(actor, id);
                        if (u == null) throw new NegocioException("El usuario ya no existe.");
                        hfIdU.Value = u.IdUsuario.ToString();
                        lblCorreoU.Text = Server.HtmlEncode(u.Email);
                        txtNomU.Text = u.Nombre;
                        txtDescU.Text = u.Descripcion;
                        Restaurar(ddlCarU, u.IdCarrera.HasValue ? u.IdCarrera.Value.ToString() : "0");
                        pnlEditU.Visible = true;
                        pnlOk.Visible = false; pnlError.Visible = false;
                        break;
                    case "suspender":
                        bool activo = neg.AlternarSuspension(actor, id);
                        Ok(activo ? "Cuenta reactivada." : "Cuenta suspendida.");
                        break;
                    case "rol":
                        bool esMod = neg.AlternarModerador(actor, id);
                        Ok(esMod ? "Ahora es moderador." : "Ya no es moderador.");
                        break;
                    case "verificar":
                        neg.VerificarCorreo(actor, id);
                        Ok("Correo marcado como verificado.");
                        break;
                }
            }
            catch (NegocioException ex)
            {
                Error(ex.Message);
            }
            Refrescar();
        }

        protected void btnGuardarU_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.EditarUsuario(actor, Entero(hfIdU.Value), txtNomU.Text, EnteroNulo(ddlCarU.SelectedValue), txtDescU.Text);
                pnlEditU.Visible = false;
            }, "Usuario actualizado.");
        }

        // ---------- publicaciones ----------
        protected void btnBuscarP_Click(object sender, EventArgs e)
        {
            Refrescar();
        }

        protected void rptPublicaciones_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Entero(e.CommandArgument);
            try
            {
                switch (e.CommandName)
                {
                    case "editar":
                        ModPublicacion p = neg.ObtenerPublicacion(actor, id);
                        if (p == null) throw new NegocioException("La publicación ya no existe.");
                        hfIdP.Value = p.IdPublicacion.ToString();
                        txtTitP.Text = p.Titulo;
                        txtDescP.Text = p.Descripcion;
                        Restaurar(ddlCatP, p.IdCategoria.ToString());
                        Restaurar(ddlCarP, p.IdCarrera.HasValue ? p.IdCarrera.Value.ToString() : "0");
                        pnlEditP.Visible = true;
                        pnlOk.Visible = false; pnlError.Visible = false;
                        break;
                    case "suspender":
                        bool susp = neg.AlternarSuspensionPublicacion(actor, id);
                        Ok(susp ? "Publicación suspendida." : "Publicación reactivada.");
                        break;
                    case "eliminar":
                        neg.EliminarPublicacion(actor, id);
                        Ok("Publicación eliminada.");
                        break;
                }
            }
            catch (NegocioException ex)
            {
                Error(ex.Message);
            }
            Refrescar();
        }

        protected void btnGuardarP_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.EditarPublicacion(actor, Entero(hfIdP.Value), txtTitP.Text, txtDescP.Text,
                    Entero(ddlCatP.SelectedValue), EnteroNulo(ddlCarP.SelectedValue));
                pnlEditP.Visible = false;
            }, "Publicación actualizada.");
        }

        // ---------- categorías ----------
        protected void rptCategorias_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Entero(e.CommandArgument);
            if (e.CommandName == "editar")
            {
                foreach (ModCategoria c in neg.ListarCategorias(actor))
                    if (c.Id == id)
                    {
                        hfIdCat.Value = c.Id.ToString();
                        txtEdCat.Text = c.Nombre;
                        txtEdPeso.Text = c.PesoKg.ToString("0.00");
                        pnlEditCat.Visible = true;
                    }
                return;
            }
            Hacer(() => neg.EliminarCategoria(actor, id), "Categoría eliminada.");
            CargarListas();
        }

        protected void btnGuardarCat_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.EditarCategoria(actor, Entero(hfIdCat.Value), txtEdCat.Text, txtEdPeso.Text);
                pnlEditCat.Visible = false;
            }, "Categoría actualizada.");
            CargarListas();
        }

        protected void btnAgregarCat_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.AgregarCategoria(actor, txtNuevaCat.Text, txtNuevoPeso.Text);
                txtNuevaCat.Text = "";
            }, "Categoría agregada.");
            CargarListas();
        }

        // ---------- facultades ----------
        protected void rptFacultades_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Entero(e.CommandArgument);
            if (e.CommandName == "editar")
            {
                foreach (ModFacultad f in neg.ListarFacultades(actor))
                    if (f.Id == id)
                    {
                        hfIdFac.Value = f.Id.ToString();
                        txtEdFac.Text = f.Nombre;
                        pnlEditFac.Visible = true;
                    }
                return;
            }
            Hacer(() => neg.EliminarFacultad(actor, id), "Facultad eliminada.");
            CargarListas();
        }

        protected void btnGuardarFac_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.EditarFacultad(actor, Entero(hfIdFac.Value), txtEdFac.Text);
                pnlEditFac.Visible = false;
            }, "Facultad actualizada.");
            CargarListas();
        }

        protected void btnAgregarFac_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.AgregarFacultad(actor, txtNuevaFac.Text);
                txtNuevaFac.Text = "";
            }, "Facultad agregada.");
            CargarListas();
        }

        // ---------- carreras ----------
        protected void rptCarreras_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Entero(e.CommandArgument);
            if (e.CommandName == "editar")
            {
                foreach (ModCarrera c in neg.ListarCarreras(actor))
                    if (c.Id == id)
                    {
                        hfIdCarr.Value = c.Id.ToString();
                        txtEdCarr.Text = c.Nombre;
                        Restaurar(ddlEdCarrFac, c.IdFacultad.ToString());
                        pnlEditCarr.Visible = true;
                    }
                return;
            }
            Hacer(() => neg.EliminarCarrera(actor, id), "Carrera eliminada.");
            CargarListas();
        }

        protected void btnGuardarCarr_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.EditarCarrera(actor, Entero(hfIdCarr.Value), txtEdCarr.Text, Entero(ddlEdCarrFac.SelectedValue));
                pnlEditCarr.Visible = false;
            }, "Carrera actualizada.");
            CargarListas();
        }

        protected void btnAgregarCarr_Click(object sender, EventArgs e)
        {
            Hacer(() =>
            {
                neg.AgregarCarrera(actor, txtNuevaCarr.Text, Entero(ddlNuevaCarrFac.SelectedValue));
                txtNuevaCarr.Text = "";
            }, "Carrera agregada.");
            CargarListas();
        }

        // ---------- reportes ----------
        protected void chkTodosRep_CheckedChanged(object sender, EventArgs e)
        {
            Refrescar();
        }

        protected void rptReportes_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int id = Entero(e.CommandArgument);
            try
            {
                switch (e.CommandName)
                {
                    case "resuelto":
                        neg.ResolverReporte(actor, id, true);
                        Ok("Reporte marcado como resuelto.");
                        break;
                    case "desestimado":
                        neg.ResolverReporte(actor, id, false);
                        Ok("Reporte desestimado.");
                        break;
                    case "irPub":
                        ModPublicacion p = neg.ObtenerPublicacion(actor, id);
                        if (p == null) throw new NegocioException("La publicación ya no existe (quizás ya se borró).");
                        Response.Redirect("Moderacion.aspx?t=publicaciones&q=" + Server.UrlEncode(p.Titulo), false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                    case "irUsu":
                        ModUsuario u = neg.ObtenerUsuario(actor, id);
                        if (u == null) throw new NegocioException("El usuario ya no existe.");
                        Response.Redirect("Moderacion.aspx?t=usuarios&q=" + Server.UrlEncode(u.Email), false);
                        Context.ApplicationInstance.CompleteRequest();
                        return;
                }
            }
            catch (NegocioException ex)
            {
                Error(ex.Message);
            }
            Refrescar();
        }
    }
}
