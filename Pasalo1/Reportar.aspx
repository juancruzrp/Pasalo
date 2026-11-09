<%@ Page Title="Reportar" Language="C#" MasterPageFile="~/Principal.Master"
    AutoEventWireup="true" CodeBehind="Reportar.aspx.cs" Inherits="Pasalo1.Reportar" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mod" style="max-width:640px">

        <a class="volver" href="Default.aspx"><i class="ti ti-arrow-left" aria-hidden="true"></i> Volver al catálogo</a>

        <h1>Reportar</h1>
        <p class="suave mod-sub">Lo que reportes lo revisa el equipo de moderación. Tu nombre no se le muestra a la persona reportada.</p>

        <asp:Panel ID="pnlForm" runat="server" CssClass="panel">
            <p><strong><asp:Label ID="lblQue" runat="server" /></strong></p>

            <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>

            <div class="mod-form">
                <div class="ancho">
                    <asp:Label runat="server" AssociatedControlID="ddlMotivo" CssClass="rotulo" Text="Motivo" />
                    <asp:DropDownList ID="ddlMotivo" runat="server" CssClass="campo">
                        <asp:ListItem Text="Contenido inapropiado u ofensivo" />
                        <asp:ListItem Text="Es spam o publicidad" />
                        <asp:ListItem Text="Parece una estafa" />
                        <asp:ListItem Text="No cumplió lo acordado" />
                        <asp:ListItem Text="Información falsa" />
                        <asp:ListItem Text="Otro" />
                    </asp:DropDownList>
                </div>
                <div class="ancho">
                    <asp:Label runat="server" AssociatedControlID="txtDetalle" CssClass="rotulo" Text="Detalle (opcional)" />
                    <asp:TextBox ID="txtDetalle" runat="server" CssClass="campo" TextMode="MultiLine" Rows="4" MaxLength="500" />
                </div>
            </div>
            <asp:Button ID="btnEnviar" runat="server" Text="Enviar reporte" CssClass="mod-btn mod-btn--ok" OnClick="btnEnviar_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlOk" runat="server" Visible="false" CssClass="auth-aviso auth-ok" role="status">
            <strong>✔ Gracias, recibimos tu reporte.</strong> Un moderador lo va a revisar.
        </asp:Panel>
        <asp:Panel ID="pnlNo" runat="server" Visible="false" CssClass="auth-aviso auth-error" role="alert">
            No encontramos lo que querés reportar.
        </asp:Panel>
    </div>
</asp:Content>
