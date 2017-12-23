using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace polunin_kursach
{
    public class Matrix
    {
        public readonly int n, m;
        double[,] data;

        public Matrix(int n, int m)
        {
            data = new double[n, m];
            this.n = n;
            this.m = m;
        }


        public double this[int i, int j]
        {
            get { return data[i, j]; }
            set { data[i, j] = value; }
        }


        public static Matrix operator *(double lhs, Matrix rhs)
        {
            Matrix res = rhs.Clone();

            for (int i = 0; i < rhs.n; i++)
                for (int j = 0; j < rhs.m; j++)
                    res[i, j] = res[i, j] * lhs;

            return res;
        }

        public static Matrix operator -(Matrix rhs)
        {
            return -1 * rhs;
        }

        public static Matrix operator +(Matrix lhs, Matrix rhs)
        {
            if (lhs.n != rhs.n || lhs.m != rhs.m)
                throw new IncompatibleSizesException();

            Matrix res = lhs.Clone();

            for (int i = 0; i < lhs.n; i++)
                for (int j = 0; j < lhs.m; j++)
                    res[i, j] = lhs[i, j] + rhs[i, j];

            return res;
        }

        public static Matrix operator -(Matrix lhs, Matrix rhs)
        {
            return lhs + -rhs;
        }

        public static Matrix operator *(Matrix lhs, Matrix rhs)
        {
            if (lhs.m != rhs.n)
                throw new IncompatibleSizesException();

            Matrix res = new Matrix(lhs.n, rhs.m);

            for (int i = 0; i < lhs.n; i++)
                for (int j = 0; j < rhs.m; j++)
                    for (int k = 0; k < lhs.m; k++)
                        res[i, j] += lhs[i, k] * rhs[k, j];

            return res;
        }


        public Matrix Transpose()
        {
            Matrix result = new Matrix(m, n);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    result[j, i] = this[i, j];

            return result;
        }


        private void MultiplyRow(int row, double multiplicator)
        {
            for (var j = 0; j < m; j++)
                this[row, j] *= multiplicator;
        }

        private void SwapRows(int row1, int row2)
        {
            double swapBuffer;

            if (row1 == row2)
                return;

            for (var j = 0; j < m; j++)
            {
                swapBuffer = this[row1, j];
                this[row1, j] = this[row2, j];
                this[row2, j] = swapBuffer;
            }
        }

        private void AddMultipliedRow(int dstRow, int srcRow, double multiplicator)
        {
            for (var j = 0; j < m; j++)
                this[dstRow, j] += multiplicator * this[srcRow, j];
        }
        
        private void MakeBasic(int row, int column)
        {
            MultiplyRow(row, 1 / this[row, column]);

            for (int i = 0; i < n; i++)
                if (i != row)
                    AddMultipliedRow(i, row, -this[i, column]);
        }

        public Matrix Invert()
        {
            if (n != m)
                throw new IncompatibleSizesException();

            Matrix system = new Matrix(n, 2 * n);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    system[i, j] = this[i, j];

            for (int i = 0; i < n; i++)
                system[i, n + i] = 1;

            bool[] used = new bool[n];

            for (int j = 0; j < n; j++)
            {
                bool done = false;

                for (int i = 0; i < n; i++)
                    if (system[i, j] != 0 && !used[i])
                    {
                        system.SwapRows(j, i);
                        system.MakeBasic(i, j);
                        used[i] = true;
                        done = true;
                    }

                if (!done)
                    throw new LinearDependencyException();
            }

            Matrix result = new Matrix(n, n);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                    result[i, j] = system[i, n + j];

            return result;
        }


        public double Module()
        {
            double sum = 0;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    sum += this[i, j] * this[i, j];

            return Math.Sqrt(sum);
        }

        
        public Matrix Clone()
        {
            Matrix result = new Matrix(this.n, this.m);

            result.data = (double[,])this.data.Clone();

            return result;
        }

        public static Matrix FromArray(double[] array)
        {
            Matrix result = new Matrix(array.Length, 1);

            for (int i = 0; i < array.Length; i++)
                result[i, 0] = array[i];

            return result;
        }

        public static Matrix FromArray(double[][] array)
        {
            Matrix result = new Matrix(array.Length, array[0].Length);

            for (int i = 0; i < array.Length; i++)
                for (int j = 0; j < array[i].Length; j++)
                    result[i, j] = array[i][j];

            return result;
        }

        public static Matrix FromArray(double[,] array)
        {
            int n = array.GetLength(0);
            int m = array.GetLength(1);

            Matrix result = new Matrix(n, m);

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    result[i, j] = array[i, j];

            return result;
        }

        public override string ToString()
        {
            string result = "";
            int width = 0;

            for (int i = 0; i < n; i++)
                for (int j = 0; j < m; j++)
                    if (this[i, j].ToString().Length > width)
                        width = this[i, j].ToString().Length;

            width += 1;

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                    result += this[i, j].ToString().PadLeft(width);

                for (int j = 0; j < (width + 1) / 1.5; j++)
                    result += '\n';
            }

            return result;
        }

        public void Print()
        {
            Console.WriteLine(ToString());
        }


        public class IncompatibleSizesException : Exception { }

        public class LinearDependencyException : Exception { }
    }
}
