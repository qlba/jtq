using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    class Program
    {
        static void Main(string[] args)
        {
            int k = 3, N = 5;

            // Func<Matrix, Matrix> 

            Func<double, double, double, double, double> funk = (x, a, b, c) => a * x * x + b * x + c;

            Func<Matrix, Matrix> zThetaI = (Matrix ThetaI) => {
                Matrix z = new Matrix(N, 1);

                z[0, 0] = funk(1, ThetaI[0, 0], ThetaI[1, 0], ThetaI[2, 0]);
                z[1, 0] = funk(2, ThetaI[0, 0], ThetaI[1, 0], ThetaI[2, 0]);
                z[2, 0] = funk(3, ThetaI[0, 0], ThetaI[1, 0], ThetaI[2, 0]);
                z[3, 0] = funk(4, ThetaI[0, 0], ThetaI[1, 0], ThetaI[2, 0]);
                z[4, 0] = funk(5, ThetaI[0, 0], ThetaI[1, 0], ThetaI[2, 0]);

                return z;
            };

            double sigma = 0.1;
            Matrix KvInv = new Matrix(N, N);
            for (int i = 0; i < N; i++)
                KvInv[i, i] = 1 / (sigma * sigma);

            Matrix ThetaTrue = new Matrix(k, 1);
            ThetaTrue[0, 0] = -2;
            ThetaTrue[1, 0] = 4;
            ThetaTrue[2, 0] = 7;

            Matrix R = zThetaI(ThetaTrue);
            /*
            R[0, 0] = 64;
            R[1, 0] = 64;
            R[2, 0] = 64;
            R[3, 0] = 64;
            R[4, 0] = 64;
            */

            Matrix Theta0 = new Matrix(k, 1);
            Theta0[0, 0] = 4;
            Theta0[1, 0] = -2;
            Theta0[2, 0] = -2;

            Matrix TError = null;

            for (int i = 0; i < 10; i++)
                Theta0 = MMP.MMP_Step(MMP.LThetaI(zThetaI, Theta0, N, k, 0.1), KvInv, R, zThetaI(Theta0), Theta0, out TError);

            Theta0.Print();
            TError.Print();
        }
    }
}
