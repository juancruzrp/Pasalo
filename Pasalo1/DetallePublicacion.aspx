<%@ Page Title="Detalle de publicación" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="DetallePublicacion.aspx.cs" Inherits="Pasalo1.DetallePublicacion" %>

<asp:Content ID="Contenido"
    ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .detalle-galeria {
            display: grid;
            grid-template-columns: repeat(2, minmax(0, 1fr));
            gap: 10px;
            align-content: start;
        }

        .detalle-galeria .galeria-foto {
            width: 100%;
            aspect-ratio: 1 / 1;
            object-fit: cover;
            border-radius: 12px;
            display: block;
            background: #f2f2f2;
        }

        .detalle-galeria .galeria-foto:first-child {
            grid-column: 1 / -1;
            aspect-ratio: 4 / 3;
        }

        .detalle-galeria-vacia {
            padding: 50px 20px;
            text-align: center;
            background: #f4f4f4;
            border-radius: 12px;
        }

        .detalle-galeria-vacia i {
            font-size: 40px;
        }

        .detalle-info-extra {
            margin-top: 20px;
            padding: 15px;
            border-radius: 12px;
            background: #f5f7f7;
        }

        .detalle-info-extra p {
            margin: 0;
            line-height: 1.6;
        }

        .detalle-no-encontrada {
            max-width: 650px;
            margin: 50px auto;
            text-align: center;
            padding: 35px;
        }

        @media (max-width: 600px) {
            .detalle-galeria {
                grid-template-columns: repeat(2, minmax(0, 1fr));
            }
        }
    </style>

    <asp:Panel ID="pnlNoEncontrada"
        runat="server"
        Visible="false">

        <div class="panel detalle-no-encontrada">
            <i class="ti ti-alert-circle"
               style="font-size:42px;"></i>

            <h2>Publicación no encontrada</h2>

            <p class="suave">
                <asp:Label ID="lblNoEncontrada"
                    runat="server" />
            </p>

            <a class="volver" href="Default.aspx">
                <i class="ti ti-arrow-left"></i>
                Volver al catálogo
            </a>
        </div>

    </asp:Panel>

    <asp:Panel ID="pnlDetalle"
        runat="server"
        Visible="false">

        <div class="detalle">

            <article class="panel" aria-labelledby="t-detalle">

                <a class="volver" href="Default.aspx">
                    <i class="ti ti-arrow-left"
                       aria-hidden="true"></i>
                    Volver al catálogo
                </a>

                <div class="detalle-cuerpo">

                    <div>
                        <div class="detalle-galeria">

                            <asp:Repeater ID="rptFotos"
                                runat="server">

                                <ItemTemplate>
                                    <asp:Image
                                        runat="server"
                                        CssClass="galeria-foto"
                                        ImageUrl='<%# ObtenerRutaFoto(Eval("Ruta")) %>'
                                        AlternateText="Fotografía del artículo" />
                                </ItemTemplate>

                            </asp:Repeater>

                        </div>

                        <asp:Panel ID="pnlSinFotos"
                            runat="server"
                            Visible="false">

                            <div class="detalle-galeria-vacia">
                                <i class="ti ti-photo-off"></i>
                                <p>No hay fotografías disponibles.</p>
                            </div>

                        </asp:Panel>
                    </div>

                    <div>

                        <p class="detalle-estado">
                            <span class="estado estado--<%= EstadoCss %>">
                                <%= EstadoTexto %>
                            </span>

                            <span class="pildora">
                                Publicado el <%= FechaPublicacionTexto %>
                            </span>
                        </p>

                        <h1 id="t-detalle">
                            <%= TituloSeguro %>
                        </h1>

                        <ul class="etiquetas"
                            aria-label="Clasificación">

                            <li><%= CategoriaTexto %></li>
                            <li><%= CarreraTexto %></li>
                            <li><%= ModalidadTexto %></li>

                        </ul>

                        <p class="descripcion">
                            <%= DescripcionSegura %>
                        </p>

                        
                        <asp:Panel ID="pnlDisponible"
                            runat="server"
                            Visible="false">

                            <div class="solicitud">

                                <asp:Panel ID="pnlPropietario"
                                    runat="server"
                                    Visible="false">

                                    <p class="aviso">
                                        <i class="ti ti-user"></i>
                                        Esta publicación es tuya.
                                    </p>

                                </asp:Panel>

                                <asp:Panel ID="pnlYaSolicitado"
                                    runat="server"
                                    Visible="false">

                                    <p class="aviso">
                                        <i class="ti ti-clock"></i>
                                        Ya enviaste una solicitud para este artículo.
                                        Está pendiente de respuesta.
                                    </p>

                                </asp:Panel>

                                <asp:Panel ID="pnlFormularioSolicitud"
                                    runat="server"
                                    Visible="false">

                                    <asp:Panel ID="pnlObjetoOfrecido"
                                        runat="server"
                                        Visible="false">

                                        <label class="rotulo">
                                            ¿Qué ofrecés a cambio?
                                        </label>

                                        <asp:TextBox ID="txtObjetoOfrecido"
                                            runat="server"
                                            TextMode="MultiLine"
                                            Rows="3"
                                            MaxLength="200"
                                            CssClass="campo"
                                            placeholder="Describí el objeto que ofrecés" />

                                    </asp:Panel>

                                    <asp:Button ID="btnSolicitar"
                                        runat="server"
                                        Text="Solicitar este objeto"
                                        CssClass="boton"
                                        OnClick="btnSolicitar_Click" />

                                    <p class="suave" style="font-size:13px;">
                                        El propietario revisará tu solicitud.
                                        Si la acepta, podrán coordinar la entrega.
                                    </p>

                                </asp:Panel>

                                <asp:Panel ID="pnlMensajeSolicitud"
                                    runat="server"
                                    Visible="false">

                                    <asp:Label ID="lblMensajeSolicitud"
                                        runat="server" />

                                </asp:Panel>

                            </div>

                        </asp:Panel>


                        <asp:Panel ID="pnlReservado"
                            runat="server"
                            Visible="false">

                            <p class="aviso" role="status">
                                <i class="ti ti-lock"></i>
                                Este artículo está reservado y
                                no recibe nuevas solicitudes.
                            </p>

                        </asp:Panel>

                        <asp:Panel ID="pnlEntregado"
                            runat="server"
                            Visible="false">

                            <p class="aviso" role="status">
                                <i class="ti ti-circle-check"></i>
                                Este artículo ya fue entregado.
                            </p>

                        </asp:Panel>

                        <div class="detalle-info-extra">
                            <p>
                                <strong>Modalidad:</strong>
                                <%= ModalidadTexto %>
                            </p>
                            <p>
                                <strong>Categoría:</strong>
                                <%= CategoriaTexto %>
                            </p>
                            <p>
                                <strong>Carrera:</strong>
                                <%= CarreraTexto %>
                            </p>
                        </div>

                    </div>

                </div>

                <section class="donante"
                    aria-label="Publicado por">

                    <span class="avatar avatar--md"
                        aria-hidden="true">
                        <%= InicialesDueno %>
                    </span>

                    <div class="donante-datos">
                        <p class="fila-titulo">
                            <a href="Perfil.aspx?id=<%= PublicacionActual.Dueno.IdUsuario %>"><%= NombreDueno %></a>
                        </p>

                        <p class="suave"
                           style="font-size:13px;">
                            Miembro desde <%= FechaRegistroTexto %>
                        </p>
                    </div>

                </section>

            </article>

            <aside class="detalle-lateral"
                aria-label="Información adicional">

                <section class="panel">
                    <h2 style="margin-bottom:14px;">
                        Sobre esta publicación
                    </h2>

                    <p class="suave">
                        Los artículos publicados en PÁSALO
                        pertenecen a estudiantes de la comunidad
                        universitaria.
                    </p>

                    <p class="suave">
                        Coordiná siempre las entregas
                        dentro de la universidad.
                    </p>
                </section>

                <section class="consejo"
                    aria-labelledby="t-consejo">

                    <h2 id="t-consejo">
                        <i class="ti ti-shield-check"></i>
                        Consejo de seguridad
                    </h2>

                    <p>
                        Para un intercambio seguro,
                        encontrate siempre dentro del campus,
                        durante el día y en lugares comunes.
                    </p>

                </section>

            </aside>

        </div>

    </asp:Panel>

</asp:Content>
