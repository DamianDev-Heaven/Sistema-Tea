using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Tea.Models
{
    public class Certificacion
    {
        public int CertificacionID { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        public ICollection<UsuarioCertificacion> UsuarioCertificaciones { get; set; }
    }
}
