using UnityEngine;

public class GrillaBehaviour : MonoBehaviour, IGrilla
{
    [SerializeField] uint _ancho, _alto, _profundidad;
    
    private IGrilla _grilla;

    public DatoSimulacion this[uint i, uint j, uint k] { get => _grilla[i, j, k]; set => _grilla[i, j, k] = value; }

    public Vector3Int Tamanio => _grilla.Tamanio;

    private void Awake()
    {
        _grilla = NuevaGrillaVacia();
    }

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad) => _grilla.AgregarDensidad(x, y, z, densidad);

    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad) => _grilla.AgregarVelocidad(x, y, z, velocidad);

    public IGrilla NuevaGrillaVacia() => new Grilla(_ancho, _alto, _profundidad);
}