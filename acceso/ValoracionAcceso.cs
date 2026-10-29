using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace acceso
{    
    public class ValoracionAcceso
    {
        private const string Conexion =
            "Data Source=localhost\\SQLEXPRESS;" +
            "Initial Catalog=Pasalo;Integrated Security=True";

        public bool YaValoro(int idSolicitud, int idEvaluador)
        {
            using (SqlConnection conexion = new SqlConnection(Conexion))
            using (SqlCommand comando = new SqlCommand(@"
                SELECT COUNT(*)
                FROM Valoracion
                WHERE IdSolicitud = @idSolicitud
                  AND IdEvaluador = @idEvaluador", conexion))
            {
                comando.Parameters.Add("@idSolicitud", SqlDbType.Int).Value = idSolicitud;
                comando.Parameters.Add("@idEvaluador", SqlDbType.Int).Value = idEvaluador;

                conexion.Open();
                return Convert.ToInt32(comando.ExecuteScalar()) > 0;
            }
        }

        public void Registrar(int idSolicitud, int idEvaluador,
                             int puntaje, string comentario)
        {
            using (SqlConnection conexion = new SqlConnection(Conexion))
            {
                conexion.Open();

                using (SqlTransaction transaccion =
                    conexion.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        string sql = @"
                            DECLARE @idPublicacion INT;
                            DECLARE @idDueno INT;
                            DECLARE @idSolicitante INT;
                            DECLARE @idEvaluado INT;
                            DECLARE @estadoSolicitud TINYINT;
                            DECLARE @fechaEntrega DATETIME2;

                            SELECT @idPublicacion = IdPublicacion
                            FROM Solicitud
                            WHERE IdSolicitud = @idSolicitud;

                            IF @idPublicacion IS NULL
                                THROW 50301, 'La solicitud no existe.', 1;

                            SELECT @idDueno = IdUsuario
                            FROM Publicacion WITH (UPDLOCK, HOLDLOCK)
                            WHERE IdPublicacion = @idPublicacion;

                            SELECT
                                @idSolicitante = IdSolicitante,
                                @estadoSolicitud = Estado,
                                @fechaEntrega = FechaEntrega
                            FROM Solicitud WITH (UPDLOCK, HOLDLOCK)
                            WHERE IdSolicitud = @idSolicitud;

                            IF @idEvaluador <> @idDueno
                               AND @idEvaluador <> @idSolicitante
                                THROW 50302,
                                    'No participaste en esta operación.', 1;

                            IF @estadoSolicitud <> 1
                               OR @fechaEntrega IS NULL
                                THROW 50303,
                                    'La entrega todavía no fue completada.', 1;

                            SET @idEvaluado =
                                CASE WHEN @idEvaluador = @idDueno
                                     THEN @idSolicitante
                                     ELSE @idDueno END;

                            IF EXISTS (
                                SELECT 1 FROM Valoracion WITH (UPDLOCK, HOLDLOCK)
                                WHERE IdSolicitud = @idSolicitud
                                  AND IdEvaluador = @idEvaluador
                            )
                                THROW 50304,
                                    'Ya calificaste a este usuario.', 1;

                            INSERT INTO Valoracion
                                (IdSolicitud, IdEvaluador, IdEvaluado,
                                 Puntaje, Comentario)
                            VALUES
                                (@idSolicitud, @idEvaluador, @idEvaluado,
                                 @puntaje, @comentario);";

                        using (SqlCommand comando =
                            new SqlCommand(sql, conexion, transaccion))
                        {
                            comando.Parameters.Add("@idSolicitud", SqlDbType.Int)
                                .Value = idSolicitud;

                            comando.Parameters.Add("@idEvaluador", SqlDbType.Int)
                                .Value = idEvaluador;

                            comando.Parameters.Add("@puntaje", SqlDbType.TinyInt)
                                .Value = puntaje;

                            comando.Parameters.Add("@comentario", SqlDbType.NVarChar, 500)
                                .Value = string.IsNullOrWhiteSpace(comentario)
                                    ? (object)DBNull.Value
                                    : comentario;

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
