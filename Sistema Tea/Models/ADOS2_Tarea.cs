namespace Sistema_Tea.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ADOS2_TareaDefinicion")]
    public class ADOS2_Tarea
    {
        [Key]
        public int TareaDefinicionID { get; set; }

        [Required]
        [StringLength(2)]
        public string Modulo { get; set; }

        [Required]
        [StringLength(150)]
        public string NombreTarea { get; set; }

        public string? Descripcion { get; set; }

        public string? InstruccionesAdministracion { get; set; }
    }
}

