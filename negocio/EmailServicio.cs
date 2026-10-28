
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Mail;
    using System.Text;
    using System.Threading.Tasks;
    using System.Configuration;

    namespace acceso
    {// SmtpModo = "Carpeta" -> guarda el correo como archivo.eml en una carpeta(desarrollo: no envía nada)
     // SmtpModo = "Real"    -> envía por SMTP con los datos de appSettings / secrets.config
        public class EmailServicio
        {
            // 'enlace' se usa solo en modo Carpeta: queda en un archivo de texto para copiarlo fácil
            public void Enviar(string destino, string asunto, string cuerpoHtml, string enlace)
            {
                string remitente = ConfigurationManager.AppSettings["CorreoRemitente"] ?? "no-reply@pasalo.local";
                string modo = ConfigurationManager.AppSettings["SmtpModo"] ?? "Carpeta";
                bool real = string.Equals(modo, "Real", StringComparison.OrdinalIgnoreCase);

                using (var msg = new MailMessage(remitente, destino, asunto, cuerpoHtml))
                using (var smtp = new SmtpClient())
                {
                    msg.IsBodyHtml = true;
                    msg.BodyEncoding = Encoding.UTF8;
                    msg.SubjectEncoding = Encoding.UTF8;

                    if (real)
                    {
                        smtp.Host = ConfigurationManager.AppSettings["SmtpHost"];
                        smtp.Port = int.Parse(ConfigurationManager.AppSettings["SmtpPuerto"] ?? "587");
                        smtp.EnableSsl = true;
                        smtp.Credentials = new NetworkCredential(
                            ConfigurationManager.AppSettings["SmtpUsuario"],
                            ConfigurationManager.AppSettings["SmtpClave"]);
                        smtp.Send(msg);
                        return;
                    }

                    string carpeta = ConfigurationManager.AppSettings["CarpetaCorreos"] ?? @"C:\PasaloCorreos";
                    Directory.CreateDirectory(carpeta);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                    smtp.PickupDirectoryLocation = carpeta;
                    smtp.Send(msg);

                    File.WriteAllText(Path.Combine(carpeta, "ultimo-enlace.txt"), enlace ?? "");
                }
            }
        }
    }
