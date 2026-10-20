using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

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
    }
}
