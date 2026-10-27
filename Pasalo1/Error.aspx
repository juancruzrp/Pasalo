<%@ Page Title="Algo salió mal" Language="C#" MasterPageFile="~/Publica.Master"
    AutoEventWireup="true" CodeBehind="Error.aspx.cs" Inherits="Pasalo1.ErrorPagina" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">
    <style>
        .error-pagina { min-height: calc(100vh - 80px); background: #f8f7f3; padding: 80px 20px; display: flex; justify-content: center; }
        .error-caja { background: #fff; border-radius: 16px; padding: 40px 36px; max-width: 460px; text-align: center; box-shadow: 0 2px 12px rgba(0,0,0,.06); }
        .error-caja h1 { font-size: 1.5rem; margin: 0 0 10px; }
        .error-caja p { color: #555; margin: 0 0 22px; line-height: 1.5; }
        .error-caja a { display: inline-block; margin: 4px; padding: 10px 20px; border-radius: 10px; background: #2f7d4f; color: #fff; text-decoration: none; font-weight: 700; }
        .error-caja a.sec { background: #eee; color: #333; }
    </style>
</asp:Content>

<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">
    <div class="error-pagina">
        <div class="error-caja">
            <h1>Ups, algo salió mal</h1>
            <p>Ocurrió un problema inesperado. Ya podés volver a intentar; si sigue pasando, avisale al equipo.</p>
            <a href="Default.aspx">Volver al catálogo</a>
            <a href="Inicio.aspx" class="sec">Ir al inicio</a>
        </div>
    </div>
</asp:Content>
