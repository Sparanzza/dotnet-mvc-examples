using Microsoft.AspNetCore.Mvc.Rendering;

namespace InventarioMVC.ViewModels;

public class EditarUsuarioViewModel
{
    public SelectList ListadoPerfiles { get; set; }
    public UsuarioEdicionDto Usuario { get; set; } = new UsuarioEdicionDto();
}