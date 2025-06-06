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
            // Hashea la contraseña ingresada
            //string hashIngresado = HashPassword(contrasena);

            // Busca el usuario por email y hash
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

        //private string HashPassword(string password)
        //{
        //    using (var sha = System.Security.Cryptography.SHA256.Create())
        //    {
        //        var bytes = System.Text.Encoding.UTF8.GetBytes(password);
        //        var hash = sha.ComputeHash(bytes);
        //        return Convert.ToBase64String(hash);
        //    }
        //}


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}