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
            {
                System.Diagnostics.Debug.WriteLine($"ConfirmarSesion: SesionID {SesionID} no encontrada o no está Pendiente.");
                return NotFound();
            }

            sesion.Estado = "EnProgreso";
            sesion.FechaInicio = DateTime.Now;
            await _context.SaveChangesAsync();

            var tareas = await _context.ADOS2_Tarea
                .Where(t => t.Modulo == sesion.Modulo)
                .OrderBy(t => t.TareaDefinicionID)
                .ToListAsync();

            if (tareas == null || tareas.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"ConfirmarSesion: No hay tareas para el módulo {sesion.Modulo}.");
                return NotFound($"No hay tareas definidas para el módulo {sesion.Modulo}.");
            }

            var primeraTarea = tareas[0];

            ViewBag.SesionId = SesionID;
            ViewBag.TareaIndex = 0;
            ViewBag.SiguienteIndex = 1;
            ViewBag.TotalTareas = tareas.Count;

            System.Diagnostics.Debug.WriteLine($"ConfirmarSesion: Iniciando SesionID {SesionID}, Módulo {sesion.Modulo}, Tarea: {primeraTarea.NombreTarea}");

            return View("TareasModulos", primeraTarea);
        }

        [HttpGet]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]
        public async Task<IActionResult> ReanudarSesion(int id)
        {
            var psicologoId = HttpContext.Session.GetInt32("UsuarioID");
            if (psicologoId == null)
            {
                System.Diagnostics.Debug.WriteLine($"ReanudarSesion: No se encontró UsuarioID en la sesión.");
                return RedirectToAction("Login", "Cuenta");
            }

            var sesion = await _context.ADOS2_Sesion
                .Include(s => s.Paciente)
                .FirstOrDefaultAsync(s => s.SesionID == id && s.PsicologoID == psicologoId);

            if (sesion == null)
            {
                System.Diagnostics.Debug.WriteLine($"ReanudarSesion: SesionID {id} no encontrada o no pertenece al psicólogo {psicologoId}.");
                return NotFound();
            }

            if (sesion.Estado != "EnProgreso" && sesion.Estado != "Pausado")
            {
                System.Diagnostics.Debug.WriteLine($"ReanudarSesion: SesionID {id} en estado inválido: {sesion.Estado}.");
                return NotFound();
            }

            var otrasSesionesEnProgreso = await _context.ADOS2_Sesion
                .Where(s => s.PsicologoID == psicologoId && s.Estado == "EnProgreso" && s.SesionID != id)
                .ToListAsync();

            foreach (var otra in otrasSesionesEnProgreso)
            {
                otra.Estado = "Pausado";
            }

            if (sesion.Estado == "Pausado")
            {
                sesion.Estado = "EnProgreso";
            }

            await _context.SaveChangesAsync();

            var tareas = await _context.ADOS2_Tarea
                .Where(t => t.Modulo == sesion.Modulo)
                .OrderBy(t => t.TareaDefinicionID)
                .ToListAsync();

            if (tareas == null || tareas.Count == 0)
            {
                System.Diagnostics.Debug.WriteLine($"ReanudarSesion: No hay tareas para el módulo {sesion.Modulo}.");
                return NotFound($"No hay tareas definidas para el módulo {sesion.Modulo}.");
            }

            var ultimaTareaPuntuada = await _context.ADOS2_ItemPuntuado
                .Where(i => i.SesionID == id)
                .OrderByDescending(i => i.TareaDefinicionID)
                .FirstOrDefaultAsync();

            int tareaIndex = 0;
            if (ultimaTareaPuntuada != null)
            {
                var ultimaTareaId = ultimaTareaPuntuada.TareaDefinicionID;
                tareaIndex = tareas.FindIndex(t => t.TareaDefinicionID == ultimaTareaId) + 1;
            }

            // 🛡️ Validación de límite
            if (tareaIndex >= tareas.Count)
            {
                return RedirectToAction("FinalizarSesion", new { id = sesion.SesionID });
            }

            ViewBag.SesionId = sesion.SesionID;
            ViewBag.TareaIndex = tareaIndex;
            ViewBag.SiguienteIndex = tareaIndex + 1;
            ViewBag.TotalTareas = tareas.Count;

            return View("TareasModulos", tareas[tareaIndex]);
        }



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

                var itemsValidos = await _context.ADOS2_ItemPuntuado
                    .Include(i => i.TareaDefinicion)  
                    .Where(i => i.SesionID == sesionId && i.CodigoObservado >= 0 && i.CodigoObservado <= 3)
                    .ToListAsync();

                decimal afectoSocialTotal = itemsValidos
                    .Where(i => i.TareaDefinicion.Dominio == "AfectoSocial")
                    .Sum(i => (decimal)i.CodigoObservado);

                decimal comportamientoRepTotal = itemsValidos
                    .Where(i => i.TareaDefinicion.Dominio == "ComportamientoRepetitivo")
                    .Sum(i => (decimal)i.CodigoObservado);

                decimal comunicacionTotal = itemsValidos
                    .Where(i => i.TareaDefinicion.Dominio == "Comunicacion")
                    .Sum(i => (decimal)i.CodigoObservado);

                decimal interaccionSocialTotal = itemsValidos
                    .Where(i => i.TareaDefinicion.Dominio == "InteraccionSocial")
                    .Sum(i => (decimal)i.CodigoObservado);

                decimal otrosDominiosTotal = itemsValidos
                    .Where(i => i.TareaDefinicion.Dominio == "OtrosDominios")
                    .Sum(i => (decimal)i.CodigoObservado);

                decimal puntajeTotal = afectoSocialTotal + comportamientoRepTotal + comunicacionTotal + interaccionSocialTotal + otrosDominiosTotal;

                string diagnostico = "No determinado";
                switch (sesion.Modulo)
                {
                    case "T":
                        diagnostico = puntajeTotal >= 12 ? "Autismo" :
                                      puntajeTotal >= 10 ? "Espectro Autista" : "No TEA";
                        break;
                    case "1":
                    case "2":
                        diagnostico = puntajeTotal >= 12 ? "Autismo" :
                                      puntajeTotal >= 8 ? "Espectro Autista" : "No TEA";
                        break;
                    case "3":
                    case "4":
                        diagnostico = puntajeTotal >= 10 ? "Autismo" :
                                      puntajeTotal >= 7 ? "Espectro Autista" : "No TEA";
                        break;
                }

                decimal cssTotalGeneral = puntajeTotal;              
                decimal cssAfectoSocial = afectoSocialTotal;          
                decimal cssComportamientoRep = comportamientoRepTotal; 

                var resultado = new ADOS2_ResultadoGlobalSesion
                {
                    SesionID = sesionId,
                    AfectoSocial_PuntuacionTotal = afectoSocialTotal,
                    ComportamientoRepetitivo_PuntuacionTotal = comportamientoRepTotal,
                    Comunicacion_PuntuacionTotal = comunicacionTotal,
                    InteraccionSocial_PuntuacionTotal = interaccionSocialTotal,
                    OtrosDominios_PuntuacionTotal = otrosDominiosTotal,
                    PuntuacionTotalAlgoritmo = puntajeTotal,
                    ClasificacionADOS = diagnostico,
                    CSS_TotalGeneral = cssTotalGeneral,
                    CSS_AfectoSocial = cssAfectoSocial,
                    CSS_ComportamientoRepetitivo = cssComportamientoRep,
                    FechaCalculo = DateTime.Now,
                    CalculadoPorUsuarioID = null,
                };

                _context.ADOS2_Resultado.Add(resultado);

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

        [HttpPost]
        public async Task<IActionResult> PausarSesion(int id, string nota)
        {
            var sesion = await _context.ADOS2_Sesion.FirstOrDefaultAsync(s => s.SesionID == id);
            if (sesion == null || sesion.Estado != "EnProgreso")
                return NotFound();

            sesion.Estado = "Pausado";
            sesion.MotivoPausaCancelacion = nota; 
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelarSesionConfirmado(int sesionId, string motivoCancelacion)
        {
            var sesion = await _context.ADOS2_Sesion.FirstOrDefaultAsync(s => s.SesionID == sesionId);
            if (sesion == null || sesion.Estado == "Cancelado" || sesion.Estado == "Completado")
                return NotFound();

            sesion.Estado = "Cancelado";
            sesion.MotivoPausaCancelacion = motivoCancelacion;
            sesion.FechaFin = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Sesión cancelada correctamente.";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> MotivoCancelacionSesion(int sesionId)
        {
            var sesion = await _context.ADOS2_Sesion
                .Include(s => s.Paciente)
                .FirstOrDefaultAsync(s => s.SesionID == sesionId);

            if (sesion == null || sesion.Estado != "Cancelado")
            {
                System.Diagnostics.Debug.WriteLine($"MotivoCancelacionSesion: SesionID {sesionId} no encontrada o no está Cancelada.");
                return NotFound();
            }

            return PartialView("_MotivoCancelacionModal", sesion);
        }
        [HttpGet]
        public async Task<IActionResult> MotivoPausaSesion(int sesionId)
        {
            var sesion = await _context.ADOS2_Sesion
                .Include(s => s.Paciente)
                .FirstOrDefaultAsync(s => s.SesionID == sesionId);

            if (sesion == null || sesion.Estado != "Pausado")
            {
                System.Diagnostics.Debug.WriteLine($"MotivoCancelacionSesion: SesionID {sesionId} no encontrada o no está Pausada.");
                return NotFound();
            }

            return PartialView("_MotivoCancelacionModal", sesion);
        }

        [HttpPost]
        public async Task<IActionResult> AgregarNotaGeneral(int id, string notaGeneral)
        {
            if (string.IsNullOrWhiteSpace(notaGeneral))
            {
                TempData["ErrorMessage"] = "La nota general es requerida.";
                return RedirectToAction("Index");
            }

            var sesion = await _context.ADOS2_Sesion.FirstOrDefaultAsync(s => s.SesionID == id);
            if (sesion == null)
            {
                TempData["ErrorMessage"] = "La sesión no fue encontrada.";
                return RedirectToAction("Index");
            }

            if (!string.IsNullOrWhiteSpace(sesion.NotasGeneralesSesion))
            {
                TempData["ErrorMessage"] = "Esta sesión ya tiene una nota general asignada.";
                return RedirectToAction("Index");
            }

            sesion.NotasGeneralesSesion = notaGeneral.Trim();
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "La nota general ha sido guardada exitosamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Resultados(int sesionId)
        {
            var resultados = await _context.ADOS2_Resultado
                .Include(r => r.ADOS2_Sesion)
                    .ThenInclude(s => s.Paciente)
                .Include(r => r.ADOS2_Sesion)
                    .ThenInclude(s => s.Psicologo)
                .FirstOrDefaultAsync(r => r.SesionID == sesionId);

            if (resultados == null)
            {
                TempData["ErrorMessage"] = $"No se encontró un resultado para la SesionID: {sesionId}";
                return NotFound();
            }

            return View(resultados);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarNotasResultadoGlobal(int sesionID, string NotasResultadoGlobal)
        {
            var resultado = await _context.ADOS2_Resultado
                .FirstOrDefaultAsync(r => r.SesionID == sesionID);

            if (resultado == null)
                return NotFound();

            resultado.NotasResultadoGlobal = NotasResultadoGlobal;
            await _context.SaveChangesAsync();

            return RedirectToAction("Resultados", new { sesionId = sesionID });
        }





    }




}
