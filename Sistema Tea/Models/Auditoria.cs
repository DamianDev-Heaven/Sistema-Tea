using Sistema_Tea.Models;

public class Auditoria
{
    public int AuditoriaID { get; set; }

    public int? UsuarioID { get; set; }

    public DateTime FechaAccion { get; set; } = DateTime.Now;

    public string TipoAccion { get; set; }
    public string Tabla { get; set; }
    public int? RegistroID { get; set; }
    public string Detalles { get; set; }

    public Usuario Usuario { get; set; }
}
