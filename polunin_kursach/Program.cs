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
            const int N = 100000;

            int cell = 0;

            for (int i = 0; i < N; i++)
            {
                double num = Gauss.Generate(1);

                if (num >= 0 && num < 1)
                    cell++;
            }

            Console.WriteLine((double)cell / N);
        }
    }
}
