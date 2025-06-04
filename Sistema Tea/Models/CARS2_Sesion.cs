using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("CARS2_Sesion")]
    public class CARS2_Sesion
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
        [StringLength(5)]
        public string VersionCARS { get; set; }

        public string? FuentesInformacion { get; set; } // NVARCHAR(MAX)

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

        public virtual ICollection<CARS2_ItemPuntuado> ItemsPuntuados { get; set; }
        public virtual CARS2_ResultadoGlobalSesion? ResultadoGlobalSesion { get; set; } // Para relación 1-a-1
        public virtual ICollection<CARS2_QPC_Respuesta> QPC_Respuestas { get; set; }


        public CARS2_Sesion()
        {
            Estado = "Pendiente"; // Valor por defecto
            FechaInicio = DateTime.Now; // Valor por defecto
            ItemsPuntuados = new HashSet<CARS2_ItemPuntuado>();
            QPC_Respuestas = new HashSet<CARS2_QPC_Respuesta>();
        }
    }
}