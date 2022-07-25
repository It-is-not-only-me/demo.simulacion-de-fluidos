using System;

public class Vector : IMatriz
{
    private Matriz _vector;

    public Vector(uint tamanio)
    {
        _vector = new Matriz(tamanio, 1);
    }

    public float this[uint i, uint j] { get => _vector[i, j]; set => _vector[i, j] = value; }
    public float this[uint i] { get => _vector[i, 0]; set => _vector[i, 0] = value; }

    public bool EsCuadrada() => _vector.EsCuadrada();

    public IMatriz Multiplicar(IMatriz otro) => _vector.Multiplicar(otro);

    public IMatriz Sumar(IMatriz otro) => _vector.Sumar(otro);

    public IMatriz Restar(IMatriz otro) => _vector.Restar(otro);

    public IMatriz Multiplicar(float valor) => _vector.Multiplicar(valor);

    public bool SonMultiplicables(IMatriz otro) => _vector.SonMultiplicables(otro);

    public Tuple<uint, uint> Tamanio() => _vector.Tamanio();

    public IMatriz Transponer() => _vector.Transponer();

    public float ModuloInfinito() => _vector.ModuloInfinito();
}