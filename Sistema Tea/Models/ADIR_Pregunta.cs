using System.ComponentModel.DataAnnotations;

public class ADIR_Pregunta
{
    [Key]
    public int PreguntaID { get; set; }

    [Required]
    [StringLength(30)]
    public string Dominio { get; set; }

    [Required]
    [StringLength(10)]
    public string Numero { get; set; }

    public string? Texto { get; set; }

    public string? Instrucciones { get; set; }
}
