using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("Auditoria")]
    public class Auditoria
    {
        [Key]
        public int AuditoriaID { get; set; }

        public int? UsuarioID { get; set; }
        [ForeignKey("UsuarioID")]
        public virtual Usuario? Usuario { get; set; }

        [Required]
        public DateTime FechaAccion { get; set; }

        [StringLength(100)]
        public string? TipoAccion { get; set; }

        [StringLength(100)]
        public string? TablaAfectada { get; set; }

        public int? RegistroAfectadoID { get; set; }

        public string? Detalles { get; set; } // NVARCHAR(MAX)

        public Auditoria()
        {
            FechaAccion = DateTime.Now; // Valor por defecto
        }
    }
}