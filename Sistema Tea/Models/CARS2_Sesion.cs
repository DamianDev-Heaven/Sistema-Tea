using Sistema_Tea.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class CARS2_Sesion
{
    [Key]
    public int SesionID { get; set; }

    [Required]
    public int PacienteID { get; set; }

    [Required]
    public int PsicologoID { get; set; }

    [Required]
    [StringLength(5)]
    [RegularExpression("ST|HF")]
    public string Version { get; set; }

    [Required]
    [StringLength(20)]
    [RegularExpression("Pendiente|EnProgreso|Pausado|Completado|Cancelado")]
    public string Estado { get; set; } = "Pendiente";

    [Required]
    public DateTime FechaInicio { get; set; } = DateTime.Now;

    public DateTime? FechaFin { get; set; }

    public string? Notas { get; set; }

    [StringLength(500)]
    public string? MotivoPausaCancelacion { get; set; }

    [ForeignKey("PacienteID")]
    public Paciente Paciente { get; set; }

    [ForeignKey("PsicologoID")]
    public Usuario Psicologo { get; set; }
}