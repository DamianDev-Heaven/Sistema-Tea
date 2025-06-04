using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("AsignacionPaciente")]
    public class AsignacionPaciente
    {
        [Key]
        public int AsignacionID { get; set; }

        [Required]
        public int PacienteID { get; set; }
        [ForeignKey("PacienteID")]
        public virtual Paciente Paciente { get; set; }

        [Required]
        public int PsicologoID { get; set; }
        [ForeignKey("PsicologoID")]
        public virtual Usuario Psicologo { get; set; }

        [Required]
        public int AsignadoPorID { get; set; }
        [ForeignKey("AsignadoPorID")]
        public virtual Usuario AsignadoPor { get; set; }

        [Required]
        public DateTime FechaAsignacion { get; set; }

        [StringLength(50)]
        public string? Estado { get; set; }

        public AsignacionPaciente()
        {
            FechaAsignacion = DateTime.Now; // Valor por defecto
        }
    }
}