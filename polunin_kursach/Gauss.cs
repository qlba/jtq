using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    class Gauss
    {
        static Random rnd = new Random();

        const int N = 6;

        public static double Generate(double sigma)
        {
            double result = 0;

            for (int i = 0; i < N; i++)
                result += rnd.NextDouble();

            result -= N / 2.0;
            result /= Math.Sqrt(N / 12.0);

            return result * sigma;
        }
    }
}
