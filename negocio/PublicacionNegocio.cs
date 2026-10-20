using System;
using System.Collections.Generic;
using System.IO;
using acceso;
using dominio;


namespace negocio
    {
        // Archivo recibido desde la página (así negocio no depende de System.Web)
        public class FotoSubida
        {
            public string NombreOriginal { get; set; }
            public byte[] Contenido { get; set; }
        }

        public class PublicacionNegocio
        {
            private const int MaximoFotos = 4;
            private const int MaximoBytes = 5 * 1024 * 1024;   // 5 MB por foto
            private const string RutaBase = "Uploads/publicaciones";

            private readonly CatalogoAcceso _catalogo = new CatalogoAcceso();
            private readonly PublicacionAcceso _acceso = new PublicacionAcceso();

            public List<Categoria> ListarCategorias() { return _catalogo.ListarCategorias(); }
            public List<Carrera> ListarCarreras() { return _catalogo.ListarCarreras(); }

            // Catálogo: publicaciones disponibles (0 = sin filtro de carrera / categoría)
            public List<Publicacion> ListarDisponibles(int idCarrera, int idCategoria, bool masRecientes)
            {
                return _acceso.ListarDisponibles(idCarrera, idCategoria, masRecientes);
            }

            // Detalle de una publicación (null si no existe)
            public Publicacion ObtenerPorId(int id)
            {
                return _acceso.ObtenerPorId(id);
            }

            // Id de categoría -> cantidad de publicaciones disponibles
            public Dictionary<int, int> ContarDisponiblesPorCategoria(int idCarrera)
            {
                return _acceso.ContarDisponiblesPorCategoria(idCarrera);
            }

            //Devuelve el Id creado.
            // carpetaFisica: carpeta del servidor donde se guardan las fotos (ruta absoluta).
            public int Publicar(Publicacion pub, List<FotoSubida> fotos, string carpetaFisica)
            {
                pub.Titulo = (pub.Titulo ?? "").Trim();
                pub.Descripcion = (pub.Descripcion ?? "").Trim();

                if (pub.Titulo == "" || pub.Descripcion == "" || pub.Categoria == null || pub.Categoria.Id <= 0 || pub.Modalidad == null)
                    throw new NegocioException("Completá título, descripción, categoría y si es donación o intercambio.");

                if (pub.Titulo.Length > 100)
                    throw new NegocioException("El título no puede superar los 100 caracteres.");
                if (pub.Descripcion.Length > 1000)
                    throw new NegocioException("La descripción no puede superar los 1000 caracteres.");

                if (fotos == null || fotos.Count == 0)
                    throw new NegocioException("Agregá al menos una foto del artículo.");
                if (fotos.Count > MaximoFotos)
                    throw new NegocioException("Podés subir hasta " + MaximoFotos + " fotos.");

                // Se valida el contenido real del archivo, no solo la extensión que declara
                List<string> extensiones = new List<string>();
                foreach (FotoSubida f in fotos)
                {
                    if (f.Contenido == null || f.Contenido.Length == 0)
                        throw new NegocioException("Una de las fotos está vacía.");
                    if (f.Contenido.Length > MaximoBytes)
                        throw new NegocioException("Cada foto puede pesar hasta 5 MB.");

                    string ext = DetectarExtension(f.Contenido);
                    if (ext == null)
                        throw new NegocioException("Las fotos deben ser imágenes JPG o PNG.");
                    extensiones.Add(ext);
                }

                Directory.CreateDirectory(carpetaFisica);
                List<string> archivosCreados = new List<string>();
                pub.Fotos = new List<FotoPublicacion>();

                try
                {
                    for (int i = 0; i < fotos.Count; i++)
                    {
                        // Nombre propio (nunca el que manda el usuario): evita pisar archivos 
                        string nombre = Guid.NewGuid().ToString("N") + extensiones[i];
                        string destino = Path.Combine(carpetaFisica, nombre);
                        File.WriteAllBytes(destino, fotos[i].Contenido);
                        archivosCreados.Add(destino);

                        pub.Fotos.Add(new FotoPublicacion { Ruta = RutaBase + "/" + nombre, Orden = i });
                    }

                    return _acceso.Agregar(pub);
                }
                catch
                {
                    // algo paso (por ejemplo la base), no dejar fotos huérfanas en el servidor
                    foreach (string archivo in archivosCreados)
                    {
                        try { File.Delete(archivo); } catch { }
                    }
                    throw;
                }
            }

            // JPG empieza con FF D8 FF; PNG con 89 50 4E 47 0D 0A 1A 0A
            private static string DetectarExtension(byte[] b)
            {
                if (b.Length > 3 && b[0] == 0xFF && b[1] == 0xD8 && b[2] == 0xFF)
                    return ".jpg";

                if (b.Length > 8 && b[0] == 0x89 && b[1] == 0x50 && b[2] == 0x4E && b[3] == 0x47
                    && b[4] == 0x0D && b[5] == 0x0A && b[6] == 0x1A && b[7] == 0x0A)
                    return ".png";

                return null;
            }
        }
    }

