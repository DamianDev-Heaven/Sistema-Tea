using System.ComponentModel.DataAnnotations;

namespace Sistema_Tea.Models
{
    public class ResultadoViewModel
    {
        public int PacienteId { get; set; }

        public string NombrePaciente { get; set; }

        [Required]
        public string ResultadoAdir { get; set; }

        [Required]
        public string ResultadoAdos { get; set; }

        public string Observaciones { get; set; }

        public string ReferidoA { get; set; }

        public string EstudiosAdicionales { get; set; }

        [Required]
        public string DiagnosticoFinal { get; set; }
    }
}
