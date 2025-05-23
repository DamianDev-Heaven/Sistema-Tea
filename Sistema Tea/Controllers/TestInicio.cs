using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Sistema_Tea.Models;

namespace Sistema_Tea.Controllers
{
    public class TestInicio : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult test()
        {
            var preguntas = ObtenerPreguntas();
            return View(preguntas);
        }

        [HttpPost]
        public IActionResult Resultado(List<int> respuestas)
        {

            int puntajeTotal = respuestas.Sum();
            string resultado = puntajeTotal >= 45 ? "Probabilidad Alta": (puntajeTotal >= 30 ? "Probabilidad Media" : "Probabilidad Baja");
            ViewBag.Puntaje = puntajeTotal;
            ViewBag.Resultado = resultado;
            return View();
        }
        private List<TestInicial> ObtenerPreguntas()
        {
            var opciones = new List<string> { "Nunca", "A veces", "Siempre" };

            return new List<TestInicial>
            {
                new TestInicial { Id = 1, Texto = "Prefiero realizar actividades por mi cuenta en lugar de con otros.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 2, Texto = "Me resulta difícil entender las expresiones faciales de otras personas.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 3, Texto = "Me siento incómodo/a en situaciones sociales.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 4, Texto = "Prefiero seguir rutinas y me siento incómodo/a cuando cambian.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 5, Texto = "Me cuesta entender las bromas o el sarcasmo.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 6, Texto = "Me fijo en detalles que otros suelen pasar por alto.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 7, Texto = "Encuentro difícil iniciar o mantener una conversación.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 8, Texto = "Prefiero dedicarme a temas o intereses específicos y profundizar en ellos.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 9, Texto = "A menudo no sé cómo actuar en situaciones sociales.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 10, Texto = "Me siento abrumado/a en entornos con mucho ruido o luz.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 11, Texto = "Me cuesta entender las intenciones o emociones de otras personas.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 12, Texto = "Prefiero planificar mis actividades con antelación y evito la espontaneidad.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 13, Texto = "Me resulta difícil interpretar las normas sociales no dichas.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 14, Texto = "Disfruto de actividades repetitivas y encuentro confort en ellas.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 15, Texto = "Me siento nervioso/a en situaciones sociales, incluso con personas que conozco bien.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 16, Texto = "A menudo me siento agotado/a después de interactuar socialmente.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 17, Texto = "Tengo dificultades para entender el lenguaje corporal de otros.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 18, Texto = "A menudo, prefiero leer o investigar sobre temas que me interesan más que salir.", Opciones = opciones, PuntajeSiempre = 3 },
                new TestInicial { Id = 19, Texto = "Me resulta difícil mirar a los ojos a las personas durante una conversación.", Opciones = opciones, PuntajeSiempre = 3 }
            };
        }

    }
}
