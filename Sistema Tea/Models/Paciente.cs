using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("Paciente")]
    public class Paciente
    {
        [Key]
        public int PacienteID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(100)]
        public string Apellidos { get; set; }

        [StringLength(100)]
        public string? Dui { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [StringLength(10)]
        public string Sexo { get; set; }

        [StringLength(500)]
        public string? DiagnosticoPrevio { get; set; }

        public int? TutorID { get; set; }
        [ForeignKey("TutorID")]
        public virtual Tutor? Tutor { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public DateTime FechaRegistro { get; set; }

        [NotMapped]
        public string NombreCompleto => $"{Nombre} {Apellidos}";

        public virtual ICollection<AsignacionPaciente> AsignacionesPacientes { get; set; }
        public virtual ICollection<Consentimiento> Consentimientos { get; set; }
        public virtual ICollection<ADOS2_Sesion> ADOS2_Sesiones { get; set; }
        public virtual ICollection<ADIR_Sesion> ADIR_Sesiones { get; set; }
        public virtual ICollection<CARS2_Sesion> CARS2_Sesiones { get; set; }
        // public virtual ICollection<TestInicial> TestIniciales { get; set; } // Si decides crear esta tabla

        public Paciente()
        {
            FechaRegistro = DateTime.Now; // Valor por defecto
            Activo = true; // Valor por defecto
            AsignacionesPacientes = new HashSet<AsignacionPaciente>();
            Consentimientos = new HashSet<Consentimiento>();
            ADOS2_Sesiones = new HashSet<ADOS2_Sesion>();
            ADIR_Sesiones = new HashSet<ADIR_Sesion>();
            CARS2_Sesiones = new HashSet<CARS2_Sesion>();
            // TestIniciales = new HashSet<TestInicial>();
        }
    }
}