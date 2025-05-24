using System;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Tea.Models
{
    public class Paciente
    {
        [Key]
        public int PacienteID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        [StringLength(100)]
        public string Apellidos { get; set; }

        public string? Dui { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El sexo es obligatorio")]
        public string Sexo { get; set; }

        [Required(ErrorMessage = "El diagnostico es obligatorio")]
        public string DiagnosticoPrevio { get; set; }

        public int? TutorID { get; set; }

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
        public bool Activo { get; set; } = true;

        public Tutor Tutor { get; set; }
    }
}
