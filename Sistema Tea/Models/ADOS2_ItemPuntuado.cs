using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADOS2_ItemPuntuado")]
    public class ADOS2_ItemPuntuado
    {
        [Key]
        public int ItemPuntuadoID { get; set; }

        [Required]
        public int SesionID { get; set; }
        [ForeignKey("SesionID")]
        public virtual ADOS2_Sesion ADOS2_Sesion { get; set; }

        [Required]
        public int TareaDefinicionID { get; set; }
        [ForeignKey("TareaDefinicionID")]
        public virtual ADOS2_TareaDefinicion TareaDefinicion { get; set; }

        [Required]
        public int CodigoObservado { get; set; }

        public string? NotasObservacionItem { get; set; } 
    }
}