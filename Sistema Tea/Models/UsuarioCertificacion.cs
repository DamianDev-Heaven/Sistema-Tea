using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("UsuarioCertificacion")]
    public class UsuarioCertificacion
    {
        [Required]
        public int UsuarioID { get; set; }
        [ForeignKey("UsuarioID")]
        public virtual Usuario Usuario { get; set; }

        [Required]
        public int CertificacionID { get; set; }
        [ForeignKey("CertificacionID")]
        public virtual Certificacion Certificacion { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FechaObtencion { get; set; }
    }
}