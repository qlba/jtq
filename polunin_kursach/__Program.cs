using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    class _Program
    {
        static void __Main(string[] args)
        {
            Func<Matrix, Matrix> zThetaI = (Matrix ThetaI) =>
            {
                double x1 = ThetaI[0, 0];
                double x2 = ThetaI[1, 0];

                return Matrix.FromArray(new double[] { x1 * x1 * x1 + x2 * x2 * x2 });
            };

            Matrix L = MMP.getL(
                zThetaI,
                Matrix.FromArray(new double[] { 1, 1 }),
                1,
                0.1
            );

            Console.WriteLine(L);
            Console.WriteLine(L * L.Transpose());

            Matrix cmpi = (L * L.Transpose()).Invert();

            Console.WriteLine("Precision: {0}", cmpi[0, 0] + cmpi[1, 1]);
        }

        static void ___Main(string[] args)
        {
            // Начальные условия
            double xk = -6e6;
            double yk = 6e6;
            double vx = 4000;
            double vy = 4000;

            // Интервал измерений (время в секундах)
            int interval = 80;
            // Количество измерений
            int count = 25;

            // Количество ориентиров
            int landmarksCount = 2;

            // Цена оборудования
            double equipmentCost = 5e5;
            // Цена одного измерения
            double gaugingCost = 4e2;
            // Цена подготовки исходных данных для одного ориентира
            double landmarkCost = 2e4;

            // Среднеквадратическая погрешность (по условию)
            //
            // Задана в угловых минутах/секундах, необходимо
            // перевести в радианы
            // 1 угловая минута -- 60-я доля градуса
            // В 180 градусах 3.14 (ПИ) радианов
            //double sigma = 0.01 * (1.0 / 60) * (Math.PI / 180);
            double sigma = (1.0 / 60) * (Math.PI / 180);

            Landmarks.LandmarkCoords landmarkCoords = Landmarks.setLandmarksGrid(0, 1, landmarksCount);

            // Задаем правило выбора ориентира
            Satellite.LandmarkSelection selection = Satellite.LandmarkSelection.NEAREST;

            Satellite.Gauging[] gaugings = Satellite.getGaugings(
                xk, yk, vx, vy,
                landmarkCoords.x0s, landmarkCoords.y0s,
                interval, count, selection
            );

            double[] _D = Satellite.getZ(gaugings, xk, yk, vx, vy,
                landmarkCoords.x0s, landmarkCoords.y0s
            );

            Matrix R = Matrix.FromArray(_D);

            Matrix KvInv = new Matrix(count, count);

            for (int i = 0; i < count; i++)
                KvInv[i, i] = Math.Pow(sigma, -2);

            Matrix theta = new Matrix(4, 1);
            theta[0, 0] = xk;
            theta[1, 0] = yk;
            theta[2, 0] = vx;
            theta[3, 0] = vy;

            Func<Matrix, Matrix> z = (Matrix _theta) =>
            {
                double _xk = _theta[0, 0];
                double _yk = _theta[1, 0];
                double _vx = _theta[2, 0];
                double _vy = _theta[3, 0];

                return Matrix.FromArray(
                    Satellite.getZ(gaugings, _xk, _yk, _vx, _vy, landmarkCoords.x0s, landmarkCoords.y0s)
                );
            };

            Matrix TError = null;

            try
            {
                Matrix L = MMP.getL(z, theta, count, 100);
                MMP.MMP_Step(L, KvInv, R, z(theta), theta, out TError);

                //Console.WriteLine(Math.Sqrt(TError[0, 0]));
                //Console.WriteLine(Math.Sqrt(TError[1, 1]));
                //Console.WriteLine(Math.Sqrt(TError[2, 2]));
                //Console.WriteLine(Math.Sqrt(TError[3, 3]));


                double precision = Math.Sqrt(TError[2, 2] + TError[3, 3]);
                double cost = equipmentCost + gaugingCost * count + landmarkCost * landmarksCount;


                Console.WriteLine("Погрешность: " + precision + " м/с");
                Console.WriteLine("Стоимость:   " + cost + " у.е.");
            }
            catch(Matrix.DegenerationException ex)
            {
                Console.WriteLine("Невозможно вычислить корреляционную матрицу погрешностей оценок, " +
                    "обратная к ней матрица вырождена.");
            }
        }
    }
}
