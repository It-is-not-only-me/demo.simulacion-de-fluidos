using UnityEngine;

public interface IGrilla
{
    public DatoSimulacion this[uint i, uint j, uint k] { get; set; }

    public Vector3Int Tamanio { get; }

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad);

    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad);
}
