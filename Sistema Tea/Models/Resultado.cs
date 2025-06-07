using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Tea.Models
{
    public class Resultado
    {
        [Key]
        public int ResultadoID { get; set; }

        [Required]
        public int PacienteID { get; set; }

        [ForeignKey("PacienteID")]
        public Paciente Paciente { get; set; }

        [Required]
        public string ResultadoAdir { get; set; }

        [Required]
        public string ResultadoAdos { get; set; }

        public string Observaciones { get; set; }

        public string ReferidoA { get; set; }

        public string EstudiosAdicionales { get; set; }

        [Required]
        public string DiagnosticoFinal { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
