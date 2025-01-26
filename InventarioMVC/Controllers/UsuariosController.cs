using AspNetCoreHero.ToastNotification.Abstractions;
using InventarioMVC.Models;
using InventarioMVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using InventarioMVC.Helpers;
using MVCInventarios.Data;
using X.PagedList.EntityFramework;
using X.PagedList.Extensions;

namespace InventarioMVC.Controllers;

//[Authorize(Roles = "Administrador")]
//[Authorize(Policy = "Administradores")]
public class UsuariosController : Controller
{
    private readonly InventariosContext _context;
    private readonly IConfiguration _configuration;
    private readonly INotyfService _servicioNotificacion;
    private readonly UsuarioFactoria _usuarioFactoria;

    public UsuariosController(InventariosContext context,
        IConfiguration configuration,
        INotyfService servicioNotificacion,
        UsuarioFactoria usuarioFactoria)
    {
        _context = context;
        _configuration = configuration;
        _servicioNotificacion = servicioNotificacion;
        _usuarioFactoria = usuarioFactoria;
    }


    public async Task<IActionResult> Index(ListadoViewModel<Usuario> viewModel)
    {
        var registrosPorPagina = _configuration.GetValue("RegistrosPorPagina", 5);

        var consulta = _context.Usuarios
            .Include(u => u.Perfil)
            .OrderBy(m => m.Nombre)
            .AsQueryable()
            .AsNoTracking();

        if (!String.IsNullOrEmpty(viewModel.FiltroBusqueda))
        {
            consulta = consulta.Where(u => u.Nombre.Contains(viewModel.FiltroBusqueda)
                                           || u.Username.Contains(viewModel.FiltroBusqueda));
        }

        viewModel.TituloCrear = "Crear Usuario";
        viewModel.Total = consulta.Count();
        var numeroPagina = viewModel?.Pagina ?? 1;

        if (numeroPagina < 1)
        {
            numeroPagina = 1;
        }
        
        viewModel.Registros = consulta.ToPagedList(numeroPagina, registrosPorPagina);

        return View(viewModel);
    }


    // GET: Usuarios/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    // GET: Usuarios/Create
    public IActionResult Create()
    {
        AgregarUsuarioViewModel viewModel = new AgregarUsuarioViewModel();
        viewModel.ListadoPerfiles = new SelectList(_context.Perfiles.AsNoTracking(), "Id", "Nombre");
        return View(viewModel);
    }

    // POST: Usuarios/Create
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("Nombre,Apellidos,Username,Contrasena,ConfirmarContrasena,CorreoElectronico,Celular,PerfilId")]
        UsuarioRegistroDto usuario)
    {
        AgregarUsuarioViewModel viewModel = new AgregarUsuarioViewModel();
        viewModel.ListadoPerfiles = new SelectList(_context.Perfiles.AsNoTracking(), "Id", "Nombre");
        viewModel.Usuario = usuario;

        if (ModelState.IsValid)
        {
            var existeUsuarioBd = _context.Usuarios
                .Any(u => u.Username.ToLower().Trim() == usuario.Username.ToLower().Trim());

            if (existeUsuarioBd)
            {
                ModelState.AddModelError("Usuario.Username", $"Ya existe un usuario con la cuenta {usuario.Username}.");
                _servicioNotificacion.Warning($"Ya existe un usuario con la cuenta {usuario.Username}");
                return View(viewModel);
            }

            try
            {
                var usuarioAgregar = _usuarioFactoria.CrearUsuario(usuario);
                _context.Usuarios.Add(usuarioAgregar);
                await _context.SaveChangesAsync();
                _servicioNotificacion.Success($"ÉXITO al crear el usuario {usuario.Username}");
            }
            catch (DbUpdateException)
            {
                _servicioNotificacion.Warning("Lo sentimos, ha ocurrido un error. Intente nuevamente.");
                return View(viewModel);
            }

            return RedirectToAction(nameof(Index));
        }

        return View(viewModel);
    }

    // GET: Usuarios/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            return NotFound();
        }

        EditarUsuarioViewModel viewModel = new EditarUsuarioViewModel();
        viewModel.ListadoPerfiles = new SelectList(_context.Perfiles.AsNoTracking(), "Id", "Nombre");
        viewModel.Usuario = _usuarioFactoria.CrearUsuarioEdicion(usuario);
        return View(viewModel);
    }

    // POST: Usuarios/Edit/5
    // To protect from overposting attacks, enable the specific properties you want to bind to.
    // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id,
        [Bind("Id,Nombre,Apellidos,Username,CorreoElectronico,Celular,PerfilId")] UsuarioEdicionDto usuario)
    {
        EditarUsuarioViewModel viewModel = new EditarUsuarioViewModel();
        viewModel.ListadoPerfiles = new SelectList(_context.Perfiles.AsNoTracking(), "Id", "Nombre");
        viewModel.Usuario = usuario;

        if (id != usuario.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                var usuarioBd = await _context.Usuarios.FindAsync(usuario.Id);
                _usuarioFactoria.ActualizarDatosUsuario(usuario, usuarioBd);
                await _context.SaveChangesAsync();
                _servicioNotificacion.Success($"ÉXITO al actualizar el usuario {usuario.Username}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(usuario.Id))
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

        return View(viewModel);
    }

    // GET: Usuarios/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (usuario == null)
        {
            return NotFound();
        }

        return View(usuario);
    }

    // POST: Usuarios/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        _servicioNotificacion.Success($"ÉXITO al eliminar el usuario {usuario.Username}");
        return RedirectToAction(nameof(Index));
    }

    private bool UsuarioExists(int id)
    {
        return _context.Usuarios.Any(e => e.Id == id);
    }


    public async Task<IActionResult> CambiarContrasena(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var usuario = await _context.Usuarios
            .Include(u => u.Perfil)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (usuario == null)
        {
            return NotFound();
        }

        CambiarContrasenaViewModel viewModel = _usuarioFactoria.CrearCambiarContrasenaViewModel(usuario);
        return View(viewModel);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CambiarContrasena(int id,
        [Bind("Id,Username,Contrasena,ConfirmarContrasena")] CambiarContrasenaViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var usuarioBd = await _context.Usuarios.FindAsync(viewModel.Id);
                _usuarioFactoria.ActualizarContrasenaUsuario(viewModel, usuarioBd);
                await _context.SaveChangesAsync();
                _servicioNotificacion.Success($"ÉXITO al actualizar la contraseña del usuario: {usuarioBd.Username}");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UsuarioExists(viewModel.Id))
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

        return View(viewModel);
    }
}