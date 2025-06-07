using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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

            var psicologo = await _context.Usuario.FindAsync(psicologoId);

            var pacientesAsignados = await (
                from ap in _context.AsignacionPaciente
                join p in _context.Paciente on ap.PacienteID equals p.PacienteID
                where ap.PsicologoID == psicologoId && ap.Estado == "Asignado"
                select new
                {
                    p.PacienteID,
                    NombreCompleto = p.Nombre + " " + p.Apellidos,
                    Tutor = p.Tutor 
                }
            ).ToListAsync();

            ViewBag.Pacientes = pacientesAsignados.Select(p => new SelectListItem
            {
                Value = p.PacienteID.ToString(),
                Text = p.NombreCompleto
            }).ToList();

            ViewBag.PacientesDatosJson = Newtonsoft.Json.JsonConvert.SerializeObject(pacientesAsignados);

            ViewBag.NombrePsicologo = psicologo?.NombreCompleto ?? "Psicólogo";

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CrearSesion(ADIR_Sesion sesion, string NuevoPacienteNombre)
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


                _context.ADIR_Sesion.Add(sesion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
  

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

            return RedirectToAction("Index", new { sesionId });
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
            var sesion = await _context.ADIR_Sesion.FindAsync(sesionId);
            if (sesion == null)
                return NotFound();

            var preguntas = await _context.ADIR_Pregunta
                .OrderBy(p => p.CodigoPregunta)
                .ToListAsync();

            var respuestasPrevias = await _context.ADIR_ItemRespondido
                .Where(r => r.SesionID == sesionId)
                .ToDictionaryAsync(r => r.PreguntaDefinicionID);

            bool esSoloLectura = sesion.Estado == "Completado";

            ViewBag.SesionID = sesionId;
            ViewBag.Respuestas = respuestasPrevias;
            ViewBag.SoloLectura = esSoloLectura;

            return View("Evaluar", preguntas);
        }



        [HttpPost]
        public async Task<IActionResult> GuardarEvaluacionPausada(int SesionID, Dictionary<int, ADIR_ItemRespondido> Respuestas, string MotivoPausa)
        {
            var sesion = await _context.ADIR_Sesion.FindAsync(SesionID);
            if (sesion == null) return NotFound();

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

            sesion.Estado = "Pausado";
            sesion.MotivoPausaCancelacion = MotivoPausa;
            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Sesión pausada correctamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> VerResultadoGlobal(int sesionId)
        {
            var datos = await (from rg in _context.ADIR_Resultado
                               join s in _context.ADIR_Sesion on rg.SesionID equals s.SesionID
                               join p in _context.Paciente on s.PacienteID equals p.PacienteID
                               join u in _context.Usuario on s.PsicologoID equals u.UsuarioID
                               where s.SesionID == sesionId
                               select new ResultadoGlobalViewModel
                               {
                                   SesionID = s.SesionID,
                                   PacienteNombre = p.Nombre + " " + p.Apellidos,
                                   FechaNacimiento = p.FechaNacimiento,
                                   FechaInicio = s.FechaInicio,
                                   PsicologoNombre = u.NombreCompleto,
                                   TipoAlgoritmoUsado = rg.TipoAlgoritmoUsado,
                                   ClasificacionADIR = rg.ClasificacionADIR,
                                   NotasResultadoGlobal = rg.NotasResultadoGlobal
                               }).FirstOrDefaultAsync();

            if (datos == null)
            {
                ViewBag.ErrorMessage = "No se encontró la sesión o resultados.";
                return View("Resultado", new ResultadoGlobalViewModel());
            }

            int edadMeses = ((datos.FechaInicio.Year - datos.FechaNacimiento.Year) * 12) + (datos.FechaInicio.Month - datos.FechaNacimiento.Month);
            datos.EdadTexto = $"{edadMeses / 12} años y {edadMeses % 12} meses";

            datos.Historial = await (from r in _context.ADIR_ItemRespondido
                                     join p in _context.ADIR_Pregunta on r.PreguntaDefinicionID equals p.PreguntaDefinicionID
                                     where r.SesionID == sesionId
                                     orderby p.CodigoPregunta
                                     select new ItemRespondidoViewModel
                                     {
                                         CodigoPregunta = p.CodigoPregunta,
                                         TextoPregunta = p.TextoPregunta,
                                         NotasObservacionItem = r.NotasObservacionItem
                                     }).ToListAsync();

            return View("Resultado", datos);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarNotaDiagnostica(ResultadoGlobalViewModel model)
        {
            var resultado = await _context.ADIR_Resultado.FirstOrDefaultAsync(r => r.SesionID == model.SesionID);
            if (resultado == null)
            {
                TempData["ErrorMessage"] = "No se encontró resultado para actualizar.";
                return RedirectToAction("VerResultadoGlobal", new { sesionId = model.SesionID });
            }

            if (string.IsNullOrWhiteSpace(resultado.NotasResultadoGlobal))
            {
                resultado.NotasResultadoGlobal = model.NotasResultadoGlobal;
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Nota guardada.";
            }
            else
            {
                TempData["ErrorMessage"] = "La nota diagnóstica ya fue registrada y no puede ser modificada.";
            }

            return RedirectToAction("VerResultadoGlobal", new { sesionId = model.SesionID });
        }




        [HttpPost]
        public async Task<IActionResult> CancelarSesion(int SesionID, string MotivoCancelacion)
        {
            var sesion = await _context.ADIR_Sesion.FindAsync(SesionID);
            if (sesion == null)
                return NotFound();

            if (string.IsNullOrWhiteSpace(MotivoCancelacion))
            {
                TempData["Error"] = "Debe especificar el motivo de la cancelación.";
                return RedirectToAction("Evaluar", new { sesionId = SesionID });
            }

            sesion.Estado = "Cancelado";
            sesion.MotivoPausaCancelacion = MotivoCancelacion;
            sesion.FechaFin = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["Mensaje"] = "Sesión cancelada correctamente.";
            return RedirectToAction("Index");
        }


        public IActionResult ReanudarSesion(int sesionId)
        {
            var sesion = _context.ADIR_Sesion
                                 .Include(s => s.Paciente)
                                 .FirstOrDefault(s => s.SesionID == sesionId);

            if (sesion == null)
            {
                TempData["ErrorMessage"] = "Sesión no encontrada.";
                return RedirectToAction("Index");
            }

            var preguntas = _context.ADIR_Pregunta
                                    .OrderBy(p => p.CodigoPregunta)
                                    .ToList();

            var respuestas = _context.ADIR_ItemRespondido
                                    .Where(r => r.SesionID == sesionId)
                                    .ToDictionary(r => r.PreguntaDefinicionID);

            ViewBag.SesionID = sesionId;
            ViewBag.Respuestas = respuestas;
            ViewBag.SoloLectura = false;

            return View("Evaluar", preguntas);
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
