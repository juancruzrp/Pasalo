using acceso;
using dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    public class ImpactoNegocio
    {
        public ImpactoComunidad ObtenerComunidad()
        {
            return new ImpactoAcceso().ObtenerComunidad();
        }
    }
}
