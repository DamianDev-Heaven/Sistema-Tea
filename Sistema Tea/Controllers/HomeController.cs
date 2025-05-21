using Microsoft.AspNetCore.Mvc;
using Sistema_Tea.Filters;
using Sistema_Tea.Models;
using System.Diagnostics;

namespace Sistema_Tea.Controllers
{
    [SesionActiva]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Dashboard()
        {
            var nombre = HttpContext.Session.GetString("Nombre");

            if (string.IsNullOrEmpty(nombre))
            {
                return RedirectToAction("Login", "Cuenta");
            }

            ViewBag.NombreUsuario = nombre;
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
