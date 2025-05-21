using System.ComponentModel.DataAnnotations;

public class ADOS2_Tarea
{
    [Key]
    public int TareaID { get; set; }

    [Required]
    [StringLength(2)]
    public string Modulo { get; set; }

    [Required]
    [StringLength(100)]
    public string NombreTarea { get; set; }

    public string? Instrucciones { get; set; }

    [Required]
    [StringLength(20)]
    public string TipoRespuesta { get; set; }
}