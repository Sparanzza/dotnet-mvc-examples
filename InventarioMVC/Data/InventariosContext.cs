using Microsoft.EntityFrameworkCore;
using InventarioMVC.Models;

// dotnet aspnet-codegenerator controller -name MarcasController -m Marca -dc MVCInventarios.Data.InventariosContext --relativeFolderPath Controllers --useDefaultLayout --referenceScriptLibraries
namespace MVCInventarios.Data
{
    public class InventariosContext : DbContext
    {
        public InventariosContext(DbContextOptions<InventariosContext> options)
            : base(options)
        {
        }

        public DbSet<Marca> Marcas { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Perfil> Perfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Marca>().ToTable("Marca");
            modelBuilder.Entity<Producto>().ToTable("Producto");
            modelBuilder.Entity<Departamento>().ToTable("Departamento");
            modelBuilder.Entity<Perfil>().ToTable("Perfil");
            modelBuilder.Entity<Usuario>().ToTable("Usuario");
            
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.Costo)
                    .HasPrecision(18, 2);

                entity.HasOne(p => p.Marca)
                    .WithMany()
                    .HasForeignKey(p => p.MarcaId);

                modelBuilder.Entity<Producto>()
                    .HasOne(p => p.Marca)
                    .WithMany(m => m.Productos)
                    .HasForeignKey(p => p.MarcaId);
            });
        }
    }
}