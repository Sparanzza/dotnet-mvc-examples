using AspNetCoreHero.ToastNotification.Abstractions;
using InventarioMVC.Models;
using InventarioMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MVCInventarios.Data;
using X.PagedList.Extensions;

namespace InventarioMVC.Controllers
{
    public class ProductosController : Controller
    {
        private readonly InventariosContext _context;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;

        public ProductosController(InventariosContext context, IConfiguration configuration, INotyfService notyfService)
        {
            _context = context;
            _configuration = configuration;
            _notyfService = notyfService;
        }


        // GET: Departamentos
        public async Task<IActionResult> Index(ListadoViewModel<Producto> viewModel)
        {
            var registrosPorPagina = _configuration.GetValue("RegistrosPorPagina", 10);

            viewModel ??= new ListadoViewModel<Producto>
            {
                Pagina = 1
            };

            // Asegurar página válida
            var numeroDePagina = Math.Max(viewModel.Pagina, 1);
            var consulta = _context.Productos.OrderBy(m => m.Nombre).AsQueryable();
            if (!string.IsNullOrWhiteSpace(viewModel.FiltroBusqueda))
            {
                consulta = consulta.Where(m =>
                    m.Nombre.Contains(viewModel.FiltroBusqueda) || m.Marca.Nombre.Contains(viewModel.FiltroBusqueda));
            }

            viewModel.Total = await consulta.CountAsync();
            viewModel.Registros = consulta.ToPagedList(numeroDePagina, registrosPorPagina);
            viewModel.TituloCrear = "Crear Producto";

            return View(viewModel);
        }

        // GET: Departamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .Include(p => p.Marca)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // GET: Productos/Create
        public IActionResult Create()
        {
            AgregarEditarProductoViewModel viewModel = new AgregarEditarProductoViewModel();
            viewModel.ListadoMarcas = new SelectList(_context.Marcas, "Id", "Nombre");
            return View("Producto", viewModel);
        }

        // POST: Productos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,MarcaId,Costo,Estatus")] Producto producto)
        {
            AgregarEditarProductoViewModel viewModel = new AgregarEditarProductoViewModel();
            viewModel.ListadoMarcas = new SelectList(_context.Marcas, "Id", "Nombre");
            viewModel.Producto = producto;
            if (ModelState.IsValid)
            {
                try
                {
                    var existeElemento = await _context.Productos.AnyAsync(m =>
                        m.Nombre != null && producto.Nombre != null &&
                        m.Nombre.ToLower() == producto.Nombre.ToLower().Trim());
                    if (existeElemento)
                    {
                        _notyfService.Warning("Ya existe una Producto con el mismo nombre.");
                        return View("Producto", viewModel);
                    }

                    _context.Add(producto);
                    await _context.SaveChangesAsync();
                    _notyfService.Success($"Creado nuevo modelo: {producto.Nombre}");
                }
                catch (DbUpdateException ex)
                {
                    _notyfService.Error("Unable to save changes. " +
                                        "Try again, and if the problem persists, " +
                                        "see your system administrator.");
                    return View("Producto", viewModel);
                }

                return RedirectToAction(nameof(Index));
            }

            return View("Producto", viewModel);
        }

        // GET: Productos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            
            AgregarEditarProductoViewModel viewModel = new AgregarEditarProductoViewModel();
            viewModel.ListadoMarcas = new SelectList(_context.Marcas, "Id", "Nombre");
            viewModel.Producto = producto;

            return View("Producto", viewModel);
        }

        // POST: Departamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Producto producto)
        {
            AgregarEditarProductoViewModel viewModel = new AgregarEditarProductoViewModel();
            viewModel.ListadoMarcas = new SelectList(_context.Marcas, "Id", "Nombre");
            viewModel.Producto = producto;
            
            if (id != producto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existeElemento = await _context.Productos.AnyAsync(m =>
                        m.Nombre != null && producto.Nombre != null &&
                        m.Nombre.ToLower() == producto.Nombre.ToLower().Trim());
                    if (existeElemento)
                    {
                        _notyfService.Warning("Ya existe un producto con el mismo nombre.");
                        return View(producto);
                    }

                    _context.Update(producto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcaExists(producto.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            return View("Producto", viewModel);
        }

        // GET: Productos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (producto == null)
            {
                return NotFound();
            }

            return View(producto);
        }

        // POST: Productos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
            }

            await _context.SaveChangesAsync();
            _notyfService.Warning($"Departamento {producto?.Nombre} eliminada.");

            return RedirectToAction(nameof(Index));
        }

        private bool MarcaExists(int id)
        {
            return _context.Departamentos.Any(e => e.Id == id);
        }
    }
}