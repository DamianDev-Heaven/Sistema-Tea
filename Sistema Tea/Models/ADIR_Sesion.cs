using Sistema_Tea.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class ADIR_Sesion
{
    [Key]
    public int SesionID { get; set; }

    [Required]
    public int PacienteID { get; set; }

    [Required]
    public int PsicologoID { get; set; }

    [StringLength(200)]
    public string? EntrevistadoPor { get; set; }

    [StringLength(100)]
    public string? RelacionConPaciente { get; set; }

    [Required]
    [StringLength(20)]
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