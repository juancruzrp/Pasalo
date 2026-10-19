<%@ Page Title="Publicar artículo"
    Language="C#"
    MasterPageFile="~/Principal.Master"
    AutoEventWireup="true"
    CodeBehind="Publicacion.aspx.cs"
    Inherits="Pasalo1.Publicacion" %>


<asp:Content ID="ContenidoHead" ContentPlaceHolderID="HeadContent" runat="server">

    <style>

        /* =========================================
           CONTENEDOR GENERAL
        ========================================= */

        .publicacion-contenedor {
            max-width: 1180px;
            margin: 32px auto 60px;
            padding: 0 24px;
        }


        /* =========================================
           VOLVER AL CATALOGO
        ========================================= */

        .volver {
            display: inline-flex;
            align-items: center;
            gap: 7px;

            margin-bottom: 18px;

            color: #536b77;
            text-decoration: none;

            font-size: 14px;
            font-weight: 500;
        }

        .volver:hover {
            color: #28666d;
        }


        /* =========================================
           ENCABEZADO
        ========================================= */

        .publicacion-encabezado {
            margin-bottom: 25px;
        }

        .publicacion-encabezado h1 {
            margin: 0 0 6px;

            color: #28666d;

            font-size: 30px;
            font-weight: 700;
        }

        .publicacion-encabezado p {
            margin: 0;

            color: #6d7e87;

            font-size: 14px;
        }


        /* =========================================
           DISTRIBUCION DE LA PAGINA
        ========================================= */

        .publicacion-layout {
            display: grid;

            grid-template-columns: minmax(0, 1fr) 310px;

            gap: 24px;

            align-items: start;
        }


        /* =========================================
           FORMULARIO
        ========================================= */

        .formulario-publicacion {
            padding: 28px;

            background: #ffffff;

            border: 1px solid #deddd8;
            border-radius: 16px;
        }


        /* =========================================
           SECCIONES
        ========================================= */

        .seccion-publicacion {
            padding-bottom: 26px;
            margin-bottom: 26px;

            border-bottom: 1px solid #e6e4df;
        }

        .seccion-publicacion:last-of-type {
            margin-bottom: 0;
        }

        .titulo-seccion {
            display: flex;
            align-items: center;

            gap: 10px;

            margin-bottom: 16px;

            color: #285766;

            font-size: 17px;
            font-weight: 700;
        }

        .numero-seccion {
            width: 29px;
            height: 29px;

            display: inline-flex;
            align-items: center;
            justify-content: center;

            flex-shrink: 0;

            border-radius: 50%;

            background: #28666d;
            color: #ffffff;

            font-size: 13px;
        }

        .ayuda-seccion {
            margin: -7px 0 16px 39px;

            color: #7b8991;

            font-size: 12px;
        }


        /* =========================================
           FOTOS
        ========================================= */

        .fotos-grid {
            display: grid;

            grid-template-columns: repeat(4, 1fr);

            gap: 12px;
        }

        .archivo-oculto {
            position: absolute;
            width: 1px;
            height: 1px;
            overflow: hidden;
            clip: rect(0 0 0 0);
        }

        .archivo-oculto:focus-visible + .foto-upload,
        .archivo-oculto:focus-visible + .foto-upload {
            outline: 3px solid #b84e36;
            outline-offset: 2px;
        }

        .foto-upload {
            height: 125px;

            display: flex;
            flex-direction: column;
            align-items: center;
            justify-content: center;

            border: 2px dashed #9bb5b7;
            border-radius: 12px;

            background: #f2f7f7;

            color: #397078;

            cursor: pointer;

            transition: 0.2s;
        }

        .foto-upload:hover {
            border-color: #28666d;

            background: #e9f3f3;
        }

        .foto-upload.principal {
            border-color: #659499;
        }

        .foto-upload i {
            margin-bottom: 7px;

            font-size: 28px;
        }

        .foto-upload span {
            font-size: 12px;
            font-weight: 700;
        }

        .foto-upload small {
            margin-top: 3px;

            color: #788b93;

            font-size: 10px;
        }

        .formatos {
            margin-top: 9px;

            color: #89949a;

            font-size: 11px;
        }


        /* =========================================
           CAMPOS
        ========================================= */

        .grupo-campo {
            margin-bottom: 19px;
        }

        .grupo-campo:last-child {
            margin-bottom: 0;
        }

        .rotulo-publicacion {
            display: block;

            margin-bottom: 7px;

            color: #365562;

            font-size: 13px;
            font-weight: 700;
        }

        .requerido {
            color: #b84e36;
        }

        .campo-publicacion {
            width: 100%;

            min-height: 44px;

            padding: 10px 12px;

            border: 1px solid #d7dad6;
            border-radius: 9px;

            background: #ffffff;

            color: #29424d;

            font-family: inherit;
            font-size: 13px;

            outline: none;
        }

        .campo-publicacion:focus {
            border-color: #4c858a;

            box-shadow: 0 0 0 3px rgba(76, 133, 138, 0.10);
        }

        textarea.campo-publicacion {
            min-height: 125px;

            resize: vertical;
        }

        .contador {
            margin-top: 5px;

            text-align: right;

            color: #8b959a;

            font-size: 11px;
        }

        .dos-columnas {
            display: grid;

            grid-template-columns: 1fr 1fr;

            gap: 17px;
        }


        /* =========================================
           DONACION / INTERCAMBIO
        ========================================= */

        .modalidades {
            display: grid;

            grid-template-columns: 1fr 1fr;

            gap: 12px;
        }

        .modalidad {
            min-height: 95px;

            display: flex;
            align-items: center;

            gap: 13px;

            padding: 16px;

            border: 1px solid #d7dad6;
            border-radius: 12px;

            cursor: pointer;

            transition: 0.2s;
        }

        .modalidad:hover {
            border-color: #6f9fa3;

            background: #f4f9f9;
        }

        .modalidad-radio input {
            width: 17px;
            height: 17px;

            accent-color: #28666d;
        }

        .modalidad-icono {
            width: 40px;
            height: 40px;

            display: flex;
            align-items: center;
            justify-content: center;

            flex-shrink: 0;

            border-radius: 50%;

            background: #e8f2f2;

            color: #28666d;

            font-size: 21px;
        }

        .modalidad-info strong {
            display: block;

            margin-bottom: 3px;

            color: #28515d;

            font-size: 14px;
        }

        .modalidad-info span {
            display: block;

            color: #73848c;

            font-size: 11px;
            line-height: 1.4;
        }


        /* =========================================
           BOTONES
        ========================================= */

        .acciones-publicacion {
            display: flex;
            align-items: center;
            justify-content: space-between;

            gap: 15px;

            margin-top: 25px;
        }

        .btn-cancelar {
            padding: 11px 25px;

            border: 1px solid #b9c2c3;
            border-radius: 9px;

            background: #ffffff;

            color: #526b76;

            font-family: inherit;
            font-size: 13px;
            font-weight: 500;

            cursor: pointer;
        }

        .btn-cancelar:hover {
            background: #f4f5f3;
        }

        .btn-publicar {
            padding: 12px 24px;

            border: none;
            border-radius: 9px;

            background: #28666d;

            color: #ffffff;

            font-family: inherit;
            font-size: 13px;
            font-weight: 700;

            cursor: pointer;
        }

        .btn-publicar:hover {
            background: #21575d;
        }


        /* =========================================
           PANEL DERECHO
        ========================================= */

        .lateral-publicacion {
            display: flex;
            flex-direction: column;

            gap: 18px;
        }

        .consejos {
            overflow: hidden;

            border: 1px solid #deddd8;
            border-radius: 15px;

            background: #ffffff;
        }

        .consejos-encabezado {
            padding: 15px 17px;

            background: #eaf3f3;

            color: #285766;

            font-size: 13px;
            font-weight: 700;
        }

        .consejos-encabezado i {
            margin-right: 6px;
        }

        .consejos-cuerpo {
            padding: 18px;
        }

        .consejo {
            display: flex;

            gap: 11px;

            margin-bottom: 16px;

            color: #5c707a;

            font-size: 11px;
            line-height: 1.5;
        }

        .consejo:last-child {
            margin-bottom: 0;
        }

        .consejo i {
            min-width: 19px;

            color: #397078;

            font-size: 17px;
        }


        /* =========================================
           IMPACTO AMBIENTAL
        ========================================= */

        .impacto-publicacion {
            padding: 18px;

            border: 1px solid #d7e5d4;
            border-radius: 15px;

            background: #eff6ed;
        }

        .impacto-publicacion h2 {
            margin: 0 0 8px;

            color: #47714b;

            font-size: 13px;
        }

        .impacto-publicacion h2 i {
            margin-right: 5px;
        }

        .impacto-publicacion p {
            margin: 0;

            color: #627365;

            font-size: 11px;
            line-height: 1.55;
        }


        /* =========================================
           RESPONSIVE
        ========================================= */

        @media (max-width: 900px) {

            .publicacion-layout {
                grid-template-columns: 1fr;
            }

            .lateral-publicacion {
                display: grid;

                grid-template-columns: 1fr 1fr;
            }

        }


        @media (max-width: 650px) {

            .publicacion-contenedor {
                padding: 0 14px;
            }

            .formulario-publicacion {
                padding: 20px;
            }

            .fotos-grid {
                grid-template-columns: 1fr 1fr;
            }

            .dos-columnas {
                grid-template-columns: 1fr;
            }

            .modalidades {
                grid-template-columns: 1fr;
            }

            .lateral-publicacion {
                grid-template-columns: 1fr;
            }

            .acciones-publicacion {
                flex-direction: column-reverse;

                align-items: stretch;
            }

            .btn-cancelar,
            .btn-publicar {
                width: 100%;
            }

        }

        
            /* ===== Fotos: vista previa ===== */
            .foto-slot { position: relative; }
            .foto-upload { position: relative; overflow: hidden; }
 
            .foto-upload.con-foto {
                background-size: cover;
                background-position: center;
                border-style: solid;
                border-color: #28666d;
            }
            .foto-upload.con-foto i,
            .foto-upload.con-foto small { display: none; }
 
            /* Nombre del archivo: barra oscura abajo, para que se lea sobre cualquier foto */
            .foto-upload.con-foto span {
                position: absolute;
                left: 0; right: 0; bottom: 0;
                padding: 18px 8px 6px;
                background: linear-gradient(transparent, rgba(0, 0, 0, .7));
                color: #fff;
                font-size: 11px;
                font-weight: 500;
                text-align: left;
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
            }
 
            /* Etiqueta "Principal" sobre la primera foto elegida */
            .foto-upload.es-principal::after {
                content: "Principal";
                position: absolute;
                top: 8px; left: 8px;
                padding: 2px 9px;
                border-radius: 999px;
                background: #28666d;
                color: #fff;
                font-size: 10px;
                font-weight: 700;
            }
 
            /* Botón para quitar la foto */
            .foto-quitar {
                position: absolute;
                top: 6px; right: 6px;
                z-index: 2;
                display: grid;
                place-items: center;
                width: 26px; height: 26px;
                padding: 0;
                border: 0;
                border-radius: 50%;
                background: rgba(0, 0, 0, .65);
                color: #fff;
                font-size: 17px;
                line-height: 1;
                cursor: pointer;
            }
            .foto-quitar:hover { background: #b84e36; }
            .foto-quitar[hidden] { display: none; }
 
            /* Avisos de error (fotos y formulario) */
            .publicacion-error {
                display: flex; gap: 10px; align-items: flex-start;
                padding: 12px 14px; margin-top: 12px; margin-bottom: 22px;
                border-radius: 11px;
                background: #fbeae5; border: 1px solid #f0c9bd; color: #8a2f17;
                font-size: 13px;
            }

    </style>

</asp:Content>



<asp:Content ID="ContenidoPrincipal" ContentPlaceHolderID="MainContent" runat="server">


    <div class="publicacion-contenedor">


        <!-- =========================================
             VOLVER AL CATALOGO
        ========================================== -->

        <a href="Default.aspx" class="volver">

            <i class="ti ti-arrow-left" aria-hidden="true"></i>

            Volver al catálogo

        </a>



        <!-- =========================================
             ENCABEZADO
        ========================================== -->

        <header class="publicacion-encabezado">

            <h1>
                Publicar un artículo
            </h1>

            <p>
                Completá la información de tu artículo para que otros estudiantes puedan encontrarlo.
            </p>

        </header>



        <div class="publicacion-layout">


            <!-- =====================================
                 FORMULARIO
            ====================================== -->

            <section class="formulario-publicacion"
                     aria-label="Formulario para publicar un artículo">
                <asp:Panel ID="pnlError" runat="server" Visible="false" CssClass="publicacion-error" role="alert">
                   <i class="ti ti-alert-circle" aria-hidden="true"></i>
                   <asp:Label ID="lblError" runat="server" />
               </asp:Panel>



                <!-- =================================
                     1 - FOTOS
                ================================== -->

                <div class="seccion-publicacion">


                    <div class="titulo-seccion">

                        <span class="numero-seccion">
                            1
                        </span>

                        <span>
                            Fotos del artículo
                        </span>

                    </div>


                    <p class="ayuda-seccion">
                        Podés subir hasta 4 imágenes. La primera será la imagen principal.
                    </p>



                    <div class="fotos-grid">


                        <!-- FOTO 1 -->

                        <asp:FileUpload
                            ID="fuFoto1"
                            runat="server"
                            CssClass="archivo-oculto"
                            accept=".jpg,.jpeg,.png" />


                        <label
                            for="<%= fuFoto1.ClientID %>"
                            class="foto-upload principal">

                            <i class="ti ti-photo-plus"
                               aria-hidden="true"></i>

                            <span>
                                Agregar foto
                            </span>

                            <small>
                                Imagen principal
                            </small>

                        </label>



                        <!-- FOTO 2 -->

                        <asp:FileUpload
                            ID="fuFoto2"
                            runat="server"
                            CssClass="archivo-oculto"
                            accept=".jpg,.jpeg,.png" />


                        <label
                            for="<%= fuFoto2.ClientID %>"
                            class="foto-upload">

                            <i class="ti ti-photo-plus"
                               aria-hidden="true"></i>

                            <span>
                                Agregar foto
                            </span>

                        </label>



                        <!-- FOTO 3 -->

                        <asp:FileUpload
                            ID="fuFoto3"
                            runat="server"
                            CssClass="archivo-oculto"
                            accept=".jpg,.jpeg,.png" />


                        <label
                            for="<%= fuFoto3.ClientID %>"
                            class="foto-upload">

                            <i class="ti ti-photo-plus"
                               aria-hidden="true"></i>

                            <span>
                                Agregar foto
                            </span>

                        </label>



                        <!-- FOTO 4 -->

                        <asp:FileUpload
                            ID="fuFoto4"
                            runat="server"
                            CssClass="archivo-oculto"
                            accept=".jpg,.jpeg,.png" />


                        <label
                            for="<%= fuFoto4.ClientID %>"
                            class="foto-upload">

                            <i class="ti ti-photo-plus"
                               aria-hidden="true"></i>

                            <span>
                                Agregar foto
                            </span>

                        </label>


                    </div>


                    <div class="formatos">
                        Formatos permitidos: JPG y PNG. Tamaño máximo: 5 MB por imagen.
                    </div>


                </div>



                <!-- =================================
                     2 - INFORMACION DEL ARTICULO
                ================================== -->

                <div class="seccion-publicacion">


                    <div class="titulo-seccion">

                        <span class="numero-seccion">
                            2
                        </span>

                        <span>
                            Información del artículo
                        </span>

                    </div>



                    <!-- TITULO -->

                    <div class="grupo-campo">


                        <asp:Label
                            ID="lblTitulo"
                            runat="server"
                            AssociatedControlID="txtTitulo"
                            CssClass="rotulo-publicacion">

                            Título
                            <span class="requerido">*</span>

                        </asp:Label>


                        <asp:TextBox
                            ID="txtTitulo"
                            runat="server"
                            CssClass="campo-publicacion"
                            MaxLength="100"
                            placeholder="Ej: Libro de Programación C#">
                        </asp:TextBox>


                    </div>



                    <!-- DESCRIPCION -->

                    <div class="grupo-campo">


                        <asp:Label
                            ID="lblDescripcion"
                            runat="server"
                            AssociatedControlID="txtDescripcion"
                            CssClass="rotulo-publicacion">

                            Descripción
                            <span class="requerido">*</span>

                        </asp:Label>


                        <asp:TextBox
                            ID="txtDescripcion"
                            runat="server"
                            CssClass="campo-publicacion"
                            TextMode="MultiLine"
                            MaxLength="1000"
                            placeholder="Contá cómo está el artículo, qué incluye, si tiene algún detalle o daño, etc.">
                        </asp:TextBox>


                        <div class="contador">
                            Máximo 1000 caracteres
                        </div>


                    </div>


                </div>



                <!-- =================================
                     3 - CATEGORIA Y CARRERA
                ================================== -->

                <div class="seccion-publicacion">


                    <div class="titulo-seccion">

                        <span class="numero-seccion">
                            3
                        </span>

                        <span>
                            Categoría y carrera
                        </span>

                    </div>



                    <div class="dos-columnas">


                        <!-- CATEGORIA -->

                        <div class="grupo-campo">


                            <asp:Label
                                ID="lblCategoria"
                                runat="server"
                                AssociatedControlID="ddlCategoria"
                                CssClass="campo-publicacion">

                                Categoría
                                <span class="requerido">*</span>

                            </asp:Label>


                            <asp:DropDownList
                                ID="ddlCategoria"
                                runat="server"
                                CssClass="campo-publicacion">

                                <asp:ListItem
                                    Text="Seleccionar categoría"
                                    Value="0" />

                                <asp:ListItem
                                    Text="Ropa"
                                    Value="1" />

                                <asp:ListItem
                                    Text="Libros"
                                    Value="2" />

                                <asp:ListItem
                                    Text="Apuntes"
                                    Value="3" />

                                <asp:ListItem
                                    Text="Electrónicos"
                                    Value="4" />

                                <asp:ListItem
                                    Text="Otros"
                                    Value="5" />

                            </asp:DropDownList>


                        </div>



                        <!-- CARRERA -->

                        <div class="grupo-campo">


                            <asp:Label
                                ID="lblCarrera"
                                runat="server"
                                AssociatedControlID="ddlCarrera"
                                CssClass="campo-publicacion">

                                Carrera

                                <span style="font-weight:400;">
                                    (opcional)
                                </span>

                            </asp:Label>


                            <asp:DropDownList
                                ID="ddlCarrera"
                                runat="server"
                                CssClass="campo-publicacion">

                                <asp:ListItem
                                    Text="Todas las carreras"
                                    Value="0" />

                                <asp:ListItem
                                    Text="Ingeniería en Sistemas"
                                    Value="1" />

                                <asp:ListItem
                                    Text="Ingeniería Industrial"
                                    Value="2" />

                                <asp:ListItem
                                    Text="Ingeniería Química"
                                    Value="3" />

                                <asp:ListItem
                                    Text="Ingeniería Mecánica"
                                    Value="4" />

                                <asp:ListItem
                                    Text="Contador Público"
                                    Value="5" />

                            </asp:DropDownList>


                        </div>


                    </div>


                </div>



                <!-- =================================
                     4 - DONACION O INTERCAMBIO
                ================================== -->

                <div class="seccion-publicacion">


                    <div class="titulo-seccion">

                        <span class="numero-seccion">
                            4
                        </span>

                        <span>
                            ¿Cómo querés ofrecerlo?
                        </span>

                    </div>


                    <p class="ayuda-seccion">
                        Elegí si querés donar el artículo o intercambiarlo por otro.
                    </p>



                    <div class="modalidades">


                        <!-- DONACION -->

                        <label class="modalidad">


                            <span class="modalidad-radio">

                                <asp:RadioButton
                                    ID="rbDonacion"
                                    runat="server"
                                    GroupName="Modalidad" />

                            </span>


                            <span class="modalidad-icono">

                                <i class="ti ti-gift"
                                   aria-hidden="true"></i>

                            </span>


                            <span class="modalidad-info">

                                <strong>
                                    Donación
                                </strong>

                                <span>
                                    Quiero regalar este artículo a otro estudiante.
                                </span>

                            </span>


                        </label>



                        <!-- INTERCAMBIO -->

                        <label class="modalidad">


                            <span class="modalidad-radio">

                                <asp:RadioButton
                                    ID="rbIntercambio"
                                    runat="server"
                                    GroupName="Modalidad" />

                            </span>


                            <span class="modalidad-icono">

                                <i class="ti ti-arrows-exchange"
                                   aria-hidden="true"></i>

                            </span>


                            <span class="modalidad-info">

                                <strong>
                                    Intercambio
                                </strong>

                                <span>
                                    Quiero recibir otro artículo a cambio.
                                </span>

                            </span>


                        </label>


                    </div>


                </div>



                <!-- =================================
                     BOTONES
                ================================== -->

                <div class="acciones-publicacion">


                    <asp:Button
                        ID="btnCancelar"
                        runat="server"
                        Text="Cancelar"
                        CssClass="btn-cancelar"
                        CausesValidation="false"
                        OnClick="btnCancelar_Click" />


                    <asp:Button
                        ID="btnPublicar"
                        runat="server"
                        Text="Publicar artículo"
                        CssClass="btn-publicar"
                        OnClick="btnPublicar_Click"  />


                </div>


            </section>



            <!-- =====================================
                 PANEL DERECHO
            ====================================== -->

            <aside class="lateral-publicacion">



                <!-- CONSEJOS -->

                <section class="consejos">


                    <div class="consejos-encabezado">

                        <i class="ti ti-info-circle"
                           aria-hidden="true"></i>

                        Consejos para una buena publicación

                    </div>



                    <div class="consejos-cuerpo">


                        <div class="consejo">

                            <i class="ti ti-camera"
                               aria-hidden="true"></i>

                            <span>
                                Usá fotos claras y de buena calidad.
                            </span>

                        </div>



                        <div class="consejo">

                            <i class="ti ti-file-text"
                               aria-hidden="true"></i>

                            <span>
                                Escribí un título descriptivo.
                            </span>

                        </div>



                        <div class="consejo">

                            <i class="ti ti-message"
                               aria-hidden="true"></i>

                            <span>
                                Detallá el estado del artículo y cualquier información importante.
                            </span>

                        </div>



                        <div class="consejo">

                            <i class="ti ti-tag"
                               aria-hidden="true"></i>

                            <span>
                                Elegí la categoría correcta.
                            </span>

                        </div>



                        <div class="consejo">

                            <i class="ti ti-school"
                               aria-hidden="true"></i>

                            <span>
                                Si es material de estudio, seleccioná la carrera.
                            </span>

                        </div>



                        <div class="consejo">

                            <i class="ti ti-heart"
                               aria-hidden="true"></i>

                            <span>
                                Indicá claramente si es una donación o un intercambio.
                            </span>

                        </div>


                    </div>


                </section>



                <!-- IMPACTO AMBIENTAL -->

                <section class="impacto-publicacion">


                    <h2>

                        <i class="ti ti-leaf"
                           aria-hidden="true"></i>

                        Juntos cuidamos el ambiente

                    </h2>


                    <p>
                        Cada objeto que vuelve a ser utilizado ayuda a reducir
                        residuos y genera un impacto positivo en nuestra
                        comunidad universitaria.
                    </p>


                </section>


            </aside>


        </div>


    </div>
    
<script>
            (function () {
                var MAX_BYTES = 5 * 1024 * 1024;   // 5 MB por foto
                var inputs = Array.prototype.slice.call(document.querySelectorAll('.archivo-oculto'));
                var grilla = document.querySelector('.fotos-grid');

                // Aviso para problemas con las fotos (los lectores de pantalla lo leen solos)
                var aviso = document.createElement('div');
                aviso.className = 'publicacion-error';
                aviso.setAttribute('role', 'alert');
                aviso.style.display = 'none';
                grilla.parentNode.appendChild(aviso);

                function mostrarAviso(texto) {
                    aviso.textContent = texto;
                    aviso.style.display = texto ? 'flex' : 'none';
                }

                // Cada foto queda en su "slot": campo + recuadro + botón para quitarla
                var slots = inputs.map(function (input, i) {
                    var label = document.querySelector('label[for="' + input.id + '"]');
                    var texto = label.querySelector('span');

                    var slot = document.createElement('div');
                    slot.className = 'foto-slot';
                    input.parentNode.insertBefore(slot, input);
                    slot.appendChild(input);
                    slot.appendChild(label);

                    var quitar = document.createElement('button');
                    quitar.type = 'button';
                    quitar.className = 'foto-quitar';
                    quitar.hidden = true;
                    quitar.setAttribute('aria-label', 'Quitar foto ' + (i + 1));
                    quitar.innerHTML = '&times;';
                    slot.appendChild(quitar);

                    return { input: input, label: label, texto: texto, original: texto.textContent, quitar: quitar, url: null };
                });

                function limpiar(s) {
                    if (s.url) URL.revokeObjectURL(s.url);
                    s.url = null;
                    s.input.value = '';
                    s.label.classList.remove('con-foto');
                    s.label.style.backgroundImage = '';
                    s.texto.textContent = s.original;
                    s.quitar.hidden = true;
                }

                // La primera foto elegida (en orden) es la imagen principal
                function marcarPrincipal() {
                    var primera = true;
                    slots.forEach(function (s) {
                        var tiene = s.input.files.length > 0;
                        s.label.classList.toggle('es-principal', tiene && primera);
                        if (tiene) primera = false;
                    });
                }

                slots.forEach(function (s) {
                    s.input.addEventListener('change', function () {
                        mostrarAviso('');
                        if (!s.input.files.length) { limpiar(s); marcarPrincipal(); return; }

                        var archivo = s.input.files[0];
                        if (!/^image\/(jpeg|png)$/.test(archivo.type)) {
                            limpiar(s); marcarPrincipal();
                            mostrarAviso('Solo se aceptan imágenes JPG o PNG.');
                            return;
                        }
                        if (archivo.size > MAX_BYTES) {
                            limpiar(s); marcarPrincipal();
                            mostrarAviso('Cada foto puede pesar hasta 5 MB.');
                            return;
                        }

                        if (s.url) URL.revokeObjectURL(s.url);
                        s.url = URL.createObjectURL(archivo);
                        s.label.style.backgroundImage = 'url("' + s.url + '")';
                        s.label.classList.add('con-foto');
                        s.texto.textContent = archivo.name;
                        s.quitar.hidden = false;
                        marcarPrincipal();
                    });

                    s.quitar.addEventListener('click', function () {
                        limpiar(s);
                        marcarPrincipal();
                        s.input.focus();
                    });
                });

                            // Contador de caracteres de la descripción
                            var desc = document.querySelector('[id$="txtDescripcion"]');
                            var contador = document.querySelector('.contador');
                            if (desc && contador) {
                                desc.maxLength = 1000;
                                var actualizar = function () { contador.textContent = desc.value.length + ' / 1000 caracteres'; };
                                desc.addEventListener('input', actualizar);
                                actualizar();
                            }

                    // Revisión antes de enviar: si falta algo avisa SIN recargar la página (así no se pierden las fotos)
                    // El servidor igual vuelve a validar todo: esto es solo para comodidad.
                    var avisoForm = document.createElement('div');
                    avisoForm.className = 'publicacion-error';
                    avisoForm.setAttribute('role', 'alert');
                    avisoForm.style.display = 'none';
                    var acciones = document.querySelector('.acciones-publicacion');
                    acciones.parentNode.insertBefore(avisoForm, acciones);

                    function campo(sufijo) { return document.querySelector('[id$="' + sufijo + '"]'); }

                    var btnPublicar = campo('btnPublicar');
                    if (btnPublicar) {
                        btnPublicar.addEventListener('click', function (ev) {
                            var faltan = [];
                            if (!campo('txtTitulo').value.trim()) faltan.push('el título');
                            if (!campo('txtDescripcion').value.trim()) faltan.push('la descripción');
                            if (campo('ddlCategoria').value === '0') faltan.push('la categoría');
                            if (!campo('rbDonacion').checked && !campo('rbIntercambio').checked) faltan.push('si es donación o intercambio');
                            if (!slots.some(function (s) { return s.input.files.length > 0; })) faltan.push('al menos una foto');

                            if (faltan.length) {
                                ev.preventDefault();
                                avisoForm.textContent = 'Falta completar: ' + faltan.join(', ') + '.';
                                avisoForm.style.display = 'flex';
                            } else {
                                avisoForm.style.display = 'none';
                            }
                        });
                    }
                })();
</script>

</asp:Content>