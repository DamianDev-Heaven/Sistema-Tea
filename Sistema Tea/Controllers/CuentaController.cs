using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models.Data;

namespace Sistema_Tea.Controllers
{
    public class CuentaController : Controller
    {
        private readonly TeaContext _context;

        public CuentaController(TeaContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string email, string contrasena)
        {
            //las contraseñas aun no estan con hash, buxos
            var usuario = _context.Usuario
                .Include(u => u.Rol)
                .FirstOrDefault(u => u.Email == email && u.ContrasenaHash == contrasena && u.Activo);

            if (usuario != null)
            {
                HttpContext.Session.SetInt32("UsuarioID", usuario.UsuarioID);
                HttpContext.Session.SetString("Rol", usuario.Rol.NombreRol);
                HttpContext.Session.SetString("Nombre", usuario.NombreCompleto);

                return RedirectToAction("Dashboard", "Home");
            }

            ViewBag.Error = "Credenciales inválidas.";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
