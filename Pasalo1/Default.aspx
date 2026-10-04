 <%@ Page Title="Catálogo" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Pasalo1._Default" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <div class="catalogo">

        <aside class="lateral" aria-label="Filtros">
            <asp:Label ID="lblCarrera" runat="server" AssociatedControlID="ddlCarrera" CssClass="rotulo" Text="Filtrar por carrera" />
            <asp:DropDownList ID="ddlCarrera" runat="server" CssClass="campo" style="margin-bottom:24px">
                <asp:ListItem Text="Todas las carreras" Value="0" />
                <asp:ListItem Text="Ingeniería en Sistemas" Value="1" />
                <asp:ListItem Text="Ingeniería Industrial" Value="2" />
                <asp:ListItem Text="Contador Público" Value="3" />
            </asp:DropDownList>

            <nav aria-label="Categorías">
                <span class="rotulo">Categorías</span>
                <ul class="categorias">
                    <li><a href="#" aria-current="true"><i class="ti ti-layout-grid" aria-hidden="true"></i>Todas<span class="cuenta">337</span></a></li>
                    <li><a href="#"><i class="ti ti-shirt" aria-hidden="true"></i>Ropa<span class="cuenta">124</span></a></li>
                    <li><a href="#"><i class="ti ti-book" aria-hidden="true"></i>Libros<span class="cuenta">86</span></a></li>
                    <li><a href="#"><i class="ti ti-pencil" aria-hidden="true"></i>Apuntes<span class="cuenta">54</span></a></li>
                    <li><a href="#"><i class="ti ti-device-laptop" aria-hidden="true"></i>Electrónicos<span class="cuenta">32</span></a></li>
                    <li><a href="#"><i class="ti ti-box" aria-hidden="true"></i>Otros<span class="cuenta">41</span></a></li>
                </ul>
            </nav>

            <%-- Datos de ejemplo: más adelante salen de las operaciones finalizadas --%>
            <section class="huella" aria-labelledby="t-huella">
                <h2 id="t-huella"><i class="ti ti-recycle" aria-hidden="true"></i>Tu huella circular</h2>
                <p>Este mes ayudaste a evitar 12 kg de residuos. ¡Seguí así!</p>
            </section>
        </aside>

        <section aria-labelledby="t-catalogo">
            <div class="encabezado">
                <div>
                    <h1 id="t-catalogo">Objetos disponibles</h1>
                    <p class="suave">Mostrando 6 de 337 objetos para donar o intercambiar</p>
                </div>
                <div class="orden">
                    <asp:Label ID="lblOrden" runat="server" AssociatedControlID="ddlOrden" Text="Ordenar por" />
                    <asp:DropDownList ID="ddlOrden" runat="server" CssClass="campo">
                        <asp:ListItem Text="Más recientes" />
                        <asp:ListItem Text="Más antiguos" />
                    </asp:DropDownList>
                </div>
            </div>

            <div class="grilla">
                <article class="tarjeta">
                    <div class="foto"><span class="chip">Libros</span><i class="ti ti-book" aria-hidden="true"></i></div>
                    <div class="tarjeta-cuerpo">
                        <h3>Cálculo de una variable - Stewart</h3>
                        <p class="suave">Todas las carreras</p>
                        <div class="tarjeta-pie">
                            <span class="estado estado--disponible">Disponible</span>
                            <a class="ir" href="#" aria-label="Ver detalle de Cálculo de una variable - Stewart"><i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        </div>
                    </div>
                </article>

                <article class="tarjeta">
                    <div class="foto"><span class="chip">Ropa</span><i class="ti ti-shirt" aria-hidden="true"></i></div>
                    <div class="tarjeta-cuerpo">
                        <h3>Bata de laboratorio blanca, talle M</h3>
                        <p class="suave">Ingeniería Química</p>
                        <div class="tarjeta-pie">
                            <span class="estado estado--disponible">Disponible</span>
                            <a class="ir" href="#" aria-label="Ver detalle de Bata de laboratorio blanca, talle M"><i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        </div>
                    </div>
                </article>

                <article class="tarjeta">
                    <div class="foto"><span class="chip">Electrónicos</span><i class="ti ti-calculator" aria-hidden="true"></i></div>
                    <div class="tarjeta-cuerpo">
                        <h3>Calculadora científica Casio FX-991</h3>
                        <p class="suave">Todas las carreras</p>
                        <div class="tarjeta-pie">
                            <span class="estado estado--disponible">Disponible</span>
                            <a class="ir" href="#" aria-label="Ver detalle de Calculadora científica Casio FX-991"><i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        </div>
                    </div>
                </article>

                <article class="tarjeta">
                    <div class="foto"><span class="chip">Apuntes</span><i class="ti ti-pencil" aria-hidden="true"></i></div>
                    <div class="tarjeta-cuerpo">
                        <h3>Apuntes de Análisis Matemático I</h3>
                        <p class="suave">Ingeniería en Sistemas</p>
                        <div class="tarjeta-pie">
                            <span class="estado estado--disponible">Disponible</span>
                            <a class="ir" href="#" aria-label="Ver detalle de Apuntes de Análisis Matemático I"><i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        </div>
                    </div>
                </article>

                <article class="tarjeta">
                    <div class="foto"><span class="chip">Otros</span><i class="ti ti-ruler-2" aria-hidden="true"></i></div>
                    <div class="tarjeta-cuerpo">
                        <h3>Regla T profesional para dibujo</h3>
                        <p class="suave">Ingeniería Mecánica</p>
                        <div class="tarjeta-pie">
                            <span class="estado estado--disponible">Disponible</span>
                            <a class="ir" href="#" aria-label="Ver detalle de Regla T profesional para dibujo"><i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        </div>
                    </div>
                </article>

                <article class="tarjeta">
                    <div class="foto"><span class="chip">Electrónicos</span><i class="ti ti-device-tablet" aria-hidden="true"></i></div>
                    <div class="tarjeta-cuerpo">
                        <h3>Tablet con pantalla dañada (para repuestos)</h3>
                        <p class="suave">Todas las carreras</p>
                        <div class="tarjeta-pie">
                            <span class="estado estado--disponible">Disponible</span>
                            <a class="ir" href="#" aria-label="Ver detalle de Tablet con pantalla dañada"><i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        </div>
                    </div>
                </article>
            </div>

            <nav class="paginacion" aria-label="Paginación">
                <button type="button" aria-label="Página anterior"><i class="ti ti-chevron-left" aria-hidden="true"></i></button>
                <span>Página 1 de 56</span>
                <button type="button" aria-label="Página siguiente"><i class="ti ti-chevron-right" aria-hidden="true"></i></button>
            </nav>
        </section>
    </div>

    <a class="publicar" href="Publicacion.aspx"><i class="ti ti-plus" aria-hidden="true"></i>Publicar artículo</a>
</asp:Content>
