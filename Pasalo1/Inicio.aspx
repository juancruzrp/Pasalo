
<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Publica.Master"
    AutoEventWireup="true" CodeBehind="Inicio.aspx.cs"
    Inherits="Pasalo1.Inicio" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">

    <style>
        .inicio {
            background: #f8f7f3;
            min-height: calc(100vh - 80px);
            font-family: 'Plus Jakarta Sans', sans-serif;
        }

        .inicio-hero {
            max-width: 1150px;
            margin: 0 auto;
            padding: 90px 24px 75px;
            text-align: center;
        }

        .inicio-etiqueta {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            background: #e4efed;
            color: #28666d;
            padding: 9px 17px;
            border-radius: 30px;
            font-size: 13px;
            font-weight: 700;
            margin-bottom: 25px;
        }

        .inicio-hero h1 {
            color: #244f53;
            font-size: clamp(32px, 5vw, 56px);
            line-height: 1.2;
            font-weight: 700;
            max-width: 850px;
            margin: 0 auto 24px;
        }

        .inicio-hero h1 span {
            color: #38918b;
        }

        .inicio-descripcion {
            max-width: 660px;
            margin: 0 auto 32px;
            color: #63777a;
            font-size: 17px;
            line-height: 1.8;
        }

        .inicio-botones {
            display: flex;
            justify-content: center;
            gap: 14px;
            flex-wrap: wrap;
        }

        .inicio-btn {
            display: inline-flex;
            align-items: center;
            justify-content: center;
            gap: 9px;
            padding: 14px 24px;
            border-radius: 12px;
            text-decoration: none;
            font-size: 14px;
            font-weight: 700;
            transition: 0.2s;
        }

        .inicio-btn-principal {
            background: #28666d;
            color: white;
        }

        .inicio-btn-principal:hover {
            background: #1d5056;
            color: white;
        }

        .inicio-btn-secundario {
            background: white;
            color: #28666d;
            border: 1px solid #d7e1df;
        }

        .inicio-btn-secundario:hover {
            background: #edf4f2;
            color: #28666d;
        }

        .inicio-seccion {
            max-width: 1150px;
            margin: 0 auto;
            padding: 65px 24px;
        }

        .inicio-titulo-seccion {
            text-align: center;
            color: #244f53;
            font-size: 30px;
            margin-bottom: 15px;
        }

        .inicio-subtitulo {
            text-align: center;
            color: #718183;
            font-size: 15px;
            line-height: 1.7;
            max-width: 650px;
            margin: 0 auto 40px;
        }

        .inicio-tarjetas {
            display: grid;
            grid-template-columns: repeat(3, 1fr);
            gap: 22px;
        }

        .inicio-tarjeta {
            background: white;
            border: 1px solid #e8ecea;
            border-radius: 18px;
            padding: 32px 25px;
            text-align: center;
        }

        .inicio-icono {
            width: 60px;
            height: 60px;
            border-radius: 16px;
            background: #e4efed;
            color: #28666d;
            display: flex;
            align-items: center;
            justify-content: center;
            font-size: 28px;
            margin: 0 auto 20px;
        }

        .inicio-tarjeta h3 {
            color: #244f53;
            font-size: 18px;
            margin-bottom: 12px;
        }

        .inicio-tarjeta p {
            color: #718183;
            font-size: 14px;
            line-height: 1.7;
        }

        .inicio-nosotros {
            background: #e8f1ed;
        }

        .inicio-nosotros-contenido {
            max-width: 850px;
            margin: 0 auto;
            padding: 70px 24px;
            text-align: center;
        }

        .inicio-nosotros h2 {
            color: #244f53;
            font-size: 30px;
            margin-bottom: 20px;
        }

        .inicio-nosotros p {
            color: #586e6c;
            line-height: 1.9;
            font-size: 15px;
        }

        .inicio-cta {
            max-width: 1100px;
            margin: 65px auto;
            padding: 55px 25px;
            border-radius: 22px;
            background: #28666d;
            text-align: center;
        }

        .inicio-cta h2 {
            color: white;
            font-size: 30px;
            margin-bottom: 15px;
        }

        .inicio-cta p {
            color: #d8e9e7;
            margin-bottom: 28px;
            font-size: 15px;
        }

        .inicio-cta .inicio-btn {
            background: white;
            color: #28666d;
        }

        .inicio-footer {
            text-align: center;
            padding: 25px;
            color: #84918e;
            font-size: 13px;
            border-top: 1px solid #e5e9e6;
        }

        @media (max-width: 768px) {
            .inicio-hero {
                padding: 65px 20px;
            }

            .inicio-tarjetas {
                grid-template-columns: 1fr;
            }

            .inicio-seccion {
                padding: 45px 20px;
            }

            .inicio-cta {
                margin: 40px 20px;
            }
        }
    </style>

</asp:Content>


<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="inicio">

        <!-- PRESENTACION -->
        <section class="inicio-hero">

            <div class="inicio-etiqueta">
                <i class="ti ti-leaf"></i>
                Comunidad UTN FRGP
            </div>

            <h1>
                Lo que ya no usás,
                <span>otro estudiante lo necesita.</span>
            </h1>

            <p class="inicio-descripcion">
                PÁSALO es una plataforma para donar e intercambiar
                artículos entre estudiantes de la UTN FRGP.
                Compartí lo que ya no necesitás, encontrá lo que
                estás buscando y ayudá a construir una comunidad
                más solidaria y sustentable.
            </p>

            <div class="inicio-botones">

                <a href="Registro.aspx" class="inicio-btn inicio-btn-principal">
                    Registrarme gratis
                    <i class="ti ti-arrow-right"></i>
                </a>

                <a href="#como-funciona" class="inicio-btn inicio-btn-secundario">
                    ¿Cómo funciona?
                    <i class="ti ti-arrow-down"></i>
                </a>

            </div>

        </section>


        <!-- COMO FUNCIONA -->
        <section class="inicio-seccion" id="como-funciona">

            <h2 class="inicio-titulo-seccion">
                ¿Cómo funciona PÁSALO?
            </h2>

            <p class="inicio-subtitulo">
                Compartir es muy sencillo. En tres pasos podés
                empezar a formar parte de nuestra comunidad.
            </p>

            <div class="inicio-tarjetas">

                <div class="inicio-tarjeta">

                    <div class="inicio-icono">
                        <i class="ti ti-user-plus"></i>
                    </div>

                    <h3>1. Registrate</h3>

                    <p>
                        Creá tu cuenta utilizando tu correo
                        institucional de la UTN FRGP.
                    </p>

                </div>

                <div class="inicio-tarjeta">

                    <div class="inicio-icono">
                        <i class="ti ti-package"></i>
                    </div>

                    <h3>2. Publicá o buscá</h3>

                    <p>
                        Publicá artículos que quieras donar o
                        intercambiar, o explorá los objetos
                        disponibles de otros estudiantes.
                    </p>

                </div>

                <div class="inicio-tarjeta">

                    <div class="inicio-icono">
                        <i class="ti ti-arrows-exchange"></i>
                    </div>

                    <h3>3. Compartí</h3>

                    <p>
                        Contactate con otro estudiante y coordiná
                        la entrega dentro de la universidad.
                    </p>

                </div>

            </div>

        </section>


        <!-- SOBRE NOSOTROS -->
        <section class="inicio-nosotros" id="sobre-nosotros">

            <div class="inicio-nosotros-contenido">

                <div class="inicio-etiqueta">
                    <i class="ti ti-heart-handshake"></i>
                    Sobre nosotros
                </div>

                <h2>Una comunidad que comparte</h2>

                <p>
                    PÁSALO nace como un proyecto universitario
                    desarrollado por estudiantes de la Tecnicatura
                    Universitaria en Programación de la UTN FRGP.
                </p>

                <p>
                    Nuestro objetivo es facilitar el intercambio
                    y la donación de artículos entre compañeros,
                    promoviendo el consumo responsable,
                    la reutilización de recursos y la colaboración
                    dentro de la comunidad educativa.
                </p>

            </div>

        </section>


        <!-- INVITACION FINAL -->
        <section class="inicio-cta">

            <h2>¿Listo para empezar a compartir?</h2>

            <p>
                Sumate a PÁSALO y dale una segunda oportunidad
                a los objetos que ya no utilizás.
            </p>

            <a href="Registro.aspx" class="inicio-btn">
                Crear mi cuenta
                <i class="ti ti-arrow-right"></i>
            </a>

        </section>


        <!-- PIE DE PAGINA -->
        <footer class="inicio-footer">
            PÁSALO - Proyecto universitario UTN FRGP
        </footer>

    </div>

</asp:Content>
