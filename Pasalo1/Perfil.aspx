<%@ Page Title="Mi perfil" Language="C#" MasterPageFile="~/Principal.Master" AutoEventWireup="true" CodeBehind="Perfil.aspx.cs" Inherits="Pasalo1.Perfil" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent" runat="server">
    <div class="perfil">

        <%-- Datos de ejemplo: más adelante salen de la tabla Usuario y de las valoraciones --%>
        <section class="panel perfil-cabecera" aria-labelledby="t-perfil">
            <div class="perfil-id">
                <div class="avatar-grande" aria-hidden="true">SR</div>
                <div>
                    <h1 id="t-perfil">Sofía Ramírez</h1>
                    <p>Ingeniería Mecánica · Facultad de Ingeniería</p>
                    <p class="suave" style="font-size:13px">Miembro desde 2024</p>
                </div>
            </div>
            <div class="perfil-valoracion">
                <p>
                    <span class="estrellas" aria-hidden="true"><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i><i class="ti ti-star-filled"></i></span>
                    <span class="nota">4,8</span><span class="solo-lectores"> de 5 de reputación</span>
                </p>
                <p class="suave">23 intercambios exitosos</p>
            </div>
        </section>

        <nav class="pestanas" aria-label="Secciones del perfil">
            <a href="Perfil.aspx?tab=publicaciones"<%= Actual("publicaciones") %>>Mis publicaciones</a>
            <a href="Perfil.aspx?tab=historial"<%= Actual("historial") %>>Historial</a>
            <a href="Perfil.aspx?tab=impacto"<%= Actual("impacto") %>>Impacto ambiental</a>
        </nav>

        <% if (TabActiva == "publicaciones") { %>
        <ul class="lista">
            <li class="fila">
                <span class="fila-icono" aria-hidden="true"><i class="ti ti-pencil"></i></span>
                <div class="fila-texto">
                    <p class="fila-titulo">Apuntes de Análisis Matemático I</p>
                    <p class="suave">Intercambio · publicada hace 3 días · <a class="enlace" href="#">2 solicitudes</a></p>
                </div>
                <span class="estado estado--disponible">Disponible</span>
            </li>
            <li class="fila">
                <span class="fila-icono" aria-hidden="true"><i class="ti ti-calculator"></i></span>
                <div class="fila-texto">
                    <p class="fila-titulo">Calculadora científica Casio FX-991</p>
                    <p class="suave">Donación · reservada para Lucas M.</p>
                </div>
                <span class="estado estado--reservado">Reservado</span>
            </li>
            <li class="fila">
                <span class="fila-icono" aria-hidden="true"><i class="ti ti-shirt"></i></span>
                <div class="fila-texto">
                    <p class="fila-titulo">Campera de abrigo</p>
                    <p class="suave">Donación · entregada el 12/09/2026</p>
                </div>
                <span class="estado estado--entregado">Entregado</span>
            </li>
        </ul>
        <% } else if (TabActiva == "historial") { %>
        <ul class="lista">
            <li class="fila">
                <span class="fila-icono" aria-hidden="true"><i class="ti ti-gift"></i></span>
                <div class="fila-texto">
                    <p class="fila-titulo">Campera de abrigo</p>
                    <p class="suave">Donación con Lucas M. · 12/09/2026</p>
                </div>
                <p class="fila-der">Te valoró con 5 de 5</p>
            </li>
            <li class="fila">
                <span class="fila-icono" aria-hidden="true"><i class="ti ti-arrows-exchange"></i></span>
                <div class="fila-texto">
                    <p class="fila-titulo">Libro de Física II</p>
                    <p class="suave">Intercambio con Martina G. · 03/09/2026</p>
                </div>
                <p class="fila-der">Te valoró con 4 de 5</p>
            </li>
            <li class="fila">
                <span class="fila-icono" aria-hidden="true"><i class="ti ti-gift"></i></span>
                <div class="fila-texto">
                    <p class="fila-titulo">Apuntes de Química</p>
                    <p class="suave">Donación con Joaquín P. · 21/08/2026</p>
                </div>
                <p class="fila-der"><a class="enlace" href="#">Valorar</a></p>
            </li>
        </ul>
        <% } else { %>
        <div class="impacto">
            <div class="col-apilada">
                <div class="panel dato">
                    <span class="dato-icono" aria-hidden="true"><i class="ti ti-gift"></i></span>
                    <div>
                        <p class="dato-valor">12 objetos</p>
                        <p class="suave">Donados o intercambiados</p>
                    </div>
                </div>
                <div class="panel dato">
                    <span class="dato-icono dato-icono--acento" aria-hidden="true"><i class="ti ti-recycle"></i></span>
                    <div>
                        <p class="dato-valor dato-valor--acento">38 kg</p>
                        <p class="suave">De residuos evitados en el campus</p>
                    </div>
                </div>
                <section class="huella huella--ancha" aria-labelledby="t-huella">
                    <div class="progreso" style="--valor:75" role="img" aria-label="75 % de tu meta del mes"><span>75%</span></div>
                    <div>
                        <h2 id="t-huella"><i class="ti ti-recycle" aria-hidden="true"></i>Tu huella circular</h2>
                        <p>Estás en el 10 % de quienes más ayudan en tu facultad. ¡Seguí así!</p>
                    </div>
                </section>
            </div>

            <section class="panel" aria-labelledby="t-fac">
                <h2 id="t-fac">Impacto por facultad</h2>
                <p class="suave" style="font-size:14px">Residuos evitados (kg) por la comunidad</p>
                <ul class="barras">
                    <li>
                        <div class="barra-fila"><span>Ingeniería (tu facultad)</span><span>142 kg</span></div>
                        <div class="barra-pista" aria-hidden="true"><div class="barra-relleno barra-relleno--propia" style="width:100%"></div></div>
                    </li>
                    <li>
                        <div class="barra-fila"><span>Ciencias Económicas</span><span>98 kg</span></div>
                        <div class="barra-pista" aria-hidden="true"><div class="barra-relleno" style="width:69%"></div></div>
                    </li>
                    <li>
                        <div class="barra-fila"><span>Ciencias de la Salud</span><span>64 kg</span></div>
                        <div class="barra-pista" aria-hidden="true"><div class="barra-relleno" style="width:45%"></div></div>
                    </li>
                    <li>
                        <div class="barra-fila"><span>Arquitectura y Diseño</span><span>48 kg</span></div>
                        <div class="barra-pista" aria-hidden="true"><div class="barra-relleno" style="width:34%"></div></div>
                    </li>
                </ul>
            </section>
        </div>
        <% } %>

    </div>
</asp:Content>

