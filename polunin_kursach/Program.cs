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
            // Начальные условия (по заданию)
            double x0 = -6e6;
            double y0 = 6e6;
            double vx = 5000;
            double vy = 5000;

            // Задаем интервал (время в секундах) и количество измерений
            int interval = 60;
            int count = 20;

            // Среднеквадратическая погрешность (по условию)
            //
            // Задана в угловых минутах/секундах, необходимо
            // перевести в радианы
            // 1 угловая минута -- 60-я доля градуса
            // В 180 градусах 3.14 (ПИ) радианов
            double sigma = (1 / 60) * (Math.PI / 180);

            // "Расставляем" ориентиры
            Landmarks.LandmarkCoords landmarkCoords = Landmarks.setLandmarksGrid(-40, 1, 10);

            // Задаем правило выбора ориентира
            Satellite.LandmarkSelection selection = Satellite.LandmarkSelection.NEAREST;


            // При известных начальных условиях вычисляем моменты
            // измерений и ориентиры, по которым каждое измерение
            // производится
            Satellite.Gauging[] gaugings = Satellite.getGaugings(
                x0, y0, vx, vy,
                landmarkCoords.x0s, landmarkCoords.y0s,
                interval, count, selection
            );

            // При известных значениях оцениваемых параметров -- начальных
            // условиях -- вычисляем математическую модель вектора измерений
            // z(тета_ист), где тета_ист -- истинные значения оцениваемых
            // параметров, указанные в задании. Этот вектор обозначается D.
            double[] _D = Satellite.getZ(gaugings, x0, y0, vx, vy,
                landmarkCoords.x0s, landmarkCoords.y0s
            );

            // Satellite.getZ возвращает массив, но для работы с ним
            // необходимо преобразовать его в матрицу.
            Matrix D = Matrix.FromArray(_D);


            // Далее мы забываем истинные значения оцениваемых параметров,
            // то есть начальных скоростей/координат КА xk, yk, vx, vy и
            // пытаемся их найти при помощи метода максимального
            // правдоподобия (ММП).


            // Сформируем вектор-столбец погрешностей
            Matrix v = new Matrix(count, 1);

            for (int i = 0; i < count; i++)
                v[i, 0] = Gauss.Generate(sigma);

            // Получаем вектор измерений с заданными погрешностями
            Matrix R = D + v;


            // Попытаемся подобрать начальное приближение оцениваемых
            // параметров, то есть такие значения xk, yk, vx, vy,
            // которые кажутся нам наиболее близкими к истинным.

            // По условию мы должны найти начальные значения координат
            // и скоростей КА на момент первого измерения. Следовательно,
            // Мы предполагаем, что КА находится над точкой, где расположен
            // ориентир, по которому мы сделали первое измерение. Если
            // умножить координаты ориентира на число k, получим точку,
            // которая в k раз дальше от центра планеты, чем этот ориентир.
            // Положим k = 2.
            x0 = 2 * landmarkCoords.x0s[gaugings[0].landmark];
            y0 = 2 * landmarkCoords.y0s[gaugings[0].landmark];

            // Скорости == 0??
            vx = 0;
            vy = 0;




            try
            {
            }
            catch(Satellite.CannotMakeThisManyGaugings ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
