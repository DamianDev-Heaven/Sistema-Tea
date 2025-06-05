using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Filters;
using Sistema_Tea.Models; 
using Sistema_Tea.Models.Data; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims; 
using System.Threading.Tasks;

namespace Sistema_Tea.Controllers
{

    public class Cars2Controller : Controller
    {
        private readonly TeaContext _context;
        public Cars2Controller(TeaContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var sesionesAdir = await _context.CARS2_Sesion 
                                    .Include(s => s.Paciente)
                                    .Include(s => s.Psicologo) 
                                    .OrderByDescending(s => s.FechaInicio)
                                    .ToListAsync();
            ViewData["Title"] = "Listado de Sesiones ADI-R"; 
            return View(sesionesAdir);
        }
    }
}