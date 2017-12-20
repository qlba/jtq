using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    class Satellite
    {
        // TODO rename to planet when finished this class
        const double EARTH_RADIUS = 6 * 1e6;
        const double EARTH_GRAVITY = 6;

        const double GAMMA_MASS = -EARTH_GRAVITY * EARTH_RADIUS * EARTH_RADIUS;

        const double PASSBAND = 60 * Math.PI / 180;

        public static double dx(double vx)
        {
            return vx;
        }

        public static double dy(double vy)
        {
            return vy;
        }

        public static double dvx(double x, double y)
        {
            return GAMMA_MASS * x / Math.Pow(x * x + y * y, 1.5);
        }

        public static double dvy(double x, double y)
        {
            return GAMMA_MASS * y / Math.Pow(x * x + y * y, 1.5);
        }

        static Func<double, double[], double>[] derivatives = new Func<double, double[], double>[] {
            (double x, double[] ys) => dx(ys[2]),
            (double x, double[] ys) => dy(ys[3]),
            (double x, double[] ys) => dvx(ys[0], ys[1]),
            (double x, double[] ys) => dvy(ys[0], ys[1])
        };


        public static double phi(double xk, double yk, double x0, double y0)
        {
            return Math.Acos((xk * (xk - x0) + yk * (yk - y0)) /
                (Math.Sqrt(xk * xk + yk * yk) * Math.Sqrt((xk - x0) * (xk - x0) + (yk - y0) * (yk - y0))));
        }


        public static bool inBand(double xk, double yk, double x0, double y0)
        {
            return (x0 * (xk - x0) + y0 * (yk - y0)) / (Math.Sqrt(x0 * x0 + y0 * y0) * Math.Sqrt((xk - x0) * (xk - x0) + (yk - y0) * (yk - y0))) >= Math.Cos(PASSBAND);
        }


        public static void fly()
        {
            double x = 0, y = EARTH_RADIUS + 200, vx = 7071, vy = 0;

            for (int i = 0; i < 3 * 60 * 60; i++)
            {
                double[,] nextSecond = RungeCutta.Integrate(derivatives, new double[] { x, y, vx, vy }, new double[] { 0, 1 });

                x = nextSecond[1, 0];
                y = nextSecond[1, 1];
                vx = nextSecond[1, 2];
                vy = nextSecond[1, 3];

                if (i % 600 == 0)
                    Console.WriteLine("{0,15} {1,15:f1} {2,15:f1}", String.Format("{0:0}:{1:00}", i / 60 / 60, i / 60 % 60), Math.Atan2(y, x) * 180 / Math.PI, Math.Sqrt(x * x + y * y) - EARTH_RADIUS);
                //Console.WriteLine("{0,15} {1,15:f1} {2,15:f1} {3,15:f1} {4,15:f1}", String.Format("{0:0}:{1:00}", i / 60 / 60, i / 60 % 60), x, y, vx, vy);
            }
        }
    }
}
