using Sistema_Tea.Models;
using System.ComponentModel.DataAnnotations;

public class AsignacionPaciente
{
    [Key]
    public int AsignacionID { get; set; }

    public int PacienteID { get; set; }
    public int PsicologoID { get; set; }
    public int AsignadoPorID { get; set; }
    public DateTime FechaAsignacion { get; set; }
    public string Estado { get; set; }

    // Relaciones
    public Paciente Paciente { get; set; }
    public Usuario Psicologo { get; set; }
    public Usuario AsignadoPor { get; set; }
}
