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
        Matriz A = new Matriz(2, 2);
        A[0, 0] = 10;
        A[0, 1] = 2;
        A[1, 0] = 2;
        A[1, 1] = -8;
        
        Vector b = new Vector(2);
        b[0] = 5;
        b[1] = 2;

        IMatriz resutaldo = Solver.LinealSolver(A, b, 20, 0.01f);

        Debug.Log("x: " + resutaldo[0, 0] + ", y: " + resutaldo[1, 0]);
    }

    private bool EnRango(uint x, uint y, uint z)
    {
        return x < _ancho && y < _alto && z < _profundidad;
    }
}
