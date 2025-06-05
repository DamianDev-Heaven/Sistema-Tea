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
                Pacientes = _context.Paciente.Include(p => p.Tutor) 
                .ToList(),
                Psicologos = _context.Usuario.Where(u => u.Rol.NombreRol == "Psicologo").ToList()
            };

            return View(viewModel);
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
        public IActionResult subirArchivo()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SubirArchivo(IFormFile archivo)
        {
            if (archivo == null || archivo.Length == 0)
            {
                ViewBag.Mensaje = "No se seleccionó ningún archivo.";
                return View();
            }

            // Crear nombre único para el archivo
            var nombreArchivo = Path.GetFileNameWithoutExtension(archivo.FileName);
            var extension = Path.GetExtension(archivo.FileName);
            var nuevoNombre = $"{nombreArchivo}_{DateTime.Now.Ticks}{extension}";

            // Ruta completa dentro de wwwroot
            var rutaGuardado = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/consentimientos_firmados", nuevoNombre);

            // Guardar el archivo
            using (var stream = new FileStream(rutaGuardado, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Generar la URL de acceso al archivo
            var urlArchivo = $"/consentimientos_firmados/{nuevoNombre}";
            ViewBag.UrlArchivo = urlArchivo;

            return View();
        }


    }
}
