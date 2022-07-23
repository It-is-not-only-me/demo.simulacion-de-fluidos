using System.Collections;
using System.Collections.Generic;

public interface ISimulacion
{
    public Dato[,,] EstadoActualDeLaSimulacion();
}

public class Simulacion : ISimulacion
{
    private Dato[,,] _datos;
    private uint _ancho, _alto, _profundidad;

    public Simulacion(Dato[,,] datos)
    {
        _datos = datos;

        _ancho = (uint)_datos.GetLength(0);
        _alto = (uint)_datos.GetLength(1);
        _profundidad = (uint)_datos.GetLength(2);
    }

    public Simulacion(uint ancho, uint alto, uint profundidad)
        : this(new Dato[ancho, alto, profundidad])
    {
    }

    

    public Dato[,,] EstadoActualDeLaSimulacion() => _datos;
}
