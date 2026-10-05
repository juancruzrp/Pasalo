using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Pasalo1
{
    public partial class DetallePublicacion : System.Web.UI.Page
    {
        // VISTA PREVIA (temporal, hasta tener la base de datos):
        //   DetallePublicacion.aspx?estado=reservado | entregado      -> cambia el estado
        //   DetallePublicacion.aspx?modalidad=intercambio              -> muestra el campo "¿Qué ofrecés a cambio?"
        protected string Estado = "disponible";
        protected string Modalidad = "donacion";

        protected void Page_Load(object sender, EventArgs e)
        {
            string est = Request.QueryString["estado"];
            if (est == "reservado" || est == "entregado")
                Estado = est;

            if (Request.QueryString["modalidad"] == "intercambio")
                Modalidad = "intercambio";
        }

        protected string EstadoTexto
        {
            get
            {
                if (Estado == "reservado") return "Reservado";
                if (Estado == "entregado") return "Entregado";
                return "Disponible";
            }
        }

        protected string ModalidadTexto
        {
            get { return Modalidad == "intercambio" ? "Intercambio" : "Donación"; }
        }
    }
}
