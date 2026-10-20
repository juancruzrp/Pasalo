<%@ Page Title="Mi perfil" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="Pasalo1.Perfil" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <% if (Datos != null) { %>
    <div class="perfil">

        <%-- Todos los datos salen de la base de datos (ver Perfil.aspx.cs y PerfilAcceso.cs) --%>
        <section class="panel perfil-cabecera" aria-labelledby="t-perfil">
            <div class="perfil-id">
                <div class="avatar-grande" aria-hidden="true"><%: Iniciales %></div>
                <div>
                    <h1 id="t-perfil"><%: Datos.Nombre %></h1>
                    <p><%: CarreraYFacultad %></p>
                    <p class="suave" style="font-size:13px"><%: MiembroDesde %></p>
                </div>
            </div>
            <div class="perfil-valoracion">
                <% if (TieneValoraciones) { %>
                <p>
                    <span class="estrellas" aria-hidden="true">
                        <% for (int i = 1; i <= 5; i++) { %><i class="ti <%= i <= EstrellasLlenas ? "ti-star-filled" : "ti-star" %>"></i><% } %>
                    </span>
                    <span class="nota"><%: PromedioTexto %></span><span class="solo-lectores"> de 5 de reputación</span>
                </p>
                <p class="suave"><%: Datos.CantidadValoraciones %> <%= Datos.CantidadValoraciones == 1 ? "valoración" : "valoraciones" %> · <%: ObjetosEntregadosTexto %></p>
                <% } else { %>
                <p class="suave">Sin valoraciones todavía</p>
                <p class="suave"><%: ObjetosEntregadosTexto %></p>
                <% } %>
            </div>
        </section>

        <nav class="pestanas" aria-label="Secciones del perfil">
            <a href="Perfil.aspx?tab=publicaciones<%= SufijoId %>"<%= Actual("publicaciones") %>><%= EsPropio ? "Mis publicaciones" : "Publicaciones" %></a>
            <% if (EsPropio) { %>
            <a href="Perfil.aspx?tab=historial"<%= Actual("historial") %>>Historial</a>
            <% } %>
            <a href="Perfil.aspx?tab=impacto<%= SufijoId %>"<%= Actual("impacto") %>>Impacto ambiental</a>
        </nav>

        <% if (TabActiva == "publicaciones") { %>
            <% if (Publicaciones.Count == 0) { %>
            <div class="panel">
                <p class="suave"><%= EsPropio ? "Todavía no publicaste ningún artículo." : "Esta persona no tiene publicaciones disponibles." %></p>
                <% if (EsPropio) { %><a class="accion" href="Publicacion.aspx">Publicar un artículo</a><% } %>
            </div>
            <% } else { %>
            <ul class="lista">
                <% foreach (dominio.PublicacionPerfil p in Publicaciones) { %>
                <li class="fila">
                    <span class="fila-icono" aria-hidden="true"><i class="ti <%= IconoCategoria(p.Categoria) %>"></i></span>
                    <div class="fila-texto">
                        <p class="fila-titulo"><a href="DetallePublicacion.aspx?id=<%= p.Id %>"><%: p.Titulo %></a></p>
                        <p class="suave"><%: TextoPublicacion(p) %><% if (EsPropio && p.SolicitudesPendientes > 0) { %> · <a class="enlace" href="DetallePublicacion.aspx?id=<%= p.Id %>"><%= p.SolicitudesPendientes %> <%= p.SolicitudesPendientes == 1 ? "solicitud" : "solicitudes" %></a><% } %></p>
                    </div>
                    <span class="estado estado--<%= ClaseEstado(p.Estado) %>"><%= TextoEstado(p.Estado) %></span>
                </li>
                <% } %>
            </ul>
            <% } %>
        <% } else if (TabActiva == "historial") { %>
            <% if (Historial.Count == 0) { %>
            <div class="panel">
                <p class="suave">Todavía no tenés donaciones o intercambios concretados. Cuando entregues o recibas un objeto, va a aparecer acá.</p>
            </div>
            <% } else { %>
            <ul class="lista">
                <% foreach (dominio.MovimientoHistorial m in Historial) { %>
                <li class="fila">
                    <span class="fila-icono" aria-hidden="true"><i class="ti <%= m.Modalidad == dominio.ModalidadPublicacion.Intercambio ? "ti-arrows-exchange" : "ti-gift" %>"></i></span>
                    <div class="fila-texto">
                        <p class="fila-titulo"><a href="DetallePublicacion.aspx?id=<%= m.IdPublicacion %>"><%: m.Titulo %></a></p>
                        <p class="suave"><%: TextoModalidad(m.Modalidad) %> con <%: m.OtraPersona %><% if (m.FechaEntrega.HasValue) { %> · <%: FechaCorta(m.FechaEntrega) %><% } %></p>
                    </div>
                    <p class="fila-der"><%= m.PuntajeRecibido.HasValue ? "Te valoró con " + m.PuntajeRecibido.Value + " de 5" : "Sin valoración" %></p>
                </li>
                <% } %>
            </ul>
            <% } %>
        <% } else { %>
        <div class="impacto">
            <div class="col-apilada">
                <div class="panel dato">
                    <span class="dato-icono" aria-hidden="true"><i class="ti ti-gift"></i></span>
                    <div>
                        <p class="dato-valor"><%: Datos.ObjetosEntregados %> <%= Datos.ObjetosEntregados == 1 ? "objeto" : "objetos" %></p>
                        <p class="suave">Donados o intercambiados</p>
                    </div>
                </div>
                <div class="panel dato">
                    <span class="dato-icono dato-icono--acento" aria-hidden="true"><i class="ti ti-recycle"></i></span>
                    <div>
                        <p class="dato-valor dato-valor--acento"><%: Kg(Datos.KgEvitados) %></p>
                        <p class="suave">De residuos evitados (estimado)</p>
                    </div>
                </div>
                <section class="huella huella--ancha" aria-labelledby="t-huella">
                    <div class="progreso" style="--valor:<%= PorcentajeAporte %>" role="img" aria-label="<%= PorcentajeAporte %> % del impacto de la facultad"><span><%= PorcentajeAporte %>%</span></div>
                    <div>
                        <h2 id="t-huella"><i class="ti ti-recycle" aria-hidden="true"></i><%= EsPropio ? "Tu huella circular" : "Su huella circular" %></h2>
                        <p><%: TextoHuella %></p>
                    </div>
                </section>
            </div>

            <section class="panel" aria-labelledby="t-fac">
                <h2 id="t-fac">Impacto por facultad</h2>
                <p class="suave" style="font-size:14px">Residuos evitados (kg) por la comunidad</p>
                <ul class="barras">
                    <% foreach (dominio.ImpactoFacultad f in Facultades) { %>
                    <li>
                        <div class="barra-fila"><span><%: f.Nombre %><%= EsFacultadPropia(f) ? (EsPropio ? " (tu facultad)" : " (su facultad)") : "" %></span><span><%: Kg(f.Kg) %></span></div>
                        <div class="barra-pista" aria-hidden="true"><div class="barra-relleno<%= EsFacultadPropia(f) ? " barra-relleno--propia" : "" %>" style="width:<%= AnchoBarra(f.Kg) %>%"></div></div>
                    </li>
                    <% } %>
                </ul>
            </section>
        </div>
        <% } %>

    </div>
    <% } %>
</asp:Content>
