namespace Sistema_Tea.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class ADOS2_Sesion
    {
        [Key]
        public int SesionID { get; set; }

        [Required]
        public int PacienteID { get; set; }

        [Required]
        public int PsicologoID { get; set; }

        [Required]
        [StringLength(2)]
        public string Modulo { get; set; }

        public int? EdadEvaluacionMeses { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; } = "Pendiente";

        [Required]
        public DateTime FechaInicio { get; set; } = DateTime.Now;

        public DateTime? FechaFin { get; set; }

        public string? NotasGeneralesSesion { get; set; }

        [StringLength(500)]
        public string? MotivoPausaCancelacion { get; set; }

        public int? ConsentimientoID { get; set; }

        // Relaciones
        [ForeignKey("PacienteID")]
        public Paciente Paciente { get; set; }

        [ForeignKey("PsicologoID")]
        public Usuario Psicologo { get; set; }

        [ForeignKey("ConsentimientoID")]
        public Consentimiento Consentimiento { get; set; }
    }
}
