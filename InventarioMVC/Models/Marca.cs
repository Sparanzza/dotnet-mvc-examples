using System.ComponentModel.DataAnnotations;
using InventarioMVC.ViewModels;

namespace InventarioMVC.Models;
public class Marca
{
    public int Id { get; set; }
    [Required(ErrorMessage = "El campo Nombre es obligatorio")]
    [Display(Name = "Nombre Marca")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "El nombre de la marca debe tener entre 3 y 50 caracteres")]
    public string? Nombre { get; set; }
    public ICollection<Producto>? Productos { get; set; }
}
