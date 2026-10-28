<%@ Page Title="Confirmar correo" Language="C#" MasterPageFile="~/Publica.Master" AutoEventWireup="true" CodeBehind="VerificarEmail.aspx.cs" Inherits="Pasalo1.VerificarEmail" %>

<%-- IMPORTANTE: ContentPlaceHolderID tiene que ser el mismo que usa tu Login.aspx --%>
<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="auth-caja">
        <h1>Confirmar correo</h1>

        <%-- Con el enlace del correo se muestra este botón; recién al hacer clic se confirma.
             Así los filtros de correo que "visitan" los enlaces no gastan el código. --%>
        <asp:Panel ID="pnlConfirmar" runat="server" Visible="false">
            <p>Un último paso: confirmá que este correo es tuyo para activar tu cuenta.</p>
            <asp:Button ID="btnConfirmar" runat="server" Text="Confirmar mi cuenta" CssClass="auth-boton" OnClick="btnConfirmar_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="auth-aviso auth-ok" role="status">
            <strong>✔ Cuenta confirmada.</strong> Ya podés ingresar.
            <p><a href="Login.aspx">Ir a iniciar sesión</a></p>
        </asp:Panel>

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
            <strong>✖ El enlace no es válido.</strong> Ya se usó o venció. Pedí uno nuevo acá abajo.
        </asp:Panel>

        <asp:Panel ID="pnlReenviar" runat="server" Visible="false">
            <h2>¿No te llegó el correo?</h2>
            <p>Escribí tu correo institucional y te mandamos un enlace nuevo.</p>
            <label for="<%= txtEmail.ClientID %>">Correo institucional</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="auth-campo" TextMode="Email" MaxLength="150" autocomplete="email" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" Display="Dynamic"
                CssClass="auth-validacion" ErrorMessage="Ingresá tu correo." Text="Ingresá tu correo." ValidationGroup="reenvio" />
            <asp:Button ID="btnReenviar" runat="server" Text="Reenviar correo" CssClass="auth-boton"
                OnClick="btnReenviar_Click" ValidationGroup="reenvio" />

            <asp:Panel ID="pnlMensaje" runat="server" Visible="false" CssClass="auth-aviso" role="status">
                <asp:Label ID="lblMensaje" runat="server" />
            </asp:Panel>
        </asp:Panel>
    </section>
</asp:Content>
