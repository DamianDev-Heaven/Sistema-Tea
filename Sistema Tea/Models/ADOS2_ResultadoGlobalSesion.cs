using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADOS2_ResultadoGlobalSesion")]
    public class ADOS2_ResultadoGlobalSesion
    {
        [Key]
        public int ResultadoGlobalID { get; set; }

        [Required]
        public int SesionID { get; set; } // SQL tiene UNIQUE, para relación 1-a-1
        [ForeignKey("SesionID")]
        public virtual ADOS2_Sesion ADOS2_Sesion { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AfectoSocial_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? ComportamientoRepetitivo_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Comunicacion_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? InteraccionSocial_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? OtrosDominios_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? PuntuacionTotalAlgoritmo { get; set; }

        [StringLength(100)]
        public string? ClasificacionADOS { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal? CSS_TotalGeneral { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal? CSS_AfectoSocial { get; set; }

        [Column(TypeName = "decimal(4, 2)")]
        public decimal? CSS_ComportamientoRepetitivo { get; set; }

        public string? NotasResultadoGlobal { get; set; } // NVARCHAR(MAX)

        [Required]
        public DateTime FechaCalculo { get; set; }

        public int? CalculadoPorUsuarioID { get; set; }
        [ForeignKey("CalculadoPorUsuarioID")]
        public virtual Usuario? CalculadoPorUsuario { get; set; }

        public ADOS2_ResultadoGlobalSesion()
        {
            FechaCalculo = DateTime.Now; // Valor por defecto
        }
    }
}