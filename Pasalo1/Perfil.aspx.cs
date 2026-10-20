using System;
using System.Collections.Generic;
using System.Globalization;
using dominio;
using negocio;

namespace Pasalo1
{
    public partial class Perfil : System.Web.UI.Page
    {
        private static readonly CultureInfo Es = new CultureInfo("es-AR");

        // Pestaña visible: publicaciones | historial | impacto (por defecto)
        protected string TabActiva = "impacto";

        // true = el usuario está viendo su propio perfil (Perfil.aspx sin ?id=)
        protected bool EsPropio;

        protected PerfilUsuario Datos;
        protected List<PublicacionPerfil> Publicaciones = new List<PublicacionPerfil>();
        protected List<MovimientoHistorial> Historial = new List<MovimientoHistorial>();
        protected List<ImpactoFacultad> Facultades = new List<ImpactoFacultad>();

        // Para que los links de las pestañas conserven el usuario que se está mirando
        protected string SufijoId = "";

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

                // ?id=5 muestra el perfil de otra persona; sin id (o el propio) es "Mi perfil"
                int idPedido;
                int idMostrar = logueado.IdUsuario;
                if (int.TryParse(Request.QueryString["id"], out idPedido) && idPedido > 0)
                    idMostrar = idPedido;

                EsPropio = idMostrar == logueado.IdUsuario;
                SufijoId = EsPropio ? "" : "&id=" + idMostrar;

                PerfilNegocio negocio = new PerfilNegocio();
                Datos = negocio.ObtenerPerfil(idMostrar);

                if (Datos == null)
                {
                    Response.Redirect("Default.aspx", false);
                    Context.ApplicationInstance.CompleteRequest();
                    return;
                }

                Title = EsPropio ? "Mi perfil" : "Perfil de " + Datos.Nombre;

                string tab = Request.QueryString["tab"];
                if (tab == "publicaciones" || (tab == "historial" && EsPropio))
                    TabActiva = tab;

                if (TabActiva == "publicaciones")
                    Publicaciones = negocio.ListarPublicaciones(idMostrar, logueado.IdUsuario);
                else if (TabActiva == "historial")
                    Historial = negocio.ListarHistorial(idMostrar, logueado.IdUsuario);
                else
                    Facultades = negocio.ImpactoPorFacultad();
            }
            catch (Exception ex)
            {
                Session.Add("Error", ex.ToString());
                Response.Redirect("Error.aspx", false);
                Context.ApplicationInstance.CompleteRequest();
            }
        }

        // Devuelve el atributo que marca la pestaña actual (lo lee el lector de pantalla)
        protected string Actual(string tab)
        {
            return TabActiva == tab ? " aria-current=\"page\"" : "";
        }

        // ---------- Cabecera ----------

        protected string Iniciales
        {
            get
            {
                string[] partes = (Datos.Nombre ?? "").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length == 0) return "?";
                if (partes.Length == 1) return partes[0].Substring(0, 1).ToUpperInvariant();
                return (partes[0].Substring(0, 1) + partes[partes.Length - 1].Substring(0, 1)).ToUpperInvariant();
            }
        }

        protected string CarreraYFacultad
        {
            get
            {
                if (string.IsNullOrEmpty(Datos.Carrera)) return "Estudiante";
                if (string.IsNullOrEmpty(Datos.Facultad)) return Datos.Carrera;
                return Datos.Carrera + " · Facultad de " + Datos.Facultad;
            }
        }

        protected string MiembroDesde
        {
            get { return "Miembro desde " + Datos.FechaRegistro.Year; }
        }

        protected bool TieneValoraciones
        {
            get { return Datos.CantidadValoraciones > 0 && Datos.PromedioValoracion.HasValue; }
        }

        protected string PromedioTexto
        {
            get { return TieneValoraciones ? Datos.PromedioValoracion.Value.ToString("0.0", Es) : ""; }
        }

        // Cantidad de estrellas llenas (el promedio redondeado)
        protected int EstrellasLlenas
        {
            get { return TieneValoraciones ? (int)Math.Round((double)Datos.PromedioValoracion.Value, MidpointRounding.AwayFromZero) : 0; }
        }

        protected string ObjetosEntregadosTexto
        {
            get { return Datos.ObjetosEntregados == 1 ? "1 objeto entregado" : Datos.ObjetosEntregados + " objetos entregados"; }
        }

        // ---------- Mis publicaciones ----------

        protected string IconoCategoria(string categoria)
        {
            string n = (categoria ?? "").ToLowerInvariant();
            if (n.StartsWith("ropa")) return "ti-shirt";
            if (n.StartsWith("libro")) return "ti-book";
            if (n.StartsWith("apunte")) return "ti-pencil";
            if (n.StartsWith("electr")) return "ti-device-laptop";
            return "ti-box";
        }

        protected string ClaseEstado(EstadoPublicacion estado)
        {
            if (estado == EstadoPublicacion.Reservado) return "reservado";
            if (estado == EstadoPublicacion.Entregado) return "entregado";
            return "disponible";
        }

        protected string TextoEstado(EstadoPublicacion estado)
        {
            if (estado == EstadoPublicacion.Reservado) return "Reservado";
            if (estado == EstadoPublicacion.Entregado) return "Entregado";
            return "Disponible";
        }

        protected string TextoModalidad(ModalidadPublicacion modalidad)
        {
            return modalidad == ModalidadPublicacion.Intercambio ? "Intercambio" : "Donación";
        }

        // Las fechas se guardan en UTC en la base
        protected string HaceCuanto(DateTime fechaUtc)
        {
            TimeSpan t = DateTime.UtcNow - fechaUtc;
            if (t.TotalMinutes < 1) return "recién";
            if (t.TotalHours < 1) return "hace " + (int)t.TotalMinutes + " min";
            if (t.TotalDays < 1) return "hace " + (int)t.TotalHours + " h";
            int d = (int)t.TotalDays;
            return "hace " + d + (d == 1 ? " día" : " días");
        }

        // "Intercambio · publicada hace 3 días · reservada para X"
        protected string TextoPublicacion(PublicacionPerfil p)
        {
            string texto = TextoModalidad(p.Modalidad) + " · publicada " + HaceCuanto(p.FechaPublicacion);
            if (EsPropio && p.Estado == EstadoPublicacion.Reservado && !string.IsNullOrEmpty(p.Receptor))
                texto += " · reservada para " + p.Receptor;
            else if (EsPropio && p.Estado == EstadoPublicacion.Entregado && !string.IsNullOrEmpty(p.Receptor))
                texto += " · entregada a " + p.Receptor;
            return texto;
        }

        // ---------- Historial ----------

        protected string FechaCorta(DateTime? fechaUtc)
        {
            if (!fechaUtc.HasValue) return "";
            DateTime local = DateTime.SpecifyKind(fechaUtc.Value, DateTimeKind.Utc).ToLocalTime();
            return local.ToString("dd/MM/yyyy", Es);
        }

        // ---------- Impacto ----------

        protected string Kg(decimal kg)
        {
            return kg.ToString("0.##", Es) + " kg";
        }

        protected decimal KgFacultadPropia
        {
            get
            {
                foreach (ImpactoFacultad f in Facultades)
                    if (Datos.IdFacultad.HasValue && f.IdFacultad == Datos.IdFacultad.Value)
                        return f.Kg;
                return 0m;
            }
        }

        // Qué parte del impacto de su facultad aporta esta persona (0 a 100)
        protected int PorcentajeAporte
        {
            get
            {
                decimal total = KgFacultadPropia;
                if (total <= 0m) return 0;
                int p = (int)Math.Round((double)(Datos.KgEvitados / total * 100m), MidpointRounding.AwayFromZero);
                return Math.Min(100, Math.Max(0, p));
            }
        }

        protected string TextoHuella
        {
            get
            {
                if (Datos.KgEvitados <= 0m)
                    return EsPropio
                        ? "Todavía no concretaste ninguna entrega. Cuando entregues tu primer objeto vas a ver acá tu aporte."
                        : "Todavía no concretó ninguna entrega.";

                if (!Datos.IdFacultad.HasValue)
                    return "¡Gracias por ayudar a reducir residuos!";

                return (EsPropio ? "Aportás " : "Aporta ") + PorcentajeAporte +
                       " % de los residuos evitados en " + (EsPropio ? "tu facultad" : "su facultad") + ". ¡Seguí así!";
            }
        }

        // Ancho de cada barra respecto de la facultad con más kg
        protected int AnchoBarra(decimal kg)
        {
            decimal max = 0m;
            foreach (ImpactoFacultad f in Facultades)
                if (f.Kg > max) max = f.Kg;
            if (max <= 0m) return 0;
            return (int)Math.Round((double)(kg / max * 100m), MidpointRounding.AwayFromZero);
        }

        protected bool EsFacultadPropia(ImpactoFacultad f)
        {
            return Datos.IdFacultad.HasValue && f.IdFacultad == Datos.IdFacultad.Value;
        }
    }
}
