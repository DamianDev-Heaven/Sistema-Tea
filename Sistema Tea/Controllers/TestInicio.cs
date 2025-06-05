using Microsoft.AspNetCore.Mvc;
using Sistema_Tea.Models;
using System.Collections.Generic;
using System.Linq;

namespace Sistema_Tea.Controllers
{
    public class TestInicioController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "Bienvenido al Test Preliminar TEA";
            return View();
        }

        public IActionResult ElegirEdad()
        {
            ViewData["Title"] = "Seleccionar Grupo de Edad";
            return View();
        }

        public IActionResult Test(string grupoEdad)
        {
            if (string.IsNullOrEmpty(grupoEdad))
            {
                return RedirectToAction("ElegirEdad");
            }

            var preguntas = ObtenerPreguntasPorGrupo(grupoEdad);

            if (preguntas == null || !preguntas.Any())
            {
                TempData["ErrorTest"] = "No se pudo cargar el cuestionario para el grupo de edad seleccionado.";
                return RedirectToAction("ElegirEdad");
            }

            ViewBag.CuestionarioTitulo = ObtenerTituloCuestionario(grupoEdad);
            TempData["GrupoEdadActual"] = grupoEdad;

            return View("Test", preguntas);
        }

        [HttpPost]
        public IActionResult Resultado(List<int> respuestas)
        {
            if (respuestas == null || !respuestas.Any())
            {
                TempData["ErrorResultado"] = "No se recibieron respuestas válidas.";
                return RedirectToAction("ElegirEdad");
            }

            int puntajeTotal = respuestas.Sum();
            string grupoEdadActual = TempData["GrupoEdadActual"] as string;
            TempData.Keep("GrupoEdadActual");

            if (string.IsNullOrEmpty(grupoEdadActual))
            {
                TempData["ErrorResultado"] = "No se pudo determinar el cuestionario evaluado.";
                return RedirectToAction("ElegirEdad");
            }

            var (resultadoTextoProbabilidad, interpretacionDetallada, claseAlerta) = EvaluarResultado(puntajeTotal, grupoEdadActual);

            ViewBag.Puntaje = puntajeTotal;
            ViewBag.Resultado = resultadoTextoProbabilidad;
            ViewBag.InterpretacionDetallada = interpretacionDetallada;
            ViewBag.ClaseAlertaBootstrap = claseAlerta;
            ViewBag.CuestionarioAplicado = ObtenerTituloCuestionario(grupoEdadActual);

            return View("Resultado");
        }

        private string ObtenerTituloCuestionario(string grupoEdad)
        {
            switch (grupoEdad?.ToLower())
            {
                case "ninospequenos": return "Cuestionario para Niños Pequeños (18-35 meses) - Reporte de Padres (Adaptado del M-CHAT-R/F™)";
                case "ninosescolares": return "Cuestionario para Niños Escolares (4-11 años) - Reporte de Padres (Inspirado en CAST)";
                case "adolescentesadultos": return "Cuestionario para Adolescentes y Adultos (16+ años) - Autoevaluación (Inspirado en AQ)";
                default: return "Test de Detección Preliminar TEA";
            }
        }

        private (string resultadoProbabilidad, string interpretacion, string claseAlerta) EvaluarResultado(int puntajeTotal, string grupoEdad)
        {
            string resultadoTextoProbabilidad, interpretacionDetallada, claseAlertaBs;

            switch (grupoEdad?.ToLower())
            {
                case "ninospequenos":
                    if (puntajeTotal >= 8)
                    {
                        resultadoTextoProbabilidad = "Alto Riesgo";
                        interpretacionDetallada = "El puntaje obtenido (<strong>" + puntajeTotal + "</strong>) sugiere una alta probabilidad de que su hijo/a pueda beneficiarse de una evaluación más profunda para el Trastorno del Espectro Autista. Se recomienda encarecidamente buscar una evaluación diagnóstica completa por un especialista en desarrollo infantil o TEA lo antes posible.";
                        claseAlertaBs = "danger";
                    }
                    else if (puntajeTotal >= 3)
                    {
                        resultadoTextoProbabilidad = "Riesgo Medio";
                        interpretacionDetallada = "El puntaje obtenido (<strong>" + puntajeTotal + "</strong>) sugiere un riesgo moderado. Esto indica la presencia de algunas señales de alerta. Es importante discutir estos resultados con su pediatra y considerar una evaluación de seguimiento o más detallada, especialmente si su hijo/a tiene menos de 24 meses. El M-CHAT-R/F original recomienda una entrevista de seguimiento (Follow-Up) para este rango de puntaje.";
                        claseAlertaBs = "warning";
                    }
                    else
                    { 
                        resultadoTextoProbabilidad = "Bajo Riesgo";
                        interpretacionDetallada = "El puntaje obtenido (<strong>" + puntajeTotal + "</strong>) es bajo. Si no tiene otras preocupaciones, continúe con los controles pediátricos habituales y siga observando el desarrollo de su hijo/a. No obstante, si algo le sigue preocupando, no dude en comentarlo con su pediatra.";
                        claseAlertaBs = "success";
                    }
                    break;

                case "ninosescolares": 
                    if (puntajeTotal >= 15)
                    {
                        resultadoTextoProbabilidad = "Probabilidad Alta"; 
                        interpretacionDetallada = "El puntaje obtenido (<strong>" + puntajeTotal + "</strong>) sugiere la presencia de varios indicadores asociados con el espectro autista. Se recomienda una evaluación completa por parte de un profesional especializado (psicólogo infantil, neuropediatra, psiquiatra infantil) para una valoración diagnóstica.";
                        claseAlertaBs = "danger";
                    }
                    else if (puntajeTotal >= 10)
                    {
                        resultadoTextoProbabilidad = "Probabilidad Media";
                        interpretacionDetallada = "El puntaje obtenido (<strong>" + puntajeTotal + "</strong>) indica la presencia de algunos rasgos que merecen atención. Sería útil monitorear el desarrollo de su hijo/a y discutir estas observaciones con un profesional, especialmente si estas características interfieren con su vida escolar, social o familiar.";
                        claseAlertaBs = "warning";
                    }
                    else
                    {
                        resultadoTextoProbabilidad = "Probabilidad Baja";
                        interpretacionDetallada = "Según este cuestionario, el puntaje obtenido (<strong>" + puntajeTotal + "</strong>) es bajo en indicadores asociados con el espectro autista. Continúe observando el desarrollo de su hijo/a y consulte con un profesional si surgen nuevas preocupaciones o si las existentes persisten.";
                        claseAlertaBs = "success";
                    }
                    break;

                case "adolescentesadultos": 
                    if (puntajeTotal >= 16)
                    {
                        resultadoTextoProbabilidad = "Probabilidad Alta";
                        interpretacionDetallada = "Tus respuestas (puntaje: <strong>" + puntajeTotal + "</strong>) sugieren una alta probabilidad de que presentes un número significativo de rasgos asociados con el espectro autista. Este resultado NO es un diagnóstico, pero es una indicación clara para buscar una evaluación diagnóstica formal con un profesional especializado en TEA en adultos/adolescentes.";
                        claseAlertaBs = "danger";
                    }
                    else if (puntajeTotal >= 10)
                    {
                        resultadoTextoProbabilidad = "Probabilidad Media";
                        interpretacionDetallada = "Tus respuestas (puntaje: <strong>" + puntajeTotal + "</strong>) sugieren una probabilidad media de que presentes algunos rasgos asociados con el espectro autista. Podría ser beneficioso explorar esto más a fondo con un profesional para entender mejor tus características y determinar si una evaluación formal sería útil.";
                        claseAlertaBs = "warning";
                    }
                    else
                    {
                        resultadoTextoProbabilidad = "Probabilidad Baja";
                        interpretacionDetallada = "Tus respuestas (puntaje: <strong>" + puntajeTotal + "</strong>) sugieren una baja probabilidad de que presentes un número significativo de rasgos del espectro autista según este cuestionario. Si tienes dudas o dificultades específicas que afectan tu bienestar, considera hablar con un profesional de la salud mental.";
                        claseAlertaBs = "success";
                    }
                    break;
                default:
                    resultadoTextoProbabilidad = "Indeterminado";
                    interpretacionDetallada = "No se pudo determinar el resultado. Por favor, seleccione un grupo de edad válido.";
                    claseAlertaBs = "secondary";
                    break;
            }
            return (resultadoTextoProbabilidad, interpretacionDetallada, claseAlertaBs);
        }

        private List<TestInicial> ObtenerPreguntasPorGrupo(string grupoEdad)
        {
            switch (grupoEdad?.ToLower())
            {
                case "ninospequenos":
                    return ObtenerPreguntasNinosPequenos_MCHATRF_Completo();
                case "ninosescolares":
                    return ObtenerPreguntasNinosEscolares_AdaptadoCAST();
                case "adolescentesadultos":
                    return ObtenerPreguntasAdolescentesAdultos_AdaptadoAQ();
                default:
                    return new List<TestInicial>();
            }
        }

        // --- CONJUNTOS DE PREGUNTAS COMPLETOS Y REALISTAS ---

        private List<TestInicial> ObtenerPreguntasNinosPequenos_MCHATRF_Completo()
        {
            // M-CHAT-R/F™ (Robins, Fein, & Barton, 2009) - 20 ítems estándar.
            // Rango de edad típico: 16-30 meses. Extensible hasta 36-47 meses con cautela.
            // Puntuación: 1 punto por cada respuesta que indica riesgo (según la clave del M-CHAT-R/F).
            var opcionesSiNo = new List<string> { "Sí", "No" };
            return new List<TestInicial>
            {
                new TestInicial { Id = 1, Texto = "Si usted señala algo al otro lado del cuarto, ¿su hijo/a lo mira?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}}, AyudaAdicional = "Ej: si señala un juguete o un animal." },
                new TestInicial { Id = 2, Texto = "¿Alguna vez se ha preguntado si su hijo/a podría ser sordo/a?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 3, Texto = "¿Su hijo/a juega a simular o “hacer como si” algo ocurriera?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}}, AyudaAdicional = "Ej: cuidar muñecos, hablar por teléfono de juguete." },
                new TestInicial { Id = 4, Texto = "¿A su hijo/a le gusta subirse a cosas?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}}, AyudaAdicional = "Ej: muebles, juegos en el parque, escaleras." },
                new TestInicial { Id = 5, Texto = "¿Su hijo/a hace movimientos inusuales con los dedos cerca de sus ojos?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}}, AyudaAdicional = "Ej: mover los dedos como si mirara a través de ellos." },
                new TestInicial { Id = 6, Texto = "¿Su hijo/a alguna vez le lleva objetos para pedirle ayuda tocando su mano o llevándoselo?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}}, AyudaAdicional="Ej: un juguete que no puede hacer funcionar." },
                new TestInicial { Id = 7, Texto = "¿Su hijo/a alguna vez le lleva objetos simplemente para mostrárselos (NO para pedir ayuda)?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}}, AyudaAdicional="Ej: le muestra una flor, un peluche." },
                new TestInicial { Id = 8, Texto = "¿Su hijo/a le mira a los ojos durante más de uno o dos segundos cuando usted le habla o juega con él/ella?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 9, Texto = "¿Parece su hijo/a ser demasiado sensible a los ruidos?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}}, AyudaAdicional="Ej: se sobresalta o tapa los oídos ante ruidos cotidianos." },
                new TestInicial { Id = 10, Texto = "¿Su hijo/a le sonríe o ríe en respuesta a su sonrisa o a su cara cuando usted se acerca?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 11, Texto = "¿Su hijo/a lo imita?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}}, AyudaAdicional="Ej: si usted hace una mueca, ¿la imita?" },
                new TestInicial { Id = 12, Texto = "¿Su hijo/a responde cuando usted lo/la llama por su nombre?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 13, Texto = "Si usted gira la cabeza para mirar algo, ¿su hijo/a mira alrededor para ver qué es lo que usted está mirando?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 14, Texto = "¿Su hijo/a camina solo/a sin ayuda?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 15, Texto = "¿Su hijo/a mira las cosas que usted está mirando y señalando?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 16, Texto = "¿Su hijo/a hace movimientos corporales repetitivos como mecerse, aletear las manos o girar sobre sí mismo/a?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 17, Texto = "¿Su hijo/a muestra interés en otros niños de su edad (los mira, les sonríe, intenta acercarse)?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 18, Texto = "¿Su hijo/a le muestra cosas señalando con el dedo índice (no solo alcanzando el objeto)?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 19, Texto = "¿Su hijo/a alguna vez ha usado su dedo índice para señalar y pedir algo que quiere?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} },
                new TestInicial { Id = 20, Texto = "¿Su hijo/a responde a las expresiones emocionales de su cara?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}}, AyudaAdicional="Ej: si usted parece triste, ¿su hijo/a muestra alguna reacción?" }
            };
        }

        private List<TestInicial> ObtenerPreguntasNinosEscolares_AdaptadoCAST()
        {
            // Inspirado en el CAST (Childhood Autism Spectrum Test - Scott, Baron-Cohen, Bolton, & Brayne, 2002).
            // El CAST original tiene 37 ítems de respuesta Sí/No. Un "Sí" indica la presencia del rasgo.
            // Aquí adaptaremos 30 ítems. Puntuación: 1 por cada "Sí" que indica rasgo.
            var opcionesSiNo = new List<string> { "Sí", "No" };
            // Para CAST, la mayoría de las preguntas son "Sí" = 1 (indica rasgo).
            // Algunas preguntas pueden ser sobre ausencia de una habilidad, donde "No" sería el riesgo.
            // Debemos ser cuidadosos. El CAST original está diseñado para que una suma de "Sí" sea el puntaje.

            return new List<TestInicial>
            {
                // Sección A: Habilidades Sociales y de Comunicación (Adaptado)
                new TestInicial { Id = 1, Texto = "¿Le cuesta a su hijo/a entender por qué la gente se enfada o se pone triste?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 2, Texto = "¿Su hijo/a prefiere jugar solo en lugar de con otros niños la mayor parte del tiempo?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 3, Texto = "¿Tiene su hijo/a dificultades para saber cómo unirse a un juego o actividad con otros niños?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 4, Texto = "¿Su hijo/a parece no darse cuenta cuando otros niños no quieren jugar con él/ella o están aburridos?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 5, Texto = "¿Le resulta difícil a su hijo/a mantener una conversación normal de ida y vuelta?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 6, Texto = "¿Su hijo/a toma las cosas de forma muy literal (ej. no entiende bien las bromas, el sarcasmo o las metáforas)?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 7, Texto = "¿Su hijo/a habla mucho sobre un tema de su interés sin darse cuenta si la otra persona está interesada?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 8, Texto = "¿Utiliza su hijo/a un tono de voz inusual o un patrón de habla peculiar?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 9, Texto = "¿Su hijo/a tiene pocas expresiones faciales o su expresión facial a menudo no coincide con lo que dice o siente?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 10, Texto = "¿Le cuesta a su hijo/a hacer amigos de su edad?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 11, Texto = "¿Su hijo/a parece tener poca conciencia de las normas sociales o de lo que es apropiado en diferentes situaciones?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 12, Texto = "¿Evita su hijo/a el contacto visual o lo usa de manera limitada o inusual?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 13, Texto = "¿Le cuesta a su hijo/a entender las perspectivas o puntos de vista de otras personas?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 14, Texto = "¿Su hijo/a prefiere la compañía de adultos o niños mucho menores en lugar de la de sus compañeros?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 15, Texto = "¿Tiene su hijo/a un mejor amigo/a con quien comparta intereses y actividades de forma recíproca?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 0}, {"No", 1}} }, // Riesgo si No

                // Sección B: Intereses y Comportamientos (Adaptado)
                new TestInicial { Id = 16, Texto = "¿Tiene su hijo/a un interés muy fuerte y absorbente en un tema o actividad muy específica?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 17, Texto = "¿Su hijo/a se molesta mucho con los cambios en la rutina o si las cosas no se hacen de una manera determinada?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 18, Texto = "¿Su hijo/a tiene rituales o rutinas que insiste en seguir exactamente igual cada vez?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 19, Texto = "¿Realiza su hijo/a movimientos repetitivos con el cuerpo, como aletear las manos, balancearse o girar?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 20, Texto = "¿Su hijo/a se interesa de forma inusual por partes de objetos (ej. ruedas de coches) o por aspectos no funcionales de los mismos?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 21, Texto = "¿Tiene su hijo/a un apego fuerte a objetos inusuales (que no sean peluches o mantas de seguridad típicas)?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 22, Texto = "¿El juego de su hijo/a tiende a ser repetitivo y poco imaginativo o creativo?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 23, Texto = "¿Muestra su hijo/a reacciones fuertes o inusuales a ciertos sonidos, luces, texturas, olores o sabores?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 24, Texto = "¿Su hijo/a parece insensible al dolor o a las temperaturas de forma inusual?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 25, Texto = "¿Tiene su hijo/a una excelente memoria para detalles o hechos específicos, especialmente en sus áreas de interés?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} }, // Característica común
                new TestInicial { Id = 26, Texto = "¿Se siente su hijo/a abrumado/a o ansioso/a en lugares concurridos o ruidosos?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 27, Texto = "¿Su hijo/a se organiza o alinea objetos de una manera particular y se molesta si se cambian?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 28, Texto = "¿Le resulta difícil a su hijo/a entender o seguir reglas complejas o que tienen excepciones?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} },
                new TestInicial { Id = 29, Texto = "¿Presenta su hijo/a torpeza motora o dificultades inusuales con la coordinación física?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} }, // Característica a veces asociada
                new TestInicial { Id = 30, Texto = "¿Le han comentado en la escuela que su hijo/a tiene dificultades para interactuar con sus compañeros o seguir las dinámicas de clase?", Opciones = opcionesSiNo, ScoresPorOpcion = new Dictionary<string, int>{{"Sí", 1}, {"No", 0}} }
            };
        }

        private List<TestInicial> ObtenerPreguntasAdolescentesAdultos_AdaptadoAQ()
        {
            // Adaptado del Autism Spectrum Quotient (AQ) de Baron-Cohen et al. (2001).
            // 25 ítems. Opciones de 4 puntos.
            // Puntuación: 1 punto para la respuesta que indica el rasgo TEA (ya sea acuerdo o desacuerdo según el ítem).
            var opcionesAQ = new List<string> { "Totalmente de acuerdo", "De acuerdo", "En desacuerdo", "Totalmente en desacuerdo" };

            return new List<TestInicial>
            {
                new TestInicial { Id = 1, Texto = "Prefiero realizar actividades por mi cuenta que con otras personas.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 2, Texto = "Me resulta fácil hacer nuevos amigos.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 0}, {"De acuerdo", 0}, {"En desacuerdo", 1}, {"Totalmente en desacuerdo", 1}} },
                new TestInicial { Id = 3, Texto = "A menudo me siento incómodo/a o ansioso/a en situaciones sociales.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 4, Texto = "Disfruto de las conversaciones triviales (charlas sobre el tiempo, etc.) y me resultan fáciles.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 0}, {"De acuerdo", 0}, {"En desacuerdo", 1}, {"Totalmente en desacuerdo", 1}} },
                new TestInicial { Id = 5, Texto = "Cuando hablo con la gente, me cuesta saber cuándo es mi turno para hablar o cuándo he hablado demasiado.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 6, Texto = "Me resulta difícil entender las expresiones faciales, el tono de voz o el lenguaje corporal de otras personas.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 7, Texto = "Otras personas a veces me dicen que soy demasiado directo/a, o que mi forma de hablar es muy formal o peculiar.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 8, Texto = "Me cuesta entender las bromas, el sarcasmo, la ironía o los dobles sentidos.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 9, Texto = "Me resulta fácil 'leer entre líneas' o comprender lo que la gente realmente quiere decir aunque no lo exprese directamente.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 0}, {"De acuerdo", 0}, {"En desacuerdo", 1}, {"Totalmente en desacuerdo", 1}} },
                new TestInicial { Id = 10, Texto = "A menudo evito el contacto visual o me siento incómodo/a manteniéndolo.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 11, Texto = "Cuando leo ficción o veo películas, me cuesta mucho imaginarme cómo son los personajes o las escenas descritas.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 12, Texto = "Me siento más cómodo/a con información basada en hechos y lógica que con ideas abstractas, sentimientos o teorías.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 13, Texto = "Me resulta difícil generar ideas nuevas o considerar múltiples soluciones a un problema.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 14, Texto = "Me gusta que las cosas sean claras, ordenadas y predecibles; la ambigüedad o la incertidumbre me generan malestar.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 15, Texto = "Suelo notar pequeños detalles, patrones o errores que otros pasan por alto.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 16, Texto = "Tengo intereses muy intensos sobre temas específicos, y puedo dedicarles mucho tiempo y aprender muchos detalles sobre ellos.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 17, Texto = "Me molesta mucho que me interrumpan cuando estoy profundamente concentrado/a en algo que me interesa.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 18, Texto = "Disfruto coleccionando y organizando información o objetos relacionados con mis intereses.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 19, Texto = "Me concentro más en las partes individuales o detalles de una situación que en la visión general o el contexto completo.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 20, Texto = "Prefiero seguir una rutina diaria establecida y me siento ansioso/a o molesto/a si mis planes o rutinas se alteran inesperadamente.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 21, Texto = "Soy particularmente sensible a ciertos estímulos sensoriales (ruidos fuertes, luces brillantes, olores intensos, texturas de ropa, etc.) que a otras personas no parecen molestarles.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 22, Texto = "Me gusta planificar las cosas con mucha antelación y no me siento cómodo/a con la espontaneidad o los imprevistos.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 23, Texto = "Realizo ciertos movimientos o acciones repetitivas (ej. balancearme, jugar con un objeto, chasquear los dedos) para calmarme o concentrarme, especialmente cuando estoy estresado/a o ansioso/a.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 24, Texto = "Me siento fácilmente abrumado/a en entornos con demasiada gente o muchos estímulos ocurriendo a la vez.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} },
                new TestInicial { Id = 25, Texto = "Si algo me interesa mucho, puedo hablar de ello durante horas sin cansarme.", Opciones = opcionesAQ, ScoresPorOpcion = new Dictionary<string, int>{{"Totalmente de acuerdo", 1}, {"De acuerdo", 1}, {"En desacuerdo", 0}, {"Totalmente en desacuerdo", 0}} }
            };
        }
    }
}