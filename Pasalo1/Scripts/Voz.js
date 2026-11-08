/* PÁSALO - voz.js
   1) Texto a voz: botón "Escuchar" en cada mensaje del chat.
   2) Dictado: botón "Dictar" para escribir hablando.
   Usa las funciones del propio navegador (Web Speech API): sin servidor ni librerías. */
(function (global) {
    'use strict';

    var cfg = {
        mensajes: '.burbuja',                 // cada burbuja del chat
        textoMensaje: 'p',                    // dentro de la burbuja: el texto (así no se lee la hora ni el botón)
        entrada: 'input[id$="txtMensaje"]',   // cuadro de texto (termina en txtMensaje aunque ASP.NET le cambie el prefijo)
        idioma: 'es-AR'
    };

    var synth = global.speechSynthesis;
    var Reconocimiento = global.SpeechRecognition || global.webkitSpeechRecognition;
    var botonActivo = null;
    var dictando = false;
    var estadoEl = null;

    function crear(tag, clase, html) {
        var el = document.createElement(tag);
        if (clase) el.className = clase;
        if (html) el.innerHTML = html;
        return el;
    }

    function avisar(texto) {
        if (estadoEl) estadoEl.textContent = texto;
    }

    // ---------- Texto a voz ----------
    function elegirVoz() {
        var voces = synth.getVoices();
        for (var i = 0; i < voces.length; i++) if (voces[i].lang === cfg.idioma) return voces[i];
        for (var j = 0; j < voces.length; j++) if (voces[j].lang.toLowerCase().indexOf('es') === 0) return voces[j];
        return null;
    }

    function reiniciarBoton(boton) {
        if (!boton) return;
        boton.setAttribute('aria-pressed', 'false');
        boton.innerHTML = '<i class="ti ti-volume voz-icono" aria-hidden="true"></i> Escuchar';
    }

    function detener() {
        if (synth) synth.cancel();
        reiniciarBoton(botonActivo);
        botonActivo = null;
    }

    function leer(texto, boton) {
        if (!synth || !texto) return;
        synth.cancel();
        reiniciarBoton(botonActivo);

        var voz = new SpeechSynthesisUtterance(texto);
        voz.lang = cfg.idioma;
        var elegida = elegirVoz();
        if (elegida) voz.voice = elegida;

        botonActivo = boton;
        boton.setAttribute('aria-pressed', 'true');
        boton.innerHTML = '<i class="ti ti-player-stop voz-icono" aria-hidden="true"></i> Detener';

        voz.onend = voz.onerror = function () {
            if (botonActivo === boton) { reiniciarBoton(boton); botonActivo = null; }
        };
        synth.speak(voz);
    }

    function agregarBotonesEscuchar() {
        if (!synth) return;
        var mensajes = document.querySelectorAll(cfg.mensajes);
        for (var i = 0; i < mensajes.length; i++) {
            (function (mensaje) {
                if (mensaje.getAttribute('data-voz-listo')) return;
                mensaje.setAttribute('data-voz-listo', '1');

                var origen = mensaje.querySelector(cfg.textoMensaje) || mensaje;
                var boton = crear('button', 'voz-boton');
                boton.type = 'button';                       // importante: dentro del form no debe enviar la página
                boton.setAttribute('aria-label', 'Escuchar este mensaje');
                reiniciarBoton(boton);
                boton.addEventListener('click', function () {
                    if (botonActivo === boton) detener();
                    else leer((origen.textContent || '').replace(/\s+/g, ' ').trim(), boton);
                });
                mensaje.appendChild(boton);
            })(mensajes[i]);
        }
    }

    // ---------- Dictado ----------
    function mensajeError(codigo) {
        switch (codigo) {
            case 'not-allowed':
            case 'service-not-allowed': return 'No se pudo usar el micrófono. Permitilo desde el candado de la barra de direcciones.';
            case 'no-speech': return 'No escuché nada. Probá de nuevo.';
            case 'audio-capture': return 'No se encontró un micrófono.';
            case 'network': return 'El dictado necesita conexión a internet.';
            default: return 'No se pudo dictar. Probá de nuevo.';
        }
    }

    function agregarDictado() {
        var entrada = document.querySelector(cfg.entrada);
        if (!entrada || entrada.getAttribute('data-voz-listo')) return;
        entrada.setAttribute('data-voz-listo', '1');

        // Cartel de estado, debajo de la fila del cuadro de texto (lo anuncian los lectores de pantalla)
        estadoEl = crear('p', 'voz-estado');
        estadoEl.setAttribute('role', 'status');
        estadoEl.setAttribute('aria-live', 'polite');
        entrada.parentNode.parentNode.insertBefore(estadoEl, entrada.parentNode.nextSibling);

        if (!Reconocimiento) {
            avisar('El dictado por voz funciona en Chrome y Edge.');
            return;
        }

        var mic = crear('button', 'voz-boton');
        mic.type = 'button';
        mic.setAttribute('aria-pressed', 'false');
        mic.setAttribute('aria-label', 'Dictar mensaje con la voz');
        mic.innerHTML = '<i class="ti ti-microphone voz-icono" aria-hidden="true"></i> Dictar';
        entrada.parentNode.insertBefore(mic, entrada.nextSibling);   // entre el cuadro y el botón Enviar

        var rec = null;
        mic.addEventListener('click', function () {
            if (rec) { rec.stop(); return; }

            detener();                                    // no leer y escuchar a la vez
            rec = new Reconocimiento();
            rec.lang = cfg.idioma;
            rec.interimResults = true;
            rec.continuous = false;

            var base = entrada.value;
            if (base && !/\s$/.test(base)) base += ' ';

            rec.onstart = function () {
                dictando = true;
                mic.setAttribute('aria-pressed', 'true');
                mic.innerHTML = '<i class="ti ti-player-stop voz-icono" aria-hidden="true"></i> Detener';
                avisar('Escuchando… hablá ahora.');
            };
            rec.onresult = function (e) {
                var dicho = '';
                for (var i = 0; i < e.results.length; i++) dicho += e.results[i][0].transcript;
                var nuevo = base + dicho;
                if (entrada.maxLength > 0) nuevo = nuevo.slice(0, entrada.maxLength);
                entrada.value = nuevo;
            };
            rec.onerror = function (e) { avisar(mensajeError(e.error)); };
            rec.onend = function () {
                rec = null;
                dictando = false;
                mic.setAttribute('aria-pressed', 'false');
                mic.innerHTML = '<i class="ti ti-microphone voz-icono" aria-hidden="true"></i> Dictar';
                if (estadoEl.textContent === 'Escuchando… hablá ahora.')
                    avisar(entrada.value ? 'Listo. Revisá el texto antes de enviar.' : '');
            };
            try { rec.start(); } catch (err) { rec = null; dictando = false; avisar('No se pudo iniciar el dictado.'); }
        });
    }

    // ---------- Arranque ----------
    function iniciar(opciones) {
        for (var k in opciones) if (Object.prototype.hasOwnProperty.call(opciones, k)) cfg[k] = opciones[k];
        agregarBotonesEscuchar();
        agregarDictado();
        global.addEventListener('beforeunload', function () { if (synth) synth.cancel(); });
    }

    // La página se recarga sola cada tanto para traer mensajes: no debe hacerlo mientras se lee o se dicta
    function ocupado() {
        return dictando || !!(synth && synth.speaking);
    }

    global.PasaloVoz = { iniciar: iniciar, detener: detener, ocupado: ocupado };
})(window);