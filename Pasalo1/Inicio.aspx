
<%@ Page Title="Inicio" Language="C#" MasterPageFile="~/Publica.Master"
    AutoEventWireup="true" CodeBehind="Inicio.aspx.cs"
    Inherits="Pasalo1.Inicio" %>
<asp:Content ID="ContentHead" ContentPlaceHolderID="HeadContent" runat="server">

    <style>
        @media (prefers-reduced-motion: no-preference) { html { scroll-behavior: smooth; } }

        .inicio {
            --verde: #28666d;
            --verde-osc: #244f53;
            --verde-claro: #e4efed;
            --fondo: #f8f7f3;
            --texto: #5b6f72;
            --borde: #e3e8e5;
            background: var(--fondo);
            color: var(--texto);
            font-family: 'Plus Jakarta Sans', sans-serif;
        }
        
        .inicio :where(h1, h2, h3, p) { margin: 0; }
        .inicio h1, .inicio h2, .inicio h3 { color: var(--verde-osc); }
        .inicio section { scroll-margin-top: 24px; }
        .i-contenedor { max-width: 1150px; margin: 0 auto; padding: 0 24px; }

        
        .completar {
            padding: 1px 7px; border: 1px dashed #b38b00; border-radius: 6px;
            background: #fff3b0; color: #5c4700; font-weight: 500;
        }
        .proximamente {
            display: inline-block; margin-left: 6px; padding: 1px 8px; border-radius: 999px;
            background: #e9e9ec; color: #44474e; font-size: 11px; font-weight: 700; vertical-align: middle;
        }

        .i-etiqueta {
            display: inline-flex; align-items: center; gap: 8px; margin-bottom: 22px; padding: 8px 16px;
            border-radius: 30px; background: var(--verde-claro); color: var(--verde); font-size: 13px; font-weight: 700;
        }

        .i-btn {
            display: inline-flex; align-items: center; justify-content: center; gap: 9px;
            padding: 14px 24px; border-radius: 12px; font-size: 14px; font-weight: 700; text-decoration: none;
        }
        .i-btn-principal { background: var(--verde); color: #fff; }
        .i-btn-principal:hover { background: #1d5056; }
        .i-btn-sec { background: #fff; color: var(--verde); border: 1px solid #d7e1df; }
        .i-btn-sec:hover { background: #edf4f2; }

        .i-titulo { text-align: center; font-size: 30px; margin-bottom: 16px; }
        .i-subtitulo { max-width: 650px; margin: 0 auto 52px; text-align: center; font-size: 16px; line-height: 1.7; }
        .i-bloque { padding: 84px 0; }

        
        .hero { padding: 70px 0 60px; }
        .hero-grid { display: grid; grid-template-columns: minmax(0, 1.1fr) minmax(0, .9fr); gap: 48px; align-items: center; }
        .hero h1 { font-size: clamp(32px, 4.5vw, 52px); line-height: 1.15; font-weight: 700; }
        .hero h1 span { color: #38918b; }
        .hero-descripcion { max-width: 560px; margin: 26px 0 34px; font-size: 17px; line-height: 1.8; }
        .botones { display: flex; flex-wrap: wrap; gap: 14px; }
        .hero-nota { display: flex; align-items: center; gap: 8px; margin-top: 24px; font-size: 14px; }
        .hero-nota i { color: var(--verde); font-size: 18px; }
        .hero-mas { display: inline-flex; align-items: center; gap: 8px; margin-top: 20px; font-size: 16px; font-weight: 700; color: var(--verde); text-decoration: none; }
        .hero-mas:hover { text-decoration: underline; }

        .vista {
            padding: 18px; border: 1px solid var(--borde); border-radius: 20px; background: #fff;
            box-shadow: 0 12px 40px rgba(35, 65, 60, .08);
        }
        .vista-cabecera { display: flex; align-items: center; justify-content: space-between; margin-bottom: 14px; font-size: 13px; font-weight: 700; color: var(--verde-osc); }
        .vista-lista { display: grid; gap: 12px; }
        .mini { display: flex; align-items: center; gap: 12px; padding: 12px; border: 1px solid var(--borde); border-radius: 14px; }
        .mini-foto {
            display: grid; place-items: center; flex: none; width: 52px; height: 52px; border-radius: 12px;
            background: var(--verde-claro); color: var(--verde); font-size: 24px;
        }
        .mini-titulo { font-size: 14px; font-weight: 700; color: var(--verde-osc); }
        .mini-meta { font-size: 12px; }
        .vista-pie { margin-top: 12px; font-size: 12px; text-align: center; }

        
        .i-estado { display: inline-flex; align-items: center; gap: 6px; margin-top: 6px; padding: 2px 9px; border-radius: 7px; font-size: 12px; font-weight: 600; }
        .i-estado::before { content: ""; width: 8px; height: 8px; background: currentColor; }
        .i-disp { background: #ddf0e3; color: #1e5b34; }
        .i-disp::before { border-radius: 50%; }
        .i-res { background: #fbe9c8; color: #7a4a00; }
        .i-res::before { transform: rotate(45deg); }

      
        .cats { display: grid; grid-template-columns: repeat(5, 1fr); gap: 16px; }
        .cat { padding: 24px 16px; border: 1px solid var(--borde); border-radius: 16px; background: #fff; text-align: center; }
        .cat i { display: grid; place-items: center; width: 52px; height: 52px; margin: 0 auto 14px; border-radius: 14px; background: var(--verde-claro); color: var(--verde); font-size: 26px; }
        .cat h3 { margin-bottom: 6px; font-size: 16px; }
        .cat p { font-size: 13px; line-height: 1.6; }

        .pasos { display: grid; grid-template-columns: repeat(4, 1fr); gap: 20px; }
        .paso { padding: 26px 20px; border: 1px solid var(--borde); border-radius: 18px; background: #fff; }
        .paso-numero {
            display: grid; place-items: center; width: 38px; height: 38px; margin-bottom: 14px; border-radius: 50%;
            background: var(--verde); color: #fff; font-weight: 700;
        }
        .paso h3 { margin-bottom: 8px; font-size: 17px; }
        .paso p { font-size: 14px; line-height: 1.7; }

        .dos { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; margin-top: 40px; }
        .dos > div { padding: 24px; border-radius: 16px; background: var(--verde-claro); }
        .dos h3 { display: flex; align-items: center; gap: 8px; margin-bottom: 8px; font-size: 17px; }
        .dos p { font-size: 14px; line-height: 1.7; color: #45595b; }

       
        .porque { display: grid; grid-template-columns: 1fr 1fr; gap: 20px; }
        .porque > div { display: flex; gap: 16px; padding: 24px; border: 1px solid var(--borde); border-radius: 18px; background: #fff; }
        .porque i { flex: none; display: grid; place-items: center; width: 48px; height: 48px; border-radius: 14px; background: var(--verde-claro); color: var(--verde); font-size: 24px; }
        .porque h3 { margin-bottom: 6px; font-size: 17px; }
        .porque p { font-size: 14px; line-height: 1.7; }

        .seccion-impacto { background: var(--verde); color: #f1f8f7; }
        .seccion-impacto .i-titulo { color: #fff; }
        .seccion-impacto .i-subtitulo { color: #f1f8f7; }

        .tres-r { display: flex; flex-wrap: wrap; justify-content: center; gap: 14px; margin: 0 0 44px; padding: 0; list-style: none; }
        .tres-r li {
            display: grid; justify-items: center; gap: 2px; min-width: 170px; padding: 14px 22px;
            border: 1px solid rgba(255, 255, 255, .35); border-radius: 14px;
            font-size: 18px; font-weight: 700; color: #fff; text-align: center;
        }
        .tres-r li small { font-size: 12px; font-weight: 700; }
        .tres-r .tres-r-activa { background: #fff; border-color: #fff; color: var(--verde); }

      
        .imp-minis { display: grid; grid-template-columns: repeat(3, 1fr); gap: 20px; margin-bottom: 52px; }
        .imp-mini { padding: 24px; border-radius: 16px; background: rgba(255, 255, 255, .12); }
        .imp-mini i {
            display: grid; place-items: center; width: 46px; height: 46px; margin-bottom: 14px; border-radius: 12px;
            background: rgba(255, 255, 255, .18); color: #fff; font-size: 24px;
        }
        .imp-mini h3 { margin-bottom: 8px; font-size: 17px; color: #fff; }
        .imp-mini p { font-size: 14px; line-height: 1.7; }
        @media (max-width: 900px) { .imp-minis { grid-template-columns: 1fr; } }
        .contadores { display: flex; flex-wrap: wrap; justify-content: center; gap: 20px; }
        .contadores > div { min-width: 200px; padding: 22px 26px; border-radius: 16px; background: rgba(255, 255, 255, .12); text-align: center; }
        .contadores strong { display: block; font-size: 34px; line-height: 1.2; color: #fff; }
        .contadores span { font-size: 14px; }
        .impacto-vacio { max-width: 560px; margin: 0 auto; padding: 18px 22px; border-radius: 14px; background: rgba(255, 255, 255, .12); text-align: center; font-size: 15px; }
        .impacto-nota { max-width: 650px; margin: 26px auto 0; font-size: 13px; text-align: center; color: #d8e9e7; }


        .faq { max-width: 800px; margin: 0 auto; display: grid; gap: 12px; }
        .faq details { border: 1px solid var(--borde); border-radius: 14px; background: #fff; }
        .faq summary {
            display: flex; align-items: center; justify-content: space-between; gap: 12px; padding: 18px 20px;
            font-size: 15px; font-weight: 700; color: var(--verde-osc); cursor: pointer; list-style: none;
        }
        .faq summary::-webkit-details-marker { display: none; }
        .faq summary::after { content: "+"; flex: none; font-size: 22px; color: var(--verde); }
        .faq details[open] summary::after { content: "−"; }
        .faq details p { padding: 0 20px 18px; font-size: 14px; line-height: 1.8; }

       
        .nosotros { background: #e8f1ed; }
        .nosotros-contenido { max-width: 850px; margin: 0 auto; padding: 70px 24px; text-align: center; }
        .nosotros h2 { margin-bottom: 20px; font-size: 30px; }
        .nosotros p { font-size: 15px; line-height: 1.9; color: #4d6361; }
        .nosotros p + p { margin-top: 14px; }

        .cta { max-width: 1100px; margin: 65px auto; padding: 55px 25px; border-radius: 22px; background: var(--verde); text-align: center; }
        .cta h2 { margin-bottom: 15px; font-size: 30px; color: #fff; }
        .cta p { margin-bottom: 28px; font-size: 15px; color: #d8e9e7; }
        .cta .i-btn { background: #fff; color: var(--verde); }

        .pie { border-top: 1px solid #e5e9e6; padding: 40px 0 28px; font-size: 14px; }
        .pie-grid { display: grid; grid-template-columns: 1.4fr 1fr 1fr; gap: 32px; }
        .pie h3 { margin-bottom: 12px; font-size: 14px; }
        .pie ul { margin: 0; padding: 0; list-style: none; display: grid; gap: 8px; }
        .pie a { color: var(--verde); text-decoration: none; }
        .pie a:hover { text-decoration: underline; }
        .pie-copia { margin-top: 28px; font-size: 13px; text-align: center; color: #84918e; }

    
        @media (max-width: 900px) {
            .hero-grid { grid-template-columns: 1fr; }
            .cats { grid-template-columns: repeat(3, 1fr); }
            .pasos { grid-template-columns: 1fr 1fr; }
            .pie-grid { grid-template-columns: 1fr; }
        }
        @media (max-width: 640px) {
            .hero { padding: 45px 0; }
            .i-bloque { padding: 45px 0; }
            .cats { grid-template-columns: 1fr 1fr; }
            .pasos, .dos, .porque { grid-template-columns: 1fr; }
            .cta { margin: 40px 20px; }
        }

       
        .barra { transition: transform .3s ease; }
        .barra--oculta { transform: translateY(-100%); }
        @media (prefers-reduced-motion: reduce) { .barra { transition: none; } }

        .js-anim .reveal {
            opacity: 0; transform: translateY(26px);
            transition: opacity .7s ease, transform .7s ease; transition-delay: var(--retraso, 0s);
        }
        .js-anim .reveal.visible { opacity: 1; transform: none; }

        @media (prefers-reduced-motion: no-preference) {
            .cat, .paso, .porque > div, .dos > div { transition: transform .25s ease, box-shadow .25s ease; }
            .cat:hover, .paso:hover, .porque > div:hover { transform: translateY(-4px); box-shadow: 0 10px 28px rgba(35, 65, 60, .10); }
            .i-btn i { transition: transform .2s ease; }
            .i-btn:hover i { transform: translateX(4px); }
            .hero-mas i { animation: rebote 1.8s ease-in-out infinite; }
        }
        @keyframes rebote { 0%, 100% { transform: translateY(0); } 50% { transform: translateY(4px); } }
    </style>

    <script>
        
        (function () {
            if (window.matchMedia('(prefers-reduced-motion: reduce)').matches) return;
            document.documentElement.classList.add('js-anim');
            
            setTimeout(function () {
                if (!window.inicioListo) document.documentElement.classList.remove('js-anim');
            }, 3000);
        })();
    </script>

</asp:Content>


<asp:Content ID="ContentMain" ContentPlaceHolderID="MainContent" runat="server">

    <div class="inicio">

        
        <section class="hero" aria-labelledby="t-hero">
            <div class="i-contenedor hero-grid">

                <div>
                    <div class="i-etiqueta"><i class="ti ti-leaf" aria-hidden="true"></i>Comunidad UTN FRGP</div>

                    <h1 id="t-hero">Lo que ya no usás, <span>otro estudiante lo necesita.</span></h1>

                    <p class="hero-descripcion">
                        PÁSALO es una plataforma para donar e intercambiar artículos entre estudiantes de la UTN FRGP.
                        Compartí lo que ya no necesitás, encontrá lo que estás buscando y ayudá a construir una
                        comunidad más solidaria y sustentable.
                    </p>

                    <div class="botones">
                        <% if (Logueado) { %>
                        <a href="Default.aspx" class="i-btn i-btn-principal">Ir al catálogo <i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        <a href="Publicacion.aspx" class="i-btn i-btn-sec">Publicar un artículo</a>
                        <% } else { %>
                        <a href="Registro.aspx" class="i-btn i-btn-principal">Registrarme gratis <i class="ti ti-arrow-right" aria-hidden="true"></i></a>
                        <a href="Login.aspx" class="i-btn i-btn-sec">Ya tengo cuenta</a>
                        <% } %>
                    </div>

                    <p class="hero-nota">
                        <i class="ti ti-shield-check" aria-hidden="true"></i>
                        Solo con correo institucional (@alumnos.frgp.utn.edu.ar)
                    </p>

                    <a href="#como-funciona" class="hero-mas">¿Cómo funciona? <i class="ti ti-arrow-down" aria-hidden="true"></i></a>
                </div>

                <div class="vista" aria-hidden="true">
                    <div class="vista-cabecera"><span>Objetos disponibles</span><i class="ti ti-search"></i></div>
                    <div class="vista-lista">
                        <div class="mini">
                            <span class="mini-foto"><i class="ti ti-book"></i></span>
                            <div>
                                <p class="mini-titulo">Cálculo de una variable</p>
                                <p class="mini-meta">Libros · Donación</p>
                                <span class="i-estado i-disp">Disponible</span>
                            </div>
                        </div>
                        <div class="mini">
                            <span class="mini-foto"><i class="ti ti-pencil"></i></span>
                            <div>
                                <p class="mini-titulo">Apuntes de Análisis Matemático I</p>
                                <p class="mini-meta">Apuntes · Intercambio</p>
                                <span class="i-estado i-disp">Disponible</span>
                            </div>
                        </div>
                        <div class="mini">
                            <span class="mini-foto"><i class="ti ti-calculator"></i></span>
                            <div>
                                <p class="mini-titulo">Calculadora científica</p>
                                <p class="mini-meta">Electrónicos · Donación</p>
                                <span class="i-estado i-res">Reservado</span>
                            </div>
                        </div>
                    </div>
                    <p class="vista-pie">Vista de ejemplo del catálogo</p>
                </div>

            </div>
        </section>


       
        <section class="i-bloque" aria-labelledby="t-compartir">
            <div class="i-contenedor">
                <h2 class="i-titulo" id="t-compartir">¿Qué podés compartir?</h2>
                <p class="i-subtitulo">Todo lo que a vos ya no te sirve y a otra persona sí puede servirle durante la cursada.</p>

                <div class="cats">
                    <div class="cat"><i class="ti ti-shirt" aria-hidden="true"></i><h3>Ropa</h3><p>Camperas, buzos, guardapolvos y más.</p></div>
                    <div class="cat"><i class="ti ti-book" aria-hidden="true"></i><h3>Libros</h3><p>Bibliografía y textos de cursada.</p></div>
                    <div class="cat"><i class="ti ti-pencil" aria-hidden="true"></i><h3>Apuntes</h3><p>Resúmenes, guías y ejercicios resueltos.</p></div>
                    <div class="cat"><i class="ti ti-device-laptop" aria-hidden="true"></i><h3>Electrónicos</h3><p>Calculadoras, cables, repuestos.</p></div>
                    <div class="cat"><i class="ti ti-box" aria-hidden="true"></i><h3>Otros</h3><p>Útiles, herramientas y todo lo que sume.</p></div>
                </div>
            </div>
        </section>


       
        <section class="i-bloque" id="como-funciona" aria-labelledby="t-como">
            <div class="i-contenedor">
                <h2 class="i-titulo" id="t-como">¿Cómo funciona PÁSALO?</h2>
                <p class="i-subtitulo">Compartir es sencillo. En cuatro pasos ya sos parte de la comunidad.</p>

                <div class="pasos">
                    <div class="paso">
                        <div class="paso-numero" aria-hidden="true">1</div>
                        <h3>Registrate</h3>
                        <p>Creá tu cuenta con tu correo institucional de la UTN FRGP.</p>
                    </div>
                    <div class="paso">
                        <div class="paso-numero" aria-hidden="true">2</div>
                        <h3>Publicá o pedí</h3>
                        <p>Publicá lo que querés donar o intercambiar, o pedí un objeto que te interese del catálogo.</p>
                    </div>
                    <div class="paso">
                        <div class="paso-numero" aria-hidden="true">3</div>
                        <h3>Coordinen</h3>
                        <p>El dueño revisa los pedidos y elige a quién dárselo. Ahí se habilita el chat para acordar el encuentro en el campus.</p>
                    </div>
                    <div class="paso">
                        <div class="paso-numero" aria-hidden="true">4</div>
                        <h3>Entregá y valorá</h3>
                        <p>Los dos confirman la entrega y el objeto pasa a "Entregado". Después podés valorar a la otra persona.</p>
                    </div>
                </div>

                <div class="dos">
                    <div>
                        <h3><i class="ti ti-gift" aria-hidden="true"></i>Donación</h3>
                        <p>Entregás un objeto sin pedir nada a cambio. Quien lo necesita lo solicita y vos elegís a quién dárselo.</p>
                    </div>
                    <div>
                        <h3><i class="ti ti-arrows-exchange" aria-hidden="true"></i>Intercambio</h3>
                        <p>Quien lo quiere te ofrece otro objeto a cambio. Vos decidís si la propuesta te sirve.</p>
                    </div>
                </div>
            </div>
        </section>


        
        <section class="i-bloque" aria-labelledby="t-porque">
            <div class="i-contenedor">
                <h2 class="i-titulo" id="t-porque">¿Por qué PÁSALO?</h2>
                <p class="i-subtitulo">Pensada por y para estudiantes, con cuidado en lo que más importa.</p>

                <div class="porque">
                    <div>
                        <i class="ti ti-school" aria-hidden="true"></i>
                        <div>
                            <h3>Solo estudiantes de la UTN FRGP</h3>
                            <p>El registro exige correo institucional, así que compartís con gente de tu propia comunidad.</p>
                        </div>
                    </div>
                    <div>
                        <i class="ti ti-shield-check" aria-hidden="true"></i>
                        <div>
                            <h3>Entregas con confianza</h3>
                            <p>Coordinan dentro del campus. Reputación entre usuarios<span class="proximamente">Próximamente</span>
                               y reporte de publicaciones inapropiadas<span class="proximamente">Próximamente</span>.</p>
                        </div>
                    </div>
                    <div>
                        <i class="ti ti-accessible" aria-hidden="true"></i>
                        <div>
                            <h3>Pensada para todas las personas</h3>
                            <p>Estados con color, forma y texto, y navegación completa con el teclado. Dictado de voz y lectura en voz alta del chat<span class="proximamente">Próximamente</span>.</p>
                        </div>
                    </div>
                    <div>
                        <i class="ti ti-recycle" aria-hidden="true"></i>
                        <div>
                            <h3>Un impacto que se puede ver</h3>
                            <p>Cada objeto entregado suma al cuidado del ambiente, y cada persona y cada facultad puede ver cuánto aporta.</p>
                        </div>
                    </div>
                </div>
            </div>
        </section>


        <%-- 5. IMPACTO AMBIENTAL (los números salen de la base; si todavía no hay entregas, no se muestran) --%>
        <section class="i-bloque seccion-impacto" aria-labelledby="t-impacto">
            <div class="i-contenedor">
                <h2 class="i-titulo" id="t-impacto">El impacto de compartir</h2>
                <p class="i-subtitulo">Cada objeto que vuelve a usarse es un objeto menos en la basura.</p>

                <ul class="tres-r" aria-label="Las tres R del consumo responsable: reducir, reutilizar y reciclar">
                    <li>Reducir</li>
                    <li class="tres-r-activa">Reutilizar<small>Acá está PÁSALO</small></li>
                    <li>Reciclar</li>
                </ul>

                <div class="imp-minis">
                    <div class="imp-mini">
                        <i class="ti ti-recycle" aria-hidden="true"></i>
                        <h3>Reutilizar vale más que reciclar</h3>
                        <p>Reciclar también gasta energía. Si un objeto entero todavía sirve, darle otra vida evita ese paso.</p>
                    </div>
                    <div class="imp-mini">
                        <i class="ti ti-droplet" aria-hidden="true"></i>
                        <h3>Fabricar algo nuevo cuesta recursos</h3>
                        <p>Cada objeto nuevo necesita materia prima, energía y agua. Usar uno que ya existe evita todo eso.</p>
                    </div>
                    <div class="imp-mini">
                        <i class="ti ti-trash" aria-hidden="true"></i>
                        <h3>Un residuo menos, entre todos</h3>
                        <p>Lo que se reutiliza no termina en la basura. Entre muchos estudiantes, esos objetos se suman.</p>
                    </div>
                </div>

                <% if (Comunidad != null) { %>
                <div class="contadores">
                    <div><strong><%= Comunidad.ObjetosEntregados %></strong><span>objetos entregados</span></div>
                    <div><strong><%= Kg(Comunidad.KgEvitados) %></strong><span>de residuos evitados (estimado)</span></div>
                    <div><strong><%= Comunidad.Estudiantes %></strong><span>estudiantes en la comunidad</span></div>
                </div>
                <% } else { %>
                <p class="impacto-vacio">Estamos arrancando: cada objeto que se entregue va a sumar acá su aporte.</p>
                <% } %>

                <p class="impacto-nota">
                    Los kilos son una estimación: cada tipo de objeto tiene un peso aproximado, y se suma uno por cada entrega confirmada. No son mediciones exactas.
                </p>
            </div>
        </section>


        <%-- 6. PREGUNTAS FRECUENTES --%>
        <section class="i-bloque" id="preguntas" aria-labelledby="t-faq">
            <div class="i-contenedor">
                <h2 class="i-titulo" id="t-faq">Preguntas frecuentes</h2>
                <p class="i-subtitulo">Lo que más se pregunta antes de empezar.</p>

                <div class="faq">
                    <details>
                        <summary>¿Cuesta algo usar PÁSALO?</summary>
                        <p>No. PÁSALO es gratuito y no se vende nada: los objetos se donan o se intercambian.</p>
                    </details>
                    <details>
                        <summary>¿Puedo usarla si no soy de la UTN FRGP?</summary>
                        <p>Por ahora el registro requiere un correo institucional @alumnos.frgp.utn.edu.ar.</p>
                    </details>
                    <details>
                        <summary>¿Qué pasa cuando alguien pide mi objeto?</summary>
                        <p>Vas a ver todos los pedidos que recibió tu publicación y elegís uno. Al aceptarlo, el objeto pasa a "Reservado", los demás pedidos se rechazan solos y se habilita el chat con esa persona.</p>
                    </details>
                    <details>
                        <summary>¿Dónde y cómo nos encontramos?</summary>
                        <p>Lo acuerdan por el chat de la publicación. Recomendamos hacerlo siempre dentro del campus, de día y en lugares comunes como la biblioteca o la cafetería.</p>
                    </details>
                    <details>
                        <summary>¿Qué pasa si la otra persona no se presenta?</summary>
                        <p><span class="completar">[COMPLETAR: definir qué pasa si la otra persona no se presenta, por ejemplo cancelar la reserva para que el objeto vuelva a estar disponible]</span></p>
                    </details>
                    <details>
                        <summary>¿Qué no se puede publicar?</summary>
                        <p><span class="completar">[COMPLETAR: lista de objetos que no se pueden publicar, según lo que decida el equipo]</span></p>
                    </details>
                </div>
            </div>
        </section>


        <%-- 7. SOBRE NOSOTROS --%>
        <section class="nosotros" id="sobre-nosotros" aria-labelledby="t-nosotros">
            <div class="nosotros-contenido">
                <div class="i-etiqueta"><i class="ti ti-heart-handshake" aria-hidden="true"></i>Sobre nosotros</div>
                <h2 id="t-nosotros">Una comunidad que comparte</h2>
                <p>
                    PÁSALO nace como un proyecto universitario desarrollado por estudiantes de la Tecnicatura
                    Universitaria en Programación de la UTN FRGP.
                </p>
                <p>
                    Nuestro objetivo es facilitar el intercambio y la donación de artículos entre compañeros,
                    promoviendo el consumo responsable, la reutilización de recursos y la colaboración dentro
                    de la comunidad educativa.
                </p>
                <p><span class="completar">[COMPLETAR: materia, año y docentes del proyecto]</span></p>
                <p><span class="completar">[COMPLETAR: integrantes del equipo]</span></p>
            </div>
        </section>


        <%-- CIERRE --%>
        <section class="cta" aria-labelledby="t-cta">
            <h2 id="t-cta">¿Listo para empezar a compartir?</h2>
            <p>Sumate a PÁSALO y dale una segunda oportunidad a los objetos que ya no utilizás.</p>
            <% if (Logueado) { %>
            <a href="Publicacion.aspx" class="i-btn">Publicar un artículo <i class="ti ti-arrow-right" aria-hidden="true"></i></a>
            <% } else { %>
            <a href="Registro.aspx" class="i-btn">Crear mi cuenta <i class="ti ti-arrow-right" aria-hidden="true"></i></a>
            <% } %>
        </section>


        <%-- PIE DE PÁGINA --%>
        <footer class="pie">
            <div class="i-contenedor">
                <div class="pie-grid">
                    <div>
                        <h3>PÁSALO</h3>
                        <p>Proyecto universitario de la Tecnicatura Universitaria en Programación, UTN FRGP.</p>
                    </div>
                    <div>
                        <h3>Explorar</h3>
                        <ul>
                            <li><a href="#como-funciona">¿Cómo funciona?</a></li>
                            <li><a href="#preguntas">Preguntas frecuentes</a></li>
                            <li><a href="#sobre-nosotros">Sobre nosotros</a></li>
                            <% if (!Logueado) { %>
                            <li><a href="Login.aspx">Iniciar sesión</a></li>
                            <li><a href="Registro.aspx">Registrarme</a></li>
                            <% } %>
                        </ul>
                    </div>
                    <div>
                        <h3>Contacto</h3>
                        <p><span class="completar">[COMPLETAR: correo o canal de contacto]</span></p>
                    </div>
                </div>
                <p class="pie-copia">PÁSALO · Comunidad universitaria UTN FRGP</p>
            </div>
        </footer>

    </div>

    <script>
        (function () {
            var animar = document.documentElement.classList.contains('js-anim');

            var barra = document.querySelector('.barra');
            if (barra) {
                var ultimo = window.pageYOffset;
                var pendiente = false;
                var revisar = function () {
                    var actual = window.pageYOffset;
                    var delta = actual - ultimo;
                    if (actual < 10) { barra.classList.remove('barra--oculta'); ultimo = actual; }
                    else if (delta > 6 && actual > 120) { barra.classList.add('barra--oculta'); ultimo = actual; }
                    else if (delta < -6) { barra.classList.remove('barra--oculta'); ultimo = actual; }
                    pendiente = false;
                };
                window.addEventListener('scroll', function () {
                    if (!pendiente) { pendiente = true; requestAnimationFrame(revisar); }
                }, { passive: true });
            
                barra.addEventListener('focusin', function () { barra.classList.remove('barra--oculta'); });
            }

            if (!animar) { window.inicioListo = true; return; }

            
            function contar(el) {
                var original = el.textContent;
                var m = original.match(/^([\d.,]+)(.*)$/);
                if (!m) return;
                var decimales = m[1].indexOf(',') >= 0 ? m[1].split(',')[1].length : 0;
                var meta = parseFloat(m[1].replace(',', '.'));
                if (isNaN(meta)) return;
                var inicio = null;
                function paso(t) {
                    if (inicio === null) inicio = t;
                    var p = Math.min((t - inicio) / 1200, 1);
                    var f = 1 - Math.pow(1 - p, 3);
                    el.textContent = (meta * f).toFixed(decimales).replace('.', ',') + m[2];
                    if (p < 1) requestAnimationFrame(paso); else el.textContent = original;
                }
                requestAnimationFrame(paso);
            }

            
            var objetivos = document.querySelectorAll(
                '.hero .i-etiqueta, .hero h1, .hero-descripcion, .botones, .hero-nota, .hero-mas, .vista, .mini, ' +
                '.i-titulo, .i-subtitulo, .cat, .paso, .dos > div, .porque > div, .contadores > div, ' +
                '.tres-r > li, .imp-mini, .impacto-vacio, .impacto-nota, .faq details, .nosotros-contenido, .cta');

            objetivos.forEach(function (el) {
                var pos = Array.prototype.indexOf.call(el.parentNode.children, el);
                el.style.setProperty('--retraso', (Math.min(pos, 5) * 0.09) + 's');
                el.classList.add('reveal');
            });

            if (!('IntersectionObserver' in window)) {
                objetivos.forEach(function (el) { el.classList.add('visible'); });
                window.inicioListo = true;
                return;
            }

            var observador = new IntersectionObserver(function (entradas) {
                entradas.forEach(function (en) {
                    if (!en.isIntersecting) return;
                    var el = en.target;
                    el.classList.add('visible');
                    observador.unobserve(el);

                    el.addEventListener('transitionend', function limpiar(ev) {
                        if (ev.propertyName !== 'transform') return;
                        el.classList.remove('reveal', 'visible');
                        el.style.removeProperty('--retraso');
                        el.removeEventListener('transitionend', limpiar);
                    });

                    if (el.parentNode.classList.contains('contadores')) {
                        var numero = el.querySelector('strong');
                        if (numero) contar(numero);
                    }
                });
            }, { threshold: 0.15, rootMargin: '0px 0px -40px 0px' });

            objetivos.forEach(function (el) { observador.observe(el); });
            window.inicioListo = true;
        })();
    </script>

</asp:Content>
