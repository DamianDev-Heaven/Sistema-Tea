using System.ComponentModel.DataAnnotations;

namespace Sistema_Tea.Models
{
    public class Tutor
    {
        [Key]
        public int TutorID { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Los apellidos son obligatorios")]
        public string Apellidos { get; set; }

        [Required(ErrorMessage = "El dui es obligatorio.")]
        [RegularExpression(@"^\d{9}$", ErrorMessage = "El dui debe tener exactamente 9 dígitos sin guión.")]
        [StringLength(100)]
        public string Dui { get; set; }

        [Required(ErrorMessage = "La fecha de nacimiento es obligatoria")]
        public DateTime FechaNacimiento { get; set; }

        [Required(ErrorMessage = "El telefono es obligatorio")]
        public string Telefono { get; set; }
        public bool Activo { get; set; } = true;

        public DateTime FechaRegistro { get; set; } = DateTime.Now;
    }
}
