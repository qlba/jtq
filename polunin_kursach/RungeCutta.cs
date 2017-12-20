using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    class RungeCutta
    {
        public static double[,] Integrate(Func<double, double[], double>[] fs, double[] y0s, double[] xs)
        {
            int n = xs.Length, m = fs.Length;
            
            double[,] result = new double[n, m];

            for (int j = 0; j < m; j++)
                result[0, j] = y0s[j];

            for (int i = 1; i < n; i++)
            {
                double h = xs[i] - xs[i - 1];


                double[] k1s = new double[m], k1_ys = new double[m];

                for (int j = 0; j < m; j++)
                    k1_ys[j] = result[i - 1, j];

                for (int j = 0; j < m; j++)
                    k1s[j] = fs[j](xs[i - 1], k1_ys);


                double[] k2s = new double[m], k2_ys = new double[m];

                for (int j = 0; j < m; j++)
                    k2_ys[j] = result[i - 1, j] + k1s[j] * h / 2;

                for (int j = 0; j < m; j++)
                    k2s[j] = fs[j](xs[i - 1], k2_ys);


                double[] k3s = new double[m], k3_ys = new double[m];

                for (int j = 0; j < m; j++)
                    k3_ys[j] = result[i - 1, j] + k2s[j] * h / 2;

                for (int j = 0; j < m; j++)
                    k3s[j] = fs[j](xs[i - 1], k3_ys);


                double[] k4s = new double[m], k4_ys = new double[m];

                for (int j = 0; j < m; j++)
                    k4_ys[j] = result[i - 1, j] + k3s[j] * h;

                for (int j = 0; j < m; j++)
                    k4s[j] = fs[j](xs[i - 1], k4_ys);


                for (int j = 0; j < m; j++)
                    result[i, j] = result[i - 1, j] + h * (k1s[j] + 2 * k2s[j] + 2 * k3s[j] + k4s[j]) / 6;
            }

            return result;
        }
    }
}
