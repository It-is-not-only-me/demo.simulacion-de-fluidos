using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class MostrarDatos : GenerarDatos
{
    public override MarchingCubeMesh MarchingCubeMesh => _mesh;
    public override bool Actualizar => _simulacion.Actualizar;


    private Bounds _bounds;
    private SimulacionBehaviour _simulacion;
    private MarchingCubeMesh _mesh;

    private void Awake()
    {
        _simulacion = GetComponent<SimulacionBehaviour>();
        
        _bounds.center = transform.position;
        _bounds.size = _simulacion.TamanioSimulacion;
    }

    private void GenerarMesh()
    {

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
}
