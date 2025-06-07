using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;

namespace Sistema_Tea.Controllers
{
    public class AdirController : Controller
    {
        private readonly TeaContext _context;
        public AdirController(TeaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetInt32("UsuarioID");
            if (id == null)
                return RedirectToAction("Login", "Cuenta");

            var sesionesAdir = await _context.ADIR_Sesion
                .Where(s => s.PsicologoID == id)
                .Include(s => s.Paciente)
                .OrderByDescending(s => s.FechaInicio)
                .ToListAsync();

            ViewBag.NombrePsicologo = _context.Usuario.Find(id)?.NombreCompleto;
            return View(sesionesAdir);
        }

        [HttpGet]
        public async Task<IActionResult> CrearSesion()
        {
            var psicologoId = HttpContext.Session.GetInt32("UsuarioID");
            if (psicologoId == null)
                return RedirectToAction("Login", "Cuenta");

            var pacientesAsignados = await (
                from ap in _context.AsignacionPaciente
                join p in _context.Paciente on ap.PacienteID equals p.PacienteID
                where ap.PsicologoID == psicologoId && ap.Estado == "Asignado"
                select new
                {
                    p.PacienteID,
                    NombreCompleto = p.Nombre + " " + p.Apellidos,
                    TutorNombre = p.Tutor
                }
            ).ToListAsync();

            ViewBag.Pacientes = pacientesAsignados.Select(p => new SelectListItem
            {
                Value = p.PacienteID.ToString(),
                Text = p.NombreCompleto
            }).ToList();

            ViewBag.PacientesDatosJson = Newtonsoft.Json.JsonConvert.SerializeObject(pacientesAsignados);

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearSesion(ADIR_Sesion sesion, string NuevoPacienteNombre, string PsicologoNombre)
        {
            var psicologoId = HttpContext.Session.GetInt32("UsuarioID");
            if (psicologoId == null)
                return RedirectToAction("Login", "Cuenta");

            sesion.PsicologoID = psicologoId.Value;

            if (!string.IsNullOrWhiteSpace(NuevoPacienteNombre))
            {
                var nombres = NuevoPacienteNombre.Split(' ', 2);
                var nuevoPaciente = new Paciente
                {
                    Nombre = nombres.Length > 0 ? nombres[0] : NuevoPacienteNombre,
                    Apellidos = nombres.Length > 1 ? nombres[1] : "",

                };

                _context.Paciente.Add(nuevoPaciente);
                await _context.SaveChangesAsync();

                sesion.PacienteID = nuevoPaciente.PacienteID;
            }
            else if (sesion.PacienteID == 0)
            {
                ModelState.AddModelError("PacienteID", "Debes seleccionar un paciente o ingresar uno nuevo.");
            }

            sesion.FechaInicio = DateTime.Now;
            sesion.Estado = "Pendiente";

            if (ModelState.IsValid)
            {
                _context.ADIR_Sesion.Add(sesion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var pacientesAsignados = await (
                from ap in _context.AsignacionPaciente
                join p in _context.Paciente on ap.PacienteID equals p.PacienteID
                where ap.PsicologoID == psicologoId && ap.Estado == "Asignado"
                select p
            ).ToListAsync();

            ViewBag.Pacientes = pacientesAsignados.Select(p => new SelectListItem
            {
                Value = p.PacienteID.ToString(),
                Text = p.Nombre + " " + p.Apellidos
            }).ToList();

            ViewBag.NombrePsicologo = PsicologoNombre;

            return View(sesion);
        }


        [HttpPost]
        public async Task<IActionResult> IniciarEvaluacion(int sesionId)
        {
            var sesion = await _context.ADIR_Sesion.FindAsync(sesionId);
            if (sesion == null || sesion.Estado != "Pendiente")
                return NotFound();

            sesion.Estado = "EnProgreso";
            sesion.FechaInicio = DateTime.Now;
            await _context.SaveChangesAsync();

            return RedirectToAction("Evaluar", new { sesionId });
        }

        [HttpGet]
        public async Task<IActionResult> ReanudarSesion(int sesionId)
        {
            var sesion = await _context.ADIR_Sesion.FindAsync(sesionId);
            if (sesion == null || (sesion.Estado != "EnProgreso" && sesion.Estado != "Pausado"))
                return NotFound();

            if (sesion.Estado == "Pausado")
                sesion.Estado = "EnProgreso";

            await _context.SaveChangesAsync();
            return RedirectToAction("Evaluar", new { sesionId });
        }

        [HttpPost]
        public async Task<IActionResult> FinalizarSesion(int sesionId)
        {
            var sesion = await _context.ADIR_Sesion.FindAsync(sesionId);
            if (sesion == null)
                return NotFound();

            sesion.Estado = "Completado";
            sesion.FechaFin = DateTime.Now;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Sesión finalizada correctamente.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Evaluar(int sesionId)
        {
            var preguntas = await _context.ADIR_Pregunta
                .OrderBy(p => p.CodigoPregunta)
                .ToListAsync();

            var respuestasPrevias = await _context.ADIR_ItemRespondido
                .Where(r => r.SesionID == sesionId)
                .ToDictionaryAsync(r => r.PreguntaDefinicionID);

            var sesion = await _context.ADIR_Sesion.FindAsync(sesionId);
            bool esSoloLectura = sesion != null && sesion.Estado == "Completado";

            ViewBag.SesionID = sesionId;
            ViewBag.Respuestas = respuestasPrevias;
            ViewBag.SoloLectura = esSoloLectura;

            return View("EvaluarADI", preguntas);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarEvaluacion(int SesionID, Dictionary<int, ADIR_ItemRespondido> Respuestas)
        {
            var sesion = await _context.ADIR_Sesion.FindAsync(SesionID);
            if (sesion == null || sesion.Estado == "Completado")
            {
                TempData["Error"] = "La sesión ya está finalizada y no puede ser modificada.";
                return RedirectToAction("Evaluar", new { sesionId = SesionID });
            }

            foreach (var entrada in Respuestas)
            {
                var item = entrada.Value;
                item.SesionID = SesionID;
                item.PreguntaDefinicionID = entrada.Key;

                var existente = await _context.ADIR_ItemRespondido
                    .FirstOrDefaultAsync(r => r.SesionID == SesionID && r.PreguntaDefinicionID == entrada.Key);

                if (existente != null)
                {
                    existente.CodigoRespuestaBruta = item.CodigoRespuestaBruta;
                    existente.NotasObservacionItem = item.NotasObservacionItem;
                }
                else
                {
                    item.CodigoRespuestaAlgoritmo = ConvertirAPuntajeAlgoritmo(item.CodigoRespuestaBruta);
                    _context.ADIR_ItemRespondido.Add(item);
                }
            }

            sesion.Estado = "EnProgreso";
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Evaluación ADI-R guardada con éxito.";
            return RedirectToAction("Evaluar", new { sesionId = SesionID });
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> CalcularResultado(int SesionID, string TipoAlgoritmoUsado)
        {
            var sesion = await _context.ADIR_Sesion.FindAsync(SesionID);
            if (sesion == null)
                return NotFound();

            var respuestas = await (from r in _context.ADIR_ItemRespondido
                                    join p in _context.ADIR_Pregunta
                                      on r.PreguntaDefinicionID equals p.PreguntaDefinicionID
                                    where r.SesionID == SesionID && p.EsParteDelAlgoritmoDiagnostico
                                    select new { Respuesta = r, Pregunta = p })
                                   .ToListAsync();

            var filtradas = respuestas.Where(r =>
                (TipoAlgoritmoUsado == "DSM-IV" && r.Pregunta.AlgoritmoTemporalidad == "Alguna Vez") ||
                (TipoAlgoritmoUsado == "DSM-5" && r.Pregunta.AlgoritmoTemporalidad == "Actual") ||
                (TipoAlgoritmoUsado == "Toddler" && r.Pregunta.AlgoritmoTemporalidad == "4-5 años")
            );

            var comunicacion = filtradas.Where(r =>
                r.Pregunta.DominioPrincipal.Contains("Lenguaje") ||
                (r.Pregunta.SubDominio != null && r.Pregunta.SubDominio.Contains("Comunicación"))
            ).Sum(r => r.Respuesta.CodigoRespuestaAlgoritmo);

            var social = filtradas.Where(r => r.Pregunta.DominioPrincipal.Contains("Interacción"))
                                 .Sum(r => r.Respuesta.CodigoRespuestaAlgoritmo);

            var repetitivos = filtradas.Where(r => r.Pregunta.DominioPrincipal.Contains("Repetitivas"))
                                      .Sum(r => r.Respuesta.CodigoRespuestaAlgoritmo);

            string clasificacion = "Sin Clasificación";
            if (TipoAlgoritmoUsado == "DSM-IV")
            {
                if (comunicacion >= 8 && social >= 10 && repetitivos >= 3)
                    clasificacion = "Cumple criterios para Autismo (DSM-IV)";
                else
                    clasificacion = "No cumple criterios DSM-IV";
            }
            else if (TipoAlgoritmoUsado == "DSM-5")
            {
                if (social + comunicacion >= 12 && repetitivos >= 2)
                    clasificacion = "Cumple criterios para TEA (DSM-5)";
                else
                    clasificacion = "No cumple criterios DSM-5";
            }
            else if (TipoAlgoritmoUsado == "Toddler")
            {
                if (social >= 9 && repetitivos >= 2)
                    clasificacion = "Probable TEA (menores de 4 años)";
                else
                    clasificacion = "No concluyente (Toddler)";
            }

            var resultado = await _context.ADIR_Resultado
                .FirstOrDefaultAsync(r => r.SesionID == SesionID);

            if (resultado == null)
            {
                resultado = new ADIR_ResultadoGlobalSesion
                {
                    SesionID = SesionID,
                    TipoAlgoritmoUsado = TipoAlgoritmoUsado,
                    ComunicacionSocial_PuntuacionTotal = comunicacion,
                    InteraccionSocialReciproca_PuntuacionTotal = social,
                    ComportamientosRepetitivos_PuntuacionTotal = repetitivos,
                    ClasificacionADIR = clasificacion,
                    FechaCalculo = DateTime.Now,
                    NotasResultadoGlobal = $"Evaluado por algoritmo {TipoAlgoritmoUsado}."
                };
                _context.ADIR_Resultado.Add(resultado);
            }
            else
            {
                resultado.TipoAlgoritmoUsado = TipoAlgoritmoUsado;
                resultado.ComunicacionSocial_PuntuacionTotal = comunicacion;
                resultado.InteraccionSocialReciproca_PuntuacionTotal = social;
                resultado.ComportamientosRepetitivos_PuntuacionTotal = repetitivos;
                resultado.ClasificacionADIR = clasificacion;
                resultado.FechaCalculo = DateTime.Now;
                resultado.NotasResultadoGlobal = $"Evaluado por algoritmo {TipoAlgoritmoUsado}.";
            }

            await _context.SaveChangesAsync();

            return View("ResultadoADI", resultado);
        }



        private int ConvertirAPuntajeAlgoritmo(int codigo)
        {
            return codigo switch
            {
                0 => 0,
                1 => 1,
                2 => 2,
                3 => 2,
                7 => 0,
                8 => 0,
                9 => 0,
                _ => 0
            };
        }
    }
}
