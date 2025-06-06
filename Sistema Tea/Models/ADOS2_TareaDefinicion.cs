using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADOS2_TareaDefinicion")]
    public class ADOS2_TareaDefinicion
    {
        [Key]
        public int TareaDefinicionID { get; set; }

        [Required]
        [StringLength(2)]
        public string Modulo { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreTarea { get; set; }

        public string? Descripcion { get; set; } // NVARCHAR(MAX)

        public string? InstruccionesAdministracion { get; set; } // NVARCHAR(MAX)

        public virtual ICollection<ADOS2_ItemPuntuado> ItemsPuntuados { get; set; }

        public ADOS2_TareaDefinicion()
        {
            ItemsPuntuados = new HashSet<ADOS2_ItemPuntuado>();
        }

        public string? Dominio { get; set; } // NVARCHAR(MAX)

    }
}