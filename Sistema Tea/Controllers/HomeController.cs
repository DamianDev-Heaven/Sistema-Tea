using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Filters;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;
using System.Diagnostics;

namespace Sistema_Tea.Controllers
{
    [SesionActiva]
    public class HomeController : Controller
    {
        private readonly TeaContext _context;

        public HomeController(TeaContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            var nombre = HttpContext.Session.GetString("Nombre");
            var rol = HttpContext.Session.GetString("Rol");

            if (string.IsNullOrEmpty(nombre))
                return RedirectToAction("Login", "Cuenta");

            ViewBag.NombreUsuario = nombre;
            ViewBag.RolUsuario = rol;

            if (rol == "Administrador")
            {
                // Total de Psicólogos
                ViewBag.TotalPsicologos = _context.Usuario
                    .Include(u => u.Rol)
                    .Count(u => u.Rol.NombreRol == "Psicólogo");

                // Total de Pacientes
                ViewBag.TotalPacientes = _context.Paciente.Count();

                // Total de Citas (sesiones de cualquier prueba)
                int totalSesiones =
                    _context.ADOS2_Sesion.Count() +
                    _context.ADIR_Sesion.Count() +
                    _context.CARS2_Sesion.Count();
                ViewBag.TotalSesiones = totalSesiones;

                // Total de Citas Pendientes
                int sesionesPendientes =
                    _context.ADOS2_Sesion.Count(s => s.Estado == "Pendiente") +
                    _context.ADIR_Sesion.Count(s => s.Estado == "Pendiente") +
                    _context.CARS2_Sesion.Count(s => s.Estado == "Pendiente");
                ViewBag.SesionesPendientes = sesionesPendientes;
            }

            return View();
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
