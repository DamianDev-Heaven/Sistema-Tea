using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    public class Usuario
    {
        public int UsuarioID { get; set; }

        [Required(ErrorMessage = "El nombre completo es obligatorio.")]
        [MinLength(5, ErrorMessage = "El nombre debe tener al menos 5 caracteres.")]
        [StringLength(200, ErrorMessage = "El nombre no debe superar los 200 caracteres.")]
        public string NombreCompleto { get; set; }

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        [StringLength(100)]
        public string Email { get; set; }

        [Required(ErrorMessage = "La contraseña es obligatoria.")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "La contraseña debe tener al menos 8 caracteres.")]
        public string ContrasenaHash { get; set; }

        [Required]
        public int RolID { get; set; }

        [ForeignKey("RolID")]
        public Rol Rol { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.Now;

        public bool Activo { get; set; } = true;

        public ICollection<UsuarioCertificacion> UsuarioCertificaciones { get; set; }
    }
}
