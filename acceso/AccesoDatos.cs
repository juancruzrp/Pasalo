using System;
using System.Data;
using System.Data.SqlClient;

namespace acceso
{
    public class AccesoDatos
    {
        private SqlConnection conexion;
        private SqlCommand comando;
        private SqlDataReader lector;

        public SqlDataReader Lector { get { return lector; } }

        public AccesoDatos()
        {
            // Si tu instancia es SQL Express, cambiar "localhost" por "localhost\SQLEXPRESS"
            conexion = new SqlConnection("Data Source=localhost\\SQLEXPRESS; Initial Catalog=Pasalo; Integrated Security=True");
            comando = new SqlCommand();
        }

        public void SetearConsulta(string consulta)
        {
            comando.CommandType = CommandType.Text;
            comando.CommandText = consulta;
        }

        // Si el valor es null se manda DBNull, asi se pueden pasar directo las propiedades que aceptan NULL (int?, etc.)
        public void SetearParametro(string nombre, object valor)
        {
            comando.Parameters.AddWithValue(nombre, valor ?? DBNull.Value);
        }

        public void LimpiarParametros()
        {
            comando.Parameters.Clear();
        }

        // Para SELECT: despues se recorre con Lector.Read()
        public void EjecutarLectura()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                lector = comando.ExecuteReader();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Para INSERT / UPDATE / DELETE
        public void EjecutarAccion()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                comando.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // Para consultas que devuelven un solo valor (COUNT, SCOPE_IDENTITY, etc.)
        public object EjecutarScalar()
        {
            comando.Connection = conexion;
            try
            {
                conexion.Open();
                return comando.ExecuteScalar();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void CerrarConexion()
        {
            if (lector != null)
                lector.Close();
            conexion.Close();
        }
    }
}
