using System;

public interface IMatriz
{
    public float this[uint i, uint j] { get; set; }

    public bool EsCuadrada();
    public bool EsSimetrica();
    public bool EsDiagonalmenteDominante();

    public IMatriz Multiplicar(IMatriz otro);
    public IMatriz Sumar(IMatriz otro);
    public IMatriz Restar(IMatriz otro);
    public IMatriz Multiplicar(float valor);
    public bool SonMultiplicables(IMatriz otro);
    public Tuple<uint, uint> Tamanio();
    public IMatriz Transponer();
    public float ModuloInfinito();
}