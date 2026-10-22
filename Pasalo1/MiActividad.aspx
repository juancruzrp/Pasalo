
<%@ Page Title="Mi actividad" Language="C#"
    MasterPageFile="~/Principal.Master"
    AutoEventWireup="true"
    CodeBehind="MiActividad.aspx.cs"
    Inherits="Pasalo1.MiActividad" %>

<asp:Content ID="Contenido"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .actividad {
            max-width: 1050px;
            margin: 40px auto;
            padding: 0 20px 50px;
        }

        .actividad h1 {
            color: #244f53;
            margin-bottom: 8px;
        }

        .actividad-subtitulo {
            color: #718183;
            margin-bottom: 28px;
        }

        .actividad-tabs {
            display: flex;
            gap: 10px;
            margin-bottom: 24px;
        }

        .actividad-tab {
            display: inline-block;
            padding: 12px 20px;
            border-radius: 10px;
            border: 1px solid #d7e1df;
            background: white;
            color: #28666d;
            font-weight: 700;
            text-decoration: none;
        }

        .actividad-tab.activa {
            background: #28666d;
            color: white;
        }

        .actividad-lista {
            display: grid;
            gap: 16px;
        }

        .actividad-card {
            background: white;
            border: 1px solid #e3e8e5;
            border-radius: 14px;
            padding: 22px;
        }

        .actividad-cabecera {
            display: flex;
            justify-content: space-between;
            align-items: flex-start;
            gap: 15px;
            margin-bottom: 15px;
        }

        .actividad-titulo {
            font-size: 18px;
            font-weight: 700;
            color: #244f53;
            text-decoration: none;
        }

        .actividad-titulo:hover {
            text-decoration: underline;
        }

        .actividad-dato {
            color: #5b6f72;
            margin: 7px 0;
            line-height: 1.5;
        }

        .actividad-oferta {
            background: #f5f7f7;
            border-radius: 10px;
            padding: 14px;
            margin-top: 12px;
            overflow-wrap: anywhere;
        }

        .actividad-estado {
            padding: 6px 12px;
            border-radius: 20px;
            font-size: 12px;
            font-weight: 700;
            white-space: nowrap;
        }

        .actividad-estado.pendiente {
            background: #fff3d6;
            color: #946200;
        }

        .actividad-estado.aceptada {
            background: #e0f2e7;
            color: #237a49;
        }

        .actividad-estado.rechazada {
            background: #fce8e8;
            color: #ad3434;
        }

        .actividad-vacio {
            background: white;
            border: 1px solid #e3e8e5;
            border-radius: 14px;
            padding: 45px 20px;
            text-align: center;
            color: #718183;
        }

        /* Botones de gestión de solicitudes */

        .actividad-acciones {
            display: flex;
            gap: 10px;
            margin-top: 18px;
            flex-wrap: wrap;
        }

        .actividad-btn-aceptar,
        .actividad-btn-rechazar {
            padding: 11px 18px;
            border-radius: 10px;
            font-weight: 700;
            cursor: pointer;
            font-family: inherit;
            transition: background 0.2s;
        }

        .actividad-btn-aceptar {
            background: #28666d;
            color: white;
            border: 1px solid #28666d;
        }

        .actividad-btn-aceptar:hover {
            background: #1d5056;
        }

        .actividad-btn-rechazar {
            background: white;
            color: #ad3434;
            border: 1px solid #e7bcbc;
        }

        .actividad-btn-rechazar:hover {
            background: #fce8e8;
        }

        .actividad-error {
            background: #fce8e8;
            color: #ad3434;
            padding: 14px;
            border-radius: 10px;
            margin-bottom: 20px;
        }

        @media (max-width: 600px) {
            .actividad-tabs {
                flex-direction: column;
            }

            .actividad-cabecera {
                flex-direction: column;
            }

            .actividad-acciones {
                flex-direction: column;
            }
        }
    </style>

    <div class="actividad">

        <h1>Mi actividad</h1>

        <p class="actividad-subtitulo">
            Consultá tus solicitudes de donación e intercambio.
        </p>

        <!-- Mensajes de error -->

        <asp:Panel ID="pnlErrorSolicitud"
            runat="server"
            Visible="false"
            CssClass="actividad-error">

            <asp:Label ID="lblErrorSolicitud"
                runat="server" />

        </asp:Panel>



        <asp:Panel ID="Panel1"
            runat="server"
            Visible="false"
            Style="background:#fce8e8;color:#ad3434;
                   padding:14px;border-radius:10px;margin-bottom:20px;">

            <asp:Label ID="Label1" runat="server" />

        </asp:Panel>

        <!-- Pestañas -->

        <nav class="actividad-tabs"
            aria-label="Tipo de solicitudes">

            <a href="MiActividad.aspx?tab=recibidas"
                class='actividad-tab <%= TabActiva == "recibidas" ? "activa" : "" %>'>
                Recibidas
            </a>

            <a href="MiActividad.aspx?tab=enviadas"
                class='actividad-tab <%= TabActiva == "enviadas" ? "activa" : "" %>'>
                Enviadas
            </a>

        </nav>

        <!-- SOLICITUDES RECIBIDAS -->

        <asp:Panel ID="pnlRecibidas" runat="server">

            <h2>Solicitudes recibidas</h2>

            <p class="actividad-subtitulo">
                Estudiantes interesados en tus publicaciones.
            </p>

            <div class="actividad-lista">

                <asp:Repeater ID="rptRecibidas" runat="server">

                    <ItemTemplate>

                        <article class="actividad-card">

                            <div class="actividad-cabecera">

                                <a class="actividad-titulo"
                                    href='DetallePublicacion.aspx?id=<%# Eval("IdPublicacion") %>'>
                                    <%#: Eval("TituloPublicacion") %>
                                </a>

                                <span class='actividad-estado <%# EstadoCss(Eval("Estado")) %>'>
                                    <%#: Eval("EstadoTexto") %>
                                </span>

                            </div>

                            <p class="actividad-dato">
                                <strong>Solicitante:</strong>
                                <%#: Eval("NombreUsuario") %>
                            </p>

                            <p class="actividad-dato">
                                <strong>Modalidad:</strong>
                                <%#: Eval("Modalidad") %>
                            </p>

                            <p class="actividad-dato">
                                <strong>Fecha:</strong>
                                <%#: Eval("FechaTexto") %>
                            </p>

                            <asp:Panel runat="server"
                                Visible='<%# TieneOferta(Eval("ObjetoOfrecido")) %>'
                                CssClass="actividad-oferta">

                                <strong>Objeto ofrecido:</strong>

                                <p>
                                    <%#: Eval("ObjetoOfrecido") %>
                                </p>

                            </asp:Panel>

                            <!-- Botones solamente para solicitudes pendientes -->

                            <asp:Panel runat="server"
                                Visible='<%# Convert.ToInt32(Eval("Estado")) == 0 %>'
                                CssClass="actividad-acciones">

                                <asp:Button ID="btnAceptar"
                                    runat="server"
                                    Text="Aceptar solicitud"
                                    CommandArgument='<%# Eval("IdSolicitud") %>'
                                    OnClick="btnAceptar_Click"
                                    OnClientClick="return confirm('¿Querés aceptar esta solicitud? Las demás solicitudes pendientes de este artículo serán rechazadas automáticamente.');"
                                    CssClass="actividad-btn-aceptar" />

                                <asp:Button ID="btnRechazar"
                                    runat="server"
                                    Text="Rechazar"
                                    CommandArgument='<%# Eval("IdSolicitud") %>'
                                    OnClick="btnRechazar_Click"
                                    OnClientClick="return confirm('¿Querés rechazar esta solicitud?');"
                                    CssClass="actividad-btn-rechazar" />

                            </asp:Panel>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

            <asp:Panel ID="pnlSinRecibidas"
                runat="server"
                Visible="false"
                CssClass="actividad-vacio">

                <i class="ti ti-inbox"
                    style="font-size:36px;"></i>

                <p>Todavía no recibiste solicitudes.</p>

            </asp:Panel>

        </asp:Panel>

        <!-- SOLICITUDES ENVIADAS -->

        <asp:Panel ID="pnlEnviadas"
            runat="server"
            Visible="false">

            <h2>Solicitudes enviadas</h2>

            <p class="actividad-subtitulo">
                Consultá el estado de tus pedidos.
            </p>

            <div class="actividad-lista">

                <asp:Repeater ID="rptEnviadas" runat="server">

                    <ItemTemplate>

                        <article class="actividad-card">

                            <div class="actividad-cabecera">

                                <a class="actividad-titulo"
                                    href='DetallePublicacion.aspx?id=<%# Eval("IdPublicacion") %>'>
                                    <%#: Eval("TituloPublicacion") %>
                                </a>

                                <span class='actividad-estado <%# EstadoCss(Eval("Estado")) %>'>
                                    <%#: Eval("EstadoTexto") %>
                                </span>

                            </div>

                            <p class="actividad-dato">
                                <strong>Publicado por:</strong>
                                <%#: Eval("NombreUsuario") %>
                            </p>

                            <p class="actividad-dato">
                                <strong>Modalidad:</strong>
                                <%#: Eval("Modalidad") %>
                            </p>

                            <p class="actividad-dato">
                                <strong>Fecha:</strong>
                                <%#: Eval("FechaTexto") %>
                            </p>

                            <asp:Panel runat="server"
                                Visible='<%# TieneOferta(Eval("ObjetoOfrecido")) %>'
                                CssClass="actividad-oferta">

                                <strong>Tu propuesta:</strong>

                                <p>
                                    <%#: Eval("ObjetoOfrecido") %>
                                </p>

                            </asp:Panel>

                        </article>

                    </ItemTemplate>

                </asp:Repeater>

            </div>

            <asp:Panel ID="pnlSinEnviadas"
                runat="server"
                Visible="false"
                CssClass="actividad-vacio">

                <i class="ti ti-send"
                    style="font-size:36px;"></i>

                <p>Todavía no enviaste solicitudes.</p>

                <a href="Default.aspx">
                    Explorar publicaciones
                </a>

            </asp:Panel>

        </asp:Panel>

    </div>

</asp:Content>
