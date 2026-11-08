<%@ Page Title="Editar perfil" Language="C#" MasterPageFile="~/Principal.Master"
    AutoEventWireup="true" CodeBehind="EditarPerfil.aspx.cs" Inherits="Pasalo1.EditarPerfil" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">

    <div class="perfil editar-perfil">

        <a class="volver" href="Perfil.aspx"><i class="ti ti-arrow-left" aria-hidden="true"></i> Volver a mi perfil</a>

        <h1 class="editar-titulo">Editar perfil</h1>

        <%-- ===== Mis datos ===== --%>
        <section class="panel editar-bloque" aria-labelledby="t-datos">
            <h2 id="t-datos">Mis datos</h2>

            <asp:Panel ID="pnlOkDatos" runat="server" Visible="false" CssClass="auth-aviso auth-ok" role="status">
                <strong>✔ Cambios guardados.</strong>
            </asp:Panel>

            <asp:Panel ID="pnlErrorDatos" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
                <asp:Label ID="lblErrorDatos" runat="server" />
            </asp:Panel>

            <div class="editar-campo">
                <asp:Label runat="server" AssociatedControlID="txtNombre" CssClass="rotulo" Text="Nombre" />
                <asp:TextBox ID="txtNombre" runat="server" CssClass="campo" MaxLength="100" autocomplete="name" />
            </div>

            <div class="editar-campo">
                <asp:Label runat="server" AssociatedControlID="ddlCarrera" CssClass="rotulo" Text="Carrera" />
                <asp:DropDownList ID="ddlCarrera" runat="server" CssClass="campo" />
            </div>

            <div class="editar-campo">
                <asp:Label runat="server" AssociatedControlID="txtDescripcion" CssClass="rotulo" Text="Descripción (opcional)" />
                <asp:TextBox ID="txtDescripcion" runat="server" CssClass="campo" TextMode="MultiLine" Rows="3"
                    maxlength="200" aria-describedby="ayudaDescripcion"
                    placeholder="Por ejemplo: curso 2.º año y tengo apuntes de primer año." />
                <p class="suave editar-ayuda" id="ayudaDescripcion">
                    Se muestra en tu perfil a las demás personas. <span id="contadorDescripcion">0 / 200</span>
                </p>
            </div>

            <div class="editar-campo">
                <span class="rotulo">Correo institucional</span>
                <p class="editar-correo"><asp:Label ID="lblCorreo" runat="server" /> <span class="suave">(no se puede cambiar)</span></p>
            </div>

            <asp:Button ID="btnGuardarDatos" runat="server" Text="Guardar cambios" CssClass="boton editar-boton"
                OnClick="btnGuardarDatos_Click" />
        </section>

        <%-- ===== Cambiar contraseña ===== --%>
        <section class="panel editar-bloque" aria-labelledby="t-clave">
            <h2 id="t-clave">Cambiar contraseña</h2>

            <asp:Panel ID="pnlOkClave" runat="server" Visible="false" CssClass="auth-aviso auth-ok" role="status">
                <strong>✔ Contraseña actualizada.</strong>
            </asp:Panel>

            <asp:Panel ID="pnlErrorClave" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
                <asp:Label ID="lblErrorClave" runat="server" />
            </asp:Panel>

            <div class="editar-campo">
                <asp:Label runat="server" AssociatedControlID="txtClaveActual" CssClass="rotulo" Text="Contraseña actual" />
                <asp:TextBox ID="txtClaveActual" runat="server" CssClass="campo" TextMode="Password" MaxLength="100" autocomplete="current-password" />
            </div>

            <div class="editar-campo">
                <asp:Label runat="server" AssociatedControlID="txtClaveNueva" CssClass="rotulo" Text="Contraseña nueva (mínimo 8 caracteres)" />
                <asp:TextBox ID="txtClaveNueva" runat="server" CssClass="campo" TextMode="Password" MaxLength="100" autocomplete="new-password" />
            </div>

            <div class="editar-campo">
                <asp:Label runat="server" AssociatedControlID="txtClaveRepetida" CssClass="rotulo" Text="Repetí la contraseña nueva" />
                <asp:TextBox ID="txtClaveRepetida" runat="server" CssClass="campo" TextMode="Password" MaxLength="100" autocomplete="new-password" />
            </div>

            <asp:Button ID="btnCambiarClave" runat="server" Text="Cambiar contraseña" CssClass="boton editar-boton"
                OnClick="btnCambiarClave_Click" />
        </section>

    </div>

    <script>
        (function () {
            var texto = document.getElementById('<%= txtDescripcion.ClientID %>');
            var contador = document.getElementById('contadorDescripcion');
            if (!texto || !contador) return;
            function actualizar() { contador.textContent = texto.value.length + ' / 200'; }
            texto.addEventListener('input', actualizar);
            actualizar();
        })();
    </script>

</asp:Content>
