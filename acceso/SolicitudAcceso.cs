using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using dominio;

namespace acceso
{
    public class SolicitudAcceso
    {
        public int Agregar(int idPublicacion,
                           int idSolicitante,
                           string objetoOfrecido)
        {
            using (SqlConnection conexion =
                new SqlConnection(
                    "Data Source=localhost\\SQLEXPRESS;" +
                    "Initial Catalog=Pasalo;" +
                    "Integrated Security=True"))
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                    conexion.BeginTransaction(
                        IsolationLevel.Serializable))
                {
                    try
                    {
                        string consulta = @"
                            DECLARE @estado TINYINT;
                            DECLARE @dueno INT;
                            DECLARE @modalidad TINYINT;

                            SELECT
                                @estado = Estado,
                                @dueno = IdUsuario,
                                @modalidad = Modalidad
                            FROM Publicacion WITH (UPDLOCK, HOLDLOCK)
                            WHERE IdPublicacion = @idPublicacion;

                            IF @estado IS NULL
                                THROW 50001,
                                    'La publicación no existe.', 1;

                            IF @estado <> 0
                                THROW 50002,
                                    'El artículo ya no está disponible.', 1;

                            IF @dueno = @idSolicitante
                                THROW 50003,
                                    'No podés solicitar tu propia publicación.', 1;

                            IF EXISTS (
                                SELECT 1
                                FROM Solicitud
                                WHERE IdPublicacion = @idPublicacion
                                  AND IdSolicitante = @idSolicitante
                            )
                                THROW 50004,
                                    'Ya enviaste una solicitud para este artículo.', 1;

                            IF @modalidad = 1 AND
                               NULLIF(LTRIM(RTRIM(@objeto)), '') IS NULL
                                THROW 50005,
                                    'Tenés que describir qué ofrecés a cambio.', 1;

                            IF @modalidad = 0 AND @objeto IS NOT NULL
                                THROW 50006,
                                    'Una donación no requiere un objeto a cambio.', 1;

                            INSERT INTO Solicitud
                                (IdPublicacion,
                                 IdSolicitante,
                                 ObjetoOfrecido)
                            VALUES
                                (@idPublicacion,
                                 @idSolicitante,
                                 @objeto);

                            SELECT CAST(SCOPE_IDENTITY() AS INT);";

                        using (SqlCommand comando =
                            new SqlCommand(
                                consulta, conexion, transaccion))
                        {
                            comando.Parameters.Add(
                                "@idPublicacion", SqlDbType.Int)
                                .Value = idPublicacion;

                            comando.Parameters.Add(
                                "@idSolicitante", SqlDbType.Int)
                                .Value = idSolicitante;

                            comando.Parameters.Add(
                                "@objeto", SqlDbType.NVarChar, 200)
                                .Value = (object)objetoOfrecido
                                         ?? DBNull.Value;

                            int id = Convert.ToInt32(
                                comando.ExecuteScalar());

                            transaccion.Commit();
                            return id;
                        }
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }

        public bool YaSolicito(int idPublicacion,
                               int idSolicitante)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.SetearConsulta(@"
                    SELECT COUNT(*)
                    FROM Solicitud
                    WHERE IdPublicacion = @idPublicacion
                      AND IdSolicitante = @idSolicitante");

                datos.SetearParametro(
                    "@idPublicacion", idPublicacion);

                datos.SetearParametro(
                    "@idSolicitante", idSolicitante);

                return Convert.ToInt32(
                    datos.EjecutarScalar()) > 0;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }



        public List<SolicitudListado> ListarRecibidas(int idUsuario)
        {
            List<SolicitudListado> lista =
                new List<SolicitudListado>();

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.SetearConsulta(@"
            SELECT
                s.IdSolicitud,
                s.IdPublicacion,
                p.Titulo,
                p.Modalidad,
                s.ObjetoOfrecido,
                s.Estado,
                s.FechaSolicitud,
                s.MotivoRespuesta,  
                u.Nombre AS NombreUsuario
            FROM Solicitud s
            INNER JOIN Publicacion p
                ON p.IdPublicacion = s.IdPublicacion
            INNER JOIN Usuario u
                ON u.IdUsuario = s.IdSolicitante
            WHERE p.IdUsuario = @idUsuario
            ORDER BY
                CASE WHEN s.Estado = 0 THEN 0 ELSE 1 END,
                s.FechaSolicitud DESC");

                datos.SetearParametro("@idUsuario", idUsuario);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(MapearSolicitud(datos.Lector));
                }
            }
            finally
            {
                datos.CerrarConexion();
            }

            return lista;
        }

        public List<SolicitudListado> ListarEnviadas(int idUsuario)
        {
            List<SolicitudListado> lista =
                new List<SolicitudListado>();

            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.SetearConsulta(@"
            SELECT
                s.IdSolicitud,
                s.IdPublicacion,
                p.Titulo,
                p.Modalidad,
                s.ObjetoOfrecido,
                s.Estado,
                s.FechaSolicitud,
                s.MotivoRespuesta,  
                u.Nombre AS NombreUsuario
            FROM Solicitud s
            INNER JOIN Publicacion p
                ON p.IdPublicacion = s.IdPublicacion
            INNER JOIN Usuario u
                ON u.IdUsuario = p.IdUsuario
            WHERE s.IdSolicitante = @idUsuario
            ORDER BY
                CASE WHEN s.Estado = 0 THEN 0 ELSE 1 END,
                s.FechaSolicitud DESC");

                datos.SetearParametro("@idUsuario", idUsuario);
                datos.EjecutarLectura();

                while (datos.Lector.Read())
                {
                    lista.Add(MapearSolicitud(datos.Lector));
                }
            }
            finally
            {
                datos.CerrarConexion();
            }

            return lista;
        }

        private SolicitudListado MapearSolicitud(SqlDataReader lector)
        {
            SolicitudListado solicitud = new SolicitudListado();

            solicitud.IdSolicitud =
                Convert.ToInt32(lector["IdSolicitud"]);

            solicitud.IdPublicacion =
                Convert.ToInt32(lector["IdPublicacion"]);

            solicitud.TituloPublicacion =
                Convert.ToString(lector["Titulo"]);

            solicitud.Modalidad =
                Convert.ToInt32(lector["Modalidad"]) == 0
                ? "Donación"
                : "Intercambio";

            solicitud.ObjetoOfrecido =
                lector["ObjetoOfrecido"] == DBNull.Value
                ? null
                : Convert.ToString(lector["ObjetoOfrecido"]);

            solicitud.NombreUsuario =
                Convert.ToString(lector["NombreUsuario"]);

            solicitud.Estado = (EstadoSolicitud)
                Convert.ToByte(lector["Estado"]);

            solicitud.FechaSolicitud =
                Convert.ToDateTime(lector["FechaSolicitud"]);

            solicitud.MotivoRespuesta =
                lector["MotivoRespuesta"] == DBNull.Value
                ? (int?)null
                : Convert.ToInt32(lector["MotivoRespuesta"]);


            return solicitud;
        }





        public void Resolver(int idSolicitud, int idDueno, bool aceptar)
        {
            using (SqlConnection conexion = new SqlConnection(
                "Data Source=localhost\\SQLEXPRESS;" +
                "Initial Catalog=Pasalo;Integrated Security=True"))
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                    conexion.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        string sql = @"
                    DECLARE @idPublicacion INT;
                    DECLARE @estadoPublicacion TINYINT;
                    DECLARE @estadoSolicitud TINYINT;

                    -- Identificar la publicación.
                    SELECT @idPublicacion = IdPublicacion
                    FROM Solicitud
                    WHERE IdSolicitud = @idSolicitud;

                    IF @idPublicacion IS NULL
                        THROW 50101, 'La solicitud no existe.', 1;

                    -- Bloquear la publicación para evitar
                    -- aceptaciones simultáneas.
                    SELECT @estadoPublicacion = Estado
                    FROM Publicacion WITH (UPDLOCK, HOLDLOCK)
                    WHERE IdPublicacion = @idPublicacion
                      AND IdUsuario = @idDueno;

                    IF @estadoPublicacion IS NULL
                        THROW 50102,
                            'No tenés permiso para gestionar esta solicitud.', 1;

                    IF @estadoPublicacion <> 0
                        THROW 50103,
                            'La publicación ya no está disponible.', 1;

                    SELECT @estadoSolicitud = Estado
                    FROM Solicitud WITH (UPDLOCK, HOLDLOCK)
                    WHERE IdSolicitud = @idSolicitud
                      AND IdPublicacion = @idPublicacion;

                    IF @estadoSolicitud <> 0
                        THROW 50104,
                            'La solicitud ya fue respondida.', 1;

               IF @aceptar = 1
        BEGIN
            UPDATE Solicitud
            SET Estado = 1,
                FechaRespuesta = SYSUTCDATETIME()
            WHERE IdSolicitud = @idSolicitud;

            -- Las demás pendientes se rechazan solas (motivo 2: se eligió a otra persona)
            UPDATE Solicitud
            SET Estado = 2,
                MotivoRespuesta = 2,
                FechaRespuesta = SYSUTCDATETIME()
            WHERE IdPublicacion = @idPublicacion
              AND IdSolicitud <> @idSolicitud
              AND Estado = 0;

            UPDATE Publicacion
            SET Estado = 1
            WHERE IdPublicacion = @idPublicacion;
        END
        ELSE
        BEGIN
            -- Motivo 1: el dueño la rechazó
            UPDATE Solicitud
            SET Estado = 2,
                MotivoRespuesta = 1,
                FechaRespuesta = SYSUTCDATETIME()
            WHERE IdSolicitud = @idSolicitud;
        END";

                        using (SqlCommand comando = new SqlCommand(
                            sql, conexion, transaccion))
                        {
                            comando.Parameters.Add("@idSolicitud",
                                SqlDbType.Int).Value = idSolicitud;

                            comando.Parameters.Add("@idDueno",
                                SqlDbType.Int).Value = idDueno;

                            comando.Parameters.Add("@aceptar",
                                SqlDbType.Bit).Value = aceptar;

                            comando.ExecuteNonQuery();
                        }

                        transaccion.Commit();
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
        }










    }
}
