using System.ComponentModel.DataAnnotations;

public class CARS2_QPC_Pregunta
{
    [Key]
    public int PreguntaID { get; set; }

    public string? Texto { get; set; }

    [StringLength(100)]
    public string? Categoria { get; set; }
}
