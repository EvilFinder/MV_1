using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace MV_Lab1
{
    class Program
    {
        static void Main(string[] args)
        {
            
            var solution = new Solution2();
            /*var a = new double[4][];
            a[0] = new[] {1.0, 3, -4, 5};
            a[1] = new[] {3.0, 3, -1, -9};
            a[2] = new[] {-4.0, -1, -3, 12};
            a[3] = new[] {5.0, -9, 12, -1};
            var a = new double[4][];
            a[0] = new[] {1.0, 3, -4, 5};
            a[1] = new[] {3.0, 3, -1, -9};
            a[2] = new[] {-4.0, -1, -3, 12};
            a[3] = new[] {5.0, -9, 12, -1};*/

            //var k = new double[] {1, 2, 3, 4};
            //var (first, second, matrix) = solution.NumberObuslovlennosti(solution._a, solution._b);
            //Console.WriteLine(first);
            /*for (var i = 0; i < 4; ++i)
            {
                for (var j = 0; j < 4; ++j)
                {
                    Console.Write(matrix[i][j] + " ");
                }
                Console.WriteLine();
            }*/
            
            for (var i = 0; i < 256; ++i)
            {
                if (i % 7 == 0)
                {
                    Console.WriteLine();
                }
                
                Console.Write(solution._y[i] + " ");
            }

            var a = new double[3][];
            a[0] = new[] {2.0, 4, -1};
            a[1] = new[] {0, 3, 1.0};
            a[2] = new[] {-2.0, 4, 8};
            var (one, L, U, P1, P2) = solution.Gauss(solution._a, solution._b);
            //var b = solution.RotateMethod(solution._a, solution._b);
            //(b, _) = solution.RelaxationMethod(solution._a, solution._b);
            var b = solution.LupSolution(L, U, P1, P2, solution._b);
            
            //var (two, _, _) = solution.SquareRootMethod(solution._a, solution._b);
            Console.WriteLine("\n\n\n");
            for (var i = 0; i < 256; ++i)
            {
                if (i % 7 == 0)
                {
                    Console.WriteLine();
                }
                
                Console.Write(b[i] + " ");
            }

            var statistics = new Statistics();
            //Console.WriteLine(time);

            var sw = new StreamWriter("statistics.txt");
            /*var (maxNumber, minNumber, averageNumber, time) = statistics.NumberObuslovlennosti();
                sw.WriteLine("Gauss-Jordan:\nMax number: {0} \t min number: {1} \t average number: {2} \t time: {3}",
                    maxNumber, minNumber, averageNumber, time);
                (maxNumber, minNumber, averageNumber, time) = statistics.DifferenceGaussNorm();
                sw.WriteLine("Gauss:\nMax difference: {0} \t min difference: {1} \t average difference: {2} \t time: {3}", maxNumber, minNumber, averageNumber, time);
                (maxNumber, minNumber, averageNumber, time) = statistics.AverageLuTime();
                sw.WriteLine("LU:\nMax difference: {0} \t min difference: {1} \t average difference: {2} \t time: {3}", maxNumber, minNumber, averageNumber, time);
                (maxNumber, minNumber, averageNumber, time) = statistics.AverageSquareRootTime();
                sw.WriteLine("Square root:\nMax difference: {0} \t min difference: {1} \t average difference: {2} \t time: {3}", maxNumber, minNumber, averageNumber, time);
                (maxNumber, minNumber, averageNumber, time) = statistics.AverageReflectionTime();
                sw.WriteLine("Reflection method:\nMax difference: {0} \t min difference: {1} \t average difference: {2} \t time: {3}", maxNumber, minNumber, averageNumber, time);
                (maxNumber, minNumber, averageNumber, time) = statistics.AverageRotationTime();
                sw.WriteLine("Rotation method:\nMax difference: {0} \t min difference: {1} \t average difference: {2} \t time: {3}", maxNumber, minNumber, averageNumber, time);
                int maxIt, minIt;
                double averageIt;
                (maxNumber, minNumber, averageNumber, time, maxIt, minIt, averageIt) = statistics.AverageRelaxationTime();
                sw.WriteLine("Relaxation method:\nMax difference: {0} \t min difference: {1} \t average difference: {2} \t time: {3} \t max iterations: {4} \t min iterations: {5} average iterations: {6}", maxNumber, minNumber, averageNumber, time, maxIt, minIt, averageIt);
*/
                var a1 = new double[4][];
            a1[0] = new[] {51.0, 5, -1, -2};
            a1[1] = new[] {5.0, -51, 2, -4};
            a1[2] = new[] {-1.0, -2, 44, -6};
            a1[3] = new[] {-2.0, -4, -6, 46};

            var _a2 = new double[8][];
            _a2[0] = new[] {1.0, 7, 8, 9, 10, 11, 12, 13};
            _a2[1] = new[] {600.0, 6000, 60000, 600000, -6000, -60000, -600000, 1};
            _a2[2] = new[] {6, 5, 4, 3, 2, 1, 0, -1.0};
            _a2[3] = new[] {-994.0, -940, -400, 5000, 59000, -6, -5, -4};
            _a2[4] = new[] {-12, 0, -1, -2, -3, -4, -5, -6.0};
            _a2[5] = new[] {-2013.0, 2014, -2015, 2016, -2017, 2018, -2019, 2020};
            _a2[6] = new[] {-1988.0, -1981, -1962, -1919, -1828, 12114, -12120, 12126};
            _a2[7] = new[] {1008.0, 2452, 60060, 590772, -65004, -49991, -610089, 10217};

            var a2 = new double[8][];
            for (var i = 0; i < 8; ++i)
            {
                a2[i] = new double[8];
            }
            
            for (var i = 0; i < 8; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    for (var k = 0; k < 8; ++k)
                    {
                        a2[j][i] += _a2[i][k] * _a2[k][j];
                    }
                }
            }
            
            var b1 = new double[4];
            var b2 = new double[8];
            for (var i = 0; i < 4; ++i)
            {
                for (var j = 0; j < 4; ++j)
                {
                    b1[i] += a1[i][j];
                }
            }
            
            for (var i = 0; i < 8; ++i)
            {
                for (var j = 0; j < 8; ++j)
                {
                    b2[i] += a2[i][j] * 0.0001;
                }
            }

            var (number1, result1) = solution.RunAllMethods(a1, b1);
            var (number2, result2) = solution.RunAllMethods(a2, b2);
            sw.WriteLine("Matrix A1: \nNumber: {0}", number1);
            foreach (var key in result1.Keys)
            {
                var current = result1[key];
                sw.WriteLine(key);
                foreach (var t in current)
                {
                    sw.WriteLine("{0}\t", t);
                }
            }
            
            sw.WriteLine("Matrix A2: \nNumber: {0}", number2);
            foreach (var key in result2.Keys)
            {
                var current = result2[key];
                sw.WriteLine(key);
                foreach (var t in current)
                {
                    sw.WriteLine("{0}\t", t);
                }
            }
            
            sw.Close();
        }
    }
}