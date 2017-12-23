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
            var xs = Satellite.getGaugings(-6e6, 6e6, 5000, 5000,
                new double[] { 6e6 },
                new double[] { 6e6 },
                600, 10,
                Satellite.LandmarkSelection.NEAREST
            );
        }
    }
}
