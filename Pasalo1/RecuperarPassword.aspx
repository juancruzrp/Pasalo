<%@ Page Title="Nueva contraseña" Language="C#" MasterPageFile="~/Publica.Master" AutoEventWireup="true" CodeBehind="RecuperarPassword.aspx.cs" Inherits="Pasalo1.RecuperarPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <section class="auth-caja">
        <h1>Crear contraseña nueva</h1>

        <asp:Panel ID="pnlForm" runat="server">
            <label for="<%= txtPassword.ClientID %>">Contraseña nueva (mínimo 8 caracteres)</label>
            <asp:TextBox ID="txtPassword" runat="server" CssClass="auth-campo" TextMode="Password" MaxLength="100" autocomplete="new-password" />

            <label for="<%= txtConfirmar.ClientID %>">Repetí la contraseña</label>
            <asp:TextBox ID="txtConfirmar" runat="server" CssClass="auth-campo" TextMode="Password" MaxLength="100" autocomplete="new-password" />

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>

            <asp:Button ID="btnGuardar" runat="server" Text="Guardar contraseña" CssClass="auth-boton" OnClick="btnGuardar_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="auth-aviso auth-ok" role="status">
            <strong>✔ Contraseña actualizada.</strong>
            <p><a href="Login.aspx">Ir a iniciar sesión</a></p>
        </asp:Panel>

        <asp:Panel ID="pnlInvalido" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
            <strong>✖ El enlace no es válido.</strong> Ya se usó o venció.
            <p><a href="SolicitarRecuperacion.aspx">Pedir un enlace nuevo</a></p>
        </asp:Panel>
    </section>
</asp:Content>
