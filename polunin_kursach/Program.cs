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
            Console.WriteLine(Satellite.phi(0.1, 2, 0, 1) * 180 / Math.PI);
            Satellite.fly();
        }
    }
}
