<%@ Page Title="Moderación" Language="C#" MasterPageFile="~/Principal.Master"
    AutoEventWireup="true" CodeBehind="Moderacion.aspx.cs" Inherits="Pasalo1.Moderacion" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">

    <div class="mod">

        <h1>Panel de moderación</h1>
        <p class="suave mod-sub">Desde acá podés editar y suspender usuarios y publicaciones, administrar los filtros y atender reportes.</p>

        <nav aria-label="Secciones de moderación">
            <ul class="mod-tabs">
                <li><a href="Moderacion.aspx?t=usuarios" id="tabUsuarios" runat="server"><i class="ti ti-users" aria-hidden="true"></i> Usuarios</a></li>
                <li><a href="Moderacion.aspx?t=publicaciones" id="tabPublicaciones" runat="server"><i class="ti ti-package" aria-hidden="true"></i> Publicaciones</a></li>
                <li><a href="Moderacion.aspx?t=filtros" id="tabFiltros" runat="server"><i class="ti ti-filter" aria-hidden="true"></i> Filtros</a></li>
                <li><a href="Moderacion.aspx?t=reportes" id="tabReportes" runat="server"><i class="ti ti-flag" aria-hidden="true"></i> Reportes<asp:Label ID="lblPendientes" runat="server" CssClass="mod-cuenta" Visible="false" /></a></li>
            </ul>
        </nav>

        <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="auth-aviso auth-ok" role="status">
            <strong><asp:Label ID="lblOk" runat="server" /></strong>
        </asp:Panel>
        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <%-- ================= USUARIOS ================= --%>
        <asp:Panel ID="pnlUsuarios" runat="server" Visible="false">

            <asp:Panel ID="pnlEditU" runat="server" Visible="false" CssClass="panel mod-editor">
                <h3>Editar usuario</h3>
                <asp:HiddenField ID="hfIdU" runat="server" />
                <p class="suave">Correo: <asp:Label ID="lblCorreoU" runat="server" /> (no se puede cambiar)</p>
                <div class="mod-form">
                    <div>
                        <asp:Label runat="server" AssociatedControlID="txtNomU" CssClass="rotulo" Text="Nombre" />
                        <asp:TextBox ID="txtNomU" runat="server" CssClass="campo" MaxLength="100" />
                    </div>
                    <div>
                        <asp:Label runat="server" AssociatedControlID="ddlCarU" CssClass="rotulo" Text="Carrera" />
                        <asp:DropDownList ID="ddlCarU" runat="server" CssClass="campo" />
                    </div>
                    <div class="ancho">
                        <asp:Label runat="server" AssociatedControlID="txtDescU" CssClass="rotulo" Text="Descripción" />
                        <asp:TextBox ID="txtDescU" runat="server" CssClass="campo" TextMode="MultiLine" Rows="2" MaxLength="200" />
                    </div>
                </div>
                <div class="mod-acciones">
                    <asp:Button ID="btnGuardarU" runat="server" Text="Guardar cambios" CssClass="mod-btn mod-btn--ok" OnClick="btnGuardarU_Click" />
                    <asp:Button ID="btnCancelarU" runat="server" Text="Cancelar" CssClass="mod-btn" CausesValidation="false" OnClick="btnCancelar_Click" />
                </div>
            </asp:Panel>

            <div class="mod-barra">
                <div>
                    <asp:Label runat="server" AssociatedControlID="txtBuscarU" CssClass="rotulo" Text="Buscar por nombre o correo" />
                    <asp:TextBox ID="txtBuscarU" runat="server" CssClass="campo" MaxLength="100" />
                </div>
                <asp:Button ID="btnBuscarU" runat="server" Text="Buscar" CssClass="mod-btn mod-btn--ok" OnClick="btnBuscarU_Click" />
            </div>

            <div class="mod-scroll">
                <asp:Repeater ID="rptUsuarios" runat="server" OnItemCommand="rptUsuarios_ItemCommand">
                    <HeaderTemplate>
                        <table class="mod-tabla">
                            <caption class="solo-lectores">Usuarios</caption>
                            <thead><tr><th scope="col">Usuario</th><th scope="col">Carrera</th><th scope="col">Estado</th><th scope="col">Alta</th><th scope="col">Acciones</th></tr></thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><strong><%# Server.HtmlEncode((string)Eval("Nombre")) %></strong><br /><span class="suave"><%# Server.HtmlEncode((string)Eval("Email")) %></span></td>
                            <td><%# Server.HtmlEncode((string)Eval("Carrera")) %></td>
                            <td>
                                <%# (int)Eval("Rol") == 1 ? "<span class=\"mod-etiqueta mod-etiqueta--mod\">Moderador</span> " : "" %>
                                <%# (bool)Eval("Activo") ? "<span class=\"mod-etiqueta mod-etiqueta--ok\">Activa</span>" : "<span class=\"mod-etiqueta mod-etiqueta--susp\">Suspendida</span>" %>
                                <%# (bool)Eval("EmailVerificado") ? "" : " <span class=\"mod-etiqueta\">Correo sin verificar</span>" %>
                            </td>
                            <td><%# ((DateTime)Eval("FechaRegistro")).ToLocalTime().ToString("dd/MM/yyyy") %></td>
                            <td>
                                <div class="mod-acciones">
                                    <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="editar" CommandArgument='<%# Eval("IdUsuario") %>' CausesValidation="false"><i class="ti ti-pencil" aria-hidden="true"></i> Editar<span class="solo-lectores"> a <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn mod-btn--peligro" CommandName="suspender" CommandArgument='<%# Eval("IdUsuario") %>' CausesValidation="false"
                                        OnClientClick='<%# (bool)Eval("Activo") ? "return confirm(\"¿Suspender esta cuenta? No va a poder iniciar sesión y sus publicaciones dejan de aparecer en el catálogo.\");" : "return true;" %>'><%# (bool)Eval("Activo") ? "Suspender" : "Reactivar" %><span class="solo-lectores"> a <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="rol" CommandArgument='<%# Eval("IdUsuario") %>' CausesValidation="false"
                                        OnClientClick='<%# (int)Eval("Rol") == 1 ? "return confirm(\"¿Sacarle el rol de moderador?\");" : "return confirm(\"¿Darle rol de moderador? Va a poder editar y suspender cuentas y publicaciones.\");" %>'><%# (int)Eval("Rol") == 1 ? "Quitar moderador" : "Hacer moderador" %><span class="solo-lectores"> a <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="verificar" CommandArgument='<%# Eval("IdUsuario") %>' CausesValidation="false"
                                        Visible='<%# !(bool)Eval("EmailVerificado") %>'>Verificar correo</asp:LinkButton>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate></tbody></table></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlVacioU" runat="server" Visible="false" CssClass="suave mod-vacio">No hay usuarios que coincidan.</asp:Panel>
            </div>
        </asp:Panel>

        <%-- ================= PUBLICACIONES ================= --%>
        <asp:Panel ID="pnlPublicaciones" runat="server" Visible="false">

            <asp:Panel ID="pnlEditP" runat="server" Visible="false" CssClass="panel mod-editor">
                <h3>Editar publicación</h3>
                <asp:HiddenField ID="hfIdP" runat="server" />
                <div class="mod-form">
                    <div class="ancho">
                        <asp:Label runat="server" AssociatedControlID="txtTitP" CssClass="rotulo" Text="Título" />
                        <asp:TextBox ID="txtTitP" runat="server" CssClass="campo" MaxLength="100" />
                    </div>
                    <div class="ancho">
                        <asp:Label runat="server" AssociatedControlID="txtDescP" CssClass="rotulo" Text="Descripción" />
                        <asp:TextBox ID="txtDescP" runat="server" CssClass="campo" TextMode="MultiLine" Rows="4" MaxLength="1000" />
                    </div>
                    <div>
                        <asp:Label runat="server" AssociatedControlID="ddlCatP" CssClass="rotulo" Text="Categoría" />
                        <asp:DropDownList ID="ddlCatP" runat="server" CssClass="campo" />
                    </div>
                    <div>
                        <asp:Label runat="server" AssociatedControlID="ddlCarP" CssClass="rotulo" Text="Carrera" />
                        <asp:DropDownList ID="ddlCarP" runat="server" CssClass="campo" />
                    </div>
                </div>
                <div class="mod-acciones">
                    <asp:Button ID="btnGuardarP" runat="server" Text="Guardar cambios" CssClass="mod-btn mod-btn--ok" OnClick="btnGuardarP_Click" />
                    <asp:Button ID="btnCancelarP" runat="server" Text="Cancelar" CssClass="mod-btn" CausesValidation="false" OnClick="btnCancelar_Click" />
                </div>
            </asp:Panel>

            <div class="mod-barra">
                <div>
                    <asp:Label runat="server" AssociatedControlID="txtBuscarP" CssClass="rotulo" Text="Buscar por título o dueño" />
                    <asp:TextBox ID="txtBuscarP" runat="server" CssClass="campo" MaxLength="100" />
                </div>
                <asp:Button ID="btnBuscarP" runat="server" Text="Buscar" CssClass="mod-btn mod-btn--ok" OnClick="btnBuscarP_Click" />
            </div>

            <div class="mod-scroll">
                <asp:Repeater ID="rptPublicaciones" runat="server" OnItemCommand="rptPublicaciones_ItemCommand">
                    <HeaderTemplate>
                        <table class="mod-tabla">
                            <caption class="solo-lectores">Publicaciones</caption>
                            <thead><tr><th scope="col">Publicación</th><th scope="col">Dueño</th><th scope="col">Estado</th><th scope="col">Fecha</th><th scope="col">Acciones</th></tr></thead>
                            <tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td><strong><%# Server.HtmlEncode((string)Eval("Titulo")) %></strong><br />
                                <span class="suave"><%# Server.HtmlEncode((string)Eval("Categoria")) %> · <%# (int)Eval("Modalidad") == 0 ? "Donación" : "Intercambio" %> · <%# Eval("Solicitudes") %> solicitud(es)</span></td>
                            <td><%# Server.HtmlEncode((string)Eval("Dueno")) %></td>
                            <td>
                                <%# (int)Eval("Estado") == 0 ? "<span class=\"mod-etiqueta mod-etiqueta--ok\">Disponible</span>" : ((int)Eval("Estado") == 1 ? "<span class=\"mod-etiqueta\">Reservado</span>" : "<span class=\"mod-etiqueta\">Entregado</span>") %>
                                <%# (bool)Eval("Suspendida") ? " <span class=\"mod-etiqueta mod-etiqueta--susp\">Suspendida</span>" : "" %>
                            </td>
                            <td><%# ((DateTime)Eval("Fecha")).ToLocalTime().ToString("dd/MM/yyyy") %></td>
                            <td>
                                <div class="mod-acciones">
                                    <a class="mod-btn" href='DetallePublicacion.aspx?id=<%# Eval("IdPublicacion") %>'>Ver<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Titulo")) %></span></a>
                                    <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="editar" CommandArgument='<%# Eval("IdPublicacion") %>' CausesValidation="false"><i class="ti ti-pencil" aria-hidden="true"></i> Editar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Titulo")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn mod-btn--peligro" CommandName="suspender" CommandArgument='<%# Eval("IdPublicacion") %>' CausesValidation="false"
                                        OnClientClick='<%# (bool)Eval("Suspendida") ? "return true;" : "return confirm(\"¿Suspender esta publicación? Deja de aparecer en el catálogo y no se puede pedir.\");" %>'><%# (bool)Eval("Suspendida") ? "Reactivar" : "Suspender" %><span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Titulo")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn mod-btn--peligro" CommandName="eliminar" CommandArgument='<%# Eval("IdPublicacion") %>' CausesValidation="false"
                                        OnClientClick='return confirm("¿Borrar esta publicación definitivamente? No se puede deshacer.");'>Borrar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Titulo")) %></span></asp:LinkButton>
                                </div>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate></tbody></table></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlVacioP" runat="server" Visible="false" CssClass="suave mod-vacio">No hay publicaciones que coincidan.</asp:Panel>
            </div>
        </asp:Panel>

        <%-- ================= FILTROS ================= --%>
        <asp:Panel ID="pnlFiltros" runat="server" Visible="false">

            <%-- Categorías --%>
            <section class="panel mod-bloque" aria-labelledby="h-cat">
                <h2 id="h-cat">Categorías</h2>
                <asp:Panel ID="pnlEditCat" runat="server" Visible="false" CssClass="mod-editor">
                    <asp:HiddenField ID="hfIdCat" runat="server" />
                    <div class="mod-form">
                        <div>
                            <asp:Label runat="server" AssociatedControlID="txtEdCat" CssClass="rotulo" Text="Nombre de la categoría" />
                            <asp:TextBox ID="txtEdCat" runat="server" CssClass="campo" MaxLength="50" />
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="txtEdPeso" CssClass="rotulo" Text="Peso estimado (kg)" />
                            <asp:TextBox ID="txtEdPeso" runat="server" CssClass="campo" MaxLength="8" />
                        </div>
                    </div>
                    <div class="mod-acciones">
                        <asp:Button ID="btnGuardarCat" runat="server" Text="Guardar categoría" CssClass="mod-btn mod-btn--ok" OnClick="btnGuardarCat_Click" />
                        <asp:Button ID="btnCancelarCat" runat="server" Text="Cancelar" CssClass="mod-btn" CausesValidation="false" OnClick="btnCancelar_Click" />
                    </div>
                </asp:Panel>
                <div class="mod-scroll">
                    <asp:Repeater ID="rptCategorias" runat="server" OnItemCommand="rptCategorias_ItemCommand">
                        <HeaderTemplate>
                            <table class="mod-tabla"><caption class="solo-lectores">Categorías</caption>
                            <thead><tr><th scope="col">Nombre</th><th scope="col">Peso estimado</th><th scope="col">En uso</th><th scope="col">Acciones</th></tr></thead><tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Server.HtmlEncode((string)Eval("Nombre")) %></td>
                                <td><%# ((decimal)Eval("PesoKg")).ToString("0.00") %> kg</td>
                                <td><%# Eval("EnUso") %> publicación(es)</td>
                                <td><div class="mod-acciones">
                                    <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="editar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Editar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn mod-btn--peligro" CommandName="eliminar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick='return confirm("¿Borrar esta categoría?");'>Borrar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                </div></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate></tbody></table></FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="mod-barra" style="margin-top:12px">
                    <div>
                        <asp:Label runat="server" AssociatedControlID="txtNuevaCat" CssClass="rotulo" Text="Nueva categoría" />
                        <asp:TextBox ID="txtNuevaCat" runat="server" CssClass="campo" MaxLength="50" />
                    </div>
                    <div>
                        <asp:Label runat="server" AssociatedControlID="txtNuevoPeso" CssClass="rotulo" Text="Peso estimado (kg)" />
                        <asp:TextBox ID="txtNuevoPeso" runat="server" CssClass="campo" MaxLength="8" Text="0,5" />
                    </div>
                    <asp:Button ID="btnAgregarCat" runat="server" Text="Agregar categoría" CssClass="mod-btn mod-btn--ok" OnClick="btnAgregarCat_Click" />
                </div>
            </section>

            <%-- Facultades --%>
            <section class="panel mod-bloque" aria-labelledby="h-fac">
                <h2 id="h-fac">Facultades</h2>
                <asp:Panel ID="pnlEditFac" runat="server" Visible="false" CssClass="mod-editor">
                    <asp:HiddenField ID="hfIdFac" runat="server" />
                    <div class="mod-form">
                        <div>
                            <asp:Label runat="server" AssociatedControlID="txtEdFac" CssClass="rotulo" Text="Nombre de la facultad" />
                            <asp:TextBox ID="txtEdFac" runat="server" CssClass="campo" MaxLength="100" />
                        </div>
                    </div>
                    <div class="mod-acciones">
                        <asp:Button ID="btnGuardarFac" runat="server" Text="Guardar facultad" CssClass="mod-btn mod-btn--ok" OnClick="btnGuardarFac_Click" />
                        <asp:Button ID="btnCancelarFac" runat="server" Text="Cancelar" CssClass="mod-btn" CausesValidation="false" OnClick="btnCancelar_Click" />
                    </div>
                </asp:Panel>
                <div class="mod-scroll">
                    <asp:Repeater ID="rptFacultades" runat="server" OnItemCommand="rptFacultades_ItemCommand">
                        <HeaderTemplate>
                            <table class="mod-tabla"><caption class="solo-lectores">Facultades</caption>
                            <thead><tr><th scope="col">Nombre</th><th scope="col">Carreras</th><th scope="col">Acciones</th></tr></thead><tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Server.HtmlEncode((string)Eval("Nombre")) %></td>
                                <td><%# Eval("Carreras") %></td>
                                <td><div class="mod-acciones">
                                    <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="editar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Editar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn mod-btn--peligro" CommandName="eliminar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick='return confirm("¿Borrar esta facultad?");'>Borrar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                </div></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate></tbody></table></FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="mod-barra" style="margin-top:12px">
                    <div>
                        <asp:Label runat="server" AssociatedControlID="txtNuevaFac" CssClass="rotulo" Text="Nueva facultad" />
                        <asp:TextBox ID="txtNuevaFac" runat="server" CssClass="campo" MaxLength="100" />
                    </div>
                    <asp:Button ID="btnAgregarFac" runat="server" Text="Agregar facultad" CssClass="mod-btn mod-btn--ok" OnClick="btnAgregarFac_Click" />
                </div>
            </section>

            <%-- Carreras --%>
            <section class="panel mod-bloque" aria-labelledby="h-car">
                <h2 id="h-car">Carreras</h2>
                <asp:Panel ID="pnlEditCarr" runat="server" Visible="false" CssClass="mod-editor">
                    <asp:HiddenField ID="hfIdCarr" runat="server" />
                    <div class="mod-form">
                        <div>
                            <asp:Label runat="server" AssociatedControlID="txtEdCarr" CssClass="rotulo" Text="Nombre de la carrera" />
                            <asp:TextBox ID="txtEdCarr" runat="server" CssClass="campo" MaxLength="100" />
                        </div>
                        <div>
                            <asp:Label runat="server" AssociatedControlID="ddlEdCarrFac" CssClass="rotulo" Text="Facultad" />
                            <asp:DropDownList ID="ddlEdCarrFac" runat="server" CssClass="campo" />
                        </div>
                    </div>
                    <div class="mod-acciones">
                        <asp:Button ID="btnGuardarCarr" runat="server" Text="Guardar carrera" CssClass="mod-btn mod-btn--ok" OnClick="btnGuardarCarr_Click" />
                        <asp:Button ID="btnCancelarCarr" runat="server" Text="Cancelar" CssClass="mod-btn" CausesValidation="false" OnClick="btnCancelar_Click" />
                    </div>
                </asp:Panel>
                <div class="mod-scroll">
                    <asp:Repeater ID="rptCarreras" runat="server" OnItemCommand="rptCarreras_ItemCommand">
                        <HeaderTemplate>
                            <table class="mod-tabla"><caption class="solo-lectores">Carreras</caption>
                            <thead><tr><th scope="col">Nombre</th><th scope="col">Facultad</th><th scope="col">En uso</th><th scope="col">Acciones</th></tr></thead><tbody>
                        </HeaderTemplate>
                        <ItemTemplate>
                            <tr>
                                <td><%# Server.HtmlEncode((string)Eval("Nombre")) %></td>
                                <td><%# Server.HtmlEncode((string)Eval("Facultad")) %></td>
                                <td><%# Eval("EnUso") %></td>
                                <td><div class="mod-acciones">
                                    <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="editar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false">Editar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                    <asp:LinkButton runat="server" CssClass="mod-btn mod-btn--peligro" CommandName="eliminar" CommandArgument='<%# Eval("Id") %>' CausesValidation="false" OnClientClick='return confirm("¿Borrar esta carrera?");'>Borrar<span class="solo-lectores"> <%# Server.HtmlEncode((string)Eval("Nombre")) %></span></asp:LinkButton>
                                </div></td>
                            </tr>
                        </ItemTemplate>
                        <FooterTemplate></tbody></table></FooterTemplate>
                    </asp:Repeater>
                </div>
                <div class="mod-barra" style="margin-top:12px">
                    <div>
                        <asp:Label runat="server" AssociatedControlID="txtNuevaCarr" CssClass="rotulo" Text="Nueva carrera" />
                        <asp:TextBox ID="txtNuevaCarr" runat="server" CssClass="campo" MaxLength="100" />
                    </div>
                    <div>
                        <asp:Label runat="server" AssociatedControlID="ddlNuevaCarrFac" CssClass="rotulo" Text="Facultad" />
                        <asp:DropDownList ID="ddlNuevaCarrFac" runat="server" CssClass="campo" />
                    </div>
                    <asp:Button ID="btnAgregarCarr" runat="server" Text="Agregar carrera" CssClass="mod-btn mod-btn--ok" OnClick="btnAgregarCarr_Click" />
                </div>
            </section>
        </asp:Panel>

        <%-- ================= REPORTES ================= --%>
        <asp:Panel ID="pnlReportes" runat="server" Visible="false">
            <div class="mod-barra">
                <asp:CheckBox ID="chkTodosRep" runat="server" Text="Mostrar también los ya atendidos" AutoPostBack="true" OnCheckedChanged="chkTodosRep_CheckedChanged" />
            </div>
            <div class="mod-scroll">
                <asp:Repeater ID="rptReportes" runat="server" OnItemCommand="rptReportes_ItemCommand">
                    <HeaderTemplate>
                        <table class="mod-tabla"><caption class="solo-lectores">Reportes</caption>
                        <thead><tr><th scope="col">Qué se reportó</th><th scope="col">Motivo</th><th scope="col">Reportó</th><th scope="col">Estado</th><th scope="col">Acciones</th></tr></thead><tbody>
                    </HeaderTemplate>
                    <ItemTemplate>
                        <tr>
                            <td>
                                <%# Eval("IdPublicacion") != null ? "Publicación: <strong>" + Server.HtmlEncode((string)Eval("Publicacion")) + "</strong>" : "Usuario: <strong>" + Server.HtmlEncode((string)Eval("UsuarioReportado")) + "</strong>" %>
                                <br /><span class="suave"><%# ((DateTime)Eval("Fecha")).ToLocalTime().ToString("dd/MM/yyyy HH:mm") %></span>
                            </td>
                            <td><%# Server.HtmlEncode((string)Eval("Motivo")) %><%# ((string)Eval("Detalle")).Length > 0 ? "<br /><span class=\"suave\">" + Server.HtmlEncode((string)Eval("Detalle")) + "</span>" : "" %></td>
                            <td><%# Server.HtmlEncode((string)Eval("Reportante")) %></td>
                            <td><%# (int)Eval("Estado") == 0 ? "<span class=\"mod-etiqueta mod-etiqueta--susp\">Pendiente</span>" : ((int)Eval("Estado") == 1 ? "<span class=\"mod-etiqueta\">Desestimado</span>" : "<span class=\"mod-etiqueta mod-etiqueta--ok\">Resuelto</span>") %></td>
                            <td><div class="mod-acciones">
                                <%# Eval("IdPublicacion") != null ? "<a class=\"mod-btn\" href=\"DetallePublicacion.aspx?id=" + Eval("IdPublicacion") + "\">Ver publicación</a>" : "" %>
                                <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="irPub" CommandArgument='<%# Eval("IdPublicacion") %>' CausesValidation="false" Visible='<%# Eval("IdPublicacion") != null %>'>Gestionar</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="irUsu" CommandArgument='<%# Eval("IdUsuarioReportado") %>' CausesValidation="false" Visible='<%# Eval("IdUsuarioReportado") != null %>'>Gestionar</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="mod-btn mod-btn--ok" CommandName="resuelto" CommandArgument='<%# Eval("IdReporte") %>' CausesValidation="false" Visible='<%# (int)Eval("Estado") == 0 %>'>Marcar resuelto</asp:LinkButton>
                                <asp:LinkButton runat="server" CssClass="mod-btn" CommandName="desestimado" CommandArgument='<%# Eval("IdReporte") %>' CausesValidation="false" Visible='<%# (int)Eval("Estado") == 0 %>'>Desestimar</asp:LinkButton>
                            </div></td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate></tbody></table></FooterTemplate>
                </asp:Repeater>
                <asp:Panel ID="pnlVacioR" runat="server" Visible="false" CssClass="suave mod-vacio">No hay reportes pendientes. 🎉</asp:Panel>
            </div>
        </asp:Panel>

    </div>
</asp:Content>
