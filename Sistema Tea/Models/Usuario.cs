using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sistema_Tea.Models
{
    [Table("Usuario")]
    public class Usuario
    {
        [Key]
        public int UsuarioID { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreCompleto { get; set; }

        [Required]
        [StringLength(100)]
        public string Dui { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(256)]
        public string ContrasenaHash { get; set; }

        [Required]
        public int RolID { get; set; }
        [ForeignKey("RolID")]
        public virtual Rol Rol { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public bool Activo { get; set; }

        public virtual ICollection<UsuarioCertificacion> UsuarioCertificaciones { get; set; }

        [InverseProperty("Psicologo")]
        public virtual ICollection<AsignacionPaciente> AsignacionesComoPsicologo { get; set; }

        [InverseProperty("AsignadoPor")]
        public virtual ICollection<AsignacionPaciente> AsignacionesRealizadas { get; set; }

        [InverseProperty("Psicologo")] // Asumiendo que en ADOS2_Sesion la prop. se llama Psicologo
        public virtual ICollection<ADOS2_Sesion> ADOS2_Sesiones { get; set; }

        [InverseProperty("CalculadoPorUsuario")] // Asumiendo prop. en ADOS2_ResultadoGlobalSesion
        public virtual ICollection<ADOS2_ResultadoGlobalSesion> ADOS2_ResultadosGlobales { get; set; }

        [InverseProperty("Psicologo")] // Asumiendo que en ADIR_Sesion la prop. se llama Psicologo
        public virtual ICollection<ADIR_Sesion> ADIR_Sesiones { get; set; }

        [InverseProperty("CalculadoPorUsuario")] // Asumiendo prop. en ADIR_ResultadoGlobalSesion
        public virtual ICollection<ADIR_ResultadoGlobalSesion> ADIR_ResultadosGlobales { get; set; }

        [InverseProperty("Psicologo")] // Asumiendo que en CARS2_Sesion la prop. se llama Psicologo
        public virtual ICollection<CARS2_Sesion> CARS2_Sesiones { get; set; }

        [InverseProperty("CalculadoPorUsuario")] // Asumiendo prop. en CARS2_ResultadoGlobalSesion
        public virtual ICollection<CARS2_ResultadoGlobalSesion> CARS2_ResultadosGlobales { get; set; }

        [InverseProperty("UsuarioQueAprobo")] // Asumiendo prop. en Consentimiento
        public virtual ICollection<Consentimiento> ConsentimientosAprobados { get; set; }

        public virtual ICollection<Auditoria> Auditorias { get; set; }


        public Usuario()
        {
            FechaCreacion = DateTime.Now; // Valor por defecto
            Activo = true; // Valor por defecto
            UsuarioCertificaciones = new HashSet<UsuarioCertificacion>();
            AsignacionesComoPsicologo = new HashSet<AsignacionPaciente>();
            AsignacionesRealizadas = new HashSet<AsignacionPaciente>();
            ADOS2_Sesiones = new HashSet<ADOS2_Sesion>();
            ADOS2_ResultadosGlobales = new HashSet<ADOS2_ResultadoGlobalSesion>();
            ADIR_Sesiones = new HashSet<ADIR_Sesion>();
            ADIR_ResultadosGlobales = new HashSet<ADIR_ResultadoGlobalSesion>();
            CARS2_Sesiones = new HashSet<CARS2_Sesion>();
            CARS2_ResultadosGlobales = new HashSet<CARS2_ResultadoGlobalSesion>();
            ConsentimientosAprobados = new HashSet<Consentimiento>();
            Auditorias = new HashSet<Auditoria>();
        }
    }
}