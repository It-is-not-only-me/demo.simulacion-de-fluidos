using UnityEngine;

public class Grilla : IGrilla
{
    private DatoSimulacion[,,] _datos;
    private uint _ancho, _alto, _profundidad;

    public Grilla(DatoSimulacion[,,] datos)
    {
        _datos = datos;

        _ancho = (uint)_datos.GetLength(0);
        _alto = (uint)_datos.GetLength(1);
        _profundidad = (uint)_datos.GetLength(2);
    }

    public Grilla(uint ancho, uint alto, uint profundidad)
        : this(new DatoSimulacion[ancho, alto, profundidad])
    {
    }

    DatoSimulacion IGrilla.this[uint i, uint j, uint k] { get => _datos[i, j, k]; set => _datos[i, j, k] = value; }

    public Vector3Int Tamanio => new Vector3Int((int)_ancho, (int)_alto, (int)_profundidad);

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

    private bool EnRango(uint x, uint y, uint z)
    {
        return x < _ancho && y < _alto && z < _profundidad;
    }
}
