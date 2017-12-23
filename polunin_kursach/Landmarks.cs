using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    // Ориентиры (landmarks)
    class Landmarks
    {
        // Структура, состоящая из двух массивов: массив
        // абсцисс (x0s) и ординат (y0s) ориентиров.
        //
        // От coordinates, сокращенно coords -- координаты
        public struct LandmarkCoords
        {
            public double[] x0s;
            public double[] y0s;
        }

        // "Расставить"(set) ориентиры(landmarks) на заданном
        // участке планеты на равном расстоянии друг от друга
        // ("сеткой" - grid), начиная с некоторой точки на
        // поверхности планеты.
        //
        // Так как рассматривается сечение планеты, точка на
        // поверхности планеты представляет собой точку на
        // окружности. Ее можно задать одним углом - например,
        // углом, который нужно пройти от северного полюса
        // планеты до этой точки по часовой стрелке.
        //
        // Существуют и другие "расстановки" ориентиров,
        // Но они рассматриваться не будут. 
        //
        // Возвращает массивы координат ориентиров.
        //
        // first -- угол, который нужно пройти от северного
        // полюса планеты до первого(first) ориентира по часовой
        // стрелке (в градусах)
        // distance -- угловое расстояние(distance), которое
        // нужно пройти по часовой стрелке от первого ориентира
        // ко второму, от второго к третьему и т.д. (в градусах)
        // count -- количество(count) ориентиров
        public static LandmarkCoords setLandmarksGrid(
            double first, double distance, int count
        )
        {
            LandmarkCoords result;

            result.x0s = new double[count];
            result.y0s = new double[count];

            // Начало отсчета в северном полюсе -- на оси ординат,
            // и отсчитывается по часовой стрелке. Синус и косинус
            // удобнее вычислять при начале отсчета на оси абсцисс
            // и возрастании угла против часовой стрелки, поэтому
            // перейдем к этой системе отсчета.
            //
            // Для этого нужно вычесть из угла first 90 градусов
            // и изменить его знак. distance же отсчитывается не
            // от полюса, а от предыдущего ориентира, поэтому
            // нужно лишь изменить его знак.
            first = -(first - 90);
            distance = -distance;

            // Вычислим значения углов в радианах.
            // 180 градусов равно 3.14 радианам,
            // следовательно 1 градус равен 3.14/180
            // радианов
            first = first * Math.PI / 180;
            distance = distance * Math.PI / 180;

            // Теперь остается найти координаты каждого из ориентиров.
            for (int i = 0; i < count; i++)
            {
                result.x0s[i] = Satellite.PLANET_RADIUS * Math.Cos(first + i * distance);
                result.y0s[i] = Satellite.PLANET_RADIUS * Math.Sin(first + i * distance);
            }

            return result;
        }

        // Если необходимо вычислить расстояние между ориентирами
        // в метрах по расстоянию в градусах или наоборот, то это
        // можно сделать следующими функциями:
        public static double angleToDistance(double degrees)
        {
            return 2 * Math.PI * Satellite.PLANET_RADIUS * degrees / 360;
        }

        public static double distanceToAngle(double meters)
        {
            return 360 * meters / (2 * Math.PI * Satellite.PLANET_RADIUS);
        }
    }
}
