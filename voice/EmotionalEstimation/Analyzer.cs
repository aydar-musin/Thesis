using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmotionalEstimation
{
    static class Analyzer
    {
        /// <summary>
        /// Calculate standard deviation of input
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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
        /// Computes Difference-Distance-Slope characteristic from signal
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
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

            dds.Distance = maxindex - minindex;
            dds.Difference = max - min;

            return dds;
        }

        /// <summary>
        /// Analyzes and retrieves features from sound characteristics
        /// </summary>
        /// <param name="sound"></param>
        /// <returns></returns>
        public static Features Analyze(SoundCharacteristics sound,Emotions emotion)
        {
            Features result = new Features();
            result.Centroid = sound.Centroid;
            result.Emotion = emotion;

            var nonZeroPitchValues = sound.PitchValues.Where(p => p > 0); ;
            var nonZeroIntensityValues = sound.IntensityValues.Where(i => i > 0);
            var nonZeroF1Values = sound.F1Values.Where(f => f > 0);
            var nonZeroF2Values = sound.F2Values.Where(f => f > 0);
            var nonZeroF3Values = sound.F3Values.Where(f => f > 0);


            result.PitchRange = nonZeroPitchValues.Max()-nonZeroPitchValues.Min();
            result.IntensityRange = nonZeroIntensityValues.Max() - nonZeroIntensityValues.Min();
            result.PitchVariance = GetVariance(nonZeroPitchValues.ToArray());
            result.IntensityVariance = GetVariance(nonZeroIntensityValues.ToArray());

            double phraseDurationMean=0;
            double silenceDurationMean = 0;

            //DDS lists for phrases results
            List<DDS> Pitch_ddsList = new List<DDS>();
            List<DDS> Intensity_ddsList = new List<DDS>();
            List<DDS> F1_ddsList = new List<DDS>();
            List<DDS> F2_ddsList = new List<DDS>();
            List<DDS> F3_ddsList = new List<DDS>();

            for (int i = 0; i < sound.Phrases.Count; i++)
            {
                int start=sound.Phrases[i].Start;
                int end=sound.Phrases[i].End;

                var pitchValues=sound.PitchValues.GetRange(start,end-start).ToArray();
                var intValues=sound.IntensityValues.GetRange(start,end-start).ToArray();

                var f1Values = sound.F1Values.GetRange(start, end - start).ToArray();
                var f2Values = sound.F2Values.GetRange(start, end - start).ToArray();
                var f3Values = sound.F3Values.GetRange(start, end - start).ToArray();

                Pitch_ddsList.Add(Analyzer.ComputeDDS(pitchValues));
                Intensity_ddsList.Add(Analyzer.ComputeDDS(intValues));
                F1_ddsList.Add(Analyzer.ComputeDDS(f1Values));
                F2_ddsList.Add(Analyzer.ComputeDDS(f2Values));
                F3_ddsList.Add(Analyzer.ComputeDDS(f3Values));

                phraseDurationMean += end - start;

                if (i < sound.Phrases.Count - 1)
                    silenceDurationMean += sound.Phrases[i + 1].Start - sound.Phrases[i].End;
            }
            //calculate mean phrase duration
            phraseDurationMean = phraseDurationMean / sound.Phrases.Count;
            silenceDurationMean = silenceDurationMean / sound.Phrases.Count - 1;

            result.PhraseDurationMean = phraseDurationMean;
            result.SilenceDurationMean = silenceDurationMean;

            //DDS results from dds lists
            result.PitchDDS = new DDS()
            {
                Difference=Pitch_ddsList.Average(d=>d.Difference),
                Distance=Pitch_ddsList.Average(d=>d.Distance)
            };
            result.IntensityDDS= new DDS()
            {
                Difference = Intensity_ddsList.Average(d => d.Difference),
                Distance = Intensity_ddsList.Average(d => d.Distance)
            };
            result.F1DDS = new DDS()
            {
                Difference = F1_ddsList.Average(d => d.Difference),
                Distance = F1_ddsList.Average(d => d.Distance)
            };
            result.F2DDS = new DDS()
            {
                Difference = F2_ddsList.Average(d => d.Difference),
                Distance = F2_ddsList.Average(d => d.Distance)
            };
            result.F3DDS = new DDS()
            {
                Difference = F3_ddsList.Average(d => d.Difference),
                Distance = F3_ddsList.Average(d => d.Distance)
            };

            return result;
        }
        public static double GetMedian(double[] sourceNumbers)
        {       
            if (sourceNumbers == null || sourceNumbers.Length == 0)
                return -1;

            //make sure the list is sorted, but use a new array
            double[] sortedPNumbers = (double[])sourceNumbers.Clone();
            sourceNumbers.CopyTo(sortedPNumbers, 0);
            Array.Sort(sortedPNumbers);

            //get the median
            int size = sortedPNumbers.Length;
            int mid = size / 2;
            double median = (size % 2 != 0) ? (double)sortedPNumbers[mid] : ((double)sortedPNumbers[mid] + (double)sortedPNumbers[mid - 1]) / 2;
            return median;
        }
    }

    [Serializable]
    public class Features
    {

        public DDS PitchDDS;
        public DDS IntensityDDS;
        public DDS F1DDS;
        public DDS F2DDS;
        public DDS F3DDS;

        public double PitchRange;
        public double IntensityRange;
        public double PitchVariance;
        public double IntensityVariance;
        public double PhraseDurationMean;
        public double SilenceDurationMean;

        public double Centroid;
        public Emotions Emotion;

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}\t{12}\t{13}\t{14}\t{15}\t{16}",
                PitchDDS.Difference,PitchDDS.Distance,IntensityDDS.Difference,IntensityDDS.Distance,
                F1DDS.Difference,F1DDS.Distance,F2DDS.Difference,F2DDS.Distance,F3DDS.Difference,F3DDS.Distance,
                PitchRange,IntensityRange,PitchVariance,IntensityVariance,PhraseDurationMean,SilenceDurationMean,Centroid);
        }

    }
    [Serializable]
    public struct DDS
    {
        public double Distance;
        public double Difference;
    }
    [Serializable]
    public enum Emotions { Anger,Happy,Sadness,Neutral}
}
