using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ADOS2_Resultado
{
    [Key]
    public int ResultadoID { get; set; }

    [Required]
    public int SesionID { get; set; }

    [Required]
    public int TareaID { get; set; }

    public decimal? CodigoComportamiento { get; set; }

    public decimal? AfectoSocialScore { get; set; }

    public decimal? ComportamientoRepetitivoScore { get; set; }

    public decimal? CSS { get; set; }

    public string? Notas { get; set; }

    [ForeignKey("SesionID")]
    public ADOS2_Sesion Sesion { get; set; }

    [ForeignKey("TareaID")]
    public ADOS2_Tarea Tarea { get; set; }
}