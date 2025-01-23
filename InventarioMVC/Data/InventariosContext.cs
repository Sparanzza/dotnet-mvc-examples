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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Producto>(entity =>
            {
                entity.Property(p => p.Costo)
                    .HasPrecision(18, 2);

                entity.HasOne(p => p.Marca)
                    .WithMany()
                    .HasForeignKey(p => p.MarcaId);
            });
        }
    }
}
