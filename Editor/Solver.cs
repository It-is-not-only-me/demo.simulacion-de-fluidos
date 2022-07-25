public static class Solver
{
    public static IMatriz LinealSolver(IMatriz matrizA, IMatriz vectorB, int cantidadDeIteracionesMaxima, float errorMaximo)
    {
        if (!CondicionDeValidez(matrizA, vectorB))
            return null;

        uint tamanio = vectorB.Tamanio().Item1;

        IMatriz x = new Vector(tamanio);
        IMatriz r = vectorB.Restar(matrizA.Multiplicar(x));
        IMatriz p = r;

        int iteracionesActual = 0;
        while (r.ModuloInfinito() > errorMaximo && iteracionesActual < cantidadDeIteracionesMaxima)
        {
            float rTransPorR = r.Transponer().Multiplicar(r)[0, 0];
            float alfa = ((rTransPorR) / p.Transponer().Multiplicar(matrizA).Multiplicar(p)[0, 0]);
            x = x.Sumar(p.Multiplicar(alfa));
            r = r.Restar(p.Multiplicar(alfa));

            float beta = ((r.Transponer().Multiplicar(r)[0, 0]) / rTransPorR);
            p = r.Sumar(p.Multiplicar(beta));

            iteracionesActual++;
        }

        return r;
    }

    private static bool CondicionDeValidez(IMatriz matrizA, IMatriz vectorB)
    {
        bool matrizCuadrada = matrizA.EsCuadrada();
        bool mismaDimension = matrizA.SonMultiplicables(vectorB);
        return matrizCuadrada && mismaDimension;
    }
}
