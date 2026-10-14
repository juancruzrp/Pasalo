using dominio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace acceso
// Datos para llenar los desplegables
{
    public class CatalogoAcceso
{
    public List<Categoria> ListarCategorias()
    {
        List<Categoria> lista = new List<Categoria>();
        AccesoDatos datos = new AccesoDatos();
        try
        {
            datos.SetearConsulta("SELECT IdCategoria, Nombre FROM Categoria ORDER BY IdCategoria");
            datos.EjecutarLectura();
            while (datos.Lector.Read())
            {
                lista.Add(new Categoria
                {
                    Id = (int)datos.Lector["IdCategoria"],
                    Nombre = (string)datos.Lector["Nombre"]
                });
            }
            return lista;
        }
        finally
        {
            datos.CerrarConexion();
        }
    }

    public List<Carrera> ListarCarreras()
    {
        List<Carrera> lista = new List<Carrera>();
        AccesoDatos datos = new AccesoDatos();
        try
        {
            datos.SetearConsulta("SELECT IdCarrera, Nombre FROM Carrera ORDER BY Nombre");
            datos.EjecutarLectura();
            while (datos.Lector.Read())
            {
                lista.Add(new Carrera
                {
                    Id = (int)datos.Lector["IdCarrera"],
                    Nombre = (string)datos.Lector["Nombre"]
                });
            }
            return lista;
        }
        finally
        {
            datos.CerrarConexion();
        }
    }
}
}
