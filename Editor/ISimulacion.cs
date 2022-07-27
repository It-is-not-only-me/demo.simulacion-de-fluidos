using UnityEngine;

public interface ISimulacion
{
    public IGrilla EstadoActualDeLaSimulacion();

    public bool AgregarDensidad(uint x, uint y, uint z, float densidad);

    public bool AgregarVelocidad(uint x, uint y, uint z, Vector3 velocidad);

    public void Simular();
}
