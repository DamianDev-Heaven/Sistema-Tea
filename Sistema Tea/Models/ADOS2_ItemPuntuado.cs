using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    public class ADOS2_ItemPuntuado
    {
        [Key]
        public int ItemPuntuadoID { get; set; }

        [Required]
        public int SesionID { get; set; }

        [Required]
        public int TareaDefinicionID { get; set; }

        [Required]
        public int CodigoObservado { get; set; }

        public string? NotasObservacionItem { get; set; }
     
        [ForeignKey("SesionID")]
        public ADOS2_Sesion Sesion { get; set; }

        [ForeignKey("TareaDefinicionID")]
        public ADOS2_Tarea TareaDefinicion { get; set; }
    }
}
