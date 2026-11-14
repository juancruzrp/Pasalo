using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
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
                            DECLARE @suspendida BIT;

                            SELECT
                                @estado = Estado,
                                @dueno = IdUsuario,
                                @modalidad = Modalidad,
                                @suspendida = Suspendida
                            FROM Publicacion WITH (UPDLOCK, HOLDLOCK)
                            WHERE IdPublicacion = @idPublicacion;

                            IF @estado IS NULL
                                THROW 50001,
                                    'La publicación no existe.', 1;

                            IF @estado <> 0 OR @suspendida = 1
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
                                AND Estado <> 3 
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

                            DECLARE @idSolicitud INT;

                            INSERT INTO Solicitud
                                (IdPublicacion,
                                 IdSolicitante,
                                 ObjetoOfrecido,
                                 Estado,
                                 FechaRespuesta,
                                 CodigoEntrega)
                            VALUES
                                (@idPublicacion,
                                 @idSolicitante,
                                 @objeto,
                                 CASE WHEN @modalidad = 0 THEN 1 ELSE 0 END,
                                 CASE WHEN @modalidad = 0
                                      THEN SYSUTCDATETIME()
                                      ELSE NULL END,
                                 CASE WHEN @modalidad = 0
                                      THEN @codigoEntrega
                                      ELSE NULL END);

                            SET @idSolicitud = CAST(SCOPE_IDENTITY() AS INT);

                            IF @modalidad = 0
                            BEGIN
                                UPDATE Publicacion
                                SET Estado = 1
                                WHERE IdPublicacion = @idPublicacion;
                            END;

                            SELECT @idSolicitud;";

                        using (SqlCommand comando =
                            new SqlCommand(
                                consulta, conexion, transaccion))
                        {

                            comando.Parameters.Add(
                            "@codigoEntrega", SqlDbType.VarChar, 6)
                            .Value = GenerarCodigoEntrega();

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
                      AND IdSolicitante = @idSolicitante
                      AND Estado <> 3");

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
                s.ConfirmoDuenio,
                s.ConfirmoSolicitante,
                s.FechaEntrega,
                s.MotivoCancelacion,
                s.IdCancelo,
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
                s.CodigoEntrega,
                p.Titulo,
                p.Modalidad,
                s.ObjetoOfrecido,
                s.Estado,
                s.FechaSolicitud,
                s.MotivoRespuesta,
                s.ConfirmoDuenio,
                s.ConfirmoSolicitante,
                s.FechaEntrega,
                s.MotivoCancelacion,
                s.IdCancelo,
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

            solicitud.ConfirmoDuenio =
                Convert.ToBoolean(lector["ConfirmoDuenio"]);

            solicitud.ConfirmoSolicitante =
                Convert.ToBoolean(lector["ConfirmoSolicitante"]);

            solicitud.FechaEntrega =
                lector["FechaEntrega"] == DBNull.Value
                    ? (DateTime?)null
                    : Convert.ToDateTime(lector["FechaEntrega"]);

                    solicitud.MotivoCancelacion =
            lector["MotivoCancelacion"] == DBNull.Value
                ? (int?)null
                : Convert.ToInt32(lector["MotivoCancelacion"]);

                    solicitud.IdCancelo =
                lector["IdCancelo"] == DBNull.Value
                    ? (int?)null
                    : Convert.ToInt32(lector["IdCancelo"]);

            int indiceCodigo;

            try
            {
                indiceCodigo = lector.GetOrdinal("CodigoEntrega");

                solicitud.CodigoEntrega =
                    lector.IsDBNull(indiceCodigo)
                        ? null
                        : lector.GetString(indiceCodigo);
            }
            catch (IndexOutOfRangeException)
            {
                solicitud.CodigoEntrega = null;
            }


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
                            FechaRespuesta = SYSUTCDATETIME(),
                            CodigoEntrega = @codigoEntrega
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

                            comando.Parameters.Add("@codigoEntrega",
                                SqlDbType.VarChar, 6).Value =
                                aceptar ? GenerarCodigoEntrega() : (object)DBNull.Value;

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



        private string GenerarCodigoEntrega()
        {
            const string caracteres = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            char[] codigo = new char[6];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                byte[] numeros = new byte[6];
                rng.GetBytes(numeros);

                for (int i = 0; i < 6; i++)
                {
                    codigo[i] = caracteres[numeros[i] % caracteres.Length];
                }
            }

            return new string(codigo);
        }


        public void ConfirmarEntregaConCodigo(
            int idSolicitud,
            int idDueno,
            string codigo)
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
                    DECLARE @duenoPublicacion INT;
                    DECLARE @estadoPublicacion TINYINT;
                    DECLARE @estadoSolicitud TINYINT;
                    DECLARE @fechaEntrega DATETIME2;
                    DECLARE @codigoGuardado VARCHAR(6);

                    SELECT @idPublicacion = IdPublicacion
                    FROM Solicitud
                    WHERE IdSolicitud = @idSolicitud;

                    IF @idPublicacion IS NULL
                        THROW 50201, 'La solicitud no existe.', 1;

                    SELECT
                        @duenoPublicacion = IdUsuario,
                        @estadoPublicacion = Estado
                    FROM Publicacion WITH (UPDLOCK, HOLDLOCK)
                    WHERE IdPublicacion = @idPublicacion;

                    SELECT
                        @estadoSolicitud = Estado,
                        @fechaEntrega = FechaEntrega,
                        @codigoGuardado = CodigoEntrega
                    FROM Solicitud WITH (UPDLOCK, HOLDLOCK)
                    WHERE IdSolicitud = @idSolicitud;

                    IF @idDueno <> @duenoPublicacion
                        THROW 50202,
                            'No tenés permiso para confirmar esta entrega.', 1;

                    IF @estadoSolicitud <> 1
                        THROW 50203,
                            'La solicitud no está aceptada.', 1;

                    IF @fechaEntrega IS NOT NULL
                        THROW 50204,
                            'Esta entrega ya fue completada.', 1;

                    IF @estadoPublicacion <> 1
                        THROW 50205,
                            'La publicación no está reservada.', 1;

                    IF @codigoGuardado IS NULL
                       OR @codigoGuardado COLLATE Latin1_General_100_BIN2
                          <> @codigo COLLATE Latin1_General_100_BIN2
                        THROW 50207,
                            'El código de entrega es incorrecto.', 1;

                    UPDATE Solicitud
                    SET FechaEntrega = SYSUTCDATETIME(),
                        ConfirmoDuenio = 1,
                        ConfirmoSolicitante = 1,
                        CodigoEntrega = NULL
                    WHERE IdSolicitud = @idSolicitud;

                    UPDATE Publicacion
                    SET Estado = 2
                    WHERE IdPublicacion = @idPublicacion;
                ";

                        using (SqlCommand comando =
                            new SqlCommand(sql, conexion, transaccion))
                        {
                            comando.Parameters.Add(
                                "@idSolicitud", SqlDbType.Int).Value = idSolicitud;

                            comando.Parameters.Add(
                                "@idDueno", SqlDbType.Int).Value = idDueno;

                            comando.Parameters.Add(
                                "@codigo", SqlDbType.VarChar, 6).Value = codigo;

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



        // Cancela un intercambio aceptado y libera la publicación.
     
        public void Cancelar(int idSolicitud, int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.SetearConsulta(@"
                    SET XACT_ABORT ON;
                    BEGIN TRANSACTION;

                    DECLARE @idPublicacion INT;
                    DECLARE @idSolicitante INT;
                    DECLARE @dueno INT;
                    DECLARE @estado TINYINT;
                    DECLARE @fechaEntrega DATETIME2;

                    SELECT @idPublicacion = IdPublicacion,
                           @idSolicitante = IdSolicitante
                    FROM Solicitud
                    WHERE IdSolicitud = @idSolicitud;

                    IF @idPublicacion IS NULL
                        THROW 50301, 'La solicitud no existe.', 1;

                    SELECT @dueno = IdUsuario
                    FROM Publicacion WITH (UPDLOCK, HOLDLOCK)
                    WHERE IdPublicacion = @idPublicacion;

                    IF @idUsuario <> @dueno AND @idUsuario <> @idSolicitante
                        THROW 50302, 'No tenés permiso para cancelar este intercambio.', 1;

                    SELECT @estado = Estado, @fechaEntrega = FechaEntrega
                    FROM Solicitud WITH (UPDLOCK, HOLDLOCK)
                    WHERE IdSolicitud = @idSolicitud;

                    IF @estado <> 1
                        THROW 50303, 'Solo se puede cancelar un intercambio aceptado.', 1;

                    IF @fechaEntrega IS NOT NULL
                        THROW 50304, 'La entrega ya fue completada.', 1;

                    UPDATE Solicitud
                    SET Estado = 3,
                        FechaCancelacion = SYSUTCDATETIME(),
                        MotivoCancelacion = 1,
                        IdCancelo = @idUsuario,
                        CancelacionVista = 0,
                        CodigoEntrega = NULL
                    WHERE IdSolicitud = @idSolicitud;

                    UPDATE Publicacion
                    SET Estado = 0
                    WHERE IdPublicacion = @idPublicacion
                      AND Estado = 1;

                    COMMIT TRANSACTION;");

                datos.SetearParametro("@idSolicitud", idSolicitud);
                datos.SetearParametro("@idUsuario", idUsuario);
                datos.EjecutarAccion();
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Cancela los intercambios aceptados que llevan más de N días sin entregarse.
        // Devuelve cuántos canceló.
        public int VencerInactivas(int dias)
        {
            AccesoDatos datos = new AccesoDatos();

            try
            {
                datos.SetearConsulta(@"
                    SET XACT_ABORT ON;
                    BEGIN TRANSACTION;

                    DECLARE @vencidas TABLE (IdSolicitud INT, IdPublicacion INT);

                    UPDATE Solicitud
                    SET Estado = 3,
                        FechaCancelacion = SYSUTCDATETIME(),
                        MotivoCancelacion = 2,
                        IdCancelo = NULL,
                        CancelacionVista = 0,
                        CodigoEntrega = NULL
                    OUTPUT inserted.IdSolicitud, inserted.IdPublicacion INTO @vencidas
                    WHERE Estado = 1
                      AND FechaEntrega IS NULL
                      AND FechaRespuesta IS NOT NULL
                      AND FechaRespuesta < DATEADD(DAY, -@dias, SYSUTCDATETIME());

                    UPDATE p
                    SET p.Estado = 0
                    FROM Publicacion p
                    INNER JOIN @vencidas v ON v.IdPublicacion = p.IdPublicacion
                    WHERE p.Estado = 1;

                    COMMIT TRANSACTION;
                    SELECT COUNT(*) FROM @vencidas;");

                datos.SetearParametro("@dias", dias);
                return Convert.ToInt32(datos.EjecutarScalar());
            }
            finally
            {
                datos.CerrarConexion();
            }
        }






    }
}
