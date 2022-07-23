using System.Collections.Generic;
using UnityEngine;

public class SimulacionBehaviour : MonoBehaviour, ISimulacion
{
    [System.Serializable]
    private struct AgregarDensidadEnPunto
    {
        public Vector3Int Punto;
        public float Densidad;
    }

    [System.Serializable]
    private struct AgregarVelocidadEnPunto
    {
        public Vector3Int Punto;
        public Vector3 Velocidad;
    }

    public Vector3Int TamanioSimulacion => _tamanioSimulacion;
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
    [SerializeField] private List<AgregarDensidadEnPunto> _densidadDeInicio;
    [SerializeField] private List<AgregarVelocidadEnPunto> _velocidadDeInicio;

    [Space]

    [SerializeField] private uint _framesParaActualizar;

    private Simulacion _simulacion;
    private bool _actualizar;
    private int _contador;

    private void Awake()
    {
        uint ancho, alto, profundidad;

        ancho = (uint)_tamanioSimulacion.x;
        alto = (uint)_tamanioSimulacion.y;
        profundidad = (uint)_tamanioSimulacion.z;

        _simulacion = new Simulacion(ancho, alto, profundidad);
        foreach (AgregarDensidadEnPunto agregarDensidad in _densidadDeInicio)
        {
            uint x = (uint)agregarDensidad.Punto.x;
            uint y = (uint)agregarDensidad.Punto.y;
            uint z = (uint)agregarDensidad.Punto.z;
            _simulacion.AgregarDensidad(x, y, z, agregarDensidad.Densidad);
        }

        foreach (AgregarVelocidadEnPunto agregarVelocidad in _velocidadDeInicio)
        {
            uint x = (uint)agregarVelocidad.Punto.x;
            uint y = (uint)agregarVelocidad.Punto.y;
            uint z = (uint)agregarVelocidad.Punto.z;
            _simulacion.AgregarVelocidad(x, y, z, agregarVelocidad.Velocidad);
        }

        _actualizar = true;
    }

    private void FixedUpdate()
    {
        _contador++;

        if (_contador % _framesParaActualizar == 0)
        {
            Simular();
            _actualizar = true;
            _contador = 0;
        }
    }

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad) => _simulacion.AgregarDensidad(x, y, z, densidad);
    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad) => _simulacion.AgregarVelocidad(x, y, z, velocidad);
    public void Simular() => _simulacion.Simular();
    public DatoSimulacion[,,] EstadoActualDeLaSimulacion() => _simulacion.EstadoActualDeLaSimulacion();
}