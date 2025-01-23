namespace EncuentasMVC.Models;

public class ServicioEncuestas
{
    private readonly List<Encuesta> _encuestas = new List<Encuesta>();
    public List<Encuesta> Encuestas => _encuestas;
    public void AgregarEncuesta(Encuesta encuesta)
    {
        _encuestas.Add(encuesta);
    }
}
