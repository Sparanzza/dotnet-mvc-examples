using InventarioMVC.Models;
using X.PagedList;

namespace InventarioMVC.ViewModels;

public class ListadoMarcasViewModel
{
    public string FiltroBusqueda { get; set; }
    public int Pagina { get; set; }
    public IPagedList<Marca> Marcas { get; set; }

    public string TituloCrear { get; set; }
    public int Total { get; set; } = 0;


    public CrearBusquedaViewModel CrearBusquedaViewModel()
    {
        return new CrearBusquedaViewModel()
        {
            Total = Total,
            TituloCrear = TituloCrear,
            FiltroBusqueda = FiltroBusqueda,
        };
    }
}