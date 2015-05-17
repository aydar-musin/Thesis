using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalEstimation
{
    static class Analyzer
    {
        public static double DifTreshold = 5;//Hz
        public static int DifStep = 3; //milliseconds
        public static double GetVariance(double[] input)
        {
            input = input.Where(n => n > 0).ToArray();
            var mean = input.Average();

            double sum = 0;

            foreach (var x in input)
            {
                sum += Math.Pow(mean - x, 2);
            }

            return sum / input.Length - 1;
        }

        /// <summary>
        /// Receives only non-zero intervals.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static Contour DetectContour(double[] input)
        {
            int rising = 0;
            int falling = 0;
            int smooth=0;

            if (!(input.Length - 1 < DifStep))
            {
                for (int i = 0; i < input.Length-DifStep; i += DifStep)
                {
                    double dif = input[i] - input[i + DifStep];

                    if (Math.Abs(dif) < DifTreshold)
                        smooth++;
                    else if (dif > 0)
                        falling++;
                    else
                        rising++;
                }
                if (falling > rising)
                    return Contour.Falling;
                else
                    return Contour.Rising;
            }
            else
            {
                double dif = input[0] - input[input.Length-1];

                if (Math.Abs(dif) < DifTreshold)
                    return Contour.Smooth;
                else if (dif > 0)
                    return Contour.Falling;
                else
                    return Contour.Rising;
            }
        }

        public static DDS ComputeDDS(double[] input)
        {
            double min = 100000;
            double max = 0;
            int minindex = -1;
            int maxindex = -1;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == 0)
                    continue;

                if (input[i] < min)
                {
                    min = input[i];
                    minindex = i;
                }

                if (input[i] > max)
                {
                    max = input[i];
                    maxindex = i;
                }
            }

            if (maxindex == -1 || minindex == -1)
                throw new Exception("Ошибка подсчета DDS");

            DDS dds = new DDS();

            if (maxindex > minindex)
                dds.Distance = maxindex - minindex;
            else
                dds.Distance = minindex - maxindex;

            dds.Difference = max - min;
            dds.Slope = Math.Sqrt(Math.Pow(dds.Difference, 2) + Math.Pow(dds.Distance, 2));

            return dds;
        }
    }

    /// <summary>
    /// Distance, Difference, Slope
    /// </summary>
    public struct DDS
    {
        public double Distance;
        public double Difference;
        public double Slope;
    }
    public enum Contour { Rising,Falling,Smooth}
}
