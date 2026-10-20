 <%@ Page Title="Catálogo" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Pasalo1._Default" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">

    <% if (Request.QueryString["publicado"] == "ok") { %>
<div class="aviso-pagina">
    <div class="banner-exito" id="avisoPublicado" role="status" tabindex="-1">
        <i class="ti ti-circle-check" aria-hidden="true"></i>
        <div class="texto">
            <strong>¡Tu artículo ya está publicado!</strong>
            <p>Ahora otros estudiantes pueden verlo y solicitarlo.</p>
        </div>
        <a class="accion" href="Publicacion.aspx">Publicar otro artículo</a>
        <button type="button" class="cerrar" aria-label="Cerrar aviso"
                onclick="document.getElementById('avisoPublicado').parentNode.remove();">&times;</button>
    </div>
</div>
<script>
    // Saca "?publicado=ok" de la dirección: al recargar la página el aviso no vuelve a aparecer
    history.replaceState(null, '', location.pathname);
    // Lleva el foco al aviso para que los lectores de pantalla lo anuncien
    document.getElementById('avisoPublicado').focus();
</script>
<% } %>


    <div class="catalogo">

        <aside class="lateral" aria-label="Filtros">
            <asp:Label ID="lblCarrera" runat="server" AssociatedControlID="ddlCarrera" CssClass="rotulo" Text="Filtrar por carrera" />
            <asp:DropDownList ID="ddlCarrera" runat="server" CssClass="campo" AutoPostBack="true" style="margin-bottom:24px" />

            <nav aria-label="Categorías">
                <span class="rotulo">Categorías</span>
                <ul class="categorias">
                    <asp:Repeater ID="rptCategorias" runat="server">
                        <ItemTemplate>
                            <li><a href="<%#: Eval("Url") %>"<%# (bool)Eval("Actual") ? " aria-current=\"true\"" : "" %>><i class="ti <%#: Eval("Icono") %>" aria-hidden="true"></i><%#: Eval("Nombre") %><span class="cuenta"><%#: Eval("Cantidad") %></span></a></li>
                        </ItemTemplate>
                    </asp:Repeater>
                </ul>
            </nav>

            <%-- Datos de ejemplo: más adelante salen de las operaciones finalizadas --%>
            <section class="huella" aria-labelledby="t-huella">
                <h2 id="t-huella"><i class="ti ti-recycle" aria-hidden="true"></i>Tu huella circular</h2>
                <p>Este mes ayudaste a evitar 12 kg de residuos. ¡Seguí así!</p>
            </section>
        </aside>

        <section aria-labelledby="t-catalogo">
            <div class="encabezado">
                <div>
                    <h1 id="t-catalogo">Objetos disponibles</h1>
                    <p class="suave"><asp:Label ID="lblResumen" runat="server" /></p>
                </div>
                <div class="orden">
                    <asp:Label ID="lblOrden" runat="server" AssociatedControlID="ddlOrden" Text="Ordenar por" />
                    <asp:DropDownList ID="ddlOrden" runat="server" CssClass="campo" AutoPostBack="true">
                        <asp:ListItem Text="Más recientes" Value="recientes" />
                        <asp:ListItem Text="Más antiguos" Value="antiguos" />
                    </asp:DropDownList>
                </div>
            </div>

            <asp:Panel ID="pnlGrilla" runat="server" CssClass="grilla">
                <asp:Repeater ID="rptPublicaciones" runat="server">
                    <ItemTemplate>
                        <article class="tarjeta">
                            <div class="foto"<%# EstiloFoto(Container.DataItem) %>><span class="chip"><%#: Eval("Categoria.Nombre") %></span><%# IconoFoto(Container.DataItem) %></div>
                            <div class="tarjeta-cuerpo">
                                <h3><%#: Eval("Titulo") %></h3>
                                <p class="suave"><%#: TextoCarrera(Container.DataItem) %></p>
                                <div class="tarjeta-pie">
                                    <span class="estado estado--disponible">Disponible</span>
                                    <a class="ir" href="DetallePublicacion.aspx?id=<%#: Eval("Id") %>" aria-label="Ver detalle de <%#: Eval("Titulo") %>"><i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                                </div>
                            </div>
                        </article>
                    </ItemTemplate>
                </asp:Repeater>
            </asp:Panel>

            <asp:Panel ID="pnlVacio" runat="server" Visible="false" style="padding: 48px 24px; text-align: center; border: 1px dashed var(--color-borde); border-radius: 16px;">
                <p><asp:Label ID="lblVacio" runat="server" /></p>
                <p><a href="Publicacion.aspx">Publicar un artículo</a></p>
            </asp:Panel>
        </section>
    </div>

    <a class="publicar" href="Publicacion.aspx"><i class="ti ti-plus" aria-hidden="true"></i>Publicar artículo</a>
</asp:Content>
