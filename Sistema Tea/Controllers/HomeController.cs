using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        private (DateTime InicioMesActual, DateTime InicioMesAnterior, DateTime FinMesAnterior) GetDateRanges()
        {
            var now = DateTime.Now;
            var inicioMesActual = new DateTime(now.Year, now.Month, 1);
            var inicioMesAnterior = inicioMesActual.AddMonths(-1);
            var finMesAnterior = inicioMesActual.AddDays(-1);
            return (inicioMesActual, inicioMesAnterior, finMesAnterior);
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
                var (inicioMesActual, inicioMesAnterior, finMesAnterior) = GetDateRanges();

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

                var pacientesMesActual = _context.Paciente
                    .Where(p => p.FechaRegistro >= inicioMesActual)
                    .Count();

                var pacientesMesAnterior = _context.Paciente
                    .Where(p => p.FechaRegistro >= inicioMesAnterior && p.FechaRegistro <= finMesAnterior)
                    .Count();

                ViewBag.TotalPacientes = pacientesMesActual;
                ViewBag.CambioPacientes = CalcularCambioPorcentual(pacientesMesAnterior, pacientesMesActual);

                var coordinadoresMesActual = _context.Usuario
                    .Include(u => u.Rol)
                    .Where(u => u.Rol.NombreRol == "Coordinador" && u.FechaCreacion >= inicioMesActual)
                    .Count();

                var coordinadoresMesAnterior = _context.Usuario
                    .Include(u => u.Rol)
                    .Where(u => u.Rol.NombreRol == "Coordinador" &&
                                u.FechaCreacion >= inicioMesAnterior && u.FechaCreacion <= finMesAnterior)
                    .Count();

                ViewBag.TotalCoordinadores = coordinadoresMesActual;
                ViewBag.CambioCoordinadores = CalcularCambioPorcentual(coordinadoresMesAnterior, coordinadoresMesActual);

                int sesionesActual = _context.ADOS2_Sesion.Count(s => s.FechaInicio >= inicioMesActual)
                                   + _context.ADIR_Sesion.Count(s => s.FechaInicio >= inicioMesActual)
                                   + _context.CARS2_Sesion.Count(s => s.FechaInicio >= inicioMesActual);

                int sesionesAnterior = _context.ADOS2_Sesion.Count(s => s.FechaInicio >= inicioMesAnterior && s.FechaInicio <= finMesAnterior)
                                     + _context.ADIR_Sesion.Count(s => s.FechaInicio >= inicioMesAnterior && s.FechaInicio <= finMesAnterior)
                                     + _context.CARS2_Sesion.Count(s => s.FechaInicio >= inicioMesAnterior && s.FechaInicio <= finMesAnterior);

                ViewBag.TotalSesiones = sesionesActual;
                ViewBag.CambioSesiones = CalcularCambioPorcentual(sesionesAnterior, sesionesActual);

                int sesionesPendientes = _context.ADOS2_Sesion.Count(s => s.Estado == "Pendiente")
                                       + _context.ADIR_Sesion.Count(s => s.Estado == "Pendiente")
                                       + _context.CARS2_Sesion.Count(s => s.Estado == "Pendiente");

                ViewBag.SesionesPendientes = sesionesPendientes;
            }

            return View("Admin/Dashboard");
        }

        public IActionResult ListarPsicologos()
        {
            var psicologos = _context.Usuario
                .Include(u => u.Rol)
                .Where(u => u.Rol.NombreRol == "Psicologo")
                .ToList();

            return View("Admin/ListarPsicologos", psicologos);
        }

        public IActionResult ListarAdministradores()
        {
            var administradores = _context.Usuario
                .Include(u => u.Rol)
                .Where(u => u.Rol.NombreRol == "Administrador")
                .ToList();
            return View("Admin/ListarAdmin", administradores);
        }
        public IActionResult CrearAdministrador()
        {
            var roles = _context.Rol.ToList();
            var rolAdministrador = roles.FirstOrDefault(r => r.NombreRol == "Administrador");

            if (rolAdministrador == null)
            {
                TempData["ErrorMessage"] = "No se encontró el rol Administrador en la base de datos.";
                return RedirectToAction("ListarAdministradores");
            }

            ViewBag.Roles = new SelectList(roles, "RolID", "NombreRol", rolAdministrador.RolID);

            var nuevoAdministrador = new Usuario
            {
                RolID = rolAdministrador.RolID
            };

            return View("Admin/CrearAdministrador", nuevoAdministrador);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearAdministrador(Usuario model)
        {
            var rolAdministrador = await _context.Rol.FirstOrDefaultAsync(r => r.NombreRol == "Administrador");
            if (rolAdministrador == null)
            {
                ModelState.AddModelError("", "No se encontró el rol Administrador.");
                return View("Admin/CrearAdministrador", model);
            }

            model.RolID = rolAdministrador.RolID;
            ModelState.Remove("RolID");

            if (await _context.Usuario.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
                var roles = await _context.Rol.ToListAsync();
                ViewBag.Roles = new SelectList(roles, "RolID", "NombreRol", model.RolID);
                return View("Admin/CrearAdministrador", model);
            }

            if (await _context.Usuario.AnyAsync(u => u.Dui == model.Dui))
            {
                ModelState.AddModelError("Dui", "El Dui ya está registrado.");
                var roles = await _context.Rol.ToListAsync();
                ViewBag.Roles = new SelectList(roles, "RolID", "NombreRol", model.RolID);
                return View("Admin/CrearAdministrador", model);
            }

            model.FechaCreacion = DateTime.Now;
            model.Activo = true;
            model.ContrasenaHash = HashPassword(model.ContrasenaHash);

            _context.Usuario.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Administrador creado correctamente.";
            return RedirectToAction("ListarAdministradores");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarAdministrador(int usuarioId)
        {
            var administrador = await _context.Usuario.FindAsync(usuarioId);
            if (administrador == null)
            {
                TempData["ErrorMessage"] = "Administrador no encontrado.";
                return RedirectToAction("ListarAdministradores");
            }

            _context.Usuario.Remove(administrador);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Administrador eliminado correctamente.";
            return RedirectToAction("ListarAdministradores");
        }


        public async Task<IActionResult> CrearPsicologo()
        {
            var rolPsicologo = await _context.Rol.FirstOrDefaultAsync(r => r.NombreRol == "Psicologo");

            if (rolPsicologo == null)
            {
                TempData["ErrorMessage"] = "No se encontró el rol Psicólogo en la base de datos.";
                return RedirectToAction("Dashboard", "Home");
            }

            ViewData["Certificaciones"] = await _context.Certificacion.OrderBy(c => c.Nombre).ToListAsync();

            var nuevoPsicologo = new Usuario
            {
                RolID = rolPsicologo.RolID
            };

            return View("Admin/CrearPsicologo", nuevoPsicologo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearPsicologo(Usuario model, List<int> certificacionesSeleccionadas)
        {
            if (await _context.Usuario.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
            }
            if (await _context.Usuario.AnyAsync(u => u.Dui == model.Dui))
            {
                ModelState.AddModelError("Dui", "El DUI ya está registrado.");
            }

            var rolPsicologo = await _context.Rol.FirstOrDefaultAsync(r => r.NombreRol == "Psicologo");
            if (rolPsicologo == null)
            {
                ModelState.AddModelError("", "No se encontró el rol Psicólogo.");
            }
            else
            {
                model.RolID = rolPsicologo.RolID;
            }
            ModelState.Remove("RolID");
            if (ModelState.IsValid)
            {
                model.ContrasenaHash = HashPassword(model.ContrasenaHash); // Asegúrate de tener tu método HashPassword
                model.FechaCreacion = DateTime.Now;
                model.Activo = true;
                _context.Usuario.Add(model);
                if (certificacionesSeleccionadas != null && certificacionesSeleccionadas.Any())
                {
                    foreach (var certId in certificacionesSeleccionadas)
                    {
                        model.UsuarioCertificaciones.Add(new UsuarioCertificacion
                        {
                            CertificacionID = certId,
                            FechaObtencion = DateTime.Now
                        });
                    }
                }
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Psicólogo creado correctamente.";
                return RedirectToAction("ListarPsicologos");
            }
            ViewData["Certificaciones"] = await _context.Certificacion.OrderBy(c => c.Nombre).ToListAsync();
            return View("Admin/CrearPsicologo", model);
        }

        private string HashPassword(string password)
        {
            using (var sha = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.UTF8.GetBytes(password);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarPsicologo(int usuarioId)
        {
            var psicologo = await _context.Usuario.FindAsync(usuarioId);
            if (psicologo == null)
            {
                TempData["ErrorMessage"] = "Psicólogo no encontrado.";
                return RedirectToAction("ListarPsicologos");
            }

            _context.Usuario.Remove(psicologo);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Psicólogo eliminado correctamente.";
            return RedirectToAction("ListarPsicologos");
        }

        [HttpGet]
        public async Task<IActionResult> ListarCoordinadores()
        {
            var coordinadores = await _context.Usuario
                .Where(u => u.Rol.NombreRol == "Coordinador")
                .ToListAsync();
            return View("Admin/ListarCoordinadores", coordinadores);
        }

        [HttpGet]
        public IActionResult CrearCoordinador()
        {
            return View("Admin/CrearCoordinador", new Usuario());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCoordinador(Usuario model)
        {
            var rolCoord = await _context.Rol.FirstOrDefaultAsync(r => r.NombreRol == "Coordinador");
            if (rolCoord == null)
            {
                ModelState.AddModelError("", "No se encontró el rol Coordinador.");
                return View("Admin/CrearCoordinador", model);
            }

            model.RolID = rolCoord.RolID;
            ModelState.Remove("RolID");

            if (await _context.Usuario.AnyAsync(u => u.Email == model.Email))
            {
                ModelState.AddModelError("Email", "El correo electrónico ya está registrado.");
                return View("Admin/CrearCoordinador", model);
            }

            if (await _context.Usuario.AnyAsync(u => u.Dui == model.Dui))
            {
                ModelState.AddModelError("Dui", "El Dui ya está registrado.");
                return View("Admin/CrearCoordinador", model);
            }

            model.FechaCreacion = DateTime.Now;
            model.Activo = true;
            model.ContrasenaHash = HashPassword(model.ContrasenaHash);

            _context.Usuario.Add(model);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Coordinador creado correctamente.";
            return RedirectToAction("ListarCoordinadores");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCoordinador(int usuarioId)
        {
            var usuario = await _context.Usuario.FindAsync(usuarioId);
            if (usuario == null)
            {
                TempData["ErrorMessage"] = "Coordinador no encontrado.";
            }
            else
            {
                _context.Usuario.Remove(usuario);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Coordinador eliminado correctamente.";
            }
            return RedirectToAction("ListarCoordinadores");
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

        private string CalcularCambioPorcentual(int anterior, int actual)
        {
            if (anterior == 0)
                return actual > 0 ? "+100%" : "0%";

            var cambio = ((double)(actual - anterior) / anterior) * 100;
            return (cambio >= 0 ? "+" : "") + Math.Round(cambio, 1) + "%";
        }
    }
}