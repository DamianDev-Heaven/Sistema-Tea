using Sistema_Tea.Models;
using System.ComponentModel.DataAnnotations;

public class Consentimiento
{
    public int ConsentimientoID { get; set; }

    public int PacienteID { get; set; }

    [Required]
    public string Tipo { get; set; }

    [Required]
    public string Version { get; set; }

    public bool Aprobado { get; set; }

    public DateTime? FechaAprobacion { get; set; }

    public string DocumentoURL { get; set; }

    public int? UsuarioQueAproboID { get; set; }

    public Paciente Paciente { get; set; }
    public Usuario UsuarioQueAprobo { get; set; }
}
