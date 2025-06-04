using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("CARS2_ItemPuntuado")]
    public class CARS2_ItemPuntuado
    {
        [Key]
        public int ItemPuntuadoID { get; set; }

        [Required]
        public int SesionID { get; set; }
        [ForeignKey("SesionID")]
        public virtual CARS2_Sesion CARS2_Sesion { get; set; }

        [Required]
        public int ItemDefinicionID { get; set; }
        [ForeignKey("ItemDefinicionID")]
        public virtual CARS2_ItemDefinicion CARS2_ItemDefinicion { get; set; }

        [Required]
        [Column(TypeName = "decimal(2, 1)")]
        public decimal PuntuacionItem { get; set; }

        public string? NotasObservacionItem { get; set; } // NVARCHAR(MAX)
    }
}