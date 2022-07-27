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
            Diffuse(_grillaAuxilear, _grillaActual, _coeficienteDeDifusion, _errorMaximo, _cantidadIteraciones);
        }
        else
        {
            Diffuse(_grillaActual, _grillaAuxilear, _coeficienteDeDifusion, _errorMaximo, _cantidadIteraciones);
        }
            

        _intercambio = !_intercambio;
    }

    public static void Diffuse(IGrilla datosAnteriores, IGrilla datosAConseguir, float coeficienteDeDifucion, float errorMaximo, int cantidadDeIteraciones)
    {
        Vector3Int tamanio = datosAnteriores.Tamanio;
        Vector3Int tamanioEcuaciones = tamanio - 2 * Vector3Int.one;

        uint cantidadEcuaciones = (uint)((tamanioEcuaciones.x) * (tamanioEcuaciones.y) * (tamanioEcuaciones.z));
        MatrizDiscreta matrizDeConstantes = new MatrizDiscreta(cantidadEcuaciones, cantidadEcuaciones);
        Vector vector = new Vector(cantidadEcuaciones);

        float coeficiente = coeficienteDeDifucion / (6 + 6 * coeficienteDeDifucion);
        for (int i = 1; i < tamanio.x - 1; i++)
            for (int j = 1; j < tamanio.y - 1; j++)
                for (int k = 1; k < tamanio.z - 1; k++)
                {
                    uint indexActual = Index(i - 1, j - 1, k - 1, tamanioEcuaciones);

                    matrizDeConstantes[indexActual, indexActual] = 1;
                    vector[indexActual] = datosAnteriores[(uint)i, (uint)j, (uint)k].Densidad;

                    for (int x = -1; x <= 1; x += 2)
                        for (int y = -1; y <= 1; y += 2)
                            for (int z = -1; z <= 1; z += 2)
                            {
                                if (EnBorde(i + x, j + y, k + z, tamanio))
                                {
                                    vector[indexActual] += datosAnteriores[(uint)i, (uint)j, (uint)k].Densidad * coeficiente;
                                }
                                else
                                {
                                    uint indexNuevo = Index(i - 1 + x, j - 1 + y, k - 1 + z, tamanioEcuaciones);
                                    matrizDeConstantes[indexActual, indexNuevo] = -coeficiente;
                                }
                            }
                }

        IMatriz resultados = LinealSolver.GradienteConjugado(matrizDeConstantes, vector, cantidadDeIteraciones, errorMaximo);
        for (int i = 1, contador = 0; i < tamanio.x - 1; i++)
            for (int j = 1; j < tamanio.y - 1; j++)
                for (int k = 1; k < tamanio.z - 1; k++, contador++)
                {
                    DatoSimulacion dato = datosAConseguir[(uint)i, (uint)j, (uint)k];
                    dato.Densidad = resultados[(uint)contador, 0];
                    datosAConseguir[(uint)i, (uint)j, (uint)k] = dato;
                }
    }

    private static uint Index(int x, int y, int z, Vector3Int tamanio)
    {
        return (uint)(x * tamanio.z * tamanio.y + y * tamanio.z + z);
    }

    private static bool EnBorde(int x, int y, int z, Vector3Int tamanio)
    {
        return x == 0 || y == 0 || z == 0 || x == tamanio.x || y == tamanio.y || z == tamanio.z;
    }
}
