using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("ADIR_PreguntaDefinicion")]
    public class ADIR_PreguntaDefinicion
    {
        [Key]
        public int PreguntaDefinicionID { get; set; }

        [Required]
        [StringLength(20)]
        public string CodigoPregunta { get; set; }

        [Required]
        public string TextoPregunta { get; set; } // NVARCHAR(MAX)

        [Required]
        [StringLength(100)]
        public string DominioPrincipal { get; set; }

        [StringLength(100)]
        public string? SubDominio { get; set; }

        public string? InstruccionesPuntuacion { get; set; } // NVARCHAR(MAX)

        [Required]
        public bool EsParteDelAlgoritmoDiagnostico { get; set; }

        [StringLength(50)]
        public string? AlgoritmoTemporalidad { get; set; }

        public virtual ICollection<ADIR_ItemRespondido> ItemsRespondidos { get; set; }

        public ADIR_PreguntaDefinicion()
        {
            ItemsRespondidos = new HashSet<ADIR_ItemRespondido>();
        }
    }
}