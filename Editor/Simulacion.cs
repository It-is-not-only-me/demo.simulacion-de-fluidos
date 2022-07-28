using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.LinealSolver;

public class Simulacion : ISimulacion
{
    private static float _errorMaximo = 0.0001f;
    
    private IGrilla _grillaActual, _grillaAuxilear;

    private int _cantidadIteraciones;
    private float _coeficienteDeDifusion, _dt;

    private bool _intercambio;

    public Simulacion(IGrilla grillaActual, IGrilla grillaAuxilear, int cantidadIteraciones, float coeficienteDeDifusion, float dt)
    {
        _grillaActual = grillaActual;
        _grillaAuxilear = grillaAuxilear;

        _cantidadIteraciones = cantidadIteraciones;
        _coeficienteDeDifusion = coeficienteDeDifusion;
        _dt = dt;

        _intercambio = false;
    }

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad) => _grillaActual.AgregarDensidad(x, y, z, densidad);
    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad) => _grillaActual.AgregarVelocidad(x, y, z, velocidad);

    public IGrilla EstadoActualDeLaSimulacion() => _intercambio ? _grillaAuxilear : _grillaActual;

    public void Simular()
    {
        if (_intercambio)
        {
            Simular(_grillaAuxilear, _grillaActual);
        }
        else
        {
            Simular(_grillaActual, _grillaAuxilear);
        }            

        _intercambio = !_intercambio;
    }

    public void Simular(IGrilla datosAnteriores, IGrilla datosAConseguir)
    {
        Diffuse(datosAnteriores, datosAConseguir, _coeficienteDeDifusion, _errorMaximo, _cantidadIteraciones);
        Project(datosAnteriores, datosAConseguir);
        Advect(datosAnteriores, datosAConseguir);
        Project(datosAnteriores, datosAConseguir);
    }

    public static void Diffuse(IGrilla datosAnteriores, IGrilla datosAConseguir, float coeficienteDeDifucion, float errorMaximo, int cantidadDeIteraciones)
    {
        Vector3Int tamanio = datosAnteriores.Tamanio;
        Vector3Int tamanioEcuaciones = tamanio - 2 * Vector3Int.one;

        uint cantidadEcuaciones = (uint)((tamanioEcuaciones.x) * (tamanioEcuaciones.y) * (tamanioEcuaciones.z));
        IMatriz matrizDeConstantes = new MatrizDiscreta(cantidadEcuaciones, cantidadEcuaciones);
        List<Vector> vectores = new List<Vector>();

        for (int i = 0; i < 4; i++)
            vectores.Add(new Vector(cantidadEcuaciones));

        float coeficiente = coeficienteDeDifucion / 6;
        List<Vector3Int> desfases = new List<Vector3Int>
        {
            Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down, Vector3Int.forward, Vector3Int.back
        };

        for (int i = 1; i < tamanio.x - 1; i++)
            for (int j = 1; j < tamanio.y - 1; j++)
                for (int k = 1; k < tamanio.z - 1; k++)
                {
                    uint indexActual = Index(i - 1, j - 1, k - 1, tamanioEcuaciones);

                    matrizDeConstantes[indexActual, indexActual] = 1 + coeficienteDeDifucion;
                    DatoSimulacion datoActual = datosAnteriores[(uint)i, (uint)j, (uint)k];
                    List<float> valoresDeDato = new List<float> { datoActual.Densidad, datoActual.Velocidad.x, datoActual.Velocidad.y, datoActual.Velocidad.z };
                    for (int w = 0; w < 4; w++)
                        vectores[w][indexActual] = valoresDeDato[w];

                    foreach (Vector3Int desfase in desfases)
                    {
                        int x = desfase.x, y = desfase.y, z = desfase.z;
                        if (EnBorde(i + x, j + y, k + z, tamanio))
                        {
                            datoActual = datosAnteriores[(uint)(i + x), (uint)(j + y), (uint)(k + z)];
                            valoresDeDato = new List<float> { datoActual.Densidad, datoActual.Velocidad.x, datoActual.Velocidad.y, datoActual.Velocidad.z };
                            for (int w = 0; w < 4; w++)
                                vectores[w][indexActual] += valoresDeDato[w] * coeficiente;
                        }
                        else
                        {
                            uint indexNuevo = Index(i + x - 1, j + y - 1, k + z - 1, tamanioEcuaciones);
                            matrizDeConstantes[indexActual, indexNuevo] = -coeficiente;
                        }
                    }
                }

        List<IMatriz> resultados = new List<IMatriz>();
        for (int i = 0; i < 4; i++)
            resultados.Add(LinealSolver.GradienteConjugado(matrizDeConstantes, vectores[i], cantidadDeIteraciones, errorMaximo));

        for (uint i = 1, contador = 0; i < tamanio.x - 1; i++)
            for (uint j = 1; j < tamanio.y - 1; j++)
                for (uint k = 1; k < tamanio.z - 1; k++, contador++)
                {
                    DatoSimulacion dato = datosAConseguir[i, j, k];
                    dato.Densidad = resultados[0][contador, 0];
                    dato.Velocidad.Set(resultados[1][contador, 0], resultados[2][contador, 0], resultados[3][contador, 0]);
                    datosAConseguir[i, j, k] = dato;
                }
    }

    public static void Advect(IGrilla datosAnteriores, IGrilla datosAConseguir)
    {

    }

    public static void Project(IGrilla datosAnteriores, IGrilla datosAConseguir)
    {

    }

    private static uint Index(int x, int y, int z, Vector3Int tamanio)
    {
        return (uint)(x * tamanio.z * tamanio.y + y * tamanio.z + z);
    }

    private static bool EnBorde(int x, int y, int z, Vector3Int tamanio)
    {
        return x == 0 || y == 0 || z == 0 || x == tamanio.x -1 || y == tamanio.y - 1 || z == tamanio.z - 1;
    }
}
