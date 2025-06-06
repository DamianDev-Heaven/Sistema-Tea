using System.Reflection.Metadata;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;
using System.Linq; 

namespace Sistema_Tea.Controllers
{
    public class ConsentimientoController : Controller
    {

        private readonly TeaContext _context;
        private readonly IWebHostEnvironment _environment;

        public ConsentimientoController(TeaContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult plantillaConsentimiento()
        {
            var viewModel = new ConsentimientoUserPacie
            {
                Pacientes = _context.Paciente
                                .Include(p => p.Tutor)
                                .Where(p => p.Activo) 
                                .ToList(),
                Psicologos = _context.Usuario
                                .Where(u => u.Rol.NombreRol == "Psicologo" && u.Activo) 
                                .ToList()
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult ObtenerPacientesFiltrados(string nombre, string apellido, string dui, DateTime? fechaRegistro)
        {
            var pacientes = _context.Paciente
                                .Include(p => p.Tutor)
                                .Where(p => p.Activo); 

            if (!string.IsNullOrEmpty(nombre))
            {
                pacientes = pacientes.Where(p => p.Nombre.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(apellido))
            {
                pacientes = pacientes.Where(p => p.Apellidos.Contains(apellido));
            }

            if (!string.IsNullOrEmpty(dui))
            {
                pacientes = pacientes.Where(p => p.Dui != null && p.Dui.Contains(dui));
            }

            if (fechaRegistro.HasValue)
            {
                pacientes = pacientes.Where(p => p.FechaRegistro.Date == fechaRegistro.Value.Date);
            }

            var pacientesFiltrados = pacientes.Select(p => new
            {
                pacienteID = p.PacienteID,
                nombreCompleto = p.Nombre + " " + p.Apellidos,
                dui = p.Dui,
                fechaNacimiento = p.FechaNacimiento.ToString("dd/MM/yyyy"),
                sexo = p.Sexo,
                diagnosticoPrevio = p.DiagnosticoPrevio,
                tutorNombre = p.Tutor != null ? p.Tutor.Nombre : "No aplica",
                tutorApellidos = p.Tutor != null ? p.Tutor.Apellidos : "-",
                tutorDui = p.Tutor != null ? p.Tutor.Dui : "-"
            }).ToList();

            return Json(pacientesFiltrados);
        }

        [HttpGet]
        public IActionResult ObtenerPsicologosFiltrados(string nombre, string dui)
        {
            var psicologos = _context.Usuario
                                .Where(u => u.Rol.NombreRol == "Psicologo" && u.Activo); 

            if (!string.IsNullOrEmpty(nombre))
            {
                psicologos = psicologos.Where(u => u.NombreCompleto.Contains(nombre));
            }

            if (!string.IsNullOrEmpty(dui))
            {
                psicologos = psicologos.Where(u => u.Dui.Contains(dui));
            }

            var psicologosFiltrados = psicologos.Select(u => new
            {
                usuarioID = u.UsuarioID,
                nombreCompleto = u.NombreCompleto
            }).ToList();

            return Json(psicologosFiltrados);
        }


        public IActionResult GenerarConsentimientoPDF(int pacienteId, int psicologoId)
        {
            var paciente = _context.Paciente.Include(p => p.Tutor).FirstOrDefault(p => p.PacienteID == pacienteId);
            var psicologo = _context.Usuario.FirstOrDefault(u => u.UsuarioID == psicologoId);

            if (paciente == null || psicologo == null)
            {
                return NotFound();
            }

            using var ms = new MemoryStream();

            var writer = new PdfWriter(ms);
            var pdf = new PdfDocument(writer);
            var document = new iText.Layout.Document(pdf);

            // Título centrado
            document.Add(new Paragraph("CLÍNICA eMind: Consentimiento Informado para la Evaluación del Trastorno del Espectro Autista con ADOS-2 y ADI-R")
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontSize(14)
                .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD))
                .SetMarginBottom(20));

            // Fecha y hora actuales
            document.Add(new Paragraph($"Fecha y hora: {DateTime.Now:dd/MM/yyyy HH:mm:ss}"));

            // Datos del paciente
            document.Add(new Paragraph($"Paciente: {paciente.Nombre} {paciente.Apellidos}"));
            document.Add(new Paragraph($"DUI: {paciente.Dui}"));
            document.Add(new Paragraph($"Fecha de Nacimiento: {paciente.FechaNacimiento:dd/MM/yyyy}"));
            document.Add(new Paragraph($"Sexo: {paciente.Sexo}"));
            document.Add(new Paragraph($"Diagnóstico Previo: {paciente.DiagnosticoPrevio ?? "Sin diagnóstico"}"));
            document.Add(new Paragraph($"Tutor Legal: {paciente.Tutor?.Nombre ?? "No aplica"}"));
            document.Add(new Paragraph($"Apellido del Tutor: {paciente.Tutor?.Apellidos ?? "-"}"));
            document.Add(new Paragraph($"DUI del Tutor: {paciente.Tutor?.Dui ?? "-"}"));

            // Psicólogo
            document.Add(new Paragraph($"Psicólogo Responsable: {psicologo.NombreCompleto}"));

            document.Add(new Paragraph("\n"));

            // Contenido del consentimiento
            document.Add(new Paragraph("Declaro que he sido informado(a) clara y detalladamente sobre el proceso de intervención psicológica que recibiré. Entiendo los objetivos de las evaluaciones, las técnicas a emplearse, así como los beneficios y posibles riesgos asociados."));
            document.Add(new Paragraph("Me comprometo a colaborar activamente en las sesiones, respetando los acuerdos establecidos con el/la psicólogo/a. Sé que tengo derecho a interrumpir el proceso si así lo decido."));
            document.Add(new Paragraph("Acepto que la información compartida durante las sesiones será tratada con estricta confidencialidad y no será divulgada a terceros sin mi consentimiento, salvo en situaciones contempladas por la ley o cuando exista riesgo para mi integridad o la de otros."));
            document.Add(new Paragraph("Conforme a la Ley de Protección de Datos Personales vigente en El Salvador, autorizo el tratamiento responsable de mis datos personales únicamente para fines clínicos y administrativos relacionados con esta atención."));

            document.Add(new Paragraph("\n\nFirma del paciente o tutor legal: ___________________________")
                .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD)));
            document.Add(new Paragraph("\n"));
            document.Add(new Paragraph("Firma del psicólogo: ___________________________")
                .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD)));

            document.Close();

            byte[] bytes = ms.ToArray();
            return File(bytes, "application/pdf", "Consentimiento_" + paciente.Nombre + ".pdf");
        }

        [HttpPost]
        public async Task<IActionResult> SubirConsentimientoFirmado(int pacienteId, string tipo, string version, IFormFile archivo)
        {
            var user = HttpContext.Session.GetInt32("UsuarioID");
            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo válido.";
                return RedirectToAction("plantillaConsentimiento");
            }

            // Generar nombre único
            var extension = Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"consentimiento_{pacienteId}_{DateTime.Now.Ticks}{extension}";
            var rutaCarpeta = Path.Combine(_environment.WebRootPath, "documentos_firmados");

            // Asegurar que la carpeta exista
            if (!Directory.Exists(rutaCarpeta))
                Directory.CreateDirectory(rutaCarpeta);

            var rutaArchivo = Path.Combine(rutaCarpeta, nombreArchivo);

            // Guardar el archivo en disco
            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Construir la URL pública
            var urlArchivo = $"/documentos_firmados/{nombreArchivo}";

            // Crear el objeto consentimiento
            var consentimiento = new Consentimiento
            {
                PacienteID = pacienteId,
                Tipo = tipo, 
                Version = version,
                Aprobado = true,
                FechaAprobacion = DateTime.Now,
                DocumentoURL = urlArchivo,
                UsuarioQueAproboID = user 
            };

            _context.Consentimiento.Add(consentimiento);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Consentimiento firmado subido exitosamente.";
            return RedirectToAction("plantillaConsentimiento", new { id = pacienteId });
        }
    }
}