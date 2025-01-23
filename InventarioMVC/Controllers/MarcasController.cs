using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventarioMVC.Models;
using MVCInventarios.Data;
using InventarioMVC.ViewModels;
using X.PagedList.Extensions;

namespace InventarioMVC.Controllers
{
    public class MarcasController : Controller
    {
        private readonly InventariosContext _context;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;

        public MarcasController(InventariosContext context, IConfiguration configuration, INotyfService notyfService)
        {
            _context = context;
            _configuration = configuration;
            _notyfService = notyfService;
        }

        // GET: Marcas
        public async Task<IActionResult> Index(ListadoMarcasViewModel viewModel)
        {
            var registrosPorPagina = _configuration.GetValue("RegistrosPorPagina", 10);
            
            // Si es primera carga (viewModel null) o navegación por paginación
            viewModel ??= new ListadoMarcasViewModel 
            { 
                Pagina = 1  // Página por defecto si es primera carga
            };

            // Asegurar página válida
            var numeroDePagina = Math.Max(viewModel.Pagina, 1);
            var consulta = _context.Marcas.OrderBy(m => m.Nombre).AsQueryable();
            if(!string.IsNullOrWhiteSpace(viewModel.FiltroBusqueda))
            {
                consulta = consulta.Where(m => m.Nombre.Contains(viewModel.FiltroBusqueda)); 
            }
            
            viewModel.Total = await consulta.CountAsync();
            viewModel.Marcas = consulta.ToPagedList(numeroDePagina, registrosPorPagina);

            return View(viewModel);
        }

        // GET: Marcas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }

        // GET: Marcas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Marcas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre")] Marca marca)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existeElemento = await _context.Marcas.AnyAsync(m =>
                        m.Nombre != null && marca.Nombre != null &&
                        m.Nombre.ToLower() == marca.Nombre.ToLower().Trim());
                    if (existeElemento)
                    {
                        // ModelState.AddModelError("", "Ya existe una marca con el mismo nombre.");

                        _notyfService.Warning("Ya existe una marca con el mismo nombre.");
                        return View(marca);
                    }

                    _context.Add(marca);
                    await _context.SaveChangesAsync();
                    _notyfService.Success($"Creado nuevo modelo: {marca.Nombre}");
                }
                catch (DbUpdateException ex)
                {
                    _notyfService.Error("Unable to save changes. " +
                                        "Try again, and if the problem persists, " +
                                        "see your system administrator.");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(marca);
        }

        // GET: Marcas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }

        // POST: Marcas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Marca marca)
        {
            if (id != marca.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existeElemento = await _context.Marcas.AnyAsync(m =>
                        m.Nombre != null && marca.Nombre != null &&
                        m.Nombre.ToLower() == marca.Nombre.ToLower().Trim());
                    if (existeElemento)
                    {
                        // ModelState.AddModelError("", "Ya existe una marca con el mismo nombre.");
                        _notyfService.Warning("Ya existe una marca con el mismo nombre.");

                        return View(marca);
                    }

                    _context.Update(marca);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcaExists(marca.Id))
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

            return View(marca);
        }

        // GET: Marcas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var marca = await _context.Marcas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (marca == null)
            {
                return NotFound();
            }

            return View(marca);
        }

        // POST: Marcas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca != null)
            {
                _context.Marcas.Remove(marca);
            }

            await _context.SaveChangesAsync();
            _notyfService.Warning($"Marca {marca?.Nombre} eliminada.");

            return RedirectToAction(nameof(Index));
        }

        private bool MarcaExists(int id)
        {
            return _context.Marcas.Any(e => e.Id == id);
        }
    }
}
