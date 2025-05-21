using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Sistema_Tea.Models
{
    public class Rol
    {
        public int RolID { get; set; }

        [Required]
        [StringLength(50)]
        public string NombreRol { get; set; }

        public ICollection<Usuario> Usuarios { get; set; }
    }
}
