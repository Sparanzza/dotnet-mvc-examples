using System.ComponentModel.DataAnnotations;

namespace EncuentasMVC.Models;
public class Encuesta
{
    public int Id { get; set; }
    
    [Required(ErrorMessage = "Por favor, ingrese su nombre")]
    public required string Nombre { get; set; }

    [Required(ErrorMessage = "Por favor, ingrese su email")]
    [EmailAddress(ErrorMessage = "Por favor, ingrese un email válido")]
    public required string Email { get; set; }
    
    public bool Asistencia { get; set; } = false;

}
