
<%@ Page Title="Iniciar sesión" Language="C#" MasterPageFile="~/Publica.Master"
    AutoEventWireup="true" CodeBehind="Login.aspx.cs"
    Inherits="Pasalo1.Login" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">

    <style>
        .login-pagina {
            min-height: calc(100vh - 80px);
            background: #f8f7f3;
            padding: 65px 20px 80px;
            font-family: 'Plus Jakarta Sans', sans-serif;
        }

        .login-contenedor {
            max-width: 470px;
            margin: 0 auto;
        }

        .login-volver {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            color: #63777a;
            font-size: 13px;
            text-decoration: none;
            margin-bottom: 25px;
        }

        .login-volver:hover {
            color: #28666d;
        }

        .login-card {
            background: white;
            border: 1px solid #e7ece9;
            border-radius: 20px;
            padding: 40px;
            box-shadow: 0 8px 35px rgba(35, 65, 60, 0.04);
        }

        .login-encabezado {
            text-align: center;
            margin-bottom: 32px;
        }

        .login-icono {
            width: 58px;
            height: 58px;
            display: flex;
            align-items: center;
            justify-content: center;
            margin: 0 auto 18px;
            background: #e4efed;
            color: #28666d;
            border-radius: 17px;
            font-size: 28px;
        }

        .login-encabezado h1 {
            font-size: 27px;
            font-weight: 700;
            color: #244f53;
            margin: 0 0 10px;
        }

        .login-encabezado p {
            font-size: 14px;
            line-height: 1.7;
            color: #718183;
            margin: 0;
        }

        .login-grupo {
            margin-bottom: 22px;
        }

        .login-label {
            display: block;
            font-size: 13px;
            font-weight: 700;
            color: #344f51;
            margin-bottom: 9px;
        }

        .login-input {
            display: block;
            width: 100%;
            box-sizing: border-box;
            padding: 13px 15px;
            border: 1px solid #dce5e2;
            border-radius: 10px;
            background: #fff;
            color: #344f51;
            font-family: inherit;
            font-size: 14px;
            outline: none;
            transition: border-color 0.2s, box-shadow 0.2s;
        }

        .login-input:focus {
            border-color: #38918b;
            box-shadow: 0 0 0 3px rgba(56, 145, 139, 0.12);
        }

        .login-input::placeholder {
            color: #a0aeaa;
        }

        .login-opciones {
            display: flex;
            justify-content: flex-end;
            margin-top: -8px;
            margin-bottom: 25px;
        }

        .login-recuperar {
            font-size: 12px;
            font-weight: 700;
            color: #28666d;
            text-decoration: none;
        }

        .login-recuperar:hover {
            text-decoration: underline;
        }

        .login-boton {
            display: block;
            width: 100%;
            padding: 15px;
            border: none;
            border-radius: 11px;
            background: #28666d;
            color: white;
            font-family: inherit;
            font-size: 14px;
            font-weight: 700;
            cursor: pointer;
            transition: background 0.2s;
        }

        .login-boton:hover {
            background: #1e5056;
        }

        .login-registro {
            text-align: center;
            margin-top: 27px;
            font-size: 13px;
            color: #718183;
        }

        .login-registro a {
            color: #28666d;
            font-weight: 700;
            text-decoration: none;
        }

        .login-registro a:hover {
            text-decoration: underline;
        }

        .login-aviso {
            display: flex;
            gap: 11px;
            align-items: flex-start;
            background: #edf5f1;
            border: 1px solid #dbece5;
            border-radius: 11px;
            padding: 14px;
            margin-top: 25px;
        }

        .login-aviso i {
            font-size: 19px;
            color: #28666d;
            flex-shrink: 0;
        }

        .login-aviso p {
            font-size: 12px;
            line-height: 1.7;
            color: #526d66;
            margin: 0;
        }

        .login-pie {
            text-align: center;
            margin-top: 25px;
            color: #8a9995;
            font-size: 12px;
        }

        @media (max-width: 550px) {
            .login-pagina {
                padding: 35px 15px 55px;
            }

            .login-card {
                padding: 28px 23px;
            }

            .login-encabezado h1 {
                font-size: 23px;
            }
        }
    </style>

</asp:Content>


<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="login-pagina">

        <div class="login-contenedor">

            <!-- VOLVER -->
            <a href="Inicio.aspx" class="login-volver">
                <i class="ti ti-arrow-left"></i>
                Volver al inicio
            </a>

            <!-- FORMULARIO -->
            <div class="login-card">

                <!-- ENCABEZADO -->
                <div class="login-encabezado">

                    <div class="login-icono">
                        <i class="ti ti-login-2"></i>
                    </div>

                    <h1>¡Bienvenido de nuevo!</h1>

                    <p>
                        Iniciá sesión para seguir compartiendo
                        con la comunidad PÁSALO.
                    </p>

                </div>


                <!-- CORREO -->
                <div class="login-grupo">

                    <asp:Label ID="lblEmail" runat="server"
                        AssociatedControlID="txtEmail"
                        CssClass="login-label"
                        Text="Correo institucional" />

                    <asp:TextBox ID="txtEmail" runat="server"
                        CssClass="login-input"
                        TextMode="Email"
                        placeholder="usuario@frgp.utn.edu.ar"
                        MaxLength="150" />

                </div>


                <!-- CONTRASEÑA -->
                <div class="login-grupo">

                    <asp:Label ID="lblPassword" runat="server"
                        AssociatedControlID="txtPassword"
                        CssClass="login-label"
                        Text="Contraseña" />

                    <asp:TextBox ID="txtPassword" runat="server"
                        CssClass="login-input"
                        TextMode="Password"
                        placeholder="Ingresá tu contraseña"
                        MaxLength="100" />

                </div>


                <!-- RECUPERAR CONTRASEÑA -->
                <div class="login-opciones">

                    <a href="#" class="login-recuperar">
                        ¿Olvidaste tu contraseña?
                    </a>

                </div>


                <!-- BOTON INICIAR SESION -->
                <asp:Button ID="btnIngresar" runat="server"
                    Text="Iniciar sesión"
                    CssClass="login-boton" />


                <!-- REGISTRO -->
                <div class="login-registro">

                    ¿Todavía no tenés una cuenta?

                    <a href="Registro.aspx">Registrate</a>

                </div>


                <!-- AVISO -->
                <div class="login-aviso">

                    <i class="ti ti-shield-lock"></i>

                    <p>
                        El acceso a PÁSALO es exclusivo para
                        estudiantes de la UTN FRGP registrados
                        con su correo institucional.
                    </p>

                </div>

            </div>


            <div class="login-pie">
                PÁSALO · Comunidad universitaria UTN FRGP
            </div>

        </div>

    </div>

</asp:Content>
