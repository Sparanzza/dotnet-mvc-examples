using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventarioMVC.ViewModels;

public class AgregarUsuarioViewModel
{
    public SelectList ListadoPerfiles { get; set; }
    public UsuarioRegistroDto Usuario { get; set; } = new UsuarioRegistroDto();
}
