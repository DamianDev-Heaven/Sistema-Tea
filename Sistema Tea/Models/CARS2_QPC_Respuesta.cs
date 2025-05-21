using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CARS2_QPC_Respuesta
{
    [Key]
    public int RespuestaID { get; set; }

    [Required]
    public int SesionID { get; set; }

    [Required]
    public int PreguntaID { get; set; }

    public string? RespuestaTexto { get; set; }

    [Required]
    public DateTime FechaRespuesta { get; set; } = DateTime.Now;

    [ForeignKey("SesionID")]
    public CARS2_Sesion Sesion { get; set; }

    [ForeignKey("PreguntaID")]
    public CARS2_QPC_Pregunta Pregunta { get; set; }
}