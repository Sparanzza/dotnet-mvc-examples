namespace InventarioMVC.Models;

public class Departamento
{
    public int Id { get; set; }
    public string? Nombre { get; set; }
    public DateTime? FechaCreacion { get; set; }
    public string? Descripcion { get; set; }
}