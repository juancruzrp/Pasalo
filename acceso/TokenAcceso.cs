
using System;
using System.Text;

namespace acceso
{
    // Enlaces de un solo uso: Tipo 1 = verificar correo, Tipo 2 = recuperar contraseña
    public class TokenAcceso
    {
        // Deja inservibles los enlaces anteriores del mismo tipo (al pedir uno nuevo, solo vale el último)
        public void InvalidarPendientes(int idUsuario, byte tipo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("UPDATE TokenUsuario SET Usado = 1 WHERE IdUsuario = @id AND Tipo = @tipo AND Usado = 0");
                datos.SetearParametro("@id", idUsuario);
                datos.SetearParametro("@tipo", tipo);
                datos.EjecutarAccion();
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void Insertar(int idUsuario, byte tipo, string tokenHash, DateTime vencimientoUtc)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("INSERT INTO TokenUsuario (IdUsuario, Tipo, TokenHash, FechaVencimiento) VALUES (@id, @tipo, @hash, @venc)");
                datos.SetearParametro("@id", idUsuario);
                datos.SetearParametro("@tipo", tipo);
                datos.SetearParametro("@hash", tokenHash);
                datos.SetearParametro("@venc", vencimientoUtc);
                datos.EjecutarAccion();
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // limita reenvios
        public bool CreadoHaceMenosDe(int idUsuario, byte tipo, int minutos)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT COUNT(*) FROM TokenUsuario " +
                    "WHERE IdUsuario = @id AND Tipo = @tipo AND FechaCreacion > DATEADD(MINUTE, -@min, SYSUTCDATETIME())");
                datos.SetearParametro("@id", idUsuario);
                datos.SetearParametro("@tipo", tipo);
                datos.SetearParametro("@min", minutos);
                return Convert.ToInt32(datos.EjecutarScalar()) > 0;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }


        public bool ExisteVigente(string tokenHash, byte tipo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "SELECT COUNT(*) FROM TokenUsuario " +
                    "WHERE TokenHash = @hash AND Tipo = @tipo AND Usado = 0 AND FechaVencimiento > SYSUTCDATETIME()");
                datos.SetearParametro("@hash", tokenHash);
                datos.SetearParametro("@tipo", tipo);
                return Convert.ToInt32(datos.EjecutarScalar()) > 0;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }


        // El UPDATE solo toca un enlace vigente y sin usar: dos clics a la vez no pueden usarlo dos veces.
        public bool ConfirmarVerificacion(string tokenHash)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SET XACT_ABORT ON; BEGIN TRANSACTION; ");
            sql.Append("DECLARE @idUsuario INT; ");
            sql.Append("UPDATE TokenUsuario SET Usado = 1, @idUsuario = IdUsuario ");
            sql.Append("WHERE TokenHash = @hash AND Tipo = 1 AND Usado = 0 AND FechaVencimiento > SYSUTCDATETIME(); ");
            sql.Append("IF @idUsuario IS NOT NULL UPDATE Usuario SET EmailVerificado = 1 WHERE IdUsuario = @idUsuario; ");
            sql.Append("COMMIT TRANSACTION; ");
            sql.Append("SELECT CASE WHEN @idUsuario IS NULL THEN 0 ELSE 1 END;");

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(sql.ToString());
                datos.SetearParametro("@hash", tokenHash);
                return Convert.ToInt32(datos.EjecutarScalar()) == 1;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Cambia la contraseña con un enlace de recuperación vigente 
        public bool RestablecerPassword(string tokenHash, string passwordHashNuevo)
        {
            StringBuilder sql = new StringBuilder();
            sql.Append("SET XACT_ABORT ON; BEGIN TRANSACTION; ");
            sql.Append("DECLARE @idUsuario INT; ");
            sql.Append("UPDATE TokenUsuario SET Usado = 1, @idUsuario = IdUsuario ");
            sql.Append("WHERE TokenHash = @hash AND Tipo = 2 AND Usado = 0 AND FechaVencimiento > SYSUTCDATETIME(); ");
            sql.Append("IF @idUsuario IS NOT NULL ");
            sql.Append("BEGIN ");
            sql.Append("   UPDATE Usuario SET PasswordHash = @passwordHash, EmailVerificado = 1 WHERE IdUsuario = @idUsuario; ");
            sql.Append("   UPDATE TokenUsuario SET Usado = 1 WHERE IdUsuario = @idUsuario AND Tipo = 2 AND Usado = 0; ");
            sql.Append("END ");
            sql.Append("COMMIT TRANSACTION; ");
            sql.Append("SELECT CASE WHEN @idUsuario IS NULL THEN 0 ELSE 1 END;");

            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(sql.ToString());
                datos.SetearParametro("@hash", tokenHash);
                datos.SetearParametro("@passwordHash", passwordHashNuevo);
                return Convert.ToInt32(datos.EjecutarScalar()) == 1;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}
