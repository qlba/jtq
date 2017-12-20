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
            var A = new Matrix(2, 3);
            A[0, 0] = 0;
            A[0, 1] = 2;
            A[0, 2] = 3;
            A[1, 0] = 4;
            A[1, 1] = 5;
            A[1, 2] = 6;

            var B = new Matrix(3, 3);
            B[0, 0] = 7;
            B[0, 1] = 10;
            B[0, 2] = 13;
            B[1, 0] = 8;
            B[1, 1] = 11;
            B[1, 2] = 14;
            B[2, 0] = 9;
            B[2, 1] = 12;
            B[2, 2] = 15;

            (A * B).Print();
        }
    }
}
