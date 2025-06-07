using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Filters;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;

namespace Sistema_Tea.Controllers
{
    [SesionActiva]
    public class PacienteController : Controller
    {
        private readonly TeaContext _context;

        public PacienteController(TeaContext context)
        {
            _context = context;
        }

        // GET: Pacientes
        public async Task<IActionResult> Index()
        {
            var pacientes = await _context.Paciente.Include(p => p.Tutor).Where(p => p.Activo).ToListAsync();
            return View(pacientes);
        }

        // GET: Pacientes/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new PacienteTutor()
            {
                Paciente = new Paciente(),
                Tutor = new Tutor()
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PacienteTutor model)
        {
            try
            {
                bool tutorLleno = !string.IsNullOrWhiteSpace(model.Tutor?.Nombre) &&
                                  !string.IsNullOrWhiteSpace(model.Tutor?.Apellidos) &&
                                  model.Tutor?.FechaNacimiento != default;

                if (tutorLleno)
                {
                    bool duiDuplicado = await _context.Tutor.AnyAsync(p => p.Dui == model.Tutor.Dui && p.Activo);
                    if (duiDuplicado)
                    {
                        TempData["ErrorMessage"] = "El DUI ya está registrado para otro tutor.";
                        return View(model);
                    }
                    _context.Tutor.Add(model.Tutor);
                    await _context.SaveChangesAsync();

                    int ultimoTutorId = model.Tutor.TutorID;
                    model.Paciente.TutorID = ultimoTutorId;
                }
                else
                {
                    model.Paciente.TutorID = null;
                }

                if (string.IsNullOrWhiteSpace(model.Paciente.Dui))
                {
                    model.Paciente.Dui = null;
                }

                _context.Paciente.Add(model.Paciente);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Paciente registrado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al guardar en la base de datos: {ex.Message}");
                return View(model);
            }
        }
        public IActionResult VerTutor(int id)
        {
            var tutor = _context.Tutor.FirstOrDefault(t => t.TutorID == id);
            if (tutor == null)
            {
                TempData["ErrorMessage"] = "No se encontró el tutor.";
                return RedirectToAction("Index");
            }
            return View("DetallesTutor", tutor);
        }


        public IActionResult VerPaciente(int id)
        {
            var paciente = _context.Paciente.FirstOrDefault(p => p.PacienteID == id);
            if (paciente == null)
            {
                TempData["ErrorMessage"] = "No se encontró el paciente.";
                return RedirectToAction("Index");
            }
            return View("DetallesPaciente", paciente);
        }

        // En PacienteController.cs
        public async Task<IActionResult> Test()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioID");
            if (userId == null) return Unauthorized();

            var certificacionesDelUsuario = await _context.UsuarioCertificacion
                .Where(uc => uc.UsuarioID == userId)
                .Include(uc => uc.Certificacion)
                .Select(uc => uc.Certificacion.Nombre)
                .ToListAsync();

            ViewBag.UserCertifications = certificacionesDelUsuario;

            ViewData["Title"] = "Seleccionar Instrumento de Evaluación";
            return View();
        }


        // GET: Pacientes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var paciente = await _context.Paciente.Include(p => p.Tutor)
                .FirstOrDefaultAsync(m => m.PacienteID == id);
            if (paciente == null) return NotFound();

            return View(paciente);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var paciente = await _context.Paciente
                .Include(p => p.Tutor)
                .FirstOrDefaultAsync(p => p.PacienteID == id);

            if (paciente != null)
            {
                paciente.Activo = false;

                await _context.SaveChangesAsync();

                if (paciente.Tutor != null)
                {
                    bool tieneOtrosActivos = await _context.Paciente
                        .AnyAsync(p => p.TutorID == paciente.Tutor.TutorID && p.Activo);

                    if (!tieneOtrosActivos)
                    {
                        paciente.Tutor.Activo = false;
                        await _context.SaveChangesAsync();
                    }
                }

                TempData["SuccessMessage"] = "Paciente desactivado correctamente.";
            }

            return RedirectToAction(nameof(Index));
        }
        //Tutores 
        public IActionResult IndexTutor()
        {
            var tutores = _context.Tutor.Where(t => t.Activo).ToList();
            return View(tutores);
        }

        [HttpGet]
        public IActionResult Registrar(int id)
        {
            ViewBag.TutorId = id;
            return View();
        }

        [HttpPost]
        public IActionResult Registrar(Paciente paciente)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(paciente.Dui))
                {
                    paciente.Dui = null;
                }

                paciente.FechaRegistro = DateTime.Now;
                paciente.Activo = true;

                _context.Paciente.Add(paciente);
                _context.SaveChanges();

                return RedirectToAction("IndexTutor", "Paciente");
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "Error al guardar en la base de datos.");
                return View(paciente);
            }

        }
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var paciente = await _context.Paciente.FindAsync(id);
            if (paciente == null)
            {
                return NotFound();
            }
            return View(paciente);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Paciente paciente)
        {
            if (paciente.PacienteID == 0)
            {
                return NotFound();
            }
            try
            {
                if (string.IsNullOrWhiteSpace(paciente.Dui))
                {
                    paciente.Dui = null;
                }

                _context.Update(paciente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Paciente actualizado correctamente.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PacienteExists(paciente.PacienteID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar el paciente: {ex.Message}");
                return View(paciente);
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditTutor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tutor = await _context.Tutor.FindAsync(id);
            if (tutor == null)
            {
                return NotFound();
            }
            return View(tutor);
        }
        private bool PacienteExists(int id)
        {
            return _context.Paciente.Any(e => e.PacienteID == id);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTutor(Tutor tutor)
        {
            if (tutor.TutorID == 0)
            {
                return NotFound();
            }

            try
            {

                bool duiDuplicado = await _context.Tutor.AnyAsync(t => t.Dui == tutor.Dui && t.TutorID != tutor.TutorID && t.Activo);
                if (duiDuplicado)
                {
                    TempData["ErrorMessage"] = "El DUI ya está registrado para otro tutor.";
                    return View(tutor);
                }

                _context.Update(tutor);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Tutor actualizado correctamente.";
                return RedirectToAction(nameof(IndexTutor));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TutorExists(tutor.TutorID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error al actualizar el tutor: {ex.Message}");
                return View(tutor);
            }
        }

        private bool TutorExists(int id)
        {
            return _context.Tutor.Any(e => e.TutorID == id);
        }

        public async Task<IActionResult> MisPacientes()
        {
            var userId = HttpContext.Session.GetInt32("UsuarioID");

            if (userId == null)
            {
                return Unauthorized("ID de usuario no encontrado en la sesión.");
            }

            var pacientes = await _context.Paciente
                .Include(p => p.Tutor)
                .Where(p => p.Activo &&
                            _context.AsignacionPaciente.Any(ap => ap.PacienteID == p.PacienteID &&
                                                                 ap.PsicologoID == userId))
                .ToListAsync();

            return View(pacientes);
        }
        public IActionResult VerTutorP(int id)
        {
            var tutor = _context.Tutor.FirstOrDefault(t => t.TutorID == id);
            if (tutor == null)
            {
                TempData["ErrorMessage"] = "No se encontró el tutor.";
                return RedirectToAction("Index");
            }
            return View("DetallesTutorP", tutor);
        }


        public IActionResult VerPacienteP(int id)
        {
            var paciente = _context.Paciente.FirstOrDefault(p => p.PacienteID == id);
            if (paciente == null)
            {
                TempData["ErrorMessage"] = "No se encontró el paciente.";
                return RedirectToAction("Index");
            }
            return View("DetallesPacienteP", paciente);
        }
        public IActionResult VerResultadoP(int id)
        {

            return View();
        }
    }


}

