using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;

namespace Sistema_Tea.Controllers
{
    public class RolController : Controller
    {
        private readonly TeaContext _context;

        public RolController(TeaContext context)
        {
            _context = context;
        }

        // GET: RolController
        public async Task<IActionResult> Index()
        {
            var roles = await _context.Rol.ToListAsync();
            return View(roles);
        }

        // GET: RolController/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var rol = await _context.Rol.FirstOrDefaultAsync(r => r.RolID == id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        // GET: RolController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RolController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Rol rol)
        {
            // Esto es solo para probar si guarda sin validación
            if (ModelState.IsValid)
            {
                _context.Rol.Add(rol);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));

            }
            else
            {
                // Si hay errores de validación, se devuelven al formulario
                return View(rol);


            }
        }


        // GET: RolController/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var rol = await _context.Rol.FindAsync(id);
            if (rol == null)
                return NotFound();

            return View(rol);
        }

        // POST: RolController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Rol rol)
        {
            if (id != rol.RolID)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(rol);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "No se pudo guardar los cambios.");
                }
            }
            return View(rol);
        }


        // POST: RolController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var rol = await _context.Rol.FindAsync(id);

            if (rol == null)
            {
                TempData["ErrorMessage"] = "Rol no encontrado.";
                return RedirectToAction(nameof(Index));
            }
            bool tieneUsuarios = await _context.Usuario.AnyAsync(u => u.RolID == id);

            if (tieneUsuarios)
            {
                TempData["ErrorMessage"] = "No se puede eliminar el rol porque tiene usuarios asociados.";
                return RedirectToAction(nameof(Index));
            }
            _context.Rol.Remove(rol);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Rol eliminado correctamente.";
            return RedirectToAction(nameof(Index));
        }

    }
}


