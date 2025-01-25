using AspNetCoreHero.ToastNotification.Abstractions;
using InventarioMVC.Models;
using InventarioMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCInventarios.Data;
using X.PagedList.Extensions;

namespace InventarioMVC.Controllers
{
    public class DepartamentosController : Controller
    {
        private readonly InventariosContext _context;
        private readonly IConfiguration _configuration;
        private readonly INotyfService _notyfService;

        public DepartamentosController(InventariosContext context, IConfiguration configuration, INotyfService notyfService)
        {
            _context = context;
            _configuration = configuration;
            _notyfService = notyfService;
        }

        // GET: Departamentos
        public async Task<IActionResult> Index(ListadoViewModel<Departamento> viewModel)
        {
            var registrosPorPagina = _configuration.GetValue("RegistrosPorPagina", 10);

            viewModel ??= new ListadoViewModel<Departamento>
            {
                Pagina = 1
            };

            // Asegurar página válida
            var numeroDePagina = Math.Max(viewModel.Pagina, 1);
            var consulta = _context.Departamentos.OrderBy(m => m.Nombre).AsQueryable();
            if (!string.IsNullOrWhiteSpace(viewModel.FiltroBusqueda))
            {
                consulta = consulta.Where(m => m.Nombre.Contains(viewModel.FiltroBusqueda));
            }

            viewModel.Total = await consulta.CountAsync();
            viewModel.Registros = consulta.ToPagedList(numeroDePagina, registrosPorPagina);
            viewModel.TituloCrear = "Crear Departamento";

            return View(viewModel);
        }

        // GET: Departamentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // GET: Departamentos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Departamentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Nombre,Descripcion,FechaCreacion")] Departamento departamento)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var existeElemento = await _context.Departamentos.AnyAsync(m =>
                        m.Nombre != null && departamento.Nombre != null &&
                        m.Nombre.ToLower() == departamento.Nombre.ToLower().Trim());
                    if (existeElemento)
                    {
                        _notyfService.Warning("Ya existe una departamento con el mismo nombre.");
                        return View(departamento);
                    }

                    _context.Add(departamento);
                    await _context.SaveChangesAsync();
                    _notyfService.Success($"Creado nuevo modelo: {departamento.Nombre}");
                }
                catch (DbUpdateException ex)
                {
                    _notyfService.Error("Unable to save changes. " +
                                        "Try again, and if the problem persists, " +
                                        "see your system administrator.");
                }

                return RedirectToAction(nameof(Index));
            }

            return View(departamento);
        }

        // GET: Departamentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos.FindAsync(id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // POST: Departamentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre")] Departamento departamento)
        {
            if (id != departamento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existeElemento = await _context.Departamentos.AnyAsync(m =>
                        m.Nombre != null && departamento.Nombre != null &&
                        m.Nombre.ToLower() == departamento.Nombre.ToLower().Trim());
                    if (existeElemento)
                    {
                        _notyfService.Warning("Ya existe un departamento con el mismo nombre.");
                        return View(departamento);
                    }

                    _context.Update(departamento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarcaExists(departamento.Id))
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

            return View(departamento);
        }

        // GET: Departamentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var departamento = await _context.Departamentos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (departamento == null)
            {
                return NotFound();
            }

            return View(departamento);
        }

        // POST: Departamentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var departamento = await _context.Departamentos.FindAsync(id);
            if (departamento != null)
            {
                _context.Departamentos.Remove(departamento);
            }

            await _context.SaveChangesAsync();
            _notyfService.Warning($"Departamento {departamento?.Nombre} eliminada.");

            return RedirectToAction(nameof(Index));
        }

        private bool MarcaExists(int id)
        {
            return _context.Departamentos.Any(e => e.Id == id);
        }
    }
}