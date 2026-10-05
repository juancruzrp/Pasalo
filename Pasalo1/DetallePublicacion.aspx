<%@ Page Title="Detalle de publicación" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="DetallePublicacion.aspx.cs" Inherits="Pasalo1.DetallePublicacion" %>
<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <div class="detalle">

        <%-- Datos de ejemplo: más adelante salen de la tabla Publicacion según el id de la dirección --%>
        <article class="panel" aria-labelledby="t-detalle">
            <a class="volver" href="Default.aspx"><i class="ti ti-arrow-left" aria-hidden="true"></i>Volver al catálogo</a>

            <div class="detalle-cuerpo">
                <div class="foto-detalle" role="img" aria-label="Foto del objeto (de ejemplo)"><i class="ti ti-book" aria-hidden="true"></i></div>

                <div>
                    <p class="detalle-estado">
                        <span class="estado estado--<%= Estado %>"><%= EstadoTexto %></span>
                        <span class="pildora">Publicado hace 2 días</span>
                    </p>
                    <h1 id="t-detalle">Cálculo de una variable: Trascendentes Tempranas (7ma edición)</h1>

                    <ul class="etiquetas" aria-label="Clasificación">
                        <li>Libros</li>
                        <li>Ingeniería Mecánica</li>
                        <li><%= ModalidadTexto %></li>
                    </ul>

                    <p class="descripcion">Tengo el libro de Cálculo de Stewart original, en perfecto estado. No tiene hojas rayadas ni subrayadas con resaltador. Ideal para cursar primer año de cualquier Ingeniería. Entrego en la biblioteca central o en el edificio de Ingeniería.</p>

                    <% if (Estado == "disponible") { %>
                    <div class="solicitud">
                        <% if (Modalidad == "intercambio") { %>
                        <label for="ofrece" class="rotulo">¿Qué ofrecés a cambio? (obligatorio)</label>
                        <textarea id="ofrece" class="campo" rows="3" aria-required="true" placeholder="Describí el objeto que ofrecés"></textarea>
                        <% } %>
                        <button type="button" class="boton"><i class="ti ti-send" aria-hidden="true"></i><%= Modalidad == "intercambio" ? "Enviar propuesta de intercambio" : "Solicitar este objeto" %></button>
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
                <span class="avatar avatar--md" aria-hidden="true">DM</span>
                <div class="donante-datos">
                    <p class="fila-titulo">Diego Méndez</p>
                    <p class="suave" style="font-size:13px">Estudiante de Ing. Química · Miembro desde 2024</p>
                </div>
                <p class="donante-rep">
                    <span class="estrellas" aria-hidden="true" style="font-size:14px"><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i></span><br />
                    <span class="suave">4,9 de 5 (18 intercambios)</span>
                </p>
            </section>
        </article>

        <aside class="detalle-lateral" aria-label="Más objetos">
            <%-- Solo se muestran objetos Disponibles, igual que el catálogo --%>
            <section class="panel" aria-labelledby="t-similares">
                <h2 id="t-similares" style="margin-bottom:14px">Objetos similares en Libros</h2>
                <ul class="lista">
                    <li class="fila">
                        <span class="fila-icono" aria-hidden="true"><i class="ti ti-book"></i></span>
                        <div class="fila-texto">
                            <p class="fila-titulo"><a href="DetallePublicacion.aspx">Cálculo de una variable - Thomas 12ed</a></p>
                            <p class="suave" style="font-size:13px">Ingeniería Mecánica</p>
                            <span class="estado estado--disponible">Disponible</span>
                        </div>
                    </li>
                    <li class="fila">
                        <span class="fila-icono" aria-hidden="true"><i class="ti ti-book"></i></span>
                        <div class="fila-texto">
                            <p class="fila-titulo"><a href="DetallePublicacion.aspx">Física Universitaria Vol. 1 - Sears</a></p>
                            <p class="suave" style="font-size:13px">Ciencias Básicas</p>
                            <span class="estado estado--disponible">Disponible</span>
                        </div>
                    </li>
                    <li class="fila">
                        <span class="fila-icono" aria-hidden="true"><i class="ti ti-book"></i></span>
                        <div class="fila-texto">
                            <p class="fila-titulo"><a href="DetallePublicacion.aspx">Álgebra lineal - Grossman</a></p>
                            <p class="suave" style="font-size:13px">Ingeniería en Sistemas</p>
                            <span class="estado estado--disponible">Disponible</span>
                        </div>
                    </li>
                </ul>
            </section>

            <section class="consejo" aria-labelledby="t-consejo">
                <h2 id="t-consejo"><i class="ti ti-shield-check" aria-hidden="true"></i>Consejo de seguridad</h2>
                <p>Para un intercambio seguro, encontrate siempre dentro del campus y durante el día, en lugares comunes como la cafetería o la biblioteca.</p>
            </section>
        </aside>

    </div>
</asp:Content>
