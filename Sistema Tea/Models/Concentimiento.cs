using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("Consentimiento")]
    public class Consentimiento
    {
        [Key]
        public int ConsentimientoID { get; set; }

        [Required]
        public int PacienteID { get; set; }
        [ForeignKey("PacienteID")]
        public virtual Paciente Paciente { get; set; }

        [Required]
        [StringLength(30)]
        public string Tipo { get; set; }

        [Required]
        [StringLength(10)]
        public string Version { get; set; }

        [Required]
        public bool Aprobado { get; set; }

        public DateTime? FechaAprobacion { get; set; }

        [StringLength(255)]
        public string? DocumentoURL { get; set; }

        public int? UsuarioQueAproboID { get; set; }
        [ForeignKey("UsuarioQueAproboID")]
        public virtual Usuario? UsuarioQueAprobo { get; set; }
    }
}