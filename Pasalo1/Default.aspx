
<%@ Page Title="Catálogo" Language="C#"
    MasterPageFile="~/Principal.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="Pasalo1._Default" %>

<asp:Content ID="Contenido"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <% if (Request.QueryString["publicado"] == "ok") { %>
        <div class="aviso-pagina">
            <div class="banner-exito" id="avisoPublicado"
                 role="status">
                <i class="ti ti-circle-check"></i>
                <div class="texto">
                    <strong>¡Tu artículo ya está publicado!</strong>
                    <p>Ahora otros estudiantes pueden verlo.</p>
                </div>
                <a class="accion" href="Publicacion.aspx">
                    Publicar otro artículo
                </a>
            </div>
        </div>
    <% } %>

    <div class="catalogo">

        <aside class="lateral" aria-label="Filtros">

            <asp:Label ID="lblCarrera"
                runat="server"
                AssociatedControlID="ddlCarrera"
                CssClass="rotulo"
                Text="Filtrar por carrera" />

            <asp:DropDownList ID="ddlCarrera"
                runat="server"
                CssClass="campo"
                AutoPostBack="true"
                OnSelectedIndexChanged="Filtros_Changed"
                style="margin-bottom:24px" />

            <asp:Label ID="lblCategoria"
                runat="server"
                AssociatedControlID="ddlCategoria"
                CssClass="rotulo"
                Text="Categorías" />

            <asp:DropDownList ID="ddlCategoria"
                runat="server"
                CssClass="campo"
                AutoPostBack="true"
                OnSelectedIndexChanged="Filtros_Changed"
                style="margin-bottom:24px" />

            <section class="huella" aria-labelledby="t-huella">
                <h2 id="t-huella">
                    <i class="ti ti-recycle"></i>
                    Comunidad PÁSALO
                </h2>
                <p>
                    Dale una segunda vida a los objetos
                    que ya no utilizás.
                </p>
            </section>

        </aside>

        <section aria-labelledby="t-catalogo">

            <div class="encabezado">
                <div>
                    <h1 id="t-catalogo">
                        Objetos disponibles
                    </h1>

                    <p class="suave">
                        <asp:Label ID="lblCantidad"
                            runat="server" />
                    </p>
                </div>

                <div class="orden">
                    <asp:Label ID="lblOrden"
                        runat="server"
                        AssociatedControlID="ddlOrden"
                        Text="Ordenar por" />

                    <asp:DropDownList ID="ddlOrden"
                        runat="server"
                        CssClass="campo"
                        AutoPostBack="true"
                        OnSelectedIndexChanged="Filtros_Changed">

                        <asp:ListItem Text="Más recientes"
                            Value="DESC" />

                        <asp:ListItem Text="Más antiguos"
                            Value="ASC" />

                    </asp:DropDownList>
                </div>
            </div>

            <div style="margin-bottom:24px;">
                <asp:TextBox ID="txtBuscar"
                    runat="server"
                    CssClass="campo"
                    MaxLength="100"
                    placeholder="Buscar artículos por nombre o descripción..."
                    style="width:100%;max-width:450px;" />

                <asp:Button ID="btnBuscar"
                    runat="server"
                    Text="Buscar"
                    OnClick="Filtros_Changed"
                    CssClass="campo"
                    style="cursor:pointer;margin-top:8px;" />
            </div>

            <asp:Label ID="lblError"
                runat="server"
                Visible="false"
                role="alert"
                CssClass="suave" />

            <asp:Panel ID="pnlSinResultados"
                runat="server"
                Visible="false">

                <div style="text-align:center;padding:50px 20px;">
                    <i class="ti ti-search"
                       style="font-size:36px;"></i>

                    <h3>No encontramos publicaciones</h3>
                    <p class="suave">
                        Probá cambiando los filtros o la búsqueda.
                    </p>
                </div>

            </asp:Panel>

            <div class="grilla">

                <asp:Repeater ID="rptPublicaciones"
                    runat="server">

                    <ItemTemplate>

                        <article class="tarjeta">

                            <div class="foto"
                                style="position:relative;overflow:hidden;">

                                <span class="chip">
                                    <%# TextoSeguro(
                                        Eval("Categoria.Nombre")) %>
                                </span>

                                <asp:Image
                                    runat="server"
                                    ImageUrl='<%# ObtenerFoto(Container.DataItem) %>'
                                    AlternateText='<%# "Foto de " + TextoSeguro(Eval("Titulo")) %>'
                                    Visible='<%# !string.IsNullOrEmpty(ObtenerFoto(Container.DataItem)) %>'
                                    style="width:100%;height:100%;object-fit:cover;display:block;" />

                            </div>

                            <div class="tarjeta-cuerpo">

                                <h3>
                                    <%# TextoSeguro(Eval("Titulo")) %>
                                </h3>

                                <p class="suave">
                                    <%# TextoSeguro(
                                        ObtenerCarrera(Container.DataItem)) %>
                                </p>

                                <p class="suave">
                                    <%# ObtenerModalidad(Container.DataItem) %>
                                </p>

                                <div class="tarjeta-pie">

                                    <span class="estado estado--disponible">
                                        Disponible
                                    </span>

                                    <asp:HyperLink
                                        runat="server"
                                        CssClass="ir"
                                        NavigateUrl='<%# ObtenerDetalle(Container.DataItem) %>'
                                        ToolTip="Ver detalle">

                                        <i class="ti ti-arrow-right"
                                           aria-hidden="true"></i>

                                    </asp:HyperLink>

                                </div>

                            </div>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

        </section>

    </div>

    <a class="publicar" href="Publicacion.aspx">
        <i class="ti ti-plus" aria-hidden="true"></i>
        Publicar artículo
    </a>

</asp:Content>
