using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using LibSVMsharp.Core;
using LibSVMsharp.Helpers;
using LibSVMsharp;

namespace EmotionalEstimation
{
    class SVMClassifier
    {
        private SVMModel Positive_Negative_Model;
        private SVMModel Active_Passive_Model;
        private List<Features> training_f_list;

        private List<SVMNode[]> training_set;
        private List<double> P_N_targets;
        private List<double> A_P_targets;

        public void FromFiles()
        {
            Active_Passive_Model = SVM.LoadModel("features_A_P.txt.model");
            Positive_Negative_Model = SVM.LoadModel("features_P_N.txt.model");

        }
        public void Train()
        {
            
            LibSVMsharp.SVMProblem P_N_problem = new LibSVMsharp.SVMProblem();//positive negative
            LibSVMsharp.SVMProblem A_P_problem = new LibSVMsharp.SVMProblem();//active passive
            
            ReadFromFileAndScale();

            for(int i=0;i<training_set.Count;i++)
            {
                P_N_problem.Add(training_set[i], P_N_targets[i]);
                A_P_problem.Add(training_set[i], A_P_targets[i]);
            }

            SVMParameter P_N_parameter = new SVMParameter();
            P_N_parameter.Type = SVMType.C_SVC;
            P_N_parameter.Kernel = SVMKernelType.LINEAR;
            P_N_parameter.C = 3;
            P_N_parameter.Gamma = 1;
            P_N_parameter.Degree = 3;

            Positive_Negative_Model = SVM.Train(P_N_problem, P_N_parameter);

            SVMParameter A_P_parameter = new SVMParameter();
            A_P_parameter.Type = SVMType.C_SVC;
            A_P_parameter.Kernel = SVMKernelType.LINEAR;
            A_P_parameter.C = 3;
            A_P_parameter.Gamma = 1;
            A_P_parameter.Degree = 3;
            A_P_parameter.Weights = new double[] { 10,10,10,1,1,1,1,1,1};
            Active_Passive_Model = SVM.Train(A_P_problem, A_P_parameter);

        }

        public Emotions Classify(Features features)
        {
            var nodes = GetScaledNodes(features);

            var result = SVM.Predict(Positive_Negative_Model, nodes);
            var result2 = SVM.Predict(Active_Passive_Model, nodes);

            if (result == 1 && result2 == 1)
                return Emotions.Happy;
            else if (result == 1 && result2 == -1)
                return Emotions.Neutral;
            else if (result == -1 && result2 == -1)
                return Emotions.Sadness;
            else
                return Emotions.Anger;
        }

        private void ReadFromFileAndScale()
        {
            
            var lines = File.ReadAllLines("analyze_result.txt");
            P_N_targets = new List<double>();
            A_P_targets = new List<double>();


            List<double> pdif = new List<double>();
            List<double> intdis = new List<double>();
            List<double> f1dif = new List<double>();
            List<double> pitchrange = new List<double>();
            List<double> intrange = new List<double>();
            List<double> pitchvar = new List<double>();
            List<double> intvar = new List<double>();
            List<double> pdur = new List<double>();
            List<double> cenrtoid = new List<double>();

            List<Features> features = new List<Features>();

            for (int i = 1; i < lines.Length; i++)
            {
                var numbers=lines[i].Split('\t');
                P_N_targets.Add(GetPositiveNegativeTarget(numbers[0]));
                A_P_targets.Add(GetActivePassiveTarget(numbers[0]));

                Features f = new Features();
                f.PitchDDS = new DDS();
                f.PitchDDS.Difference = Convert.ToDouble(numbers[1]);
                f.IntensityDDS = new DDS();
                f.IntensityDDS.Distance = Convert.ToDouble(numbers[2]);
                f.F1DDS = new DDS();
                f.F1DDS.Difference = Convert.ToDouble(numbers[3]);
                f.F3DDS = new DDS();
                f.F3DDS.Difference = Convert.ToDouble(numbers[4]);
                f.PitchRange = Convert.ToDouble(numbers[5]);
                f.IntensityRange = Convert.ToDouble(numbers[6]);
                f.PitchVariance = Convert.ToDouble(numbers[7]);
                f.IntensityVariance = Convert.ToDouble(numbers[8]);
                f.PhraseDurationMean = Convert.ToDouble(numbers[9]);
                f.Centroid = Convert.ToDouble(numbers[10]);


                features.Add(f);
            }
            training_f_list = features;

            List<SVMNode[]> scaledNodes = new List<SVMNode[]>();
            
            foreach (var feature in features)
            {
                Features scaled_f = new Features();
                scaled_f.PitchDDS = new DDS();
                scaled_f.PitchDDS.Difference = Scale(feature.PitchDDS.Difference, features.Min(f=>f.PitchDDS.Difference), features.Max(f=>f.PitchDDS.Difference), -1, 1);
                scaled_f.IntensityDDS = new DDS();
                scaled_f.IntensityDDS.Distance = Scale(feature.IntensityDDS.Distance, features.Min(f => f.IntensityDDS.Distance), features.Max(f => f.IntensityDDS.Distance), -1, 1);
                scaled_f.F1DDS = new DDS();
                scaled_f.F1DDS.Difference = Scale(feature.F1DDS.Difference, features.Min(f => f.F1DDS.Difference), features.Max(f => f.F1DDS.Difference), -1, 1);
                scaled_f.F3DDS = new DDS();
                scaled_f.F3DDS.Difference = Scale(feature.F3DDS.Difference, features.Min(f => f.F3DDS.Difference), features.Max(f => f.F3DDS.Difference), -1, 1);

                scaled_f.PitchRange = Scale(feature.PitchRange, features.Min(f => f.PitchRange), features.Max(f => f.PitchRange), -1, 1);
                scaled_f.IntensityRange = Scale(feature.IntensityRange, features.Min(f => f.IntensityRange), features.Max(f => f.IntensityRange), -1, 1);
                scaled_f.PitchVariance = Scale(feature.PitchVariance, features.Min(f => f.PitchVariance), features.Max(f => f.PitchVariance), -1, 1);
                scaled_f.IntensityVariance = Scale(feature.IntensityVariance, features.Min(f => f.IntensityVariance), features.Max(f => f.IntensityVariance), -1, 1);
                scaled_f.PhraseDurationMean = Scale(feature.PhraseDurationMean, features.Min(f => f.PhraseDurationMean), features.Max(f => f.PhraseDurationMean), -1, 1);
                scaled_f.Centroid = Scale(feature.Centroid, features.Min(f => f.Centroid), features.Max(f => f.Centroid), -1, 1);

                scaledNodes.Add(GetNodesFromFeatures(scaled_f));
            }
            training_set = scaledNodes;
        }
        public static List<Features> ScaleFeatures(List<Features> features, double C, double D)
        {
            List<Features> scaledFeatures = new List<Features>();

            foreach (var feature in features)
            {
                Features scaled_f = new Features();
                scaled_f.PitchDDS = new DDS();
                scaled_f.PitchDDS.Difference = Scale(feature.PitchDDS.Difference, features.Min(f => f.PitchDDS.Difference), features.Max(f => f.PitchDDS.Difference), -1, 1);
                scaled_f.IntensityDDS = new DDS();
                scaled_f.IntensityDDS.Distance = Scale(feature.IntensityDDS.Distance, features.Min(f => f.IntensityDDS.Distance), features.Max(f => f.IntensityDDS.Distance), -1, 1);
                scaled_f.F1DDS = new DDS();
                scaled_f.F1DDS.Difference = Scale(feature.F1DDS.Difference, features.Min(f => f.F1DDS.Difference), features.Max(f => f.F1DDS.Difference), -1, 1);
                scaled_f.F3DDS = new DDS();
                scaled_f.F3DDS.Difference = Scale(feature.F3DDS.Difference, features.Min(f => f.F3DDS.Difference), features.Max(f => f.F3DDS.Difference), -1, 1);

                scaled_f.PitchRange = Scale(feature.PitchRange, features.Min(f => f.PitchRange), features.Max(f => f.PitchRange), -1, 1);
                scaled_f.IntensityRange = Scale(feature.IntensityRange, features.Min(f => f.IntensityRange), features.Max(f => f.IntensityRange), -1, 1);
                scaled_f.PitchVariance = Scale(feature.PitchVariance, features.Min(f => f.PitchVariance), features.Max(f => f.PitchVariance), -1, 1);
                scaled_f.IntensityVariance = Scale(feature.IntensityVariance, features.Min(f => f.IntensityVariance), features.Max(f => f.IntensityVariance), -1, 1);
                scaled_f.PhraseDurationMean = Scale(feature.PhraseDurationMean, features.Min(f => f.PhraseDurationMean), features.Max(f => f.PhraseDurationMean), -1, 1);
                scaled_f.Centroid = Scale(feature.Centroid, features.Min(f => f.Centroid), features.Max(f => f.Centroid), -1, 1);
                
                scaledFeatures.Add(scaled_f);
                
            }
            return scaledFeatures;
        }
        public static List<Features> ScaleFeatures(List<Features> features)
        {
            List<Features> scaledFeatures = new List<Features>();

            foreach (var feature in features)
            {
                Features scaled_f = new Features();
                scaled_f.PitchDDS = new DDS();
                scaled_f.PitchDDS.Difference = Scale(feature.PitchDDS.Difference,GetMinScale(0),GetMaxScale(0) , -1, 1);
                scaled_f.IntensityDDS = new DDS();
                scaled_f.IntensityDDS.Distance = Scale(feature.IntensityDDS.Distance, GetMinScale(1), GetMaxScale(1), -1, 1);
                scaled_f.F1DDS = new DDS();
                scaled_f.F1DDS.Difference = Scale(feature.F1DDS.Difference, GetMinScale(2), GetMaxScale(2), -1, 1);
                scaled_f.F3DDS = new DDS();
                scaled_f.F3DDS.Difference = Scale(feature.F3DDS.Difference, GetMinScale(3), GetMaxScale(3), -1, 1);

                scaled_f.PitchRange = Scale(feature.PitchRange, GetMinScale(4), GetMaxScale(4), -1, 1);
                scaled_f.IntensityRange = Scale(feature.IntensityRange, GetMinScale(5), GetMaxScale(5), -1, 1);
                scaled_f.PitchVariance = Scale(feature.PitchVariance, GetMinScale(6), GetMaxScale(6), -1, 1);
                scaled_f.IntensityVariance = Scale(feature.IntensityVariance, GetMinScale(7), GetMaxScale(7), -1, 1);
                scaled_f.PhraseDurationMean = Scale(feature.PhraseDurationMean, GetMinScale(8), GetMaxScale(8), -1, 1);
                scaled_f.Centroid = Scale(feature.Centroid, GetMinScale(9), GetMaxScale(9), -1, 1);

                scaledFeatures.Add(scaled_f);

            }
            return scaledFeatures;
        }
        private SVMNode[] GetScaledNodes(Features feature)
        {
            double[] values = new double[10];

            //values[0] = Scale(feature.PitchDDS.Difference, training_f_list.Min(f => f.PitchDDS.Difference), training_f_list.Max(f => f.PitchDDS.Difference), -1, 1);
            //values[1] = Scale(feature.IntensityDDS.Distance, training_f_list.Min(f => f.IntensityDDS.Distance), training_f_list.Max(f => f.IntensityDDS.Distance), -1, 1);
            //values[2] = Scale(feature.F1DDS.Difference, training_f_list.Min(f => f.F1DDS.Difference), training_f_list.Max(f => f.F1DDS.Difference), -1, 1);
            //values[3] = Scale(feature.F3DDS.Difference, training_f_list.Min(f => f.F3DDS.Difference), training_f_list.Max(f => f.F3DDS.Difference), -1, 1);
            //values[4] = Scale(feature.PitchRange, training_f_list.Min(f => f.PitchRange), training_f_list.Max(f => f.PitchRange), -1, 1);
            //values[5] = Scale(feature.IntensityRange, training_f_list.Min(f => f.IntensityRange), training_f_list.Max(f => f.IntensityRange), -1, 1);
            //values[6] = Scale(feature.PitchVariance, training_f_list.Min(f => f.PitchVariance), training_f_list.Max(f => f.PitchVariance), -1, 1);
            //values[7] = Scale(feature.IntensityVariance, training_f_list.Min(f => f.IntensityVariance), training_f_list.Max(f => f.IntensityVariance), -1, 1);
            //values[8] = Scale(feature.PhraseDurationMean, training_f_list.Min(f => f.PhraseDurationMean), training_f_list.Max(f => f.PhraseDurationMean), -1, 1);
            //values[9] = Scale(feature.Centroid, training_f_list.Min(f => f.Centroid), training_f_list.Max(f => f.Centroid), -1, 1);

            values[0] = Scale(feature.PitchDDS.Difference, GetMinScale(0), GetMaxScale(0), -1, 1);
            values[1] = Scale(feature.IntensityDDS.Distance, GetMinScale(1), GetMaxScale(1), -1, 1);
            values[2] = Scale(feature.F1DDS.Difference, GetMinScale(2), GetMaxScale(2), -1, 1);
            values[3] = Scale(feature.F3DDS.Difference, GetMinScale(3), GetMaxScale(3), -1, 1);
            values[4] = Scale(feature.PitchRange, GetMinScale(4), GetMaxScale(4), -1, 1);
            values[5] = Scale(feature.IntensityRange, GetMinScale(5), GetMaxScale(5), -1, 1);
            values[6] = Scale(feature.PitchVariance, GetMinScale(6), GetMaxScale(6), -1, 1);
            values[7] = Scale(feature.IntensityVariance, GetMinScale(7), GetMaxScale(7), -1, 1);
            values[8] = Scale(feature.PhraseDurationMean, GetMinScale(8), GetMaxScale(8), -1, 1);
            values[9] = Scale(feature.Centroid, GetMinScale(9), GetMaxScale(9), -1, 1);
            SVMNode[] nodes=new SVMNode[values.Length];

            for (int i = 0; i < values.Length; i++)
            {
                nodes[i] = new SVMNode(i, values[i]);
            }
            return nodes;
        }
        private SVMNode[] GetNodesFromFeatures(Features features)
        {
            var f_array=features.GetFeaturesArray();

            SVMNode[] nodes = new SVMNode[f_array.Length];

            for (int i = 0; i < f_array.Length; i++)
            {
                nodes[i] = new SVMNode(i, f_array[i]);
            }
            return nodes;
        }
        private static double Scale(double x, double A, double B, double C, double D)
        {
            return C*(1-((x-A)/(B-A)))+D*((x-A)/(B-A));
        }
        public static double GetPositiveNegativeTarget(string filename)
        {
            if (filename.Contains("happiness") || filename.Contains("neutral"))
                return 1;
            else return -1;
        }
        public static double GetActivePassiveTarget(string filename)
        {
            if (filename.Contains("anger") || filename.Contains("happy"))
                return 1;
            else return -1;
        }

        public static double GetMinScale(int index)
        {
            switch (index)
            {
                case 0: return 0;
                case 1: return -50;
                case 2: return 0;
                case 3: return 0;
                case 4: return 0;
                case 5: return 0;
                case 6: return 0;
                case 7: return 0;
                case 8: return 0;
                case 9: return 0;
                default: return -1;
            }
        }
        public static double GetMaxScale(int index)
        {
            switch (index)
            {
                case 0: return 1000;
                case 1: return 100;
                case 2: return 1000;
                case 3: return 1000;
                case 4: return 1000;
                case 5: return 50;
                case 6: return 100000;
                case 7: return 100;
                case 8: return 100;
                case 9: return 1000;
                default: return -1;
            }
        }
        
    }
}
