
<%@ Page Title="Calificar usuario" Language="C#"
    MasterPageFile="~/Principal.Master"
    AutoEventWireup="true"
    CodeBehind="Calificar.aspx.cs"
    Inherits="Pasalo1.Calificar" %>

<asp:Content ID="Contenido" ContentPlaceHolderID="MainContent"
    runat="server">

    <style>
        .calificar-contenedor {
            max-width: 580px;
            margin: 45px auto;
            padding: 0 20px;
        }

        .calificar-card {
            background: white;
            border: 1px solid #e3e8e5;
            border-radius: 16px;
            padding: 30px;
        }

        .calificar-card h1 {
            color: #244f53;
            margin-bottom: 10px;
        }

        .calificar-subtitulo {
            color: #718183;
            line-height: 1.6;
            margin-bottom: 25px;
        }

        .calificar-label {
            display: block;
            font-weight: 700;
            color: #244f53;
            margin-bottom: 10px;
        }

        .calificar-estrellas {
            display: flex;
            gap: 8px;
            margin-bottom: 25px;
            flex-wrap: wrap;
        }

        .calificar-estrellas input {
            position: absolute;
            opacity: 0;
        }

        .calificar-estrellas label {
            cursor: pointer;
            font-size: 35px;
            color: #cbd5d1;
            transition: color .15s;
        }

        .calificar-estrellas label.seleccionada {
            color: #e7aa29;
        }

        .calificar-estrellas input:focus-visible + label {
            outline: 2px solid #28666d;
            outline-offset: 3px;
            border-radius: 4px;
        }

        .calificar-comentario {
            width: 100%;
            min-height: 110px;
            padding: 12px;
            border: 1px solid #d7e1df;
            border-radius: 10px;
            resize: vertical;
            font: inherit;
            box-sizing: border-box;
        }

        .calificar-boton {
            margin-top: 20px;
            background: #28666d;
            color: white;
            border: none;
            border-radius: 10px;
            padding: 12px 20px;
            font-weight: 700;
            cursor: pointer;
        }

        .calificar-error {
            background: #fce8e8;
            color: #ad3434;
            padding: 12px;
            border-radius: 10px;
            margin-bottom: 18px;
        }

        .calificar-volver {
            display: inline-block;
            margin-top: 18px;
            color: #28666d;
            text-decoration: none;
        }
    </style>

    <div class="calificar-contenedor">
        <div class="calificar-card">

            <h1>Calificar usuario</h1>

            <p class="calificar-subtitulo">
                Contanos cómo fue tu experiencia en esta donación
                o intercambio.
            </p>

            <asp:Panel ID="pnlError" runat="server"
                Visible="false" CssClass="calificar-error">
                <asp:Label ID="lblError" runat="server" />
            </asp:Panel>

            <asp:Panel ID="pnlFormulario" runat="server">

                <span class="calificar-label">
                    ¿Cómo fue tu experiencia?
                </span>

                <div class="calificar-estrellas" id="grupoEstrellas">
                    <input type="radio" name="puntajeVisual"
                        id="estrella1" value="1" />
                    <label for="estrella1" aria-label="1 estrella">★</label>

                    <input type="radio" name="puntajeVisual"
                        id="estrella2" value="2" />
                    <label for="estrella2" aria-label="2 estrellas">★</label>

                    <input type="radio" name="puntajeVisual"
                        id="estrella3" value="3" />
                    <label for="estrella3" aria-label="3 estrellas">★</label>

                    <input type="radio" name="puntajeVisual"
                        id="estrella4" value="4" />
                    <label for="estrella4" aria-label="4 estrellas">★</label>

                    <input type="radio" name="puntajeVisual"
                        id="estrella5" value="5" />
                    <label for="estrella5" aria-label="5 estrellas">★</label>
                </div>

                <asp:HiddenField ID="hfPuntaje" runat="server" />

                <label class="calificar-label" for="txtComentario">
                    Comentario (opcional)
                </label>

                <asp:TextBox ID="txtComentario" runat="server"
                    ClientIDMode="Static"
                    TextMode="MultiLine"
                    MaxLength="500"
                    CssClass="calificar-comentario"
                    placeholder="Contá brevemente cómo fue la experiencia..." />

                <asp:Button ID="btnGuardar" runat="server"
                    Text="Enviar calificación"
                    CssClass="calificar-boton"
                    OnClick="btnGuardar_Click" />

            </asp:Panel>

            <a href="MiActividad.aspx" class="calificar-volver">
                Volver a Mi actividad
            </a>
        </div>
    </div>

    <script>
        (function () {
            var grupo = document.getElementById("grupoEstrellas");
            if (!grupo) return;

            var radios = grupo.querySelectorAll('input[type="radio"]');
            var hidden = document.getElementById('<%= hfPuntaje.ClientID %>');

            function pintar(valor) {
                radios.forEach(function (radio) {
                    var label = radio.nextElementSibling;
                    label.classList.toggle(
                        "seleccionada",
                        Number(radio.value) <= valor
                    );
                });
            }

            radios.forEach(function (radio) {
                radio.addEventListener("change", function () {
                    hidden.value = radio.value;
                    pintar(Number(radio.value));
                });
            });

            if (hidden.value) {
                radios.forEach(function (radio) {
                    radio.checked = radio.value === hidden.value;
                });
                pintar(Number(hidden.value));
            }
        })();
    </script>

</asp:Content>
