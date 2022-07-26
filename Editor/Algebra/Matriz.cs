using System;
using UnityEngine;

public class Matriz : IMatriz
{
    private static float epsilon = 0.001f;

    private float[,] _matriz;
    private uint _tamanioX, _tamanioY;

    public Matriz(uint tamanioX, uint tamanioY)
    {
        _matriz = new float[tamanioX, tamanioY];
        _tamanioX = tamanioX;
        _tamanioY = tamanioY;
    }

    public float this[uint i, uint j] { get => _matriz[i, j]; set => _matriz[i, j] = value; }

    public bool EsCuadrada() => _tamanioX == _tamanioY;
    public bool SonMultiplicables(IMatriz otro) => _tamanioY == otro.Tamanio().Item1;

    public Tuple<uint, uint> Tamanio() => new Tuple<uint, uint>(_tamanioX, _tamanioY);

    public IMatriz Transponer()
    {
        Matriz matrizTranspuesta = new Matriz(_tamanioY, _tamanioX);

        for (uint i = 0; i < _tamanioX; i++)
            for (uint j = 0; j < _tamanioY; j++)
                matrizTranspuesta[j, i] = this[i, j];

        return matrizTranspuesta;
    }

    public IMatriz Multiplicar(IMatriz otro)
    {
        if (!SonMultiplicables(otro))
            return null;

        uint tamanioFinalX = _tamanioX, tamanioFinalY = otro.Tamanio().Item2;
        Matriz resultado = new Matriz(tamanioFinalX, tamanioFinalY);

        uint cantidadIguales = _tamanioY;
        for (uint i = 0; i < tamanioFinalX; i++)
            for (uint j = 0; j < tamanioFinalY; j++)
            {
                for (uint k = 0; k < cantidadIguales; k++)
                    resultado[i, j] += this[i, k] * otro[k, j];
            }

        return resultado;
    }

    public IMatriz Sumar(IMatriz otro)
    {
        if (!SonDelMismoTamanio(otro))
            return null;

        Matriz resultado = new Matriz(_tamanioX, _tamanioY);

        for (uint i = 0; i < _tamanioX; i++)
            for (uint j = 0; j < _tamanioY; j++)
                resultado[i, j] = this[i, j] + otro[i, j];

        return resultado;
    }

    private bool SonDelMismoTamanio(IMatriz otro)
    {
        return Tamanio().Item1 == otro.Tamanio().Item1 && Tamanio().Item2 == Tamanio().Item2;
    }

    public IMatriz Restar(IMatriz otro)
    {
        return Sumar(otro.Multiplicar(-1));
    }

    public IMatriz Multiplicar(float valor)
    {
        Matriz resultado = new Matriz(_tamanioX, _tamanioY);

        for (uint i = 0; i < _tamanioX; i++)
            for (uint j = 0; j < _tamanioY; j++)
                resultado[i, j] = this[i, j] * valor;

        return resultado;
    }

    public float ModuloInfinito()
    {
        float modulo = 0;
        for (uint i = 0; i < _tamanioX; i++)
        {
            float moduloActual = 0;
            for (uint j = 0; j < _tamanioY; j++)
                moduloActual += Mathf.Abs(this[i, j]);
            modulo = Mathf.Max(modulo, moduloActual);
        }
        return modulo;
    }

    public bool EsSimetrica()
    {
        if (!EsCuadrada())
            return false;

        bool esSimetrica = true;
        for (uint i = 0; i < _tamanioX && esSimetrica; i++)
            for (uint j = i + 1; j < _tamanioY && esSimetrica; j++)
                esSimetrica &= (this[i, j] < this[j, i] + epsilon && this[i, j] > this[j, i] - epsilon);
        return esSimetrica;
    }

    public bool EsDiagonalmenteDominante()
    {
        if (!EsCuadrada())
            return false;

        bool esDiagonalmenteDominante = true;
        for (uint i = 0; i < _tamanioX && esDiagonalmenteDominante; i++)
        {
            float valor = 0;
            for (uint j = 0; j < _tamanioY && esDiagonalmenteDominante; j++)
                valor += (i == j) ? 0 : Mathf.Abs(this[i, j]);
            esDiagonalmenteDominante &= Mathf.Abs(this[i, i]) > valor;
        }

        return esDiagonalmenteDominante;
    }
}
