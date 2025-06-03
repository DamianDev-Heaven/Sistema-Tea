using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;

namespace Sistema_Tea.Controllers
{
    public class Ados2Controller : Controller
    {

        private readonly TeaContext _context;
        public Ados2Controller(TeaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var id = HttpContext.Session.GetInt32("UsuarioID");
            if (id == null)
            {
                return RedirectToAction("Login", "Cuenta");
            }

            var psicologo = _context.Usuario.FirstOrDefault(u => u.UsuarioID == id && u.Activo && u.RolID == 2);

            if (psicologo != null)
            {
                ViewBag.NombrePsicologo = psicologo.NombreCompleto;
                ViewBag.id = psicologo.UsuarioID;

                var sesiones = await _context.ADOS2_Sesion
                               .Where(s => s.PsicologoID == psicologo.UsuarioID)
                               .Include(s => s.Paciente)
                               .OrderByDescending(s => s.FechaInicio)
                               .ToListAsync();

                return View(sesiones);
            }

            return RedirectToAction("Login", "Cuenta");
        }

        [HttpGet]
        public async Task<IActionResult> CrearSesion()
        {
            var psicologoId = HttpContext.Session.GetInt32("UsuarioID");

            if (psicologoId == null)
                return RedirectToAction("Login", "Cuenta");

            var pacientesAsignados = await _context.AsignacionPaciente
                .Where(a => a.PsicologoID == psicologoId && a.Estado == "Asignado")
                .Select(a => a.Paciente)
                .ToListAsync();

            ViewBag.Pacientes = pacientesAsignados.Select(p => new SelectListItem
            {
                Value = p.PacienteID.ToString(),
                Text = p.Nombre + " " + p.Apellidos
            }).ToList();

            return View(new ADOS2_Sesion());
        }

        [HttpPost]
        public async Task<IActionResult> CrearSesion(ADOS2_Sesion nuevaSesion)
        {
            var psicologoId = HttpContext.Session.GetInt32("UsuarioID");
            if (psicologoId == null)
                return RedirectToAction("Login", "Cuenta");

            nuevaSesion.PsicologoID = psicologoId.Value;
            nuevaSesion.FechaInicio = DateTime.Now;
            nuevaSesion.Estado = "Pendiente";

            ModelState.Remove("Paciente");
            ModelState.Remove("Psicologo");
            ModelState.Remove("Consentimiento");

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .Select(x => new {
                        Field = x.Key,
                        Errors = x.Value.Errors.Select(e => e.ErrorMessage)
                    })
                    .ToList();

                foreach (var error in errors)
                {
                    System.Diagnostics.Debug.WriteLine($"Campo: {error.Field}, Errores: {string.Join(", ", error.Errors)}");
                }
            }

            System.Diagnostics.Debug.WriteLine($"PacienteID: {nuevaSesion.PacienteID}");
            System.Diagnostics.Debug.WriteLine($"Modulo: {nuevaSesion.Modulo}");
            System.Diagnostics.Debug.WriteLine($"EdadEvaluacionMeses: {nuevaSesion.EdadEvaluacionMeses}");
            System.Diagnostics.Debug.WriteLine($"PsicologoID: {nuevaSesion.PsicologoID}");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(nuevaSesion);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al guardar: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");

                    ModelState.AddModelError("", "Error al guardar la sesión: " + ex.Message);
                }
            }

            var pacientesAsignados = await _context.AsignacionPaciente
                .Where(a => a.PsicologoID == psicologoId && a.Estado == "Asignado")
                .Select(a => a.Paciente)
                .ToListAsync();

            ViewBag.Pacientes = pacientesAsignados.Select(p => new SelectListItem
            {
                Value = p.PacienteID.ToString(),
                Text = p.Nombre + " " + p.Apellidos
            }).ToList();

            return View(nuevaSesion);
        }

        [HttpGet]
        public JsonResult ObtenerEdadYModulos(int pacienteId)
        {
            var paciente = _context.Paciente.FirstOrDefault(p => p.PacienteID == pacienteId);
            if (paciente == null)
                return Json(new { error = "Paciente no encontrado" });

            var edadMeses = ((DateTime.Now.Year - paciente.FechaNacimiento.Year) * 12) + DateTime.Now.Month - paciente.FechaNacimiento.Month;

            if (DateTime.Now.Day < paciente.FechaNacimiento.Day)
                edadMeses--;

            List<string> modulos = new();
            if (edadMeses >= 12 && edadMeses <= 30) modulos.Add("T");
            if (edadMeses >= 31 && edadMeses <= 71) modulos.AddRange(new[] { "1", "2" });
            if (edadMeses >= 72 && edadMeses <= 119) modulos.AddRange(new[] { "2", "3" });
            if (edadMeses >= 120) modulos.AddRange(new[] { "3", "4" });

            return Json(new { edadMeses, modulos });
        }

        [HttpGet]
        public async Task<IActionResult> IniciarTestADOS2(int id)
        {
            var sesion = await _context.ADOS2_Sesion
                                          .Include(s => s.Paciente)
                                          .Include(s => s.Psicologo)
                                          .FirstOrDefaultAsync(s => s.SesionID == id);
            if (sesion == null || sesion.Estado != "Pendiente")
                return NotFound();

            if (sesion.Modulo == "T")
            {
                return View("ModuloT", sesion);
            }
            else if (int.Parse(sesion.Modulo) == 1)
            {
                return View("Modulo1", sesion);

            }
            else if (int.Parse(sesion.Modulo) == 2)
            {
                return View("Modulo2", sesion);

            }
            else if (int.Parse(sesion.Modulo) == 3)
            {
                return View("Modulo3", sesion);

            }
            else if (int.Parse(sesion.Modulo) == 4)
            {
                return View("IniciarTestADOS2", sesion);
            }

            return View(sesion);
        }


        [HttpGet]
        public async Task<IActionResult> ConfirmarSesion(int SesionID)
        {
            var sesion = await _context.ADOS2_Sesion
                                         .Include(s => s.Paciente)
                                         .FirstOrDefaultAsync(s => s.SesionID == SesionID);

            if (sesion == null || sesion.Estado != "Pendiente")
                return NotFound();

            sesion.Estado = "EnProgreso";
            sesion.FechaInicio = DateTime.Now;
            await _context.SaveChangesAsync();

            var tareas = await _context.ADOS2_Tarea
                                       .Where(t => t.Modulo == "T")
                                       .OrderBy(t => t.TareaDefinicionID)
                                       .ToListAsync();

            if (tareas == null || tareas.Count == 0)
                return NotFound("No hay tareas definidas para el módulo T.");

            var primeraTarea = tareas[0];

            ViewBag.SiguienteIndex = 0; 
            ViewBag.SesionId = SesionID;

            return View("TareasModulos", primeraTarea);
        }

        [HttpGet]
        public async Task<IActionResult> TareasModulo(int sesionId, int tareaIndex)
        {
            var sesion = await _context.ADOS2_Sesion
                .Include(s => s.Paciente)
                .FirstOrDefaultAsync(s => s.SesionID == sesionId);
            if (sesion == null || sesion.Estado != "EnProgreso")
                return NotFound();

            var tareas = await _context.ADOS2_Tarea
                .Where(t => t.Modulo == sesion.Modulo)
                .OrderBy(t => t.TareaDefinicionID)
                .ToListAsync();

            if (tareas.Count == 0)
                return NotFound("No hay tareas definidas para este módulo.");

            if (tareaIndex < 0 || tareaIndex >= tareas.Count)
            {
                sesion.Estado = "Completado";
                sesion.FechaFin = DateTime.Now;
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            var tareaActual = tareas[tareaIndex];

            ViewBag.SesionId = sesionId;
            ViewBag.TareaIndex = tareaIndex;
            ViewBag.SiguienteIndex = tareaIndex + 1;
            ViewBag.TotalTareas = tareas.Count;

            return View("TareasModulos", tareaActual);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarYContinuar(int sesionId, int tareaDefinicionId, int codigoObservado, string notasObservacionItem, int tareaIndex)
        {
            var itemPuntuadoExistente = await _context.ADOS2_ItemPuntuado
                .FirstOrDefaultAsync(i => i.SesionID == sesionId && i.TareaDefinicionID == tareaDefinicionId);

            if (itemPuntuadoExistente != null)
            {
                itemPuntuadoExistente.CodigoObservado = codigoObservado;
                itemPuntuadoExistente.NotasObservacionItem = notasObservacionItem;
                _context.ADOS2_ItemPuntuado.Update(itemPuntuadoExistente);
            }
            else
            {
                var nuevoItem = new ADOS2_ItemPuntuado
                {
                    SesionID = sesionId,
                    TareaDefinicionID = tareaDefinicionId,
                    CodigoObservado = codigoObservado,
                    NotasObservacionItem = notasObservacionItem
                };
                _context.ADOS2_ItemPuntuado.Add(nuevoItem);
            }

            await _context.SaveChangesAsync();

            int siguienteIndex = tareaIndex + 1;

            return RedirectToAction("TareasModulo", new { sesionId = sesionId, tareaIndex = siguienteIndex });
        }
    }
    
}
