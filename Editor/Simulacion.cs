using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISimulacion
{
    public DatoSimulacion[,,] EstadoActualDeLaSimulacion();

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad);

    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad);

    public void Simular();
}

public class Simulacion : ISimulacion
{
    private DatoSimulacion[,,] _datos;
    private uint _ancho, _alto, _profundidad;

    public Simulacion(DatoSimulacion[,,] datos)
    {
        _datos = datos;

        _ancho = (uint)_datos.GetLength(0);
        _alto = (uint)_datos.GetLength(1);
        _profundidad = (uint)_datos.GetLength(2);
    }

    public Simulacion(uint ancho, uint alto, uint profundidad)
        : this(new DatoSimulacion[ancho, alto, profundidad])
    {
    }

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad)
    {
        if (!EnRango(x, y, z))
            return false;

        DatoSimulacion dato = _datos[x, y, z];
        dato.Densidad = densidad;
        _datos[x, y, z] = dato;

        return true;
    }

    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad)
    {
        if (!EnRango(x, y, z))
            return false;

        DatoSimulacion dato = _datos[x, y, z];
        dato.Velocidad = velocidad;
        _datos[x, y, z] = dato;

        return true;
    }

    public DatoSimulacion[,,] EstadoActualDeLaSimulacion() => _datos;

    public void Simular()
    {
        
    }

    private bool EnRango(uint x, uint y, uint z)
    {
        return x < _ancho && y < _alto && z < _profundidad;
    }
}
