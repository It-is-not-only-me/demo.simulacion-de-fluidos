using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
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
        Project(datosAnteriores, datosAConseguir, _errorMaximo, _cantidadIteraciones);
        Advect(datosAnteriores, datosAConseguir);
        Project(datosAnteriores, datosAConseguir, _errorMaximo, _cantidadIteraciones);
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
                    List<float> valoresDeDato = DatosIndividuales(datosAnteriores, (uint)i, (uint)j, (uint)k);
                    for (int w = 0; w < 4; w++)
                        vectores[w][indexActual] = valoresDeDato[w];

                    foreach (Vector3Int desfase in desfases)
                    {
                        int x = desfase.x, y = desfase.y, z = desfase.z;
                        if (EnBorde(i + x, j + y, k + z, tamanio))
                        {
                            valoresDeDato = DatosIndividuales(datosAnteriores, (uint)(i + x), (uint)(j + y), (uint)(k + z));
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

    public static void Project(IGrilla datosAnteriores, IGrilla datosAConseguir, float errorMaximo, int cantidadDeIteraciones)
    {
        Vector3Int tamanio = datosAnteriores.Tamanio;
        Vector3Int tamanioEcuaciones = tamanio - 2 * Vector3Int.one;

        uint cantidadEcuaciones = (uint)((tamanioEcuaciones.x) * (tamanioEcuaciones.y) * (tamanioEcuaciones.z));
        IMatriz matrizDeFuncionP = new MatrizDiscreta(cantidadEcuaciones, cantidadEcuaciones);
        Vector vector = new Vector(cantidadEcuaciones);

        List<Vector3Int> desfases = new List<Vector3Int>
        {
            Vector3Int.right, Vector3Int.left, Vector3Int.up, Vector3Int.down, Vector3Int.forward, Vector3Int.back
        };

        for (int i = 1; i < tamanio.x - 1; i++)
            for (int j = 1; j < tamanio.y - 1; j++)
                for (int k = 1; k < tamanio.z - 1; k++)
                {
                    uint indexActual = Index(i - 1, j - 1, k - 1, tamanioEcuaciones);
                    matrizDeFuncionP[indexActual, indexActual] = -6.0f;
                    vector[indexActual] = GradienteDeVelocidad(datosAnteriores, (uint)i, (uint)j, (uint)k);

                    foreach (Vector3Int desfase in desfases)
                    {
                        int x = desfase.x, y = desfase.y, z = desfase.z;
                        uint indexNuevo;

                        if (EnBorde(i + x, j + y, k + z, tamanio))
                        {
                            indexNuevo = Index(i - x - 1, j - y - 1, k - z - 1, tamanioEcuaciones);

                            int componenteVelocidad = (Mathf.Abs(x) + Mathf.Abs(y) * 2 + Mathf.Abs(z) * 3) - 1;
                            float velocidad = datosAnteriores[(uint)(i + x), (uint)(j + y), (uint)(k + z)].Velocidad[componenteVelocidad];

                            vector[indexActual] += 2 * (EnBordeInferior(i + x, j + y, k + z) ? -velocidad : velocidad);
                        }
                        else
                        {
                            indexNuevo = Index(i + x - 1, j + y - 1, k + z - 1, tamanioEcuaciones);
                        }
                        
                        matrizDeFuncionP[indexActual, indexNuevo] += 1;
                    }
                }

        IMatriz valoresDeP = LinealSolver.GaussSeidel(matrizDeFuncionP, vector, cantidadDeIteraciones, errorMaximo);

        for (int i = 1; i < tamanio.x - 1; i++)
            for (int j = 1; j < tamanio.y - 1; j++)
                for (int k = 1; k < tamanio.z - 1; k++)
                {
                    DatoSimulacion dato = datosAConseguir[(uint)i, (uint)j, (uint)k];

                    for (int w = 0; w < 3; w++)
                    {
                        int x = 0, y = 0, z = 0;
                        if (EnBorde(i + x, j + y, k + z, tamanio) || EnBorde(i - x, j - y, k - z, tamanio))
                        {
                            dato.Velocidad[w] = datosAnteriores[(uint)i, (uint)j, (uint)k].Velocidad[w];
                        }
                        else
                        {
                            uint indexAdelante = Index(i + x - 1, j + y - 1, k + z - 1, tamanioEcuaciones);
                            uint indexAtras = Index(i + x - 1, j + y - 1, k + z - 1, tamanioEcuaciones);
                            dato.Velocidad[w] = (valoresDeP[indexAdelante, 0] -valoresDeP[indexAtras, 0]) / 2;
                        }
                    }
                    datosAConseguir[(uint)i, (uint)j, (uint)k] = dato;
                }
    }

    private static float GradienteDeVelocidad(IGrilla datos, uint x, uint y, uint z)
    {

        float gradiente = 0;

        gradiente += (datos[x + 1, y, z].Velocidad.x - datos[x - 1, y, z].Velocidad.x) / 2;
        gradiente += (datos[x, y + 1, z].Velocidad.y - datos[x, y + 1, z].Velocidad.y) / 2;
        gradiente += (datos[x, y, z + 1].Velocidad.z - datos[x, y, z + 1].Velocidad.z) / 2;

        return gradiente;
    }

    private static List<float> DatosIndividuales(IGrilla datos, uint i, uint j, uint k)
    {
        DatoSimulacion dato = datos[i, j, k];
        List<float> valores = GenericPool<List<float>>.Get();
        valores.Add(dato.Densidad);
        for (int w = 0; w < 3; w++)
            valores.Add(dato.Velocidad[w]);
        return valores;
    }

    private static uint Index(int x, int y, int z, Vector3Int tamanio)
    {
        return (uint)(x * tamanio.z * tamanio.y + y * tamanio.z + z);
    }

    private static bool EnBorde(int x, int y, int z, Vector3Int tamanio)
    {
        return EnBordeInferior(x, y, z) || EnBordeSuperior(x, y, z, tamanio);
    }

    private static bool EnBordeInferior(int x, int y, int z) => x == 0 || y == 0 || z == 0;

    private static bool EnBordeSuperior(int x, int y, int z, Vector3Int tamanio) => x == tamanio.x - 1 || y == tamanio.y - 1 || z == tamanio.z - 1;
}
