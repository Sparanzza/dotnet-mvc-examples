using InventarioMVC.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventarioMVC.ViewModels;

public class AgregarEditarProductoViewModel
{
    public SelectList ListadoMarcas { get; set; }
    public Producto Producto { get; set; } = new();
}