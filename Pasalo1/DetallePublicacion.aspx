<%@ Page Title="Detalle de publicación" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="DetallePublicacion.aspx.cs" Inherits="Pasalo1.DetallePublicacion" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <div class="detalle">

        <article class="panel" aria-labelledby="t-detalle">
            <a class="volver" href="Default.aspx"><i class="ti ti-arrow-left" aria-hidden="true"></i>Volver al catálogo</a>

            <div class="detalle-cuerpo">
                <div class="foto-detalle" role="img" aria-label="Foto del objeto"><%= FotoHtml %></div>

                <div>
                    <p class="detalle-estado">
                        <span class="estado estado--<%= Estado %>"><%= EstadoTexto %></span>
                        <span class="pildora"><%= PublicadoHace %></span>
                    </p>
                    <h1 id="t-detalle"><%: Pub.Titulo %></h1>

                    <ul class="etiquetas" aria-label="Clasificación">
                        <li><%: Pub.Categoria.Nombre %></li>
                        <li><%: CarreraTexto %></li>
                        <li><%= ModalidadTexto %></li>
                    </ul>

                    <p class="descripcion"><%: Pub.Descripcion %></p>

                    <% if (Estado == "disponible") { %>
                    <div class="solicitud">
                        <% if (EsIntercambio) { %>
                        <label for="ofrece" class="rotulo">¿Qué ofrecés a cambio? (obligatorio)</label>
                        <textarea id="ofrece" class="campo" rows="3" aria-required="true" placeholder="Describí el objeto que ofrecés"></textarea>
                        <% } %>
                        <button type="button" class="boton"><i class="ti ti-send" aria-hidden="true"></i><%= EsIntercambio ? "Enviar propuesta de intercambio" : "Solicitar este objeto" %></button>
                        <p class="suave" style="font-size:13px">El dueño revisa las solicitudes y elige con quién concretar. Si te acepta, se habilita el chat para coordinar la entrega.</p>
                    </div>
                    <% } else if (Estado == "reservado") { %>
                    <p class="aviso" role="status"><i class="ti ti-lock" aria-hidden="true"></i>Este objeto ya está reservado y no recibe nuevas solicitudes.</p>
                    <% } else { %>
                    <p class="aviso" role="status"><i class="ti ti-circle-check" aria-hidden="true"></i>Este objeto ya fue entregado.</p>
                    <% } %>

                    <a class="reportar" href="#"><i class="ti ti-flag" aria-hidden="true"></i>Reportar publicación inapropiada</a>
                </div>
            </div>

            <section class="donante" aria-label="Publicado por">
                <span class="avatar avatar--md" aria-hidden="true"><%: DuenoIniciales %></span>
                <div class="donante-datos">
                    <p class="fila-titulo"><%: Pub.Dueno.Nombre %></p>
                    <p class="suave" style="font-size:13px"><%: DuenoDetalle %></p>
                </div>
            </section>
        </article>

        <aside class="detalle-lateral" aria-label="Más objetos">
            <section class="panel" aria-labelledby="t-similares">
                <h2 id="t-similares" style="margin-bottom:14px">Objetos similares en <%: Pub.Categoria.Nombre %></h2>
                <p class="suave">Próximamente.</p>
            </section>

            <section class="consejo" aria-labelledby="t-consejo">
                <h2 id="t-consejo"><i class="ti ti-shield-check" aria-hidden="true"></i>Consejo de seguridad</h2>
                <p>Para un intercambio seguro, encontrate siempre dentro del campus y durante el día, en lugares comunes como la cafetería o la biblioteca.</p>
            </section>
        </aside>

    </div>
</asp:Content>
