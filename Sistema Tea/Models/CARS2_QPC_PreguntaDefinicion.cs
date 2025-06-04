using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("CARS2_QPC_PreguntaDefinicion")]
    public class CARS2_QPC_PreguntaDefinicion
    {
        [Key]
        public int PreguntaQPCID { get; set; }

        [Required]
        public string TextoPregunta { get; set; } // NVARCHAR(MAX)

        [StringLength(100)]
        public string? Categoria { get; set; }

        public string? InstruccionesAdicionales { get; set; } // NVARCHAR(MAX)

        public virtual ICollection<CARS2_QPC_Respuesta> QPC_Respuestas { get; set; }

        public CARS2_QPC_PreguntaDefinicion()
        {
            QPC_Respuestas = new HashSet<CARS2_QPC_Respuesta>();
        }
    }
}