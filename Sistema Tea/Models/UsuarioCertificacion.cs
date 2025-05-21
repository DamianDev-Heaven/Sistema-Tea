using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    public class UsuarioCertificacion
    {
        public int UsuarioID { get; set; }

        public int CertificacionID { get; set; }

        public DateTime? FechaObtencion { get; set; }

        [ForeignKey("UsuarioID")]
        public Usuario Usuario { get; set; }

        [ForeignKey("CertificacionID")]
        public Certificacion Certificacion { get; set; }
    }
}
