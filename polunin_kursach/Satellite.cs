using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    class Satellite
    {
        const double PLANET_RADIUS = 6 * 1e6;
        const double PLANET_GRAVITY = 6;

        const double GAMMA_MASS = -PLANET_GRAVITY * PLANET_RADIUS * PLANET_RADIUS;

        const double PASSBAND = 70 * Math.PI / 180;

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


        public static bool inBand(double xk, double yk, double x0, double y0)
        {
            return (x0 * (xk - x0) + y0 * (yk - y0)) / (Math.Sqrt(x0 * x0 + y0 * y0) * Math.Sqrt((xk - x0) * (xk - x0) + (yk - y0) * (yk - y0))) >= Math.Cos(PASSBAND);
        }

        // TODO !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! delete:
        public static double gamma(double xk, double yk, double x0, double y0)
        {
            return Math.Acos(
                (x0 * (xk - x0) + y0 * (yk - y0)) / (Math.Sqrt(x0 * x0 + y0 * y0) * Math.Sqrt((xk - x0) * (xk - x0) + (yk - y0) * (yk - y0)))
            );
        }


        public static double phi(double xk, double yk, double x0, double y0)
        {
            return Math.Acos((xk * (xk - x0) + yk * (yk - y0)) /
                (Math.Sqrt(xk * xk + yk * yk) * Math.Sqrt((xk - x0) * (xk - x0) + (yk - y0) * (yk - y0))));
        }


        public struct GaugingPosition
        {
            // Конструктор структуры
            //
            // Создан для того, чтобы не присваивать поля 
            // по отдельности
            public GaugingPosition(double t, List<int> landmarks)
            {
                this.t = t;
                this.landmarks = landmarks;
            }

            // Момент времени, в который делается (можно сделать)
            // измерение
            public double t;

            // Список ориентиров(landmarks), для которых КА в момент
            // времени t попадает в их 70-градусную область (где нет
            // дополнительных погрешностей)
            public List<int> landmarks;
        }


        // Функция, возвращающая моменты времени, в которые выполняются
        // измерения, и, для каждого момента, по каким ориентирам можно
        // выполнить измерение в этот момент (для каких ориентиров угол
        // ГАММА в этот момент времени не превышает 70 градусов).
        //
        // xk, yk -- координаты КА в условный момент времени 0
        // vx, xy -- скорости КА в условный момент времени 0
        // x0s, y0s -- координаты ориентиров: x0s -- абсциссы, y0s -- ординаты
        // interval -- минимальный интервал времени, с которым производятся
        // измерения. Положения КА рассматриваются с таким интервалом времени.
        public static List<GaugingPosition> getMoments(
            double xk, double yk, double vx, double vy, double[] x0s, double[] y0s, double interval, int count
        ) {
            // За количество ориентиров примем длину массива абсцисс ориентиров
            int orCount = x0s.Length;

            // Пролетал ли КА над данным ориентиром?
            //
            // КА пролетает над ориентиром -- то есть находится в данный
            // момент в области, для которой угол ГАММА относительно
            // данного ориентира не  превышает 70 градусов
            //
            // (где, согласно заданию, измерения могут быть проведены без
            // дополнительных погрешностей -- из других точек мы просто
            // не будем делать измерения)
            bool[] hasFliedOverLandmark = new bool[orCount];


            // Результат
            //
            // Список, в котором содержатся моменты времени,
            // в которые производятся измерения, и 
            List<GaugingPosition> times = new List<GaugingPosition>();


            // Количесиво моментов для интегрирования уравнения движений КА
            // до следующего момента измерения
            //
            // Равно количеству целых секунд, + еще один момент,
            // если интервал не целый
            int h = (int)Math.Ceiling(interval) + 1;

            // В какие моменты времени вычислять (с каким шагом
            // интегрировать) уравнения движения КА для вычисления
            // его положения во время следующего измерения (через
            // интервал)
            //
            // Начинается с нуля, а не с текущего времени t, так
            // как RungeCutta.Integrate берет только разности 
            // между моментами времени xs[1] - xs[0], xs[2] - xs[1], ...
            //
            // Таким образом, [0, 1, 2, ..., interval] эквивалентно
            // [t, t + 1, t + 2, ..., t + interval] с точки зрения
            // RungeCutta.Integrate
            double[] xs = new double[h];

            // Интегрировать с шагом в 1 секунду времени, пока не
            // дойдем до интервала измерений, например:
            //
            // Для interval = 3   в массив записывается [0, 1, 2]
            // Для interval = 3.1                       [0, 1, 2, 3]
            // Для interval = 3.9                       [0, 1, 2, 3]
            for (int i = 0; i < interval; i++)
                xs[i] = i;

            // Последний момент времени равен заданному шагу,
            // который в общем случае можно сделать нецелым.
            xs[h - 1] = interval;
            // После выполнения предыдущей строки кода:
            //
            // Для interval = 3   в массиве содержится [0, 1, 2, 3]
            // Для interval = 3.1                      [0, 1, 2, 3, 3.1]
            // Для interval = 3.9                      [0, 1, 2, 3, 3.9]


            // Счетчик времени, показывает текущее время модели с
            // начального момента, для которого заданы координаты
            // и скорости (переданы через параметры)
            //
            // Положим в начальный момент времени t = 0
            double t = 0;


            for (; ; )
            {
                // КА уже пролетал над всеми ориентирами?
                bool fliedOverAll = true;

                for (int or = 0; or < orCount; or++)
                    if (hasFliedOverLandmark[or] == false)
                    {
                        fliedOverAll = false;
                        break;
                    }

                // В этот список будут помещаться номера ориентиров,
                // над которыми сейчас (при текущем значении t) пролетает КА
                List<int> landmarks = new List<int>();

                for (int or = 0; or < orCount; or++)
                    if (inBand(xk, yk, x0s[or], y0s[or]))
                    {
                        Console.WriteLine("{0,8} {1,16:f} {2,16:f} {3,16:f} {4,16:f}",
                            String.Format("{0}:{1:00}", (int)t / 60 / 60, (int)t / 60 % 60),
                            gamma(xk, yk, x0s[or], y0s[or]) * 180 / Math.PI,
                            phi(xk, yk, x0s[or], y0s[or]) * 180 / Math.PI,
                            Math.Atan2(yk, xk) * 180 / Math.PI,
                            Math.Sqrt(xk * xk + yk * yk) - PLANET_RADIUS
                        );

                        // КА пролетал над ориентиром or по крайней мере сейчас
                        hasFliedOverLandmark[or] = true;

                        // Внесем номер (индекс) ориентира в список
                        landmarks.Add(or);
                    }

                // Если КА сейчас пролетает хотя бы над одним из ориентиров,
                // записать данный момент времени и номера ориентиров, по
                // которым сейчас может быть выполнено измерение угла ФИ
                //
                // Угол ФИ -- измеряемая величина
                if (landmarks.Count > 0)
                    times.Add(new GaugingPosition(t, landmarks));


                // Если КА уже пролетал над всеми ориентирами и уже ни над одним
                // не пролетает, измерения уже нельзя сделать
                if (fliedOverAll && landmarks.Count == 0)
                    break;

                // Процесс может зацикливаться, если есть такой ориентир, над
                // которым КА не пролетает
                //
                // Введем условие остановки вычислений по времени модели t
                //
                // Положим t равным одним суткам, можно выбрать и другую величину
                const double maxTime = 1 * 24 * 60 * 60;

                // Остановить процесс, если текущее время (в модели) больше maxTime
                if (t > maxTime)
                    break;


                // Находим координаты/скорости КА по прошествии интервала измерений
                // (на момент t + interval)
                double[,] nextPosition =
                    RungeCutta.Integrate(derivatives, new double[] { xk, yk, vx, vy }, xs);

                // Обновляем текущие значения координат
                // Значения на последнем шаге интегрирования [h - 1] соответствуют значениям
                // через интервал interval (интервал между измерениями)
                xk = nextPosition[h - 1, 0];
                yk = nextPosition[h - 1, 1];
                vx = nextPosition[h - 1, 2];
                vy = nextPosition[h - 1, 3];

                // Обновляем текущее время
                t += interval;
            }

            return times;
        }


    }
}
