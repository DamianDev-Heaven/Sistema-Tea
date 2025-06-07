using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;

namespace Sistema_Tea.Controllers
{
    public class ResultadoController : Controller
    {
        private readonly TeaContext _context;

        public ResultadoController(TeaContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> AgregarResultado(int id)
        {
            var paciente = await _context.Paciente.FindAsync(id);

            var adir = await _context.ADIR_Sesion.FirstOrDefaultAsync(s => s.PacienteID == id && s.Estado == "Completado");
            var ados = await _context.ADOS2_Sesion.FirstOrDefaultAsync(s => s.PacienteID == id && s.Estado == "Completado");

            if (adir == null || ados == null)
            {
                TempData["Error"] = "El paciente debe completar ADI-R y ADOS antes de registrar resultados.";
                return RedirectToAction("Index", "Paciente");
            }

            var model = new ResultadoViewModel
            {
                PacienteId = id,
                NombrePaciente = paciente.Nombre + " " + paciente.Apellidos
            };

            return View(model);
        }

        // POST: AgregarResultado
        [HttpPost] 
        public async Task<IActionResult> AgregarResultado(ResultadoViewModel model)
        {
            // Crea una nueva instancia de la entidad Resultado
            var resultado = new Resultado
            {
                PacienteID = model.PacienteId,
                ResultadoAdir = model.ResultadoAdir,
                ResultadoAdos = model.ResultadoAdos,
                Observaciones = model.Observaciones,
                ReferidoA = model.ReferidoA,
                EstudiosAdicionales = model.EstudiosAdicionales,
                DiagnosticoFinal = model.DiagnosticoFinal,
                FechaRegistro = DateTime.Now // Se establece la fecha y hora actual
            };

            _context.Resultado.Add(resultado);

            await _context.SaveChangesAsync();

            return RedirectToAction("VerResultado", new { id = resultado.ResultadoID });
        }

        public async Task<IActionResult> VerResultado(int id)
        {
            var resultado = await _context.Resultado
                .Include(r => r.Paciente)
                .FirstOrDefaultAsync(r => r.ResultadoID == id);

            if (resultado == null)
                return NotFound();

            return View(resultado);
        }
        public async Task<IActionResult> VerResultadoP(int id)
        {
            var resultado = await _context.Resultado
                .Include(r => r.Paciente)
                .FirstOrDefaultAsync(r => r.ResultadoID == id);

            if (resultado == null)
                return NotFound();

            return View(resultado);
        }
        public async Task<IActionResult> VerResultadoPorPaciente(int id)
        {
            var resultado = await _context.Resultado
                                          .Include(r => r.Paciente)
                                          .Where(r => r.PacienteID == id)
                                          .OrderByDescending(r => r.FechaRegistro) 
                                          .FirstOrDefaultAsync(); 

            if (resultado == null)
            {
                return NotFound();
            }

            return View("VerResultado", resultado);
        }
    }
}