using EncuentasMVC.Data;
using EncuentasMVC.Models;
using Microsoft.AspNetCore.Mvc;

namespace EncuentasMVC.Controllers;

public class HomeController : Controller
{
    private readonly EncuestaContext _context;

    public HomeController(EncuestaContext encuestaContext)
    {
        _context = encuestaContext;
    }

    public IActionResult Index() => View();

    [HttpGet]
    public IActionResult Encuesta() => View();

    [HttpPost]
    public ViewResult Encuesta(Encuesta encuesta)
    {
        if (encuesta != null && ModelState.IsValid)
        {
            _context.Encuestas.Add(encuesta);
            _context.SaveChanges();
            return View("Gracias", encuesta);
        }
        return View();
    }

    public ViewResult VerAsistencias()
    {
        var asistencias = _context.Encuestas.Where(e => e.Asistencia).ToList();
        return View("VerAsistencias", asistencias);
    }
}
