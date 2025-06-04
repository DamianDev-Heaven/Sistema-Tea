using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADIR_ResultadoGlobalSesion")]
    public class ADIR_ResultadoGlobalSesion
    {
        [Key]
        public int ResultadoGlobalID { get; set; }

        [Required]
        public int SesionID { get; set; } // SQL tiene UNIQUE, para relación 1-a-1
        [ForeignKey("SesionID")]
        public virtual ADIR_Sesion ADIR_Sesion { get; set; }

        [Required]
        [StringLength(100)]
        public string TipoAlgoritmoUsado { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? ComunicacionSocial_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? InteraccionSocialReciproca_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? ComunicacionLenguaje_PuntuacionTotal { get; set; }

        [Column(TypeName = "decimal(5, 2)")]
        public decimal? ComportamientosRepetitivos_PuntuacionTotal { get; set; }

        [StringLength(100)]
        public string? ClasificacionADIR { get; set; }

        public string? NotasResultadoGlobal { get; set; } // NVARCHAR(MAX)

        [Required]
        public DateTime FechaCalculo { get; set; }

        public int? CalculadoPorUsuarioID { get; set; }
        [ForeignKey("CalculadoPorUsuarioID")]
        public virtual Usuario? CalculadoPorUsuario { get; set; }

        public ADIR_ResultadoGlobalSesion()
        {
            FechaCalculo = DateTime.Now; // Valor por defecto
        }
    }
}