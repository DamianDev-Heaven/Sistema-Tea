using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("CARS2_ResultadoGlobalSesion")]
    public class CARS2_ResultadoGlobalSesion
    {
        [Key]
        public int ResultadoGlobalID { get; set; }

        [Required]
        public int SesionID { get; set; } // SQL tiene UNIQUE, para relación 1-a-1
        [ForeignKey("SesionID")]
        public virtual CARS2_Sesion CARS2_Sesion { get; set; }

        [Required]
        [Column(TypeName = "decimal(4, 1)")]
        public decimal PuntuacionBrutaTotal { get; set; }

        public int? PuntuacionT { get; set; }
        public int? Percentil { get; set; }

        [Required]
        [StringLength(100)]
        public string NivelSeveridad { get; set; }

        public string? NotasResultadoGlobal { get; set; } // NVARCHAR(MAX)

        [Required]
        public DateTime FechaCalculo { get; set; }

        public int? CalculadoPorUsuarioID { get; set; }
        [ForeignKey("CalculadoPorUsuarioID")]
        public virtual Usuario? CalculadoPorUsuario { get; set; }

        public CARS2_ResultadoGlobalSesion()
        {
            FechaCalculo = DateTime.Now; // Valor por defecto
        }
    }
}