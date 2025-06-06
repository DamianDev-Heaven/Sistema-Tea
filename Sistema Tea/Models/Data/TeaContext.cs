using Microsoft.EntityFrameworkCore;

namespace Sistema_Tea.Models.Data
{
    public class TeaContext : DbContext
    {
        public TeaContext(DbContextOptions<TeaContext> options) : base(options)
        {
        }
        public DbSet<Usuario> Usuario { get; set; }
        public DbSet<Rol> Rol { get; set; }
        public DbSet<Certificacion> Certificacion { get; set; }
        public DbSet<UsuarioCertificacion> UsuarioCertificacion { get; set; }

        public DbSet<Paciente> Paciente { get; set; }
        public DbSet<Consentimiento> Consentimiento { get; set; }

        public DbSet<ADOS2_Sesion> ADOS2_Sesion { get; set; }
        public DbSet<ADOS2_TareaDefinicion> ADOS2_Tarea { get; set; }
        public DbSet<ADOS2_ResultadoGlobalSesion> ADOS2_Resultado { get; set; }
        public DbSet<ADOS2_ItemPuntuado> ADOS2_ItemPuntuado { get; set; }

        public DbSet<ADIR_Sesion> ADIR_Sesion { get; set; }
        public DbSet<ADIR_PreguntaDefinicion> ADIR_Pregunta { get; set; }
        public DbSet<ADIR_ResultadoGlobalSesion> ADIR_Resultado { get; set; }

        public DbSet<CARS2_Sesion> CARS2_Sesion { get; set; }
        public DbSet<CARS2_ItemDefinicion> CARS2_Item { get; set; }
        public DbSet<CARS2_ItemPuntuado> CARS2_Resultado { get; set; }

        public DbSet<CARS2_QPC_PreguntaDefinicion> CARS2_QPC_Pregunta { get; set; }
        public DbSet<CARS2_QPC_Respuesta> CARS2_QPC_Respuesta { get; set; }

        public DbSet<Auditoria> Auditoria { get; set; }
        public DbSet<AsignacionPaciente> AsignacionPaciente { get; set; }
        public DbSet<Tutor> Tutor { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioCertificacion>(entity =>
            {
                entity.HasKey(uc => new { uc.UsuarioID, uc.CertificacionID });
                entity.HasOne(uc => uc.Usuario)
                      .WithMany(u => u.UsuarioCertificaciones)
                      .HasForeignKey(uc => uc.UsuarioID);
                entity.HasOne(uc => uc.Certificacion)
                      .WithMany(c => c.UsuarioCertificaciones)
                      .HasForeignKey(uc => uc.CertificacionID);
            });
        }

    }
}