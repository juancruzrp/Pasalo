<%@ Page Title="Chat" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Chat.aspx.cs" Inherits="Pasalo1.Chat" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">

    <style>
        .chat { max-width: 1050px; margin: 40px auto; padding: 0 20px 50px; }
        .chat h1 { color: #244f53; margin-bottom: 8px; }
        .chat-subtitulo { color: #718183; margin-bottom: 24px; }

        .chat-error { background: #fce8e8; color: #ad3434; padding: 14px; border-radius: 10px; margin-bottom: 20px; }

        .chat-vacio { background: white; border: 1px solid #e3e8e5; border-radius: 14px; padding: 45px 20px; text-align: center; color: #718183; }
        .chat-vacio a { color: #28666d; font-weight: 700; }

        .chat-grid { display: grid; grid-template-columns: 320px 1fr; gap: 20px; align-items: start; }

        /* Lista de conversaciones */
        .chat-lista { background: white; border: 1px solid #e3e8e5; border-radius: 14px; overflow: hidden; }
        .chat-item { display: flex; gap: 12px; padding: 14px 16px; border-bottom: 1px solid #eef2f0; text-decoration: none; color: inherit; }
        .chat-item:last-child { border-bottom: 0; }
        .chat-item:hover { background: #f5f7f7; }
        .chat-item.activa { background: #e8f1f1; }
        .chat-avatar { flex: 0 0 40px; height: 40px; border-radius: 50%; background: #28666d; color: white; display: flex; align-items: center; justify-content: center; font-weight: 700; }
        .chat-item-texto { min-width: 0; }
        .chat-item-nombre { font-weight: 700; color: #244f53; margin: 0; }
        .chat-item-cab { display: flex; justify-content: space-between; align-items: center; gap: 8px; }
        .chat-nuevo { background: #d9534f; color: white; border-radius: 10px; min-width: 20px; padding: 1px 6px; font-size: 12px; font-weight: 700; text-align: center; }
        .chat-item--nuevo .chat-item-ultimo { color: #244f53; font-weight: 700; }
        .chat-item-art, .chat-item-ultimo { margin: 2px 0 0; font-size: 13px; color: #718183; white-space: nowrap; overflow: hidden; text-overflow: ellipsis; }

        /* Panel de la conversación */
        .chat-panel { background: white; border: 1px solid #e3e8e5; border-radius: 14px; display: flex; flex-direction: column; min-height: 460px; }
        .chat-cabecera { padding: 16px 20px; border-bottom: 1px solid #eef2f0; display: flex; justify-content: space-between; align-items: center; gap: 12px; flex-wrap: wrap; }
        .chat-cabecera h2 { margin: 0; font-size: 18px; color: #244f53; }
        .chat-cabecera p { margin: 2px 0 0; font-size: 14px; color: #718183; }
        .chat-cabecera a { color: #28666d; font-weight: 700; text-decoration: none; }
        .chat-cabecera a:hover { text-decoration: underline; }

        .chat-mensajes { flex: 1; max-height: 420px; overflow-y: auto; padding: 18px 20px; display: flex; flex-direction: column; gap: 10px; background: #fafbfa; }
        .chat-sin-mensajes { margin: auto; text-align: center; color: #718183; }
        .burbuja { max-width: 70%; padding: 10px 14px; border-radius: 14px; overflow-wrap: anywhere; line-height: 1.4; }
        .burbuja p { margin: 0; }
        .burbuja small { display: block; margin-top: 4px; font-size: 11px; opacity: .75; }
        .burbuja--mia { align-self: flex-end; background: #28666d; color: white; border-bottom-right-radius: 4px; }
        .burbuja--otra { align-self: flex-start; background: #eef2f0; color: #244f53; border-bottom-left-radius: 4px; }

        .chat-enviar { display: flex; gap: 10px; padding: 14px 20px; border-top: 1px solid #eef2f0; }
        .chat-texto { flex: 1; padding: 11px 14px; border: 1px solid #d7e1df; border-radius: 10px; font-family: inherit; font-size: 15px; }
        .chat-boton { padding: 11px 20px; border-radius: 10px; border: 1px solid #28666d; background: #28666d; color: white; font-weight: 700; font-family: inherit; cursor: pointer; }
        .chat-boton:hover { background: #1d5056; }
        .chat-elegir { margin: auto; padding: 40px 20px; text-align: center; color: #718183; }

        @media (max-width: 800px) {
            .chat-grid { grid-template-columns: 1fr; }
            .burbuja { max-width: 88%; }
        }
    </style>

    <div class="chat">

        <h1>Chat</h1>
        <p class="chat-subtitulo">Coordiná la entrega con la otra persona. El chat se habilita cuando una solicitud es aceptada.</p>

        <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="chat-error" role="alert">
            <asp:Label ID="lblError" runat="server" />
        </asp:Panel>

        <%-- Sin solicitudes aceptadas todavía --%>
        <asp:Panel ID="pnlVacio" runat="server" Visible="false" CssClass="chat-vacio">
            <i class="ti ti-messages" style="font-size:36px;" aria-hidden="true"></i>
            <p>Todavía no tenés conversaciones.</p>
            <p>Cuando el dueño de un artículo acepte tu solicitud (o vos aceptes una), el chat aparece acá.</p>
            <a href="MiActividad.aspx">Ver mi actividad</a>
        </asp:Panel>

        <asp:Panel ID="pnlGrid" runat="server" Visible="false" CssClass="chat-grid">

            <%-- Columna izquierda: una conversación por solicitud aceptada (ver Chat.aspx.cs y MensajeAcceso.cs) --%>
            <nav class="chat-lista" aria-label="Conversaciones">
                <% foreach (dominio.ConversacionChat c in Conversaciones) { %>
                <a class="chat-item<%= (Actual != null && Actual.IdSolicitud == c.IdSolicitud) ? " activa" : "" %><%= c.NoLeidos > 0 ? " chat-item--nuevo" : "" %>"
                   href="Chat.aspx?id=<%= c.IdSolicitud %>"<%= (Actual != null && Actual.IdSolicitud == c.IdSolicitud) ? " aria-current=\"page\"" : "" %>>
                    <span class="chat-avatar" aria-hidden="true"><%: Inicial(c.NombreOtraPersona) %></span>
                    <span class="chat-item-texto">
                        <span class="chat-item-cab">
                            <p class="chat-item-nombre"><%: c.NombreOtraPersona %></p>
                            <% if (c.NoLeidos > 0) { %><span class="chat-nuevo" title="Mensajes nuevos"><%= c.NoLeidos %></span><% } %>
                        </span>
                        <p class="chat-item-art"><%: c.TituloPublicacion %></p>
                        <p class="chat-item-ultimo"><%: Resumen(c) %></p>
                    </span>
                </a>
                <% } %>
            </nav>

            <%-- Columna derecha: la conversación elegida --%>
            <section class="chat-panel" aria-label="Conversación">

                <% if (Actual == null) { %>
                <div class="chat-elegir">Elegí una conversación de la lista.</div>
                <% } %>

                <asp:Panel ID="pnlChat" runat="server" Visible="false" DefaultButton="btnEnviar" Style="display:flex;flex-direction:column;flex:1;">

                    <div class="chat-cabecera">
                        <div>
                            <h2><a href="Perfil.aspx?id=<%= Actual != null ? Actual.IdOtraPersona : 0 %>"><%: Actual != null ? Actual.NombreOtraPersona : "" %></a></h2>
                            <p>Sobre: <a href="DetallePublicacion.aspx?id=<%= Actual != null ? Actual.IdPublicacion : 0 %>"><%: Actual != null ? Actual.TituloPublicacion : "" %></a></p>
                        </div>
                        <a href="Chat.aspx?id=<%= Actual != null ? Actual.IdSolicitud : 0 %>"><i class="ti ti-refresh" aria-hidden="true"></i> Actualizar</a>
                    </div>

                    <div class="chat-mensajes" id="chatMensajes" aria-live="polite">
                        <% if (Mensajes.Count == 0) { %>
                        <p class="chat-sin-mensajes">Todavía no hay mensajes. ¡Escribí el primero para coordinar la entrega!</p>
                        <% } %>
                        <% foreach (dominio.MensajeChat m in Mensajes) { %>
                        <div class="burbuja <%= m.IdEmisor == IdYo ? "burbuja--mia" : "burbuja--otra" %>">
                            <p><%: m.Texto %></p>
                            <small><%: Hora(m.FechaEnvio) %></small>
                        </div>
                        <% } %>
                    </div>

                    <div class="chat-enviar">
                        <asp:TextBox ID="txtMensaje" runat="server" CssClass="chat-texto" MaxLength="1000"
                            placeholder="Escribí un mensaje…" autocomplete="off" aria-label="Mensaje" />
                        <asp:Button ID="btnEnviar" runat="server" Text="Enviar" CssClass="chat-boton" OnClick="btnEnviar_Click" />
                    </div>

                    <script>
                        (function () {
                            // Abre la conversación mostrando lo último que se escribió
                            var caja = document.getElementById('chatMensajes');
                            if (caja) caja.scrollTop = caja.scrollHeight;

                            // Cada 20 s trae los mensajes nuevos, pero solo si no estás escribiendo
                            var txt = document.getElementById('<%= txtMensaje.ClientID %>');
                            setInterval(function () {
                                if (txt && txt.value === '' && document.activeElement !== txt) {
                                    window.location.href = 'Chat.aspx?id=<%= Actual != null ? Actual.IdSolicitud : 0 %>';
                                }
                            }, 20000);
                        })();
                    </script>

                </asp:Panel>
            </section>
        </asp:Panel>

    </div>

</asp:Content>
