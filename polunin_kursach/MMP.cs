using System;

namespace polunin_kursach
{
    public class MMP
    {
        public static Matrix MMP_Step(Matrix L, Matrix KvInv, Matrix R, Matrix zThetaI, Matrix ThetaI, out Matrix TError)
        {
            Matrix LKvInv = L * KvInv;

            TError = (LKvInv * L.Transpose()).Invert();

            return ThetaI + TError * LKvInv * (R - zThetaI);
        }

        public static Matrix LThetaI(Func<Matrix, Matrix> zThetaI, Matrix ThetaI, int N, int k, double delta)
        {
            Matrix L = new Matrix(k, N);

            for (int i = 0; i < k; i++)
            {
                Matrix thetaPlus = ThetaI.Clone(), thetaMinus = ThetaI.Clone();

                thetaPlus[i, 0] += delta;
                thetaMinus[i, 0] -= delta;

                Matrix zPlus = zThetaI(thetaPlus), zMinus = zThetaI(thetaMinus);

                Matrix dzdti = (1 / (2 * delta)) * (zPlus - zMinus);

                for (int j = 0; j < N; j++)
                    L[i, j] = dzdti[j, 0];
            }

            return L;
        }
    }
}
