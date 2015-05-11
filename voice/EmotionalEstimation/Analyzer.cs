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
    }

    public enum Contour { Rising,Falling,Smooth}
}
