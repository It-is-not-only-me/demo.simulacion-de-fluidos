using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ItIsNotOnlyMe.MarchingCubes;

public class MostrarDatos : GenerarDatos
{
    [SerializeField] private Bounds _bounds;

    public override MarchingCubeMesh MarchingCubeMesh => throw new System.NotImplementedException();

    public override Vector3Int NumeroDePuntosPorEje => throw new System.NotImplementedException();

    public override bool Actualizar => throw new System.NotImplementedException();

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(_bounds.center, _bounds.size);
    }
}
