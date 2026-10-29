using acceso;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace negocio
{
    public class ValoracionNegocio
    {
        private readonly ValoracionAcceso acceso = new ValoracionAcceso();

        public bool YaValoro(int idSolicitud, int idUsuario)
        {
            if (idSolicitud <= 0 || idUsuario <= 0)
                return false;

            return acceso.YaValoro(idSolicitud, idUsuario);
        }

        public void Registrar(int idSolicitud, int idUsuario,
                             int puntaje, string comentario)
        {
            if (idSolicitud <= 0 || idUsuario <= 0)
                throw new NegocioException("Datos de valoración inválidos.");

            if (puntaje < 1 || puntaje > 5)
                throw new NegocioException("Seleccioná entre 1 y 5 estrellas.");

            comentario = (comentario ?? "").Trim();

            if (comentario.Length > 500)
                throw new NegocioException(
                    "El comentario no puede superar los 500 caracteres.");

            try
            {
                acceso.Registrar(idSolicitud, idUsuario, puntaje, comentario);
            }
            catch (SqlException ex)
            {
                if (ex.Number >= 50301 && ex.Number <= 50304)
                    throw new NegocioException(ex.Message);

                if (ex.Number == 2601 || ex.Number == 2627)
                    throw new NegocioException(
                        "Ya registraste una valoración para esta operación.");

                throw;
            }
        }
    }
}
