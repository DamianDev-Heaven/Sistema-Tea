using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADIR_ItemRespondido")]
    public class ADIR_ItemRespondido
    {
        [Key]
        public int ItemRespondidoID { get; set; }

        [Required]
        public int SesionID { get; set; }
        [ForeignKey("SesionID")]
        public virtual ADIR_Sesion ADIR_Sesion { get; set; }

        [Required]
        public int PreguntaDefinicionID { get; set; }
        [ForeignKey("PreguntaDefinicionID")]
        public virtual ADIR_PreguntaDefinicion ADIR_PreguntaDefinicion { get; set; }

        [Required]
        public int CodigoRespuestaBruta { get; set; }

        [Required]
        public int CodigoRespuestaAlgoritmo { get; set; }

        [StringLength(100)]
        public string? EdadComportamiento { get; set; }

        public string? NotasObservacionItem { get; set; } // NVARCHAR(MAX)
    }
}