using System;

namespace MV_Lab1
{
    public class Statistics
    {
        public Solution2[] Sol { get; }
        public double[][] MaxMatrix { get; private set; }

        public Statistics()
        {
            Sol = new Solution2[100];
            for (var i = 0; i < 100; ++i)
            {
                Sol[i] = new Solution2();
            }
        }
        public (double, double, double, TimeSpan) NumberObuslovlennosti()
        {
            var averageTime = new TimeSpan();
            double min = double.MaxValue, max = double.MinValue, average = 0.0;
            for (var i = 0; i < 100; ++i)
            {
                var startTime = DateTime.Now;
                var (first, _, _) = Sol[i].NumberObuslovlennosti(Sol[i]._a, Sol[i]._b);
                var endTime = DateTime.Now;
                var currentDifference = endTime - startTime;
                if (first > max)
                {
                    max = first;
                    MaxMatrix = Sol[i].CopyMatrix(Sol[i]._a);
                }

                if (first < min)
                {
                    min = first;
                }

                average += first / 100;
                averageTime += currentDifference / 100;

            }

            return (max, min, average, averageTime);
        }

        public (double, double, double, TimeSpan) DifferenceGaussNorm()
        {
            double max = double.MinValue, min = double.MaxValue, average = 0.0;
            var time = new TimeSpan();
            for (var i = 0; i < 100; ++i)
            {
                var startTime = DateTime.Now;
                var (temp, _, _, _, _) = Sol[i].Gauss(Sol[i]._a, Sol[i]._b);
                var endTime = DateTime.Now;
                for (var j = 0; j < temp.Length; ++j)
                {
                    temp[j] -= Sol[i]._y[j];
                }

                CalculateDifference(temp, ref max, ref min, ref average);
                time += (endTime - startTime) / 100;
            }

            return (max, min, average, time);
        }

        public (double, double, double, TimeSpan) AverageLuTime()
        {
            var time = new TimeSpan();
            double max = double.MinValue, min = double.MaxValue, average = 0.0;
            for (var i = 0; i < 100; ++i)
            {
                var startTime = DateTime.Now;
                var temp = Sol[i].LupSolution(Sol[i].L, Sol[i].U, Sol[i].P1, Sol[i].P2, Sol[i]._b);
                var endTime = DateTime.Now;
                for (var j = 0; j < temp.Length; ++j)
                {
                    temp[j] -= Sol[i]._y[j];
                }
                
                CalculateDifference(temp, ref max, ref min, ref average);
                time += (endTime - startTime) / 100;
            }

            return (max, min, average, time);
        }
        
        public (double, double, double, TimeSpan) AverageSquareRootTime()
        {
            var time = new TimeSpan();
            double max = double.MinValue, min = double.MaxValue, average = 0.0;
            for (var i = 0; i < 100; ++i)
            {
                var startTime = DateTime.Now;
                var (temp, _, _) = Sol[i].SquareRootMethod(Sol[i]._a, Sol[i]._b);
                var endTime = DateTime.Now;
                for (var j = 0; j < temp.Length; ++j)
                {
                    temp[j] -= Sol[i]._y[j];
                }
                CalculateDifference(temp, ref max, ref min, ref average);
                time += (endTime - startTime) / 100;
            }

            return (max, min, average, time);
        }
        
        public (double, double, double, TimeSpan, int, int, double) AverageRelaxationTime()
        {
            var time = new TimeSpan();
            var minIt = int.MaxValue;
            var maxIt = int.MinValue;
            double max = double.MinValue, min = double.MaxValue, average = 0.0;
            var averageIt = 0.0;
            for (var i = 0; i < 100; ++i)
            {
                var startTime = DateTime.Now;
                var (temp, iterations) = Sol[i].RelaxationMethod(Sol[i]._a, Sol[i]._b);
                var endTime = DateTime.Now;
                for (var j = 0; j < temp.Length; ++j)
                {
                    temp[j] -= Sol[i]._y[j];
                }
                
                CalculateDifference(temp, ref max, ref min, ref average);
                
                if (iterations > maxIt)
                {
                    maxIt = iterations;
                }

                if (iterations < minIt)
                {
                    minIt = iterations;
                }

                averageIt += iterations * 1.0 / 100;
                time += (endTime - startTime) / 100;
            }

            return (max, min, average, time, maxIt, minIt, averageIt);
        }
        
        public (double, double, double, TimeSpan) AverageReflectionTime()
        {
            var time = new TimeSpan();
            double max = double.MinValue, min = double.MaxValue, average = 0.0;
            for (var i = 0; i < 100; ++i)
            {
                var startTime = DateTime.Now;
                var temp = Sol[i].ReflectionMethod(Sol[i]._a, Sol[i]._b);
                var endTime = DateTime.Now;
                for (var j = 0; j < temp.Length; ++j)
                {
                    temp[j] -= Sol[i]._y[j];
                }
                
                CalculateDifference(temp, ref max, ref min, ref average);
                time += (endTime - startTime) / 100;
            }

            return (max, min, average, time);
        }
        
        public (double, double, double, TimeSpan) AverageRotationTime()
        {
            var time = new TimeSpan();
            double max = double.MinValue, min = double.MaxValue, average = 0.0;
            for (var i = 0; i < 100; ++i)
            {
                var startTime = DateTime.Now;
                var temp = Sol[i].RotateMethod(Sol[i]._a, Sol[i]._b);
                var endTime = DateTime.Now;
                for (var j = 0; j < temp.Length; ++j)
                {
                    temp[j] -= Sol[i]._y[j];
                }
                
                CalculateDifference(temp, ref max, ref min, ref average);
                time += (endTime - startTime) / 100;
            }

            return (max, min, average, time);
        }

        private void CalculateDifference(double[] vector, ref double max, ref double min, ref double average)
        {
            var difference = Sol[0].CalculateVectorNorm(vector);
            if (difference > max)
            {
                max = difference;
            }

            if (difference < min)
            {
                min = difference;
            }

            average += difference / 100;
        }
    }
}