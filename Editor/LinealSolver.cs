
using UnityEngine;

public static class LinealSolver
{
    // matriz A tiene que ser simetrica y definida positiva
    public static IMatriz GradienteConjugado(IMatriz matrizA, IMatriz vectorB, int cantidadDeIteracionesMaximas, float errorMaximo)
    {
        if (!CondicionDeValidez(matrizA, vectorB))
            return null;

        uint tamanio = vectorB.Tamanio().Item1;

        IMatriz x = new Vector(tamanio);
        IMatriz r = vectorB.Restar(matrizA.Multiplicar(x));
        IMatriz p = r;

        int iteracionesActual = 0;
        while (iteracionesActual < cantidadDeIteracionesMaximas)
        {
            float rTransPorR = (r.Transponer()).Multiplicar(r)[0, 0];
            float alfa = ((rTransPorR) / ((p.Transponer()).Multiplicar(matrizA)).Multiplicar(p)[0, 0]);

            x = x.Sumar(p.Multiplicar(alfa));
            r = r.Restar(matrizA.Multiplicar(p.Multiplicar(alfa)));

            if (r.ModuloInfinito() < errorMaximo)
                break;

            float beta = (((r.Transponer()).Multiplicar(r)[0, 0]) / rTransPorR);
            p = r.Sumar(p.Multiplicar(beta));

            iteracionesActual++;
        }

        Debug.Log("Cantidad de iteracion: " + iteracionesActual + " en Gradiente conjugado");

        return x;
    }

    // matriz A tiene que ser diagonalmente dominante
    public static IMatriz Jacobi(IMatriz matrizA, IMatriz vectorB, int cantidadDeIteracionesMaximas, float errorMaximo)
    {
        if (!CondicionDeValidez(matrizA, vectorB))
            return null;

        uint tamanioX = matrizA.Tamanio().Item1, tamanioY = matrizA.Tamanio().Item2;
        Matriz matrizE = new Matriz(tamanioX, tamanioY);
        Vector vectorC = new Vector(tamanioX);

        for (uint i = 0; i < tamanioX; i++)
        {
            for (uint j = 0; j < tamanioY; j++)
            {
                if (i == j)
                    continue;
                matrizE[i, j] = -matrizA[i, j] / matrizA[i, i];
            }
            vectorC[i] = ((Vector)vectorB)[i] / matrizA[i, i];
        }

        IMatriz xAnterior = vectorC;
        IMatriz x = OperacionIteracion(matrizE, vectorC, xAnterior);

        int iteracionesActual = 1;
        while (CalculoDelError(x, xAnterior) > errorMaximo && iteracionesActual < cantidadDeIteracionesMaximas)
        {
            xAnterior = x;
            x = OperacionIteracion(matrizE, vectorC, x);
            iteracionesActual++;
        }

        Debug.Log("Cantidad de iteracion: " + iteracionesActual + " en Jacobi");

        return x;
    }

    // matriz A tiene que ser diagonalmente dominante
    public static IMatriz GaussSeidel(IMatriz matrizA, IMatriz vectorB, int cantidadDeIteracionesMaximas, float errorMaximo)
    {
        if (!CondicionDeValidez(matrizA, vectorB))
            return null;

        uint tamanio = matrizA.Tamanio().Item1;
        Matriz matrizN = new Matriz(tamanio, tamanio);
        Matriz matrizP = new Matriz(tamanio, tamanio);

        for (uint i = 0; i < tamanio; i++)
            for (uint j = 0; j < tamanio; j++)
            {
                if (i <= j)
                    matrizN[i, j] = matrizA[i, j];
                else
                    matrizP[i, j] = -matrizA[i, j];
            }

        Matriz matrizNInversa = MatrizTrianguladaSuperiorInversa(matrizN);

        IMatriz matrizE = matrizNInversa.Multiplicar(matrizP);
        IMatriz vectorC = matrizNInversa.Multiplicar(vectorB);

        IMatriz xAnterior = vectorC;
        IMatriz x = OperacionIteracion(matrizE, vectorC, xAnterior);

        int iteracionesActual = 1;
        while (CalculoDelError(x, xAnterior) > errorMaximo && iteracionesActual < cantidadDeIteracionesMaximas)
        {
            xAnterior = x;
            x = OperacionIteracion(matrizE, vectorC, x);
            iteracionesActual++;
        }

        Debug.Log("Cantidad de iteracion: " + iteracionesActual + " en GaussSeidel");

        return x;
    }

    private static IMatriz OperacionIteracion(IMatriz matrizE, IMatriz vectorC, IMatriz vectorX)
    {
        return vectorC.Sumar(matrizE.Multiplicar(vectorX));
    }

    private static float CalculoDelError(IMatriz vectorX, IMatriz vectorXAnterior)
    {
        return (vectorX.Restar(vectorXAnterior)).ModuloInfinito();
    }

    private static Matriz MatrizTrianguladaSuperiorInversa(Matriz matriz)
    {
        uint tamanio = matriz.Tamanio().Item1;
        Matriz matrizInversa = new Matriz(tamanio, tamanio);

        for (int x = (int)(tamanio - 1); x >= 0; x--)
            for (int y = (int)(tamanio - 1); y >= x; y--)
            {
                uint i = (uint)x, j = (uint)y;

                matrizInversa[i, j] = 1 / matriz[j, j];
                if (i != j)
                    matrizInversa[i, j] *= -matriz[i, j] / matriz[i, i];
            }

        return matrizInversa;
    }

    private static bool CondicionDeValidez(IMatriz matrizA, IMatriz vectorB)
    {
        bool matrizCuadrada = matrizA.EsCuadrada();
        bool mismaDimension = matrizA.SonMultiplicables(vectorB);
        return matrizCuadrada && mismaDimension;
    }
}
