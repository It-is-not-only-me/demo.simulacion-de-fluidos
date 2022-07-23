using UnityEngine;

public class SimulacionBehaviour : MonoBehaviour, ISimulacion
{
    public Vector3 TamanioSimulacion => _tamanioSimulacion;
    public bool Actualizar
    {
        get
        {
            bool actualizar = _actualizar;
            _actualizar &= false;
            return actualizar;
        }
    }

    [SerializeField] private Vector3Int _tamanioSimulacion;

    private Simulacion _simulacion;
    private bool _actualizar;

    private void Awake()
    {
        uint ancho, alto, profundidad;

        ancho = (uint)_tamanioSimulacion.x;
        alto = (uint)_tamanioSimulacion.y;
        profundidad = (uint)_tamanioSimulacion.z;

        _simulacion = new Simulacion(ancho, alto, profundidad);

        _actualizar = true;
    }

    public Dato[,,] EstadoActualDeLaSimulacion() => _simulacion.EstadoActualDeLaSimulacion();
}