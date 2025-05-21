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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UsuarioCertificacion>()
                .HasKey(uc => new { uc.UsuarioID, uc.CertificacionID });
        }

    }
}