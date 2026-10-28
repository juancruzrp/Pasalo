<%@ Page Title="Recuperar contraseña" Language="C#" MasterPageFile="~/Publica.Master" AutoEventWireup="true" CodeBehind="SolicitarRecuperacion.aspx.cs" Inherits="Pasalo1.SolicitarRecuperacion" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="auth-caja">
        <h1>Recuperar contraseña</h1>
        <p>Ingresá el correo con el que te registraste y te enviamos un enlace para crear una contraseña nueva.</p>

        <asp:Panel ID="pnlForm" runat="server">
            <label for="<%= txtEmail.ClientID %>">Correo institucional</label>
            <asp:TextBox ID="txtEmail" runat="server" CssClass="auth-campo" TextMode="Email" MaxLength="150" autocomplete="email" />
            <asp:RequiredFieldValidator runat="server" ControlToValidate="txtEmail" Display="Dynamic"
                CssClass="auth-validacion" ErrorMessage="Ingresá tu correo." Text="Ingresá tu correo." />
            <asp:Button ID="btnEnviar" runat="server" Text="Enviar enlace" CssClass="auth-boton" OnClick="btnEnviar_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlListo" runat="server" Visible="false" CssClass="auth-aviso auth-ok" role="status">
            <strong>Listo.</strong> Si el correo está registrado, en unos minutos te llega un enlace (vale 1 hora). Revisá también el spam.
        </asp:Panel>

        <p><a href="Login.aspx">Volver a iniciar sesión</a></p>
    </section>
</asp:Content>
