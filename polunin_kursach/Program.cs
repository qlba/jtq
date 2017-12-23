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
            Console.WriteLine(Satellite.gamma(-6e6, 6e6, -4.3e6, 4.3e6));

            try
            {
                var xs = Satellite.getGaugings(-6e6, 6e6, 5000, 5000,
                    new double[] { -4.3e6, 0 },
                    new double[] { 4.3e6, 6e6 },
                    60, 100,
                    Satellite.LandmarkSelection.NEAREST
                );
            }
            catch(Satellite.CannotMakeThisManyGaugings ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
