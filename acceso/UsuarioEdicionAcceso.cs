using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace acceso
{
    // Cambios sobre los datos propios del usuario (pantalla "Editar perfil").
    // Va aparte de PerfilAcceso, que solo lee.
    public class UsuarioEdicionAcceso
    {
        public void ActualizarPerfil(int idUsuario, string nombre, int idCarrera, string descripcion)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta(
                    "UPDATE Usuario SET Nombre = @nombre, IdCarrera = @carrera, Descripcion = @descripcion " +
                    "WHERE IdUsuario = @id AND Activo = 1");
                datos.SetearParametro("@nombre", nombre);
                datos.SetearParametro("@carrera", idCarrera);
                datos.SetearParametro("@descripcion", descripcion);   // null se guarda como NULL
                datos.SetearParametro("@id", idUsuario);
                datos.EjecutarAccion();
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        // Hash guardado de la contraseña (null si el usuario no existe)
        public string ObtenerPasswordHash(int idUsuario)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("SELECT PasswordHash FROM Usuario WHERE IdUsuario = @id AND Activo = 1");
                datos.SetearParametro("@id", idUsuario);
                object resultado = datos.EjecutarScalar();
                return (resultado == null || resultado is DBNull) ? null : (string)resultado;
            }
            finally
            {
                datos.CerrarConexion();
            }
        }

        public void CambiarPasswordHash(int idUsuario, string passwordHashNuevo)
        {
            AccesoDatos datos = new AccesoDatos();
            try
            {
                datos.SetearConsulta("UPDATE Usuario SET PasswordHash = @hash WHERE IdUsuario = @id AND Activo = 1");
                datos.SetearParametro("@hash", passwordHashNuevo);
                datos.SetearParametro("@id", idUsuario);
                datos.EjecutarAccion();
            }
            finally
            {
                datos.CerrarConexion();
            }
        }
    }
}

