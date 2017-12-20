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
            var fs = new Func<double, double[], double>[] {
                (double x, double[] ys) => 1,
                (double x, double[] ys) => 2 * ys[0],
                (double x, double[] ys) => 3 * ys[1],
                (double x, double[] ys) => 4 * ys[2]
            };

            double[] y0s = new double[] { 0, 0, 0, 0 };
            double[] xs = new double[] { 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1 };

            var X = RungeCutta.Integrate(fs, y0s, xs);

            Console.WriteLine(X);
        }
    }
}
