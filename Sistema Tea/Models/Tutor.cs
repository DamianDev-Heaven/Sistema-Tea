using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("Tutor")]
    public class Tutor
    {
        [Key]
        public int TutorID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(100)]
        public string Dui { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [StringLength(500)]
        public string? Telefono { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        public virtual ICollection<Paciente> Pacientes { get; set; }

        public Tutor()
        {
            FechaRegistro = DateTime.Now; // Valor por defecto
            Activo = true; // Valor por defecto
            Pacientes = new HashSet<Paciente>();
        }
    }
}