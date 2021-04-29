using System;

namespace MV_Lab1
{
    public class Solution
    {
        private readonly double[,] _a;
        private readonly double[] _y;
        private readonly double[] _b;

        public Solution()
        {
            _a = new double[256, 256];//инициализация матрицы и векторов
            _y = new double[256];
            _b = new double[256];
            FillMatrix();//заполнение матрицы случайными элементами на промежутке [-2^1.25, 2^1.25)
            CalculateB();
        }

        private void FillMatrix()
        {
            var rnd = new Random();
            for (var i = 0; i < 256; ++i)
            {
                _y[i] = GetNextRandom(rnd);
                for (var j = i + 1; j < 256; ++j)
                {
                    _a[i, j] = _a[j, i] = GetNextRandom(rnd);
                    _a[i, i] += Math.Abs(_a[i, j]);
                }
            }
        }

        private void CalculateB()
        {
            for (var i = 0; i < 256; ++i)
            {
                for (var j = 0; j < 256; ++j)
                {
                    _b[i] += _a[i, j] * _y[j];
                }
            }
        }
        private static double GetNextRandom(Random rnd)
        {
            return rnd.NextDouble() * Math.Pow(2, 1.25) * (rnd.NextDouble() > 0.5 ? 1 : -1);
        }

        public (double, double[]) NumberObuslovlennosti(double[,] a, double[] b)
        {
            var a1 = new double[256, 256];//обратная матрица
            var b1 = new double[256];
            b.CopyTo(b1, 0);
            var normA = 0.0;
            var normA1 = 0.0;
            for (var i = 0; i < 256; ++i)
            {
                a1[i, i] = 1;
            }

            for (var i = 0; i < 256; ++i)//прямой ход метода гаусса
            {
                var tempNorm = 0.0;
                for (var j = i + 1; j < 256; ++j)
                {
                    a1[j, i] = -(a[j, i] / a[i, i]);
                    b1[j] -= (a[j, i] / a[i, i]) * b1[i];
                    tempNorm += Math.Abs(a[i, j]);
                }

                if (tempNorm > normA)
                {
                    normA = tempNorm;
                }
            }

            for (var i = 255; i > -1; --i)//обратный ход метода гаусса
            {
                for (var k = 254; k > -1; --k)
                {
                    var tempNorm = 0.0;
                    var minus = a[k, i] / a[k + 1, i];
                    for (var j = 0; j < 256; ++j)
                    {
                        a1[k, j] -= a[k + 1, j] * minus;
                        b1[k] -= b[i] * minus;
                        tempNorm += Math.Abs(a1[k + 1, j]);
                    }

                    b1[i] /= a[i, i];
                    if (tempNorm > normA1)
                    {
                        normA1 = tempNorm;
                    }
                }
            }

            return (normA * normA1, b1);
        }

        private (double[], double[,], double[,], byte[], byte[]) Gauss()
        {
            var matrix = CopyMatrix(_a);
            var L = new double[256, 256];
            var U = new double[256, 256];
            var P1 = new byte[256];
            var P2 = new byte[256];
            var b = new double[256];
            _b.CopyTo(b, 0);
            for (byte i = 0; i < 256; ++i)
            {
                L[i, i] = 1;
                P1[i] = i;
                P2[i] = i;
            }
            for (var i = 1; i < 256; ++i)
            {
                var (item1, item2) = FindMax(matrix, i - 1, i - 1);
                SwapLines(matrix, i - 1, item1);
                SwapRows(matrix, i - 1, item2);
                var (tmp1, tmp2) = (P1[i - 1], P2[i - 1]);
                P1[i - 1] = P1[item1];
                P1[item1] = tmp1;
                P2[i - 1] = P2[item2];
                P2[item2] = tmp2;
                for (var j = i - 1; j < 256; ++j)
                {
                    var minus = matrix[i, i - 1] / matrix[i - 1, i - 1];
                    matrix[i, j] -= matrix[i - i, j] * minus;
                    b[i] -= b[i - 1] * minus;
                    L[i, j] = minus;
                }
            }
            for (var i = 0; i < 256; ++i)
            {
                for (var j = i; j < 256; ++j)
                {
                    U[i, j] = matrix[i, j];
                }
            }
            
            var x = new double[256];
            x[255] = b[255] / matrix[255, 255];
            for (var i = 254; i >= 0; --i)
            {
                for (var j = 255; j >= i; --j)
                {
                    b[i] -= x[i + 1] * matrix[i, j];
                }

                x[i] = b[i] / matrix[i, i];
            }

            return (x, L, U, P1, P2);
        }

        private double[] LupSolution(double[,] L, double[,] U, double[,] P1, double[,] P2)
        {
            var (_, solution) = NumberObuslovlennosti(P1, _b);
            (_, solution) = NumberObuslovlennosti(L, solution);
            (_, solution) = NumberObuslovlennosti(U, solution);
            (_, solution) = NumberObuslovlennosti(P2, solution);
            return solution;
        }

        /*private (double[], double[,], double[]) SquareRootMethod(double[,] A, double[] b)
        {
            
        }

        private double[] ReflectionMethod(double[,] A, double[] b)
        {
            var result = new double[256];
            var matrix = new double[256, 256];
            var _b = new double[256];
            for (var i = 0; i < 256; ++i)
            {
                for (var j = 0; j < 256; ++j)
                {
                    matrix[i, j] = A[i, j];
                }

                _b[i] = b[i];
            }

            for (var i = 255; i > 0; --i)
            {
                var curr_a = new double[i + 1];
                var a = new double[i + 1];
                var norm = 0.0;
                for (var j = i; j > 0; --j)
                {
                    norm += matrix[255 - i, 255 - j] * matrix[255 - i, 255 - j];
                    curr_a[255 - j] = matrix[255 - i, 255 - j];
                }

                a[0] = -Math.Sqrt(norm);
                
            }
        }*/
        private void SwapLines(double[,] matrix, int firstLine, int secondLine)
        {
            for (var i = 0; i < matrix.GetLength(1); ++i)
            {
                var temp = matrix[firstLine, i];
                matrix[firstLine, i] = matrix[secondLine, i];
                matrix[secondLine, i] = temp;
            }
        }

        private void SwapRows(double[,] matrix, int firstRow, int secondRow)
        {
            for (var i = 0; i < matrix.GetLength(0); ++i)
            {
                var temp = matrix[i, firstRow];
                matrix[i, firstRow] = matrix[i, secondRow];
                matrix[i, secondRow] = temp;
            }
        }
        private double[,] CopyMatrix(double[,] matrix)
        {
            var first = matrix.GetLength(0);
            var second = matrix.GetLength(1);
            var result = new double[first, second];
            for (var i = 0; i < first; ++i)
            {
                for (var j = 0; j < second; ++j)
                {
                    result[i, j] = matrix[i, j];
                }
            }

            return result;
        }
        

        private static (int, int) FindMax(double[,] matrix, int first, int second)
        {
            var tuple = (0, 0);
            var max = 0.0;
            for (var i = first; i < matrix.GetLength(0); ++i)
            {
                for (var j = second; j < matrix.GetLength(1); ++j)
                {
                    if (Math.Abs(matrix[i, j]) <= max) continue;
                    max = Math.Abs(matrix[i, j]);
                    tuple.Item1 = i;
                    tuple.Item2 = j;
                }
            }

            return tuple;
        }
        
    }
}