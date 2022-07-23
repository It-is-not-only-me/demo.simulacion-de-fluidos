using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class MostrarDatos : GenerarDatos
{
    public override MarchingCubeMesh MarchingCubeMesh => _mesh;
    public override bool Actualizar
    {
        get
        {
            bool seNecesitaActualizar = _simulacion.Actualizar;
            if (seNecesitaActualizar)
                GenerarMesh();
            return seNecesitaActualizar;
        }
    }

    private SimulacionBehaviour _simulacion;
    private MarchingCubeMesh _mesh;

    private void Awake()
    {
        _simulacion = GetComponent<SimulacionBehaviour>();
    }

    private void GenerarMesh()
    {
        Vector3Int puntosPorEje = _simulacion.TamanioSimulacion + Vector3Int.one;
        int cantidadDeDatos = puntosPorEje.x * puntosPorEje.y * puntosPorEje.z;

        DatoSimulacion[,,] datosSimulacion = _simulacion.EstadoActualDeLaSimulacion();

        Dato[] datos = new Dato[cantidadDeDatos];
        Color[] colores = new Color[cantidadDeDatos];

        int contador = 0;
        for (int i = 0; i < puntosPorEje.x; i++)
            for (int j = 0; j < puntosPorEje.y; j++)
                for (int k = 0; k < puntosPorEje.z; k++)
                {
                    colores[contador] = new Color(0.5f, 0.5f, 1);
                    float valor = (i == 0 || j == 0 || k == 0) ? 0 : datosSimulacion[i - 1, j - 1, k - 1].Densidad;
                    datos[contador++].CargarDatos(new Vector3(i, j, k), valor);
                }

        _mesh.Datos = datos;
        _mesh.Colores = colores;

        int cantidadDeindices = ((puntosPorEje.x - 1) * (puntosPorEje.y - 1) * (puntosPorEje.z - 1)) * 8;
        int[] indices = new int[cantidadDeindices];
        for (int i = 0, posicion = 0; i < puntosPorEje.x - 1; i++)
            for (int j = 0; j < puntosPorEje.y - 1; j++)
                for (int k = 0; k < puntosPorEje.z - 1; k++, posicion += 8)
                {
                    indices[posicion] = IndicePorEje(i, j, k, puntosPorEje);
                    indices[posicion + 1] = IndicePorEje(i + 1, j, k, puntosPorEje);
                    indices[posicion + 2] = IndicePorEje(i + 1, j, k + 1, puntosPorEje);
                    indices[posicion + 3] = IndicePorEje(i, j, k + 1, puntosPorEje);
                    indices[posicion + 4] = IndicePorEje(i, j + 1, k, puntosPorEje);
                    indices[posicion + 5] = IndicePorEje(i + 1, j + 1, k, puntosPorEje);
                    indices[posicion + 6] = IndicePorEje(i + 1, j + 1, k + 1, puntosPorEje);
                    indices[posicion + 7] = IndicePorEje(i, j + 1, k + 1, puntosPorEje);
                }

        _mesh.Indices = indices;
    }

    private int IndicePorEje(int x, int y, int z, Vector3Int puntosPorEje)
    {
        return z + y * puntosPorEje.z + x * puntosPorEje.z * puntosPorEje.y;
    }

    private void OnDrawGizmos()
    {
        if (_simulacion == null)
            _simulacion = GetComponent<SimulacionBehaviour>();

        Vector3 ancho = _simulacion.TamanioSimulacion;
        Gizmos.DrawWireCube(transform.position + 0.5f * ancho, ancho);
    }
}
