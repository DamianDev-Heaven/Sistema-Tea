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
                var now = DateTime.Now;
                var inicioMesActual = new DateTime(now.Year, now.Month, 1);
                var inicioMesAnterior = inicioMesActual.AddMonths(-1);
                var finMesAnterior = inicioMesActual.AddDays(-1);

                // ==== Psicólogos ====
                var psicologosMesActual = _context.Usuario
                    .Include(u => u.Rol)
                    .Where(u => u.Rol.NombreRol == "Psicologo" && u.FechaCreacion >= inicioMesActual)
                    .Count();

                var psicologosMesAnterior = _context.Usuario
                    .Include(u => u.Rol)
                    .Where(u => u.Rol.NombreRol == "Psicologo" &&
                                u.FechaCreacion >= inicioMesAnterior && u.FechaCreacion <= finMesAnterior)
                    .Count();

                ViewBag.TotalPsicologos = psicologosMesActual;
                ViewBag.CambioPsicologos = CalcularCambioPorcentual(psicologosMesAnterior, psicologosMesActual);

                // ==== Pacientes ====
                var pacientesMesActual = _context.Paciente
                    .Where(p => p.FechaRegistro >= inicioMesActual)
                    .Count();

                var pacientesMesAnterior = _context.Paciente
                    .Where(p => p.FechaRegistro >= inicioMesAnterior && p.FechaRegistro <= finMesAnterior)
                    .Count();

                ViewBag.TotalPacientes = pacientesMesActual;
                ViewBag.CambioPacientes = CalcularCambioPorcentual(pacientesMesAnterior, pacientesMesActual);

                // ==== Sesiones ====
                int sesionesActual = _context.ADOS2_Sesion.Count(s => s.FechaInicio >= inicioMesActual)
                                   + _context.ADIR_Sesion.Count(s => s.FechaInicio >= inicioMesActual)
                                   + _context.CARS2_Sesion.Count(s => s.FechaInicio >= inicioMesActual);

                int sesionesAnterior = _context.ADOS2_Sesion.Count(s => s.FechaInicio >= inicioMesAnterior && s.FechaInicio <= finMesAnterior)
                                     + _context.ADIR_Sesion.Count(s => s.FechaInicio >= inicioMesAnterior && s.FechaInicio <= finMesAnterior)
                                     + _context.CARS2_Sesion.Count(s => s.FechaInicio >= inicioMesAnterior && s.FechaInicio <= finMesAnterior);

                ViewBag.TotalSesiones = sesionesActual;
                ViewBag.CambioSesiones = CalcularCambioPorcentual(sesionesAnterior, sesionesActual);

                // ==== Sesiones Pendientes ====
                int sesionesPendientes = _context.ADOS2_Sesion.Count(s => s.Estado == "Pendiente")
                                       + _context.ADIR_Sesion.Count(s => s.Estado == "Pendiente")
                                       + _context.CARS2_Sesion.Count(s => s.Estado == "Pendiente");

                ViewBag.SesionesPendientes = sesionesPendientes;
            }

            return View("Admin/Dashboard");

        }

        private string CalcularCambioPorcentual(int anterior, int actual)
        {
            if (anterior == 0)
                return actual > 0 ? "+100%" : "0%";

            var cambio = ((double)(actual - anterior) / anterior) * 100;
            return (cambio >= 0 ? "+" : "") + Math.Round(cambio, 1) + "%";
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
