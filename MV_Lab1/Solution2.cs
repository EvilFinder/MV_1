using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SolverFoundation.Common;
using System.Reflection.Metadata;

namespace MV_Lab1
{
    public class Solution2
    {
        public double[][] _a { get; private set; }//буду хранить стобцы в строках и наоборот(матрица симметричная)
        public double[] _y { get; }
        public double[] _b { get; private set; }
        
        public double[][] L { get; private set; }
        
        public double[][] U { get; private set; }
        
        public byte[] P1 { get; private set; }
        
        public byte[] P2 { get; private set; }
        public Solution2()
        {
            _a = new double[256][];//инициализация матрицы и векторов
            for (var i = 0; i < 256; ++i)
            {
                _a[i] = new double[256];
            }
            _y = new double[256];
            _b = new double[256];
            FillMatrix();//заполнение матрицы случайными элементами на промежутке [-2^1.25, 2^1.25)
            CalculateB();
            (_, L, U, P1, P2) = Gauss(_a, _b);
        }
        
        private void FillMatrix()
        {
            var rnd = new Random();
            for (var i = 0; i < 256; ++i)
            {
                _y[i] = GetNextRandom(rnd);
                for (var j = i + 1; j < 256; ++j)
                {
                    _a[i][j] = _a[j][i] = GetNextRandom(rnd);
                }

                for (var j = 0; j < 256; ++j)
                {
                    if (j == i) continue;
                    _a[i][i] += Math.Abs(_a[i][j]);
                }
            }
        }
        
        private void CalculateB()
        {
            for (var i = 0; i < 256; ++i)
            {
                for (var j = 0; j < 256; ++j)
                {
                    _b[i] += _a[i][j] * _y[j];
                }
            }
        }
        
        private static double GetNextRandom(Random rnd)
        {
            return rnd.NextDouble() * Math.Pow(2, 1.5) * (rnd.NextDouble() > 0.5 ? 1 : -1);
        }
        
        public (double, double[], double[][]) NumberObuslovlennosti(double[][] A, double[] b)
        {
            var a = CopyMatrix(A);
            var a1 = new double[a[0].Length][];
            var b1 = new double[b.Length];
            b.CopyTo(b1, 0);
            var normA = CalculateNorm(a);
            for (var i = 0; i < a[0].Length; ++i)
            {
                a1[i] = new double[a[0].Length];
                a1[i][i] = 1;
            }

            for (var i = 0; i < a[0].Length; ++i)
            {
                for (var j = i + 1; j < a[0].Length; ++j)
                {
                    var minus = a[j][i] / a[i][i];
                    for (var k = 0; k < a[0].Length; ++k)
                    {
                        a[j][k] -= a[i][k] * minus;
                        a1[j][k] -= a1[i][k] * minus;
                    }

                    
                    b1[j] -= b1[i] * minus;
                }
            }

            for (var i = a[0].Length - 1; i >= 0; --i)
            {
                for (var j = 0; j < a[0].Length; ++j)
                {
                    a1[i][j] /= a[i][i];
                }

                b1[i] /= a[i][i];
                //a[i][i] = 1;
                for (var j = i - 1; j >= 0; --j)
                {
                    var minus = a[j][i];
                    for (var k = 0; k < a[0].Length; ++k)
                    {
                        a1[j][k] -= a1[i][k] * minus;
                    }

                    b1[j] -= b1[i] * minus;
                }
            }

            var normA1 = CalculateNorm(a1);
            return (normA * normA1, b1, a1);
            
            /*var a1 = new double[256][];//обратная матрица
            var b1 = new double[256];
            b.CopyTo(b1, 0);
            var normA = 0.0;
            var normA1 = 0.0;
            for (var i = 0; i < 256; ++i)
            {
                a1[i] = new double[256];
                a1[i][i] = 1;
            }

            for (var i = 0; i < 256; ++i)//прямой ход метода гаусса
            {
                var tempNorm = 0.0;
                for (var j = i + 1; j < 256; ++j)
                {
                    a1[j][i] = -(a[j][i] / a[i][i]);
                    b1[j] -= (a[j][i] / a[i][i]) * b1[i];
                    tempNorm += Math.Abs(a[i][j]);
                }
                
                if (tempNorm > normA)
                {
                    normA = tempNorm;
                }
            }
            for (var i = 255; i > -1; --i)//обратный ход метода гаусса
            {
                for (var j = 0; j < 256; ++j)
                {
                    a1[i][j] /= a[i][i];
                }
                for (var k = 254; k > -1; --k)
                {
                    var tempNorm = 0.0;
                    var minus = a[k][i] / a[k + 1][i];
                    for (var j = 0; j < 256; ++j)
                    {
                        a1[k][j] -= a1[k + 1][j] * minus;
                        b1[k] -= b[i] * minus;
                        tempNorm += Math.Abs(a1[k + 1][j]);
                    }

                    b1[i] /= a[i][i];
                    tempNorm = Math.Sqrt(tempNorm);
                    if (tempNorm > normA1)
                    {
                        normA1 = tempNorm;
                    }
                }
            }

            return (normA * normA1, b1);*/
        }
        
        public (double[], double[][], double[][], byte[], byte[]) Gauss(double[][] A, double[] B)//разобраться с L
        {
            var matrix = CopyMatrix(A);
            var L = new double[matrix[0].Length][]; //храню столбцы в строках
            var U = new double[matrix[0].Length][];
            var P = new List<byte[]>();
            var _P = new List<byte[]>();
            var P1 = new byte[matrix[0].Length];
            var P2 = new byte[matrix[0].Length];
            var b = new double[matrix[0].Length];
            B.CopyTo(b, 0);
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                U[i] = new double[matrix[0].Length - i];
                L[i] = new double[matrix[0].Length - i];
                L[i][0] = 1;
                P1[i] = (byte)i;
                P2[i] = (byte)i;
            }
            for (var i = 1; i < matrix[0].Length; ++i)
            {
                var (item1, item2) = FindMax(matrix, i - 1, i - 1);
                P.Add(new[]{(byte)(i - 1), (byte)item2});
                _P.Add(new[]{(byte)(i - 1), (byte)item1});
                var temp1 = P1[item1];
                P1[item1] = P1[i - 1];
                P1[i - 1] = temp1;
                temp1 = P2[item2];
                P2[item2] = P2[i - 1];
                P2[i - 1] = temp1;
                SwapLines(matrix, i - 1, item1);
                var temp = b[i - 1];
                b[i - 1] = b[item1];
                b[item1] = temp;
                SwapRows(matrix, i - 1, item2);
                var (tmp1, tmp2) = (P1[i - 1], P2[i - 1]);
                for (var j = i; j < matrix[0].Length; ++j)
                {
                    var minus = matrix[j][i - 1] / matrix[i - 1][i - 1];
                    for (var k = i; k < matrix[0].Length; ++k)
                    {
                        matrix[j][k] -= matrix[i - 1][k] * minus;
                    }
                    
                    b[j] -= b[i - 1] * minus;
                    L[i - 1][j - i + 1] = minus;
                }
            }
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                U[i] = matrix[i][i..];
            }
            
            var x = new double[matrix[0].Length];
            x[matrix[0].Length - 1] = b[matrix[0].Length - 1] / matrix[matrix[0].Length - 1][matrix[0].Length - 1];
            for (var i = matrix[0].Length - 2; i >= 0; --i)
            {
                for (var j = matrix[0].Length - 1; j > i; --j)
                {
                    b[i] -= x[j] * matrix[i][j];
                }

                x[i] = b[i] / matrix[i][i];
            }
            
            for (var i = P.Count - 1; i >= 0; --i)
            {
                var temp = x[P[i][1]];
                x[P[i][1]] = x[P[i][0]];
                x[P[i][0]] = temp;
            }

            for (var i = 0; i < matrix[0].Length - 2; ++i)
            {
                var temp = L[i][_P[i + 1][0] - i];
                L[i][_P[i + 1][0] - i] = L[i][_P[i + 1][1] - i];
                L[i][_P[i + 1][1] - i] = temp;
            }
            
            return (x, L, U, P1, P2);
        }

        public double[] LupSolution(double[][] L, double[][] U, byte[] P1, byte[] P2, double[] B)
        {
            var b1 = new double[L[0].Length];
            B.CopyTo(b1, 0);
            var b2 = new double[L[0].Length];
            for (var i = 0; i < L[0].Length; ++i)
            {
                b2[P1[i]] = b1[i];
            }
            for (var i = 0; i < L[0].Length; ++i)
            {
                for (var j = i + 1; j < L[0].Length; ++j)
                {
                    b2[j] -= b2[i] * L[i][j - i];
                }
            }
            
            for (var i = L[0].Length - 1; i >= 0; --i)
            {
                b2[i] /= U[i][0];
                for (var j = i - 1; j >= 0; --j)
                {
                    b2[j] -= b2[i] * U[j][i - j];
                }
            }

            var x = new double[L[0].Length];
            for (var i = 0; i < L[0].Length; ++i)
            {
                x[i] = b2[P2[i]];
            }

            return x;
        }
        public (double[], double[][], double[]) SquareRootMethod(double[][] A, double[] B)
        {
            var matrix = CopyMatrix(A);
            var b = new double[matrix[0].Length];
            B.CopyTo(b, 0);
            var L = new double[matrix[0].Length][];
            var D = new double[matrix[0].Length];
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                L[i] = new double[matrix[0].Length - i];
                L[i][0] = 1;
            }

            for (var i = 1; i < matrix[0].Length; ++i)
            {
                for (var j = i; j < matrix[0].Length; ++j)
                {
                    var minus = matrix[j][i - 1] / matrix[i - 1][i - 1];
                    for (var k = i; k < matrix[0].Length; ++k)
                    {
                        matrix[j][k] -= matrix[i - 1][k] * minus;
                    }

                    L[i - 1][j - i + 1] = -minus;
                }

                D[i - 1] = matrix[i - 1][i - 1];
            }

            D[matrix[0].Length - 1] = matrix[matrix[0].Length - 1][matrix[0].Length - 1];
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                for (var j = i + 1; j < matrix[0].Length; ++j)
                {
                    b[j] -= b[i] * L[i][j - i];
                }

                b[i] /= D[i];
            }
            
            for (var i = matrix[0].Length - 1; i >= 0; --i)
            {
                for (var j = i - 1; j >= 0; --j)
                {
                    b[j] -= b[i] * L[j][matrix[0].Length - 1 - j];
                }
                
            }

            return (b, L, D);
        }
        
        public double[] ReflectionMethod(double[][] A, double[] b)
        {
            var matrix = CopyMatrix(A);
            var _b = new double[matrix[0].Length];
            b.CopyTo(_b,0);
            for (var i = 0; i < matrix[0].Length - 1; ++i)
            {
                var currA = new double[matrix[0].Length - i];
                currA = matrix[i][i..];
                var a = CalculateAForReflection(currA);
                currA[0] -= a[0];
                var norm = CalculateVectorNorm(currA);
                for (var j = 0; j < currA.Length; ++j)
                {
                    currA[j] /= norm;
                }

                var w = new double[currA.Length];
                currA.CopyTo(w, 0);
                a.CopyTo(matrix[i], i);
                var scalar1 = 0.0;
                for (var j = i; j < matrix[0].Length; ++j)
                {
                    scalar1 += _b[j] * w[j - i];
                }

                scalar1 *= 2;
                for (var j = i; j < matrix[0].Length; ++j)
                {
                    _b[j] -= scalar1 * w[j - i];
                }
                for (var j = i + 1; j < matrix[0].Length; ++j)
                {
                    currA = matrix[j][i..];
                    var scalar = 2 * currA.Select((t, k) => t * w[k]).Sum();
                    for (var k = 0; k < currA.Length; ++k)
                    {
                        currA[k] -= scalar * w[k];
                    }
                    currA.CopyTo(matrix[j], i);
                }
            }

            var x = new double[matrix[0].Length];
            x[matrix[0].Length - 1] = _b[matrix[0].Length - 1] / matrix[matrix[0].Length - 1][matrix[0].Length - 1];
            for (var i = matrix[0].Length - 2; i >= 0; --i)
            {
                for (var j = matrix[0].Length - 1; j > i; --j)
                {
                    _b[i] -= x[j] * matrix[j][i];
                }

                x[i] = _b[i] / matrix[i][i];
            }

            return x;
        }

        public double[] RotateMethod(double[][] A, double[] B)
        {
            var matrix = CopyMatrix(A);
            var b = new double[B.Length];
            B.CopyTo(b, 0);
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                for (var j = i + 1; j < matrix[0].Length; ++j)
                {
                    var del = Math.Sqrt(matrix[i][i] * matrix[i][i] + matrix[j][i] * matrix[j][i]);
                    var cos = matrix[i][i] / del;
                    var sin = matrix[j][i] / del;
                    var temp = new double[matrix[0].Length];
                    for (var k = 0; k < matrix[0].Length; ++k)
                    {
                        temp[k] = cos * matrix[i][k] + sin * matrix[j][k];
                        matrix[j][k] = cos * matrix[j][k] - sin * matrix[i][k];
                    }

                    matrix[i] = temp;
                    var tempB = cos * b[i] + sin * b[j];
                    b[j] = cos * b[j] - sin * b[i];
                    b[i] = tempB;
                }
            }
            
            var x = new double[matrix[0].Length];
            x[matrix[0].Length - 1] = b[matrix[0].Length - 1] / matrix[matrix[0].Length - 1][matrix[0].Length - 1];
            for (var i = matrix[0].Length - 2; i >= 0; --i)
            {
                for (var j = matrix[0].Length - 1; j > i; --j)
                {
                    b[i] -= x[j] * matrix[i][j];
                }

                x[i] = b[i] / matrix[i][i];
            }
            
            return x;
        }

        public (double[], int) RelaxationMethod(double[][] A, double[] B)
        {
            var matrix = CopyMatrix(A);
            var b = new double[B.Length];
            B.CopyTo(b, 0);
            var K = new double[matrix[0].Length][];
            var g = new double[matrix[0].Length];
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                K[i] = new double[matrix[0].Length];
                for (var j = 0; j < matrix[0].Length; ++j)
                {
                    if (j == i) continue;
                    K[i][j] = -(matrix[i][j] / matrix[i][i]);
                }

                g[i] = b[i] / matrix[i][i];
            }

            var x = new List<double[]>();
            var first = new double[matrix[0].Length];
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                first[i] = 1;
            }
            
            x.Add(first);
            var minus = new double[matrix[0].Length];
            var iterations = 0;
            double e = 0;
            do
            {
                var current = new double[matrix[0].Length];
                for (var i = 0; i < matrix[0].Length; ++i)
                {
                    var matrixPiece = 0.0;
                    for (var j = 0; j < matrix[0].Length; ++j)
                    {
                        if (j < i)
                        {
                            matrixPiece += K[i][j] * current[j];
                        }
                        else
                        {
                            matrixPiece += K[i][j] * x[^1][j];
                        }
                    }

                    current[i] = -0.28 * x[^1][i] + 1.28 * (matrixPiece + g[i]);
                }

                
                
                for (var i = 0; i < matrix[0].Length; ++i)
                {
                    minus[i] = current[i] - x[^1][i];
                }
                x.Add(current);
                x.Remove(x[0]);
                iterations++;
                e = CalculateVectorNorm(minus);
            } while (e > 0.0000001);

            return (x[^1], iterations);
        }

        public (double, Dictionary<string, double[]>) RunAllMethods(double[][] a, double[] b)
        {
            var (number, _, _) = NumberObuslovlennosti(a, b);
            var result = new Dictionary<string, double[]>();
            var L = new double[a[0].Length][];
            var U = new double[a[0].Length][];
            var P1 = new byte[a[0].Length];
            var P2 = new byte[a[0].Length];
            (result["Gauss: "], L, U, P1, P2) = Gauss(a, b);
            result["LUP: "] = LupSolution(L, U, P1, P2, b);
            (result["Square root: "], _, _) = SquareRootMethod(a, b);
            result["Reflection method: "] = ReflectionMethod(a, b);
            result["Rotate method: "] = RotateMethod(a, b);
            (result["Relaxation method: "], _) = RelaxationMethod(a, b);
            return (number, result);
        }
        
        public double[][] CopyMatrix(double[][] matrix)
        {
            var result = new double[matrix[0].Length][];
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                result[i] = new double[matrix[0].Length];
                for (var j = 0; j < matrix[0].Length; ++j)
                {
                    result[i][j] = matrix[i][j];
                }
            }

            return result;
        }
        
        private static (int, int) FindMax(double[][] matrix, int first, int second)
        {
            var tuple = (first, second);
            var max = 0.0;
            for (var i = first; i < matrix[0].Length; ++i)
            {
                for (var j = second; j < matrix[0].Length; ++j)
                {
                    if (Math.Abs(matrix[i][j]) <= max) continue;
                    max = Math.Abs(matrix[i][j]);
                    tuple.Item1 = i;
                    tuple.Item2 = j;
                }
            }

            return tuple;
        }
        
        private void SwapLines(IList<double[]> matrix, int firstLine, int secondLine)
        {
            var temp = matrix[firstLine];
            matrix[firstLine] = matrix[secondLine];
            matrix[secondLine] = temp;
        }

        private void SwapRows(IList<double[]> matrix, int firstRow, int secondRow)
        {
            for (var i = 0; i < matrix[0].Length; ++i)
            {
                var temp = matrix[i][firstRow];
                matrix[i][firstRow] = matrix[i][secondRow];
                matrix[i][secondRow] = temp;
            }
        }

        private static double CalculateNorm(double[][] matrix)
        {
            var norm = matrix.Select(a => a.Sum(d => Math.Abs(d))).Prepend(0.0).Max();
            return norm;
        }

        public double CalculateVectorNorm(double[] vector)
        {
            var max = vector.Max(d => Math.Abs(d));
            return Math.Sqrt(max);
        }
        private static double[] CalculateAForReflection(double[] a)
        {
            var result = new double[a.Length];
            result[0] = -Math.Sqrt(a.Sum(d => d * d));
            return result;
        }

        private double[] GetX(double[][] A, double[] B)
        {
            var matrix = CopyMatrix(A);
            var b = new double[B.Length];
            B.CopyTo(b, 0);
            for (var i = matrix[0].Length - 1; i >= 0; --i)
            {
                for (var j = i - 1; j >= 0; --j)
                {
                    var minus = matrix[j][matrix[0].Length - 1 - j] / matrix[i][matrix[0].Length - 1 - i];
                    b[j] -= b[i] * minus;
                }
            }

            return b;
        }
    }
}