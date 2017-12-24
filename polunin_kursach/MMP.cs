using System;

namespace polunin_kursach
{
    public class MMP
    {
        public static Matrix MMP_Step(Matrix L, Matrix KvInv, Matrix R, Matrix z_i, Matrix theta, out Matrix thetasSigma)
        {
            Matrix LKvInv = L * KvInv;

            thetasSigma = (LKvInv * L.Transpose()).Invert2();

            return theta + thetasSigma * LKvInv * (R - z_i);
        }

        public static Matrix getL(Func<Matrix, Matrix> getZ, Matrix theta, int N, double delta)
        {
            int k = theta.n;

            Matrix L = new Matrix(k, N);

            for (int i = 0; i < k; i++)
            {
                Matrix thetaPlus = theta.Clone(), thetaMinus = theta.Clone();

                thetaPlus[i, 0] += delta;
                thetaMinus[i, 0] -= delta;

                Matrix zPlus = getZ(thetaPlus), zMinus = getZ(thetaMinus);

                Matrix dzdti = (1 / (2 * delta)) * (zPlus - zMinus);

                for (int j = 0; j < N; j++)
                    L[i, j] = dzdti[j, 0];
            }

            return L;
        }
    }
}
