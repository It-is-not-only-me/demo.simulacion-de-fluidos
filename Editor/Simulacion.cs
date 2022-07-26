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
        uint tamanio = 5;

        Matriz A = new Matriz(tamanio, tamanio);
        Vector b = new Vector(tamanio);

        List<float> valoresA = new List<float>
        {
            25, 2, 5, 7, 7, 
            2, -15, 2, 2, 4, 
            5, 2, 20, 0, 3,
            7, 2, 0, 20, 7,
            7, 4, 3, 7, 30
        };
        List<float> valoresB = new List<float>
        {
            9, 8, 9, 5, 0
        };

        for (uint i = 0; i < tamanio; i++)
        {
            for (uint j = 0; j < tamanio; j++)
            {
                uint posicion = i * tamanio + j;
                A[i, j] = valoresA[(int)posicion];
            }
            b[i] = valoresB[(int)i];
        }

        IMatriz resultadoConjugado = LinealSolver.GradienteConjugado(A, b, 50, 0.0001f);
        IMatriz resultadoJacobi = LinealSolver.Jacobi(A, b, 50, 0.0001f);
        IMatriz resultadoGaussSeidel = LinealSolver.GaussSeidel(A, b, 50, 0.0001f);

        if (A.EsSimetrica())
            Debug.Log("Es simetrica");
        if (A.EsDiagonalmenteDominante())
            Debug.Log("Es diagonalmente dominante");

        string stringGradienteConjugado = "Gradiente Conjugado =";
        string stringJacobi = "Jacobi = ";
        string stringGaussSeidel = "Gauss Seidel = ";

        for (uint i = 0; i < tamanio; i++)
        {
            stringGradienteConjugado += " [" + i + "]: " + resultadoConjugado[i, 0];
            stringJacobi += " [" + i + "]: " + resultadoJacobi[i, 0];
            stringGaussSeidel += " [" + i + "]: " + resultadoGaussSeidel[i, 0];
        }

        Debug.Log(stringGradienteConjugado);
        Debug.Log(stringJacobi);
        Debug.Log(stringGaussSeidel);
    }

    private bool EnRango(uint x, uint y, uint z)
    {
        return x < _ancho && y < _alto && z < _profundidad;
    }
}
