using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADOS2_Sesion")]
    public class ADOS2_Sesion
    {
        [Key]
        public int SesionID { get; set; }

        [Required]
        public int PacienteID { get; set; }
        [ForeignKey("PacienteID")]
        public virtual Paciente Paciente { get; set; }

        [Required]
        public int PsicologoID { get; set; }
        [ForeignKey("PsicologoID")]
        public virtual Usuario Psicologo { get; set; }

        [Required]
        [StringLength(2)]
        public string Modulo { get; set; }

        public int? EdadEvaluacionMeses { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        public DateTime? FechaFin { get; set; }

        public string? NotasGeneralesSesion { get; set; } // NVARCHAR(MAX)

        [StringLength(500)]
        public string? MotivoPausaCancelacion { get; set; }

        public int? ConsentimientoID { get; set; }
        [ForeignKey("ConsentimientoID")]
        public virtual Consentimiento? Consentimiento { get; set; }

        public virtual ICollection<ADOS2_ItemPuntuado> ItemsPuntuados { get; set; }
        public virtual ADOS2_ResultadoGlobalSesion? ResultadoGlobalSesion { get; set; } // Para relación 1-a-1

        public ADOS2_Sesion()
        {
            FechaInicio = DateTime.Now; // Valor por defecto
            Estado = "Pendiente"; // Valor por defecto
            ItemsPuntuados = new HashSet<ADOS2_ItemPuntuado>();
        }
    }
}