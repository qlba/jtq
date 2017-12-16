using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    public class Matrix<T>
    {
        private T[,] data;

        public Matrix(int n, int m)
        {
            data = new T[n, m];
        }

        //public T[](int i, int j)
        //    {
        //    }

        public static T operator+(Matrix<T> lhs, Matrix<T> rhs)
        {

        }
    }
}
