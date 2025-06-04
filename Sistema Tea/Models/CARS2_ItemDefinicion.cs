using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("CARS2_ItemDefinicion")]
    public class CARS2_ItemDefinicion
    {
        [Key]
        public int ItemDefinicionID { get; set; }

        [Required]
        [StringLength(5)]
        public string VersionCARS { get; set; }

        [Required]
        public int NumeroItem { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreItem { get; set; }

        public string? DescripcionCriteriosPuntuacion { get; set; } // NVARCHAR(MAX)

        public virtual ICollection<CARS2_ItemPuntuado> ItemsPuntuados { get; set; }

        public CARS2_ItemDefinicion()
        {
            ItemsPuntuados = new HashSet<CARS2_ItemPuntuado>();
        }
    }
}