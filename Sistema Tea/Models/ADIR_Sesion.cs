using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADIR_Sesion")]
    public class ADIR_Sesion
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
        [StringLength(200)]
        public string EntrevistadoPorNombre { get; set; }

        [Required]
        [StringLength(100)]
        public string RelacionConPaciente { get; set; }

        public int? EdadMentalAproximadaAnios { get; set; }
        public int? EdadMentalAproximadaMeses { get; set; }

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

        public virtual ICollection<ADIR_ItemRespondido> ItemsRespondidos { get; set; }
        public virtual ADIR_ResultadoGlobalSesion? ResultadoGlobalSesion { get; set; } // Para relación 1-a-1


        public ADIR_Sesion()
        {
            Estado = "Pendiente"; // Valor por defecto
            FechaInicio = DateTime.Now; // Valor por defecto
            ItemsRespondidos = new HashSet<ADIR_ItemRespondido>();
        }
    }
}