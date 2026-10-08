using System;
using dominio;

namespace acceso
{
    public class UsuarioAcceso
    {
        // Inserta el usuario y devuelve el IdUsuario generado.
        // Rol, Activo y FechaRegistro no se cargan: la base les pone el valor por defecto.
        public int Agregar(Usuario nuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("INSERT INTO Usuario (Email, Nombre, PasswordHash, IdCarrera) VALUES (@Email, @Nombre, @PasswordHash, @IdCarrera); SELECT SCOPE_IDENTITY();");
                datos.SetearParametro("@Email", nuevo.Email);
                datos.SetearParametro("@Nombre", nuevo.Nombre);
                datos.SetearParametro("@PasswordHash", nuevo.PasswordHash);
                datos.SetearParametro("@IdCarrera", nuevo.IdCarrera);   // si es null, se guarda NULL
                return Convert.ToInt32(datos.EjecutarScalar());
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public bool ExisteEmail(string email)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT COUNT(*) FROM Usuario WHERE Email = @Email");
                datos.SetearParametro("@Email", email);
                return Convert.ToInt32(datos.EjecutarScalar()) > 0;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Devuelve el usuario o null si no existe
        public Usuario ObtenerPorEmail(string email)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT IdUsuario, Email, Nombre, PasswordHash, IdCarrera, Rol, Activo, FechaRegistro FROM Usuario WHERE Email = @Email");
                datos.SetearParametro("@Email", email);
                datos.EjecutarLectura();

                if (!datos.Lector.Read())
                    return null;

                Usuario usuario = new Usuario();
                usuario.IdUsuario = (int)datos.Lector["IdUsuario"];
                usuario.Email = (string)datos.Lector["Email"];
                usuario.Nombre = (string)datos.Lector["Nombre"];
                usuario.PasswordHash = (string)datos.Lector["PasswordHash"];

                // IdCarrera puede ser NULL en la base
                if (!(datos.Lector["IdCarrera"] is DBNull))
                    usuario.IdCarrera = (int)datos.Lector["IdCarrera"];

                // Rol es TINYINT en SQL Server (llega como byte): hay que usar Convert, un cast directo a int falla
                usuario.Rol = Convert.ToInt32(datos.Lector["Rol"]);
                usuario.Activo = (bool)datos.Lector["Activo"];
                usuario.FechaRegistro = (DateTime)datos.Lector["FechaRegistro"];
                return usuario;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}
