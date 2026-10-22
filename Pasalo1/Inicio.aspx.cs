using System;
using System.Collections.Generic;
using System.Globalization;
using dominio;
using negocio;


namespace Pasalo1
{
    public partial class Inicio : System.Web.UI.Page
    {
        private static readonly CultureInfo Es = new CultureInfo("es-AR");

        
        protected ImpactoComunidad Comunidad;

        protected bool Logueado
        {
            get { return Session["Usuario"] != null; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                ImpactoComunidad c = new ImpactoNegocio().ObtenerComunidad();
                if (c != null && c.ObjetosEntregados > 0)
                    Comunidad = c;
            }
            catch (Exception)
            {
                Comunidad = null;
            }
        }

        protected string Kg(decimal kg)
        {
            return kg.ToString("0.##", Es) + " kg";
        }
    }
}
