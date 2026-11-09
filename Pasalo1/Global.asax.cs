using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Pasalo1
{
    public class Global : HttpApplication
    {
        void Application_Start(object sender, EventArgs e)
        {
            // Código que se ejecuta al iniciar la aplicación
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        // Errores sin capturar. Lo que no se trata acá lo resuelve <customErrors> del Web.config (va a Error.aspx).
        void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            if (ex == null)
                return;

            // El detalle técnico queda en la ventana Salida de Visual Studio; el usuario nunca lo ve.
            System.Diagnostics.Debug.WriteLine(ex.ToString());

            // Texto con <etiquetas> en un formulario: ASP.NET lo bloquea (es una defensa contra XSS).
            // En vez de la pantalla amarilla, se explica qué pasó.
            if (ex is HttpRequestValidationException ||
                (ex.InnerException is HttpRequestValidationException))
            {
                Server.ClearError();
                Response.Redirect("~/Error.aspx?c=texto", false);
                CompleteRequest();
            }
        }
    }
}