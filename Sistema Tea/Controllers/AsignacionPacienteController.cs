using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models; // Asegúrate que este sea tu namespace de modelos
using Sistema_Tea.Models.Data;
using Sistema_Tea.Filters; // Para tu SesionActivaAttribute
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http; // Para HttpContext.Session

namespace Sistema_Tea.Controllers // Asegúrate que este sea tu namespace de controladores
{
    public class AsignacionPacienteController : Controller
    {
        private readonly TeaContext _context;

        public AsignacionPacienteController(TeaContext context)
        {
            _context = context;
        }

        // GET: AsignacionPaciente (Listar asignaciones)
        public async Task<IActionResult> Index()
        {
            var asignaciones = await _context.AsignacionPaciente // Usando el nombre del DbSet en TeaContext
                                       .Include(a => a.Paciente)
                                       .Include(a => a.Psicologo) // Usuario que es el Psicólogo
                                       .Include(a => a.AsignadoPor) // Usuario que realizó la asignación
                                       .OrderByDescending(a => a.FechaAsignacion)
                                       .ToListAsync();
            return View(asignaciones);
        }

        // GET: AsignacionPaciente/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            // Pre-llenar el estado si siempre es el mismo al crear
            var model = new AsignacionPaciente
            {
                Estado = "Asignado" // Estado por defecto para nuevas asignaciones
            };
            return View(model);
        }

        // POST: AsignacionPaciente/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PacienteID,PsicologoID,Estado")] AsignacionPaciente asignacionPaciente)
        {
            // Remover propiedades que no deben venir del bind o se setean manualmente/automáticamente
            ModelState.Remove("FechaAsignacion");
            ModelState.Remove("AsignadoPorID");
            ModelState.Remove("AsignacionID");
            ModelState.Remove("Paciente");
            ModelState.Remove("Psicologo");
            ModelState.Remove("AsignadoPor");

            // Obtener el UsuarioID del usuario logueado (quien asigna)
            var asignadoPorUsuarioId = HttpContext.Session.GetInt32("UsuarioID");
            if (asignadoPorUsuarioId == null)
            {
                ModelState.AddModelError(string.Empty, "No se pudo identificar al usuario que realiza la asignación. Inicie sesión nuevamente.");
                await PopulateDropdownsAsync(asignacionPaciente);
                return View(asignacionPaciente);
            }
            asignacionPaciente.AsignadoPorID = asignadoPorUsuarioId.Value;
            asignacionPaciente.FechaAsignacion = DateTime.Now;
            // El estado "Asignado" ya debería venir del GET o estar en el bind. Si no, asígnalo:
            // if (string.IsNullOrEmpty(asignacionPaciente.Estado)) asignacionPaciente.Estado = "Asignado";


            if (ModelState.IsValid)
            {
                bool existeAsignacionActiva = await _context.AsignacionPaciente
                    .AnyAsync(a => a.PacienteID == asignacionPaciente.PacienteID &&
                                   a.PsicologoID == asignacionPaciente.PsicologoID &&
                                   a.Estado == "Asignado");

                if (existeAsignacionActiva)
                {
                    ModelState.AddModelError(string.Empty, "Este paciente ya tiene una asignación activa con este psicólogo.");
                    await PopulateDropdownsAsync(asignacionPaciente);
                    return View(asignacionPaciente);
                }

                _context.Add(asignacionPaciente);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Paciente asignado correctamente.";
                return RedirectToAction(nameof(Index));
            }

            // Si llegamos aquí, algo falló, repoblar dropdowns y mostrar vista
            await PopulateDropdownsAsync(asignacionPaciente);
            return View(asignacionPaciente);
        }

        // GET: AsignacionPaciente/Delete/5 (O Desasignar/Finalizar)
        public async Task<IActionResult> Delete(int? id) // Considera renombrar a "Manage" o "ChangeState" si no es un borrado físico
        {
            if (id == null)
            {
                return NotFound();
            }

            var asignacionPaciente = await _context.AsignacionPaciente
                .Include(a => a.Paciente)
                .Include(a => a.Psicologo)
                .FirstOrDefaultAsync(m => m.AsignacionID == id);

            if (asignacionPaciente == null)
            {
                return NotFound();
            }
            // En lugar de un borrado físico, podrías querer cambiar el estado a "Desasignado" o "Finalizado".
            // Si es un borrado físico, la vista Delete.cshtml es apropiada.
            // Si es cambio de estado, necesitarías una vista diferente o modificar esta.
            return View(asignacionPaciente);
        }

        // POST: AsignacionPaciente/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int AsignacionID)
        {
            var asignacionPaciente = await _context.AsignacionPaciente.FindAsync(AsignacionID);
            if (asignacionPaciente != null)
            {
                // Alternativa: Cambiar estado en lugar de borrar
                // asignacionPaciente.Estado = "Desasignado"; // o "Finalizado"
                // _context.Update(asignacionPaciente);

                _context.AsignacionPaciente.Remove(asignacionPaciente); // Borrado físico
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Asignación eliminada/actualizada correctamente.";
            }
            else
            {
                TempData["ErrorMessage"] = "No se encontró la asignación.";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateDropdownsAsync(AsignacionPaciente? selectedValues = null)
        {
            var rolPsicologo = await _context.Rol.FirstOrDefaultAsync(r => r.NombreRol == Models.Rol.Psicologo); // Usando la constante de tu clase Rol
            int rolPsicologoId = 0;

            if (rolPsicologo != null)
            {
                rolPsicologoId = rolPsicologo.RolID;
            }
            else
            {
                ViewBag.ErrorRol = "El rol de Psicólogo ('" + Models.Rol.Psicologo + "') no se encontró en la base de datos.";
            }

            var pacientesQuery = _context.Paciente.Where(p => p.Activo == true);
            if (selectedValues?.PacienteID != 0 && selectedValues?.PacienteID != null) // Incluir el paciente seleccionado aunque esté inactivo, si se está editando una asignación existente.
            {
                //pacientesQuery = pacientesQuery.Union(_context.Paciente.Where(p => p.PacienteID == selectedValues.PacienteID));
            }

            var pacientesList = await pacientesQuery
                                   .OrderBy(p => p.Nombre).ThenBy(p => p.Apellidos)
                                   //.Select(p => new { p.PacienteID, NombreCompleto = p.Nombre + " " + p.Apellidos }) // Usar propiedad [NotMapped] si existe
                                   .ToListAsync();

            ViewBag.Pacientes = new SelectList(
                pacientesList.Select(p => new { p.PacienteID, NombreCompleto = $"{p.Nombre} {p.Apellidos}" }),
                "PacienteID",
                "NombreCompleto",
                selectedValues?.PacienteID);


            var psicologosQuery = _context.Usuario.Where(u => u.RolID == rolPsicologoId && u.Activo == true);
            if (selectedValues?.PsicologoID != 0 && selectedValues?.PsicologoID != null) // Incluir psicólogo seleccionado aunque esté inactivo
            {
                //psicologosQuery = psicologosQuery.Union(_context.Usuario.Where(u => u.UsuarioID == selectedValues.PsicologoID));
            }
            var psicologosList = await psicologosQuery
                                    .OrderBy(u => u.NombreCompleto)
                                    //.Select(u => new { u.UsuarioID, u.NombreCompleto })
                                    .ToListAsync();

            ViewBag.Psicologos = new SelectList(
                psicologosList.Select(u => new { u.UsuarioID, u.NombreCompleto }),
                "UsuarioID",
                "NombreCompleto",
                selectedValues?.PsicologoID);
        }
    }
}