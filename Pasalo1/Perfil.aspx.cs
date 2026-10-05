using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pasalo1
{
    public partial class Perfil : System.Web.UI.Page
    {
        // Pestaña visible: publicaciones | historial | impacto (por defecto)
        protected string TabActiva = "impacto";

        protected void Page_Load(object sender, EventArgs e)
        {
            string tab = Request.QueryString["tab"];
            if (tab == "publicaciones" || tab == "historial")
                TabActiva = tab;
        }

        // Devuelve el atributo que marca la pestaña actual (lo lee el lector de pantalla)
        protected string Actual(string tab)
        {
            return TabActiva == tab ? " aria-current=\"page\"" : "";
        }
    }
}
