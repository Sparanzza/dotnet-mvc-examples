using InventarioMVC.Models;
using X.PagedList;

namespace InventarioMVC.ViewModels;

public class ListadoMarcasViewModel
{
    public string FiltroBusqueda { get; set; }
    public int Pagina { get; set; }
    public IPagedList<Marca> Marcas { get; set; }

    public int Total { get; set; } = 0;
}