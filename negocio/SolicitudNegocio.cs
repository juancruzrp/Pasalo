using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using acceso;
using dominio;

namespace negocio
{
    public class SolicitudNegocio
    {
        private readonly SolicitudAcceso acceso =
            new SolicitudAcceso();

        public int Solicitar(int idPublicacion,
                            int idSolicitante,
                            string objetoOfrecido)
        {
            if (idPublicacion <= 0 || idSolicitante <= 0)
                throw new NegocioException(
                    "Los datos de la solicitud no son válidos.");

            dominio.Publicacion publicacion =
                new PublicacionNegocio()
                    .ObtenerPorId(idPublicacion);

            if (publicacion == null)
                throw new NegocioException(
                    "La publicación no existe.");

            if (publicacion.Estado !=
                EstadoPublicacion.Disponible)
                throw new NegocioException(
                    "Este artículo ya no está disponible.");

            if (publicacion.Dueno.IdUsuario == idSolicitante)
                throw new NegocioException(
                    "No podés solicitar tu propio artículo.");

            objetoOfrecido = (objetoOfrecido ?? "").Trim();

            if (publicacion.Modalidad ==
                ModalidadPublicacion.Intercambio)
            {
                if (string.IsNullOrWhiteSpace(objetoOfrecido))
                    throw new NegocioException(
                        "Describí qué objeto ofrecés a cambio.");

                if (objetoOfrecido.Length > 200)
                    throw new NegocioException(
                        "La descripción no puede superar los 200 caracteres.");
            }
            else
            {
                objetoOfrecido = null;
            }

            try
            {
                return acceso.Agregar(
                    idPublicacion,
                    idSolicitante,
                    objetoOfrecido);
            }
            catch (SqlException ex)
            {
                if (ex.Number >= 50001 &&
                    ex.Number <= 50006)
                    throw new NegocioException(ex.Message);

                if (ex.Number == 2601 || ex.Number == 2627)
                    throw new NegocioException(
                        "Ya enviaste una solicitud para este artículo.");

                throw;
            }
        }

        public bool YaSolicito(int idPublicacion,
                               int idSolicitante)
        {
            return acceso.YaSolicito(
                idPublicacion, idSolicitante);
        }



        public List<SolicitudListado> ListarRecibidas(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new NegocioException(
                    "El usuario no es válido.");

            return acceso.ListarRecibidas(idUsuario);
        }

        public List<SolicitudListado> ListarEnviadas(int idUsuario)
        {
            if (idUsuario <= 0)
                throw new NegocioException(
                    "El usuario no es válido.");

            return acceso.ListarEnviadas(idUsuario);
        }



        public void Aceptar(int idSolicitud, int idDueno)
        {
            Resolver(idSolicitud, idDueno, true);
        }

        public void Rechazar(int idSolicitud, int idDueno)
        {
            Resolver(idSolicitud, idDueno, false);
        }

        private void Resolver(int idSolicitud, int idDueno, bool aceptar)
        {
            if (idSolicitud <= 0 || idDueno <= 0)
                throw new NegocioException(
                    "Los datos de la solicitud no son válidos.");

            try
            {
                acceso.Resolver(idSolicitud, idDueno, aceptar);
            }
            catch (SqlException ex)
            {
                if (ex.Number >= 50101 && ex.Number <= 50104)
                    throw new NegocioException(ex.Message);

                throw;
            }
        }


        public void ConfirmarEntrega(int idSolicitud, int idUsuario)
        {
            if (idSolicitud <= 0 || idUsuario <= 0)
                throw new NegocioException(
                    "Los datos de la entrega no son válidos.");

            try
            {
                acceso.ConfirmarEntrega(idSolicitud, idUsuario);
            }
            catch (SqlException ex)
            {
                if (ex.Number >= 50201 && ex.Number <= 50206)
                    throw new NegocioException(ex.Message);

                throw;
            }
        }











    }
}
