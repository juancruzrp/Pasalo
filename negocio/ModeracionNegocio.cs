using System;
using System.Collections.Generic;
using acceso;
using dominio;

namespace negocio
{
    // Reglas del panel de moderación. Cada acción vuelve a comprobar en la base que quien la pide
    // sigue siendo moderador activo (no alcanza con lo que dice la sesión).
    public class ModeracionNegocio
    {
        private readonly ModeracionAcceso acceso = new ModeracionAcceso();

        // ---------- permisos ----------
        private void ExigirModerador(Usuario actor)
        {
            if (actor == null || !acceso.EsModeradorActivo(actor.IdUsuario))
                throw new NegocioException("No tenés permisos de moderación.");
        }

        public bool EsModerador(Usuario actor)
        {
            return actor != null && acceso.EsModeradorActivo(actor.IdUsuario);
        }

        // Para refrescar la sesión en cada pedido. false = ya no existe.
        public bool LeerEstado(int idUsuario, out bool activo, out int rol)
        {
            return acceso.LeerEstado(idUsuario, out activo, out rol);
        }

        public bool PublicacionSuspendida(int idPublicacion)
        {
            return acceso.PublicacionSuspendida(idPublicacion);
        }

        private static string Texto(string valor, string campo, int max, bool obligatorio)
        {
            valor = (valor ?? "").Trim();
            if (obligatorio && valor.Length == 0)
                throw new NegocioException("Completá " + campo + ".");
            if (valor.Length > max)
                throw new NegocioException(char.ToUpper(campo[0]) + campo.Substring(1) + " no puede superar los " + max + " caracteres.");
            return valor;
        }

        // ---------- usuarios ----------
        public List<ModUsuario> ListarUsuarios(Usuario actor, string busqueda)
        {
            ExigirModerador(actor);
            return acceso.ListarUsuarios((busqueda ?? "").Trim());
        }

        public ModUsuario ObtenerUsuario(Usuario actor, int id)
        {
            ExigirModerador(actor);
            return acceso.ObtenerUsuario(id);
        }

        public void EditarUsuario(Usuario actor, int id, string nombre, int? idCarrera, string descripcion)
        {
            ExigirModerador(actor);
            if (acceso.ObtenerUsuario(id) == null)
                throw new NegocioException("El usuario ya no existe.");
            nombre = Texto(nombre, "el nombre", 100, true);
            descripcion = Texto(descripcion, "la descripción", 200, false);
            if (idCarrera.HasValue && !acceso.ExisteIdCarrera(idCarrera.Value))
                throw new NegocioException("La carrera elegida no existe.");
            acceso.EditarUsuario(id, nombre, idCarrera, descripcion);
        }

        // true = quedó activo, false = quedó suspendido
        public bool AlternarSuspension(Usuario actor, int id)
        {
            ExigirModerador(actor);
            ModUsuario u = acceso.ObtenerUsuario(id);
            if (u == null)
                throw new NegocioException("El usuario ya no existe.");
            if (u.IdUsuario == actor.IdUsuario)
                throw new NegocioException("No podés suspender tu propia cuenta.");
            if (u.Activo && u.Rol == 1)
                throw new NegocioException("Primero sacale el rol de moderador para poder suspenderlo.");
            acceso.CambiarActivo(id, !u.Activo);
            return !u.Activo;
        }

        // true = ahora es moderador
        public bool AlternarModerador(Usuario actor, int id)
        {
            ExigirModerador(actor);
            ModUsuario u = acceso.ObtenerUsuario(id);
            if (u == null)
                throw new NegocioException("El usuario ya no existe.");
            if (u.Rol == 1)
            {
                if (u.IdUsuario == actor.IdUsuario)
                    throw new NegocioException("No podés sacarte a vos mismo el rol de moderador.");
                acceso.CambiarRol(id, 0);
                return false;
            }
            if (!u.Activo)
                throw new NegocioException("Una cuenta suspendida no puede ser moderadora. Reactivala primero.");
            acceso.CambiarRol(id, 1);
            return true;
        }

        public void VerificarCorreo(Usuario actor, int id)
        {
            ExigirModerador(actor);
            if (acceso.ObtenerUsuario(id) == null)
                throw new NegocioException("El usuario ya no existe.");
            acceso.VerificarCorreo(id);
        }

        // ---------- publicaciones ----------
        public List<ModPublicacion> ListarPublicaciones(Usuario actor, string busqueda)
        {
            ExigirModerador(actor);
            return acceso.ListarPublicaciones((busqueda ?? "").Trim());
        }

        public ModPublicacion ObtenerPublicacion(Usuario actor, int id)
        {
            ExigirModerador(actor);
            return acceso.ObtenerPublicacion(id);
        }

        public void EditarPublicacion(Usuario actor, int id, string titulo, string descripcion, int idCategoria, int? idCarrera)
        {
            ExigirModerador(actor);
            if (acceso.ObtenerPublicacion(id) == null)
                throw new NegocioException("La publicación ya no existe.");
            titulo = Texto(titulo, "el título", 100, true);
            descripcion = Texto(descripcion, "la descripción", 1000, true);
            if (!acceso.ExisteIdCategoria(idCategoria))
                throw new NegocioException("La categoría elegida no existe.");
            if (idCarrera.HasValue && !acceso.ExisteIdCarrera(idCarrera.Value))
                throw new NegocioException("La carrera elegida no existe.");
            acceso.EditarPublicacion(id, titulo, descripcion, idCategoria, idCarrera);
        }

        // true = quedó suspendida
        public bool AlternarSuspensionPublicacion(Usuario actor, int id)
        {
            ExigirModerador(actor);
            ModPublicacion p = acceso.ObtenerPublicacion(id);
            if (p == null)
                throw new NegocioException("La publicación ya no existe.");
            acceso.CambiarSuspension(id, !p.Suspendida);
            return !p.Suspendida;
        }

        public void EliminarPublicacion(Usuario actor, int id)
        {
            ExigirModerador(actor);
            ModPublicacion p = acceso.ObtenerPublicacion(id);
            if (p == null)
                throw new NegocioException("La publicación ya no existe.");
            if (p.Solicitudes > 0)
                throw new NegocioException("Tiene solicitudes o conversaciones asociadas, así que no se puede borrar. Suspendela para ocultarla.");
            acceso.EliminarPublicacion(id);
        }

        // ---------- categorías ----------
        public List<ModCategoria> ListarCategorias(Usuario actor)
        {
            ExigirModerador(actor);
            return acceso.ListarCategorias();
        }

        private static decimal LeerPeso(string peso)
        {
            decimal p;
            if (!decimal.TryParse((peso ?? "").Trim().Replace('.', ','), System.Globalization.NumberStyles.Number,
                    new System.Globalization.CultureInfo("es-AR"), out p) || p < 0 || p > 9999)
                throw new NegocioException("El peso estimado tiene que ser un número entre 0 y 9999 (en kg).");
            return Math.Round(p, 2);
        }

        public void AgregarCategoria(Usuario actor, string nombre, string peso)
        {
            ExigirModerador(actor);
            nombre = Texto(nombre, "el nombre de la categoría", 50, true);
            decimal p = LeerPeso(peso);
            if (acceso.ExisteCategoria(nombre, 0))
                throw new NegocioException("Ya existe una categoría con ese nombre.");
            acceso.AgregarCategoria(nombre, p);
        }

        public void EditarCategoria(Usuario actor, int id, string nombre, string peso)
        {
            ExigirModerador(actor);
            if (!acceso.ExisteIdCategoria(id))
                throw new NegocioException("La categoría ya no existe.");
            nombre = Texto(nombre, "el nombre de la categoría", 50, true);
            decimal p = LeerPeso(peso);
            if (acceso.ExisteCategoria(nombre, id))
                throw new NegocioException("Ya existe otra categoría con ese nombre.");
            acceso.EditarCategoria(id, nombre, p);
        }

        public void EliminarCategoria(Usuario actor, int id)
        {
            ExigirModerador(actor);
            if (!acceso.ExisteIdCategoria(id))
                throw new NegocioException("La categoría ya no existe.");
            int usos = acceso.UsosCategoria(id);
            if (usos > 0)
                throw new NegocioException("No se puede borrar: la usan " + usos + " publicación(es). Renombrala o cambiá esas publicaciones primero.");
            acceso.EliminarCategoria(id);
        }

        // ---------- facultades ----------
        public List<ModFacultad> ListarFacultades(Usuario actor)
        {
            ExigirModerador(actor);
            return acceso.ListarFacultades();
        }

        public void AgregarFacultad(Usuario actor, string nombre)
        {
            ExigirModerador(actor);
            nombre = Texto(nombre, "el nombre de la facultad", 100, true);
            if (acceso.ExisteFacultad(nombre, 0))
                throw new NegocioException("Ya existe una facultad con ese nombre.");
            acceso.AgregarFacultad(nombre);
        }

        public void EditarFacultad(Usuario actor, int id, string nombre)
        {
            ExigirModerador(actor);
            if (!acceso.ExisteIdFacultad(id))
                throw new NegocioException("La facultad ya no existe.");
            nombre = Texto(nombre, "el nombre de la facultad", 100, true);
            if (acceso.ExisteFacultad(nombre, id))
                throw new NegocioException("Ya existe otra facultad con ese nombre.");
            acceso.EditarFacultad(id, nombre);
        }

        public void EliminarFacultad(Usuario actor, int id)
        {
            ExigirModerador(actor);
            if (!acceso.ExisteIdFacultad(id))
                throw new NegocioException("La facultad ya no existe.");
            int n = acceso.CarrerasDeFacultad(id);
            if (n > 0)
                throw new NegocioException("No se puede borrar: tiene " + n + " carrera(s). Movelas o borralas primero.");
            acceso.EliminarFacultad(id);
        }

        // ---------- carreras ----------
        public List<ModCarrera> ListarCarreras(Usuario actor)
        {
            ExigirModerador(actor);
            return acceso.ListarCarreras();
        }

        public void AgregarCarrera(Usuario actor, string nombre, int idFacultad)
        {
            ExigirModerador(actor);
            nombre = Texto(nombre, "el nombre de la carrera", 100, true);
            if (!acceso.ExisteIdFacultad(idFacultad))
                throw new NegocioException("Elegí una facultad válida.");
            if (acceso.ExisteCarrera(nombre, 0))
                throw new NegocioException("Ya existe una carrera con ese nombre.");
            acceso.AgregarCarrera(nombre, idFacultad);
        }

        public void EditarCarrera(Usuario actor, int id, string nombre, int idFacultad)
        {
            ExigirModerador(actor);
            if (!acceso.ExisteIdCarrera(id))
                throw new NegocioException("La carrera ya no existe.");
            nombre = Texto(nombre, "el nombre de la carrera", 100, true);
            if (!acceso.ExisteIdFacultad(idFacultad))
                throw new NegocioException("Elegí una facultad válida.");
            if (acceso.ExisteCarrera(nombre, id))
                throw new NegocioException("Ya existe otra carrera con ese nombre.");
            acceso.EditarCarrera(id, nombre, idFacultad);
        }

        public void EliminarCarrera(Usuario actor, int id)
        {
            ExigirModerador(actor);
            if (!acceso.ExisteIdCarrera(id))
                throw new NegocioException("La carrera ya no existe.");
            int usos = acceso.UsosCarrera(id);
            if (usos > 0)
                throw new NegocioException("No se puede borrar: la usan " + usos + " perfil(es) o publicación(es).");
            acceso.EliminarCarrera(id);
        }

        // ---------- reportes ----------
        public List<ModReporte> ListarReportes(Usuario actor, bool incluirResueltos)
        {
            ExigirModerador(actor);
            return acceso.ListarReportes(incluirResueltos);
        }

        public int ReportesPendientes(Usuario actor)
        {
            return EsModerador(actor) ? acceso.CantidadReportesPendientes() : 0;
        }

        // resuelto = true -> "resuelto" (se tomó acción); false -> "desestimado"
        public void ResolverReporte(Usuario actor, int idReporte, bool resuelto)
        {
            ExigirModerador(actor);
            acceso.ResolverReporte(idReporte, resuelto ? 2 : 1, actor.IdUsuario);
        }

        // Lo puede usar cualquier usuario con sesión.
        public void Reportar(Usuario reportante, int? idPublicacion, int? idUsuario, string motivo, string detalle)
        {
            if (reportante == null)
                throw new NegocioException("Iniciá sesión para reportar.");
            if (idPublicacion.HasValue == idUsuario.HasValue)
                throw new NegocioException("Indicá qué querés reportar.");
            motivo = Texto(motivo, "el motivo", 100, true);
            detalle = Texto(detalle, "el detalle", 500, false);

            if (idPublicacion.HasValue)
            {
                int idDueno; string titulo;
                if (acceso.DuenoPublicacion(idPublicacion.Value, out idDueno, out titulo) == null)
                    throw new NegocioException("La publicación ya no existe.");
                if (idDueno == reportante.IdUsuario)
                    throw new NegocioException("No podés reportar tu propia publicación.");
            }
            else
            {
                if (idUsuario.Value == reportante.IdUsuario)
                    throw new NegocioException("No podés reportarte a vos mismo.");
                if (acceso.NombreUsuario(idUsuario.Value) == null)
                    throw new NegocioException("El usuario ya no existe.");
            }

            if (!acceso.Reportar(reportante.IdUsuario, idPublicacion, idUsuario, motivo, detalle))
                throw new NegocioException("Ya tenés un reporte pendiente sobre esto. Lo va a revisar un moderador.");
        }
    }
}
