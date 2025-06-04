using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("CARS2_QPC_Respuesta")]
    public class CARS2_QPC_Respuesta
    {
        [Key]
        public int RespuestaQPCID { get; set; }

        [Required]
        public int SesionID { get; set; }
        [ForeignKey("SesionID")]
        public virtual CARS2_Sesion CARS2_Sesion { get; set; }

        [Required]
        public int PreguntaQPCID { get; set; }
        [ForeignKey("PreguntaQPCID")]
        public virtual CARS2_QPC_PreguntaDefinicion CARS2_QPC_PreguntaDefinicion { get; set; }

        public string? RespuestaTexto { get; set; } // NVARCHAR(MAX)

        [Required]
        public DateTime FechaRespuesta { get; set; }

        [StringLength(200)]
        public string? RespondidoPor { get; set; }

        public CARS2_QPC_Respuesta()
        {
            FechaRespuesta = DateTime.Now; // Valor por defecto
        }
    }
}