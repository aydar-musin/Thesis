using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using FLS.MembershipFunctions;
using FLS;

namespace EmotionalEstimation
{
    class Classifier
    {
        private LinguisticVariable PitchDif;
        private LinguisticVariable PitchDis;
        private LinguisticVariable IntDis;
        private LinguisticVariable IntDif;

        private LinguisticVariable F1Dif;
        private LinguisticVariable F1Dis;
        private LinguisticVariable F2Dif;
        private LinguisticVariable F2Dis;
        private LinguisticVariable F3Dif;
        private LinguisticVariable F3Dis;

        private LinguisticVariable PitchRange;
        private LinguisticVariable PitchVariance;

        private LinguisticVariable IntensityRange;
        private LinguisticVariable IntensityVariance;

        private LinguisticVariable PhraseDurationMean;
        private LinguisticVariable Centroid;

        private Dictionary<string, double> FeatureWeights;

        /// <summary>
        /// buils lingustic varibles. !!!!see min and max of membership functions!!!!
        /// </summary>
        /// <param name="features"></param>
        public void BuildEngine(List<Features> features)
        {
            FeatureWeights = new Dictionary<string, double>();

            var angerList = features.Where(f => f.Emotion == Emotions.Anger);
            var happyList = features.Where(f => f.Emotion == Emotions.Happy);
            var sadnessList = features.Where(f => f.Emotion == Emotions.Sadness);
            var neutralList = features.Where(f => f.Emotion == Emotions.Neutral);

            #region pitchdds
            //pitchDif

            double[] bounds = GetBoundaries(angerList.Select(f => f.PitchDDS.Difference).ToArray(), happyList.Select(f => f.PitchDDS.Difference).ToArray(),
                sadnessList.Select(f => f.PitchDDS.Difference).ToArray(), neutralList.Select(f => f.PitchDDS.Difference).ToArray());

            PitchDif = BuildVarible("PitchDif", bounds, 0, 1000);

            //pitchDis

            bounds = GetBoundaries(angerList.Select(f => f.PitchDDS.Distance).ToArray(), happyList.Select(f => f.PitchDDS.Distance).ToArray(),
                sadnessList.Select(f => f.PitchDDS.Distance).ToArray(), neutralList.Select(f => f.PitchDDS.Distance).ToArray());

            PitchDis = BuildVarible("PitchDis", bounds, -50, 100);
            #endregion pitchdds
            #region F1dds

            //Dif
            bounds = GetBoundaries(angerList.Select(f => f.F1DDS.Difference).ToArray(), happyList.Select(f => f.F1DDS.Difference).ToArray(),
                sadnessList.Select(f => f.F1DDS.Difference).ToArray(), neutralList.Select(f => f.F1DDS.Difference).ToArray());

            F1Dif = BuildVarible("F1Dif", bounds, 0, 2500);

            //Dis
            bounds = GetBoundaries(angerList.Select(f => f.F1DDS.Distance).ToArray(), happyList.Select(f => f.F1DDS.Distance).ToArray(),
                sadnessList.Select(f => f.F1DDS.Distance).ToArray(), neutralList.Select(f => f.F1DDS.Distance).ToArray());

            F1Dis = BuildVarible("F1Dis", bounds, -50, 2500);
            #endregion F1dds
            #region F2dds

            //Dif
            bounds = GetBoundaries(angerList.Select(f => f.F2DDS.Difference).ToArray(), happyList.Select(f => f.F2DDS.Difference).ToArray(),
                sadnessList.Select(f => f.F2DDS.Difference).ToArray(), neutralList.Select(f => f.F2DDS.Difference).ToArray());

            F2Dif = BuildVarible("F1Dif", bounds, 0, 2500);

            //Dis
            bounds = GetBoundaries(angerList.Select(f => f.F2DDS.Distance).ToArray(), happyList.Select(f => f.F2DDS.Distance).ToArray(),
                sadnessList.Select(f => f.F2DDS.Distance).ToArray(), neutralList.Select(f => f.F2DDS.Distance).ToArray());

            F2Dis = BuildVarible("F2Dis", bounds, -50, 2500);
            #endregion F2dds
            #region F3dds

            //Dif
            bounds = GetBoundaries(angerList.Select(f => f.F3DDS.Difference).ToArray(), happyList.Select(f => f.F3DDS.Difference).ToArray(),
                sadnessList.Select(f => f.F3DDS.Difference).ToArray(), neutralList.Select(f => f.F3DDS.Difference).ToArray());

            F3Dif = BuildVarible("F3Dif", bounds, 0, 2500);

            //Dis
            bounds = GetBoundaries(angerList.Select(f => f.F3DDS.Distance).ToArray(), happyList.Select(f => f.F3DDS.Distance).ToArray(),
                sadnessList.Select(f => f.F3DDS.Distance).ToArray(), neutralList.Select(f => f.F3DDS.Distance).ToArray());

            F3Dis = BuildVarible("F3Dis", bounds, -50, 2500);
            #endregion F2dds
            #region pitch
            bounds = GetBoundaries(angerList.Select(f => f.PitchRange).ToArray(), happyList.Select(f => f.PitchRange).ToArray(),
                sadnessList.Select(f => f.PitchRange).ToArray(), neutralList.Select(f => f.PitchRange).ToArray());

            PitchRange = BuildVarible("PitchRange", bounds, 0, 1000);

            bounds = GetBoundaries(angerList.Select(f => f.PitchVariance).ToArray(), happyList.Select(f => f.PitchVariance).ToArray(),
                sadnessList.Select(f => f.PitchVariance).ToArray(), neutralList.Select(f => f.PitchVariance).ToArray());

            PitchVariance = BuildVarible("PitchVariance", bounds, 0, 100000);

            #endregion pitch
            #region intensity

            bounds = GetBoundaries(angerList.Select(f => f.IntensityDDS.Difference).ToArray(), happyList.Select(f => f.IntensityDDS.Difference).ToArray(),
                sadnessList.Select(f => f.IntensityDDS.Difference).ToArray(), neutralList.Select(f => f.IntensityDDS.Difference).ToArray());

            IntDif = BuildVarible("IntDif", bounds, 0, 1000);

            bounds = GetBoundaries(angerList.Select(f => f.IntensityDDS.Distance).ToArray(), happyList.Select(f => f.IntensityDDS.Distance).ToArray(),
                sadnessList.Select(f => f.IntensityDDS.Distance).ToArray(), neutralList.Select(f => f.IntensityDDS.Distance).ToArray());

            IntDis = BuildVarible("IntDis", bounds, -50, 100);

            bounds = GetBoundaries(angerList.Select(f => f.IntensityRange).ToArray(), happyList.Select(f => f.IntensityRange).ToArray(),
                sadnessList.Select(f => f.IntensityRange).ToArray(), neutralList.Select(f => f.IntensityRange).ToArray());

            IntensityRange = BuildVarible("IntensityRange", bounds, 0, 1000);

            bounds = GetBoundaries(angerList.Select(f => f.IntensityVariance).ToArray(), happyList.Select(f => f.IntensityVariance).ToArray(),
                sadnessList.Select(f => f.IntensityVariance).ToArray(), neutralList.Select(f => f.IntensityVariance).ToArray());

            IntensityVariance = BuildVarible("IntensityVariance", bounds, 0, 100000);
            #endregion intensity

            bounds = GetBoundaries(angerList.Select(f => f.PhraseDurationMean).ToArray(), happyList.Select(f => f.PhraseDurationMean).ToArray(),
                sadnessList.Select(f => f.PhraseDurationMean).ToArray(), neutralList.Select(f => f.PhraseDurationMean).ToArray());

            PhraseDurationMean = BuildVarible("PhraseDurationMean", bounds, 0, 100);

            bounds = GetBoundaries(angerList.Select(f => f.Centroid).ToArray(), happyList.Select(f => f.Centroid).ToArray(),
                sadnessList.Select(f => f.Centroid).ToArray(), neutralList.Select(f => f.Centroid).ToArray());

            Centroid = BuildVarible("Centroid", bounds, 0, 1000);
        }

        /// <summary>
        /// Recieves 4 ORDERED lists and returns 2 boundary numbers for triangloid membership function
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private double[] GetBoundaries(double[] list1, double[] list2, double[] list3, double[] list4)
        {
            double median1 = Analyzer.GetMedian(list1);
            double median2 = Analyzer.GetMedian(list2);
            double median3 = Analyzer.GetMedian(list3);
            double median4 = Analyzer.GetMedian(list4);

            var ordered_medians = (new double[]{ median1,median2,median3,median4 }).OrderBy(d=>d).ToArray();

            return new double[] { (ordered_medians[0] + ordered_medians[1]) / 2, (ordered_medians[2] + ordered_medians[3]) / 2 };
        }

        private LinguisticVariable BuildVarible(string name,double[] boundaries,double min,double max)
        {
            double intersection = (boundaries[0] * 20) / 100;
            LinguisticVariable variable = new LinguisticVariable(name);

            variable.MembershipFunctions.AddTriangle(name+"_IsLow", min, boundaries[0], boundaries[1] - intersection);
            variable.MembershipFunctions.AddTriangle(name + "_IsHigh", boundaries[0]+intersection, boundaries[1], max);

            if (!FeatureWeights.ContainsKey(name))
            {
                FeatureWeights.Add(name, 1);
            }

            return variable;
        }

        public EmotionsResult Classify(Features features)
        {
            //double d = fuzzyEngine.Defuzzify(new { pitchDDS_Dif = pitch_dds.PitchDifference, pitchDDS_Dis = pitch_dds.PitchDistance, IntensityDDS=intensity_dds.PitchDistance });

            double anger = 0;
            anger += PitchDif.IsHigh(features.PitchDDS.Difference)*FeatureWeights[PitchDif.Name];
            anger += PitchDis.IsLow(features.PitchDDS.Difference) * FeatureWeights[PitchDis.Name];
            anger += F1Dif.IsHigh(features.F1DDS.Difference) * FeatureWeights[F1Dif.Name];
            anger += F1Dis.IsHigh(features.F1DDS.Distance) * FeatureWeights[F1Dis.Name];
            anger += F2Dif.IsHigh(features.F2DDS.Difference) * FeatureWeights[F2Dif.Name];
            anger += F2Dis.IsHigh(features.F2DDS.Distance) * FeatureWeights[F2Dis.Name];
            anger += F3Dif.IsLow(features.F3DDS.Difference) * FeatureWeights[F3Dif.Name];
            anger += F3Dis.IsHigh(features.F3DDS.Distance) * FeatureWeights[F3Dis.Name];
            anger += PitchRange.IsHigh(features.PitchRange) * FeatureWeights[PitchRange.Name];
            anger += IntensityRange.IsHigh(features.IntensityRange) * FeatureWeights[IntensityRange.Name];
            anger += PitchVariance.IsHigh(features.PitchVariance) * FeatureWeights[PitchVariance.Name];
            anger += IntensityVariance.IsHigh(features.IntensityVariance) * FeatureWeights[IntensityVariance.Name];
            anger += PhraseDurationMean.IsLow(features.PhraseDurationMean) * FeatureWeights[PhraseDurationMean.Name];
            anger += Centroid.IsHigh(features.Centroid) * FeatureWeights[Centroid.Name];
            anger += IntDif.IsHigh(features.IntensityDDS.Difference) * FeatureWeights[IntDif.Name];
            anger += IntDis.IsHigh(features.IntensityDDS.Distance) * FeatureWeights[IntDis.Name];

            

            double happy = 0;
            happy += PitchDif.IsHigh(features.PitchDDS.Difference)*FeatureWeights[PitchDif.Name];
            happy += PitchDis.IsLow(features.PitchDDS.Distance)*FeatureWeights[PitchDis.Name];
            happy += F1Dif.IsHigh(features.F1DDS.Difference) * FeatureWeights[F1Dif.Name];
            happy += F1Dis.IsLow(features.F1DDS.Distance) * FeatureWeights[F1Dis.Name];
            happy += F2Dif.IsLow(features.F2DDS.Difference) * FeatureWeights[F2Dif.Name];
            happy += F2Dis.IsLow(features.F2DDS.Distance) * FeatureWeights[F2Dis.Name];
            happy += F3Dif.IsLow(features.F3DDS.Difference) * FeatureWeights[F3Dif.Name];
            happy += F3Dis.IsLow(features.F3DDS.Distance) * FeatureWeights[F3Dis.Name];
            happy += PitchRange.IsHigh(features.PitchRange)* FeatureWeights[PitchRange.Name];
            happy += IntensityRange.IsHigh(features.IntensityRange)* FeatureWeights[IntensityRange.Name];
            happy += PitchVariance.IsHigh(features.PitchVariance)* FeatureWeights[PitchVariance.Name];
            happy += IntensityVariance.IsHigh(features.IntensityVariance)* FeatureWeights[IntensityVariance.Name];
            happy += PhraseDurationMean.IsHigh(features.PhraseDurationMean)* FeatureWeights[PhraseDurationMean.Name];
            happy += Centroid.IsHigh(features.Centroid) * FeatureWeights[Centroid.Name];
            happy += IntDif.IsHigh(features.IntensityDDS.Difference) * FeatureWeights[IntDif.Name];
            happy += IntDis.IsLow(features.IntensityDDS.Distance) * FeatureWeights[IntDis.Name];

            


            double sadness = 0;
            sadness += PitchDif.IsLow(features.PitchDDS.Difference)*FeatureWeights[PitchDif.Name];
            sadness += PitchDis.IsLow(features.PitchDDS.Distance)*FeatureWeights[PitchDis.Name];
            sadness += F1Dif.IsLow(features.F1DDS.Difference)* FeatureWeights[F1Dif.Name];
            sadness += F1Dis.IsLow(features.F1DDS.Distance)* FeatureWeights[F1Dis.Name];
            sadness += F2Dif.IsHigh(features.F2DDS.Difference)* FeatureWeights[F2Dif.Name];
            sadness += F2Dis.IsLow(features.F2DDS.Distance)* FeatureWeights[F2Dis.Name];
            sadness += F3Dif.IsHigh(features.F3DDS.Difference)* FeatureWeights[F3Dif.Name];
            sadness += F3Dis.IsHigh(features.F3DDS.Distance)* FeatureWeights[PitchRange.Name];
            sadness += PitchRange.IsLow(features.PitchRange)* FeatureWeights[PitchRange.Name];
            sadness += IntensityRange.IsLow(features.IntensityRange)* FeatureWeights[IntensityRange.Name];
            sadness += PitchVariance.IsLow(features.PitchVariance)* FeatureWeights[PitchVariance.Name];
            sadness += IntensityVariance.IsLow(features.IntensityVariance)* FeatureWeights[IntensityVariance.Name];
            sadness += PhraseDurationMean.IsHigh(features.PhraseDurationMean)* FeatureWeights[PhraseDurationMean.Name];
            sadness += Centroid.IsLow(features.Centroid) * FeatureWeights[Centroid.Name];
            sadness += IntDif.IsLow(features.IntensityDDS.Difference) * FeatureWeights[IntDif.Name];
            sadness += IntDis.IsLow(features.IntensityDDS.Distance) * FeatureWeights[IntDis.Name];

            double neutral = 0;
            neutral += PitchDif.IsLow(features.PitchDDS.Difference) * FeatureWeights[PitchDif.Name];
            neutral += PitchDis.IsHigh(features.PitchDDS.Distance) * FeatureWeights[PitchDis.Name];
            neutral += F1Dif.IsLow(features.F1DDS.Difference) * FeatureWeights[F1Dif.Name];
            neutral += F1Dis.IsHigh(features.F1DDS.Distance) * FeatureWeights[F1Dis.Name];
            neutral += F2Dif.IsLow(features.F2DDS.Difference) * FeatureWeights[F2Dif.Name];
            neutral += F2Dis.IsHigh(features.F2DDS.Distance) * FeatureWeights[F2Dis.Name];
            neutral += F3Dif.IsHigh(features.F3DDS.Difference) * FeatureWeights[F3Dif.Name];
            neutral += F3Dis.IsHigh(features.F3DDS.Distance) * FeatureWeights[PitchRange.Name];
            neutral += PitchRange.IsLow(features.PitchRange) * FeatureWeights[PitchRange.Name];
            neutral += IntensityRange.IsLow(features.IntensityRange) * FeatureWeights[IntensityRange.Name];
            neutral += PitchVariance.IsLow(features.PitchVariance) * FeatureWeights[PitchVariance.Name];
            neutral += IntensityVariance.IsLow(features.IntensityVariance) * FeatureWeights[IntensityVariance.Name];
            neutral += PhraseDurationMean.IsLow(features.PhraseDurationMean) * FeatureWeights[PhraseDurationMean.Name];
            neutral += Centroid.IsLow(features.Centroid) * FeatureWeights[Centroid.Name];
            neutral += IntDif.IsLow(features.IntensityDDS.Difference) * FeatureWeights[IntDif.Name];
            neutral += IntDis.IsHigh(features.IntensityDDS.Distance) * FeatureWeights[IntDis.Name];

            EmotionsResult result = new EmotionsResult();
            result.Anger = anger;
            result.Happiness = happy;
            result.Sadness = sadness;
            result.Neutral = neutral;

            return result;
        }

        /// <summary>
        /// Edit feature weights. Parameters weights should consist of N numbers, where N is number of features
        /// </summary>
        /// <param name="weigths"></param>
        public void EditFeatureWeights(double[] weigths)
        {
            int i = 0;
            Dictionary<string, double> new_weights = new Dictionary<string, double>();

            foreach (var key in FeatureWeights.Keys)
            {
                new_weights.Add(key, weigths[i]);
                i++;
            }
            FeatureWeights = new_weights;
        }
    }
    public static class Instruments
    {
        public static double IsLow(this LinguisticVariable variable, double value)
        {
            return variable.MembershipFunctions[0].Fuzzify(value);
        }
        public static double IsHigh(this LinguisticVariable variable, double value)
        {
            return variable.MembershipFunctions[1].Fuzzify(value);
        }
    }
    public class EmotionsResult
    {
        public double Anger;
        public double Happiness;
        public double Sadness;
        public double Neutral;

        public Emotions GetMax()
        {
            if (Anger > Happiness && Anger > Sadness && Anger > Neutral)
                return Emotions.Anger;
            else if (Happiness > Anger && Happiness > Sadness && Happiness > Neutral)
                return Emotions.Happy;
            else/* if (Sadness > Anger && Sadness > Happiness && Sadness > Neutral)*/
                return Emotions.Sadness;
            /*else
                return Emotions.Neutral;*/
        }
    }

}
