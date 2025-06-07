// --- USINGS: Se han limpiado para evitar conflictos ---
// Los 'usings' de iText ya no son estrictamente necesarios porque usamos los nombres completos,
// pero los dejamos para claridad y para las clases que no tienen conflicto.
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sistema_Tea.Models;
using Sistema_Tea.Models.Data;
using System;
using System.IO; // Necesario para MemoryStream y FileStream
using System.Linq;
using System.Threading.Tasks;

// Se usan nombres completos para las clases de iText para resolver la ambigüedad
// con la biblioteca antigua iTextSharp que también está en el proyecto.

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

        // --- MÉTODOS ORIGINALES (SIN CAMBIOS) ---
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
                pacientes = pacientes.Where(p => p.Nombre.Contains(nombre));
            if (!string.IsNullOrEmpty(apellido))
                pacientes = pacientes.Where(p => p.Apellidos.Contains(apellido));
            if (!string.IsNullOrEmpty(dui))
                pacientes = pacientes.Where(p => p.Dui != null && p.Dui.Contains(dui));
            if (fechaRegistro.HasValue)
                pacientes = pacientes.Where(p => p.FechaRegistro.Date == fechaRegistro.Value.Date);

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
                psicologos = psicologos.Where(u => u.NombreCompleto.Contains(nombre));
            if (!string.IsNullOrEmpty(dui))
                psicologos = psicologos.Where(u => u.Dui.Contains(dui));

            var psicologosFiltrados = psicologos.Select(u => new
            {
                usuarioID = u.UsuarioID,
                nombreCompleto = u.NombreCompleto
            }).ToList();
            return Json(psicologosFiltrados);
        }

        // --- MÉTODO 'GenerarConsentimientoPDF' CORREGIDO CON NOMBRES COMPLETOS ---
        public IActionResult GenerarConsentimientoPDF(int pacienteId, int psicologoId)
        {
            var paciente = _context.Paciente.Include(p => p.Tutor).FirstOrDefault(p => p.PacienteID == pacienteId);
            var psicologo = _context.Usuario.FirstOrDefault(u => u.UsuarioID == psicologoId);

            if (paciente == null || psicologo == null)
            {
                return NotFound();
            }

            using var memoryStream = new MemoryStream();
            var writer = new iText.Kernel.Pdf.PdfWriter(memoryStream);
            var pdfDocument = new iText.Kernel.Pdf.PdfDocument(writer);
            var document = new iText.Layout.Document(pdfDocument, iText.Kernel.Geom.PageSize.LETTER);
            document.SetMargins(50, 36, 50, 36);

            iText.Kernel.Font.PdfFont boldFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD);
            iText.Kernel.Font.PdfFont regularFont = iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA);

            CrearEncabezado(document, boldFont);
            CrearTablaDeDatos(document, paciente, psicologo, regularFont, boldFont);
            CrearCuerpoDelConsentimiento(document, regularFont);
            CrearSeccionDeFirmas(document, regularFont);

            int numberOfPages = pdfDocument.GetNumberOfPages();
            for (int i = 1; i <= numberOfPages; i++)
            {
                iText.Kernel.Pdf.PdfPage page = pdfDocument.GetPage(i);
                iText.Kernel.Geom.Rectangle pageSize = page.GetPageSize();
                var pdfCanvas = new iText.Kernel.Pdf.Canvas.PdfCanvas(page.NewContentStreamBefore(), page.GetResources(), pdfDocument);
                pdfCanvas.SetStrokeColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                         .MoveTo(pageSize.GetLeft() + 36, pageSize.GetBottom() + 36)
                         .LineTo(pageSize.GetRight() - 36, pageSize.GetBottom() + 36)
                         .Stroke();
                document.ShowTextAligned(
                    new iText.Layout.Element.Paragraph($"Generado el: {DateTime.Now:G} - Confidencial")
                        .SetFont(regularFont).SetFontSize(8).SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY),
                    pageSize.GetLeft() + 36, pageSize.GetBottom() + 20, i, iText.Layout.Properties.TextAlignment.LEFT, iText.Layout.Properties.VerticalAlignment.MIDDLE, 0);
                document.ShowTextAligned(
                    new iText.Layout.Element.Paragraph($"Página {i} de {numberOfPages}")
                        .SetFont(regularFont).SetFontSize(8).SetFontColor(iText.Kernel.Colors.ColorConstants.GRAY),
                    pageSize.GetRight() - 36, pageSize.GetBottom() + 20, i, iText.Layout.Properties.TextAlignment.RIGHT, iText.Layout.Properties.VerticalAlignment.MIDDLE, 0);
            }

            document.Close();

            byte[] bytes = memoryStream.ToArray();
            return File(bytes, "application/pdf", $"Consentimiento_{paciente.Nombre}_{paciente.Apellidos}.pdf");
        }

        // --- MÉTODO 'SubirConsentimientoFirmado' CORREGIDO ---
        [HttpPost]
        public async Task<IActionResult> SubirConsentimientoFirmado(int pacienteId, string tipo, string version, IFormFile archivo)
        {
            var user = HttpContext.Session.GetInt32("UsuarioID");
            if (archivo == null || archivo.Length == 0)
            {
                TempData["Error"] = "Debe seleccionar un archivo válido.";
                return RedirectToAction("plantillaConsentimiento");
            }

            var extension = System.IO.Path.GetExtension(archivo.FileName);
            var nombreArchivo = $"consentimiento_{pacienteId}_{DateTime.Now.Ticks}{extension}";
            var rutaCarpeta = System.IO.Path.Combine(_environment.WebRootPath, "documentos_firmados");

            if (!Directory.Exists(rutaCarpeta))
                Directory.CreateDirectory(rutaCarpeta);

            var rutaArchivo = System.IO.Path.Combine(rutaCarpeta, nombreArchivo);

            using (var stream = new FileStream(rutaArchivo, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            var urlArchivo = $"/documentos_firmados/{nombreArchivo}";
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

        #region Métodos Privados Auxiliares para PDF (con nombres de clase completos)
        private void CrearEncabezado(iText.Layout.Document document, iText.Kernel.Font.PdfFont titleFont)
        {
            document.Add(new iText.Layout.Element.Paragraph("CLÍNICA eMind")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFont(titleFont)
                .SetFontSize(18).SetMarginBottom(5));
            document.Add(new iText.Layout.Element.Paragraph("Consentimiento Informado para Evaluación Psicológica")
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER).SetFont(titleFont)
                .SetFontSize(12).SetMarginBottom(25));
        }

        private void CrearTablaDeDatos(iText.Layout.Document document, Paciente paciente, Usuario psicologo, iText.Kernel.Font.PdfFont regularFont, iText.Kernel.Font.PdfFont boldFont)
        {
            var table = new iText.Layout.Element.Table(iText.Layout.Properties.UnitValue.CreatePercentArray(new float[] { 25, 75 })).UseAllAvailableWidth();
            table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

            iText.Layout.Element.Cell cellHeaderPaciente = new iText.Layout.Element.Cell(1, 2)
                .Add(new iText.Layout.Element.Paragraph("I. DATOS DEL PACIENTE").SetFont(boldFont).SetFontSize(11))
                .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetPadding(5);
            table.AddHeaderCell(cellHeaderPaciente);

            table.AddCell(CrearCeldaEtiqueta("Nombre Completo:", boldFont));
            table.AddCell(CrearCeldaValor($"{paciente.Nombre} {paciente.Apellidos}", regularFont));
            table.AddCell(CrearCeldaEtiqueta("DUI:", boldFont));
            table.AddCell(CrearCeldaValor(paciente.Dui ?? "N/A", regularFont));
            table.AddCell(CrearCeldaEtiqueta("Fecha de Nacimiento:", boldFont));
            table.AddCell(CrearCeldaValor($"{paciente.FechaNacimiento:dd/MM/yyyy}", regularFont));
            if (paciente.Tutor != null)
            {
                table.AddCell(CrearCeldaEtiqueta("Tutor Legal:", boldFont));
                table.AddCell(CrearCeldaValor($"{paciente.Tutor.Nombre} {paciente.Tutor.Apellidos}", regularFont));
                table.AddCell(CrearCeldaEtiqueta("DUI del Tutor:", boldFont));
                table.AddCell(CrearCeldaValor(paciente.Tutor.Dui ?? "N/A", regularFont));
            }

            iText.Layout.Element.Cell cellHeaderPsicologo = new iText.Layout.Element.Cell(1, 2)
                .Add(new iText.Layout.Element.Paragraph("II. DATOS DEL PROFESIONAL").SetFont(boldFont).SetFontSize(11))
                .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY).SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetPadding(5).SetMarginTop(10);
            table.AddCell(cellHeaderPsicologo);

            table.AddCell(CrearCeldaEtiqueta("Psicólogo Responsable:", boldFont));
            table.AddCell(CrearCeldaValor(psicologo.NombreCompleto, regularFont));

            document.Add(table);
            document.Add(new iText.Layout.Element.LineSeparator(new iText.Kernel.Pdf.Canvas.Draw.SolidLine(0.5f)).SetMarginTop(10).SetMarginBottom(15));
        }

        private void CrearCuerpoDelConsentimiento(iText.Layout.Document document, iText.Kernel.Font.PdfFont font)
        {
            document.Add(new iText.Layout.Element.Paragraph("III. ACUERDOS Y CONDICIONES")
                .SetFont(iText.Kernel.Font.PdfFontFactory.CreateFont(iText.IO.Font.Constants.StandardFonts.HELVETICA_BOLD)).SetFontSize(11).SetMarginBottom(10));

            var paragraphStyle = new iText.Layout.Style()
                .SetFont(font).SetFontSize(10.5f)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.JUSTIFIED).SetMarginBottom(10);

            document.Add(new iText.Layout.Element.Paragraph("Declaro que he sido informado(a) clara y detalladamente sobre el proceso de intervención psicológica que recibiré. Entiendo los objetivos de las evaluaciones (ADOS-2 y ADI-R), las técnicas a emplearse, así como los beneficios y posibles riesgos asociados.").AddStyle(paragraphStyle));
            document.Add(new iText.Layout.Element.Paragraph("Me comprometo a colaborar activamente en las sesiones, respetando los acuerdos establecidos con el/la psicólogo/a. Sé que tengo derecho a interrumpir el proceso si así lo decido.").AddStyle(paragraphStyle));
            document.Add(new iText.Layout.Element.Paragraph("Acepto que la información compartida durante las sesiones será tratada con estricta confidencialidad y no será divulgada a terceros sin mi consentimiento, salvo en situaciones contempladas por la ley o cuando exista riesgo para mi integridad o la de otros.").AddStyle(paragraphStyle));
            document.Add(new iText.Layout.Element.Paragraph("Conforme a la Ley de Protección de Datos Personales vigente en El Salvador, autorizo el tratamiento responsable de mis datos personales únicamente para fines clínicos y administrativos relacionados con esta atención.").AddStyle(paragraphStyle));
        }

        private void CrearSeccionDeFirmas(iText.Layout.Document document, iText.Kernel.Font.PdfFont font)
        {
            document.Add(new iText.Layout.Element.Paragraph().SetMarginTop(60));
            var table = new iText.Layout.Element.Table(iText.Layout.Properties.UnitValue.CreatePercentArray(2)).UseAllAvailableWidth();
            table.SetBorder(iText.Layout.Borders.Border.NO_BORDER);

            var cellFirmaPaciente = new iText.Layout.Element.Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new iText.Layout.Element.Paragraph("___________________________").SetFont(font))
                .Add(new iText.Layout.Element.Paragraph("Firma del paciente o tutor legal").SetFont(font).SetFontSize(10));

            var cellFirmaPsicologo = new iText.Layout.Element.Cell()
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                .Add(new iText.Layout.Element.Paragraph("___________________________").SetFont(font))
                .Add(new iText.Layout.Element.Paragraph("Firma del psicólogo").SetFont(font).SetFontSize(10));

            table.AddCell(cellFirmaPaciente);
            table.AddCell(cellFirmaPsicologo);
            document.Add(table);
        }

        private iText.Layout.Element.Cell CrearCeldaEtiqueta(string texto, iText.Kernel.Font.PdfFont font)
        {
            return new iText.Layout.Element.Cell()
                .Add(new iText.Layout.Element.Paragraph(texto).SetFont(font).SetFontSize(10))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetPadding(2).SetPaddingLeft(5)
                .SetTextAlignment(iText.Layout.Properties.TextAlignment.RIGHT);
        }

        private iText.Layout.Element.Cell CrearCeldaValor(string texto, iText.Kernel.Font.PdfFont font)
        {
            return new iText.Layout.Element.Cell()
                .Add(new iText.Layout.Element.Paragraph(texto).SetFont(font).SetFontSize(10))
                .SetBorder(iText.Layout.Borders.Border.NO_BORDER).SetPadding(2).SetPaddingLeft(10);
        }
        #endregion
    }
}