using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("Certificacion")]
    public class Certificacion
    {
        [Key]
        public int CertificacionID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        public virtual ICollection<UsuarioCertificacion> UsuarioCertificaciones { get; set; }

        public Certificacion()
        {
            UsuarioCertificaciones = new HashSet<UsuarioCertificacion>();
        }
    }
}