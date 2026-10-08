
<%@ Page Title="Registrarse" Language="C#" MasterPageFile="~/Publica.Master"
    AutoEventWireup="true" CodeBehind="Registro.aspx.cs"
    Inherits="Pasalo1.Registro" %>

<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">

    <style>
        .registro-pagina {
            min-height: calc(100vh - 80px);
            background: #f8f7f3;
            padding: 55px 20px 80px;
            font-family: 'Plus Jakarta Sans', sans-serif;
        }

        .registro-contenedor {
            max-width: 510px;
            margin: 0 auto;
        }

        .registro-volver {
            display: inline-flex;
            align-items: center;
            gap: 8px;
            color: #63777a;
            font-size: 13px;
            text-decoration: none;
            margin-bottom: 25px;
        }

        .registro-volver:hover {
            color: #28666d;
        }

        .registro-card {
            background: white;
            border: 1px solid #e7ece9;
            border-radius: 20px;
            padding: 38px;
            box-shadow: 0 8px 35px rgba(35, 65, 60, 0.04);
        }

        .registro-encabezado {
            text-align: center;
            margin-bottom: 32px;
        }

        .registro-icono {
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

        .registro-encabezado h1 {
            font-size: 27px;
            font-weight: 700;
            color: #244f53;
            margin: 0 0 10px;
        }

        .registro-encabezado p {
            font-size: 14px;
            line-height: 1.7;
            color: #718183;
            margin: 0;
        }

        .registro-grupo {
            margin-bottom: 21px;
        }

        .registro-label {
            display: block;
            font-size: 13px;
            font-weight: 700;
            color: #344f51;
            margin-bottom: 9px;
        }

        .registro-input {
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

        .registro-input:focus {
            border-color: #38918b;
            box-shadow: 0 0 0 3px rgba(56, 145, 139, 0.12);
        }

        .registro-input::placeholder {
            color: #a0aeaa;
        }

        .registro-ayuda {
            display: block;
            margin-top: 7px;
            font-size: 12px;
            color: #879591;
            line-height: 1.5;
        }

        .registro-aviso {
            display: flex;
            gap: 11px;
            align-items: flex-start;
            background: #edf5f1;
            border: 1px solid #dbece5;
            border-radius: 11px;
            padding: 14px;
            margin-bottom: 25px;
        }

        .registro-aviso i {
            font-size: 19px;
            color: #28666d;
            flex-shrink: 0;
        }

        .registro-aviso p {
            font-size: 12px;
            line-height: 1.7;
            color: #526d66;
            margin: 0;
        }

        .registro-boton {
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

        .registro-boton:hover {
            background: #1e5056;
        }

        .registro-login {
            text-align: center;
            margin-top: 25px;
            font-size: 13px;
            color: #718183;
        }

        .registro-login a {
            color: #28666d;
            font-weight: 700;
            text-decoration: none;
        }

        .registro-login a:hover {
            text-decoration: underline;
        }

        .registro-pie {
            text-align: center;
            margin-top: 25px;
            color: #8a9995;
            font-size: 12px;
        }

        @media (max-width: 550px) {
            .registro-pagina {
                padding: 30px 15px 55px;
            }

            .registro-card {
                padding: 27px 22px;
            }

            .registro-encabezado h1 {
                font-size: 23px;
            }
        }
    </style>

</asp:Content>


<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="registro-pagina">

        <div class="registro-contenedor">

            <!-- VOLVER -->
            <a href="Inicio.aspx" class="registro-volver">
                <i class="ti ti-arrow-left"></i>
                Volver al inicio
            </a>

            <!-- FORMULARIO -->
            <div class="registro-card">

                <div class="registro-encabezado">

                    <div class="registro-icono">
                        <i class="ti ti-user-plus"></i>
                    </div>

                    <h1>Creá tu cuenta</h1>

                    <p>
                        Sumate a PÁSALO y empezá a compartir
                        con la comunidad UTN FRGP.
                    </p>

                </div>


                <!-- NOMBRE -->
                <div class="registro-grupo">

                    <asp:Label ID="lblNombre" runat="server"
                        AssociatedControlID="txtNombre"
                        CssClass="registro-label"
                        Text="Nombre y apellido" />

                    <asp:TextBox ID="txtNombre" runat="server"
                        CssClass="registro-input"
                        placeholder="Ej: Juan Pérez"
                        MaxLength="100" />

                </div>


                <!-- CORREO -->
                <div class="registro-grupo">

                    <asp:Label ID="lblEmail" runat="server"
                        AssociatedControlID="txtEmail"
                        CssClass="registro-label"
                        Text="Correo institucional" />

                    <asp:TextBox ID="txtEmail" runat="server"
                        CssClass="registro-input"
                        TextMode="Email"
                        placeholder="usuario@frgp.utn.edu.ar"
                        MaxLength="150" />

                    <span class="registro-ayuda">
                        Utilizá tu correo institucional de la UTN FRGP.
                    </span>

                </div>


                <!-- CARRERA -->
                <div class="registro-grupo">

                    <asp:Label ID="lblCarrera" runat="server"
                        AssociatedControlID="ddlCarrera"
                        CssClass="registro-label"
                        Text="Carrera" />

                    <asp:DropDownList ID="ddlCarrera" runat="server"
                        CssClass="registro-input">

                        <asp:ListItem Text="Seleccioná tu carrera" Value="" />

                        <asp:ListItem Text="Tecnicatura Universitaria en Programación"
                            Value="TUP" />

                        <asp:ListItem Text="Ingeniería en Sistemas de Información"
                            Value="ISI" />

                        <asp:ListItem Text="Ingeniería Mecánica"
                            Value="MEC" />

                        <asp:ListItem Text="Ingeniería Eléctrica"
                            Value="ELE" />

                        <asp:ListItem Text="Ingeniería Industrial"
                            Value="IND" />

                        <asp:ListItem Text="Otra carrera"
                            Value="OTRA" />

                    </asp:DropDownList>

                </div>


                <!-- CONTRASEÑA -->
                <div class="registro-grupo">

                    <asp:Label ID="lblPassword" runat="server"
                        AssociatedControlID="txtPassword"
                        CssClass="registro-label"
                        Text="Contraseña" />

                    <asp:TextBox ID="txtPassword" runat="server"
                        CssClass="registro-input"
                        TextMode="Password"
                        placeholder="Ingresá una contraseña"
                        MaxLength="100" />

                    <span class="registro-ayuda">
                        Utilizá al menos 8 caracteres.
                    </span>

                </div>


                <!-- CONFIRMAR CONTRASEÑA -->
                <div class="registro-grupo">

                    <asp:Label ID="lblConfirmar" runat="server"
                        AssociatedControlID="txtConfirmar"
                        CssClass="registro-label"
                        Text="Confirmar contraseña" />

                    <asp:TextBox ID="txtConfirmar" runat="server"
                        CssClass="registro-input"
                        TextMode="Password"
                        placeholder="Repetí tu contraseña"
                        MaxLength="100" />

                </div>


                <!-- AVISO -->
                <div class="registro-aviso">

                    <i class="ti ti-shield-check"></i>

                    <p>
                        PÁSALO es una comunidad exclusiva para
                        estudiantes de la UTN FRGP.
                        El registro requiere un correo institucional.
                    </p>

                </div>


                <!-- BOTON -->
                <asp:Button ID="btnRegistrarse" runat="server"
                    Text="Crear mi cuenta"
                    CssClass="registro-boton" />


                <!-- INICIAR SESION -->
                <div class="registro-login">

                    ¿Ya tenés una cuenta?

                    <a href="Login.aspx">Iniciar sesión</a>

                </div>

            </div>


            <div class="registro-pie">
                PÁSALO · Comunidad universitaria UTN FRGP
            </div>

        </div>

    </div>

</asp:Content>
