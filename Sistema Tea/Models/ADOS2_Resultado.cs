using Sistema_Tea.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

[Table("ADOS2_ResultadoGlobalSesion")]
public class ADOS2_Resultado
{
    [Key]
    public int ResultadoGlobalID { get; set; }

    [Required]
    public int SesionID { get; set; }

    public decimal? AfectoSocial_PuntuacionTotal { get; set; }

    public decimal? ComportamientoRepetitivo_PuntuacionTotal { get; set; }

    public decimal? Comunicacion_PuntuacionTotal { get; set; }

    public decimal? InteraccionSocial_PuntuacionTotal { get; set; }

    public decimal? OtrosDominios_PuntuacionTotal { get; set; }

    public decimal? PuntuacionTotalAlgoritmo { get; set; }

    [StringLength(100)]
    public string? ClasificacionADOS { get; set; }

    public decimal? CSS_TotalGeneral { get; set; }

    public decimal? CSS_AfectoSocial { get; set; }

    public decimal? CSS_ComportamientoRepetitivo { get; set; }

    public string? NotasResultadoGlobal { get; set; }

    [Required]
    public DateTime FechaCalculo { get; set; } = DateTime.Now;

    public int? CalculadoPorUsuarioID { get; set; }

    // Relaciones
    [ForeignKey("SesionID")]
    public ADOS2_Sesion Sesion { get; set; }

    [ForeignKey("CalculadoPorUsuarioID")]
    public Usuario? CalculadoPorUsuario { get; set; }
}