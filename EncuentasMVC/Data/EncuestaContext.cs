using EncuentasMVC.Models;
using Microsoft.EntityFrameworkCore;

namespace EncuentasMVC.Data;

public class EncuestaContext : DbContext
{

    public EncuestaContext(DbContextOptions<EncuestaContext> options) : base(options)
    {
    }

    public DbSet<Encuesta> Encuestas { get; set; }

}
