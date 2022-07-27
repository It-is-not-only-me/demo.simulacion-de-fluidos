using UnityEngine;

public class GrillaBehaviour : MonoBehaviour, IGrilla
{
    [SerializeField] uint _ancho, _alto, _profundidad;

    private IGrilla _grillaActual 
    {
        get
        {
            if (_grilla == null)
                _grilla = NuevaGrillaVacia();
            return _grilla;
        }
    }
    private IGrilla _grilla;
    public IGrilla NuevaGrillaVacia() => new Grilla(_ancho, _alto, _profundidad);
    

    public DatoSimulacion this[uint i, uint j, uint k] { get => _grillaActual[i, j, k]; set => _grillaActual[i, j, k] = value; }

    public Vector3Int Tamanio => _grillaActual.Tamanio;

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad) => _grillaActual.AgregarDensidad(x, y, z, densidad);

    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad) => _grillaActual.AgregarVelocidad(x, y, z, velocidad);


    private void OnDrawGizmos()
    {
        Vector3 ancho = Tamanio;
        Gizmos.DrawWireCube(transform.position + 0.5f * ancho, ancho);
    }
}