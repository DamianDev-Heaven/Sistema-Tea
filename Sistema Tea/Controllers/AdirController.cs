using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var sesionesAdir = await _context.ADIR_Sesion 
                                    .Include(s => s.Paciente)
                                    .Include(s => s.Psicologo) 
                                    .OrderByDescending(s => s.FechaInicio) 
                                    .ToListAsync();
            ViewData["Title"] = "Listado de Sesiones ADI-R";
            return View(sesionesAdir);
        }
    }
}
