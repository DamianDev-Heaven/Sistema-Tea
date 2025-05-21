using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ADIR_Resultado
{
    [Key]
    public int ResultadoID { get; set; }

    [Required]
    public int SesionID { get; set; }

    [Required]
    public int PreguntaID { get; set; }

    public int? CodigoRespuesta { get; set; }

    public string? Notas { get; set; }

    public int? ComunicacionScore { get; set; }

    public int? InteraccionSocialScore { get; set; }

    public int? ComportamientoRepetitivoScore { get; set; }

    [ForeignKey("SesionID")]
    public ADIR_Sesion Sesion { get; set; }

    [ForeignKey("PreguntaID")]
    public ADIR_Pregunta Pregunta { get; set; }
}