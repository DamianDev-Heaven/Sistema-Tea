using Sistema_Tea.Models;
using System.ComponentModel.DataAnnotations;

public class Paciente
{
    public int PacienteID { get; set; }

    [Required, StringLength(100)]
    public string Nombre { get; set; }

    [Required, StringLength(100)]
    public string Apellidos { get; set; }

    public DateTime FechaNacimiento { get; set; }

    [Required]
    public string Sexo { get; set; }

    public string DiagnosticoPrevio { get; set; }

    public int? TutorID { get; set; }

    public DateTime FechaRegistro { get; set; } = DateTime.Now;

    public Tutor Tutor { get; set; }
    public ICollection<AsignacionPaciente> Asignaciones { get; set; }

}
