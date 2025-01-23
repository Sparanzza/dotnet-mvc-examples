using InventarioMVC.Models;
using X.PagedList;

namespace InventarioMVC.ViewModels;

public class ListadoViewModel<T>
{
    public string FiltroBusqueda { get; set; }
    public int Pagina { get; set; }
    public IPagedList<T> Registros { get; set; }

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