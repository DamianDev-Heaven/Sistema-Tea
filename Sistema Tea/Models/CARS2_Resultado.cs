using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CARS2_Resultado
{
    [Key]
    public int ResultadoID { get; set; }

    [Required]
    public int SesionID { get; set; }

    [Required]
    public int ItemID { get; set; }

    public int? Puntuacion { get; set; }

    public string? Notas { get; set; }

    public int? PuntajeTotal { get; set; }

    [StringLength(20)]
    [RegularExpression("SinTEA|LeveModerado|Severo")]
    public string? Severidad { get; set; }

    [ForeignKey("SesionID")]
    public CARS2_Sesion Sesion { get; set; }

    [ForeignKey("ItemID")]
    public CARS2_Item Item { get; set; }
}