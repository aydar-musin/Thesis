using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;

namespace EmotionalEstimation
{
    public partial class Form1 : Form
    {
        private Classifier classifier;
        private SVMClassifier svm_Classifier;

        public Form1()
        {
            InitializeComponent();
            svm_Classifier = new SVMClassifier();
            svm_Classifier.FromFiles();

        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = "*.wav|Звуковые файлы";
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.FileName))
                return;

            infoLabel.Text = "Processing...";
            this.Text = dialog.FileName;
            bool active_passive = ActivePassiveCheckBox.Checked;

            Task.Run(() =>
                {
                    Extractor extractor = new Extractor();
                    var sound = extractor.ExtractValues(dialog.FileName);
                    ShowGraphics(sound);
                    
                    if (classifier != null)
                    {
                        var features = Analyzer.GetFeatures(sound,Emotions.Neutral);

                        if (active_passive)
                        {
                            var result = classifier.ClassifyByActivePassive(features);
                            string result_string = string.Format("Active- {0}{2}Passive- {1}{2}", result.Active, result.Passive, Environment.NewLine);
                            Message_Result(result_string);
                        }
                        else
                        {
                            var result = classifier.Classify(features);

                            string result_string = string.Format("Anger- {0}{3}Happiness- {1}{3}Sadness- {2}{3}Neutral- {4}{3}{3}", result.Anger, result.Happiness, result.Sadness, Environment.NewLine, result.Neutral);
                            Message_Result(result_string);
                        }
                    }
                });
        }
        private void ShowGraphics(SoundCharacteristics sound)
        {
            #region PitchSeries
            Series PitchSeries = new Series("Pitch");
            PitchSeries.Color = Color.Blue;
            PitchSeries.IsVisibleInLegend = false;
            PitchSeries.ChartType = SeriesChartType.Line;
            PitchSeries.ChartArea = "ChartArea1";
            PitchSeries.Legend = "Legend1";
            
            sound.PitchValues.ForEach(p => { PitchSeries.Points.AddY(p); });
            #endregion PitchSeries

            #region F1Series
            Series F1 = new Series("F1");
            F1.Color = Color.FromArgb(30,144,255);
            F1.IsVisibleInLegend = false;
            F1.ChartType = SeriesChartType.Line;
            F1.ChartArea = "ChartArea1";
            F1.Legend = "Legend1";
            sound.F1Values.ForEach(f => F1.Points.AddY(f));

            #endregion F1Series

            #region F2Series
            Series F2 = new Series("F2");
            F2.Color = Color.FromArgb(0,191,255);
            F2.IsVisibleInLegend = false;
            F2.ChartType = SeriesChartType.Line;
            F2.ChartArea = "ChartArea1";
            F2.Legend = "Legend1";
            sound.F2Values.ForEach(f => F2.Points.AddY(f));

            #endregion F2Series

            #region F3Series
            Series F3 = new Series("F3");
            F3.Color = Color.FromArgb(135,206,250);
            F3.IsVisibleInLegend = false;
            F3.ChartType = SeriesChartType.Line;
            F3.ChartArea = "ChartArea1";
            F3.Legend = "Legend1";
            sound.F3Values.ForEach(f => F3.Points.AddY(f));

            #endregion F3Series

            Series IntSeries = new Series("Intensity");
            IntSeries.Color = Color.Red;
            IntSeries.IsVisibleInLegend = false;
            IntSeries.ChartType = SeriesChartType.Line;
            IntSeries.ChartArea = "ChartArea1";
            IntSeries.Legend = "Legend1";

            sound.IntensityValues.ForEach(i => { IntSeries.Points.AddY(i); });

            double maxY = sound.PitchValues.Max();

            Series phraseSeries = new Series("phrases");
            phraseSeries.Color = Color.Green;
            phraseSeries.IsVisibleInLegend = false;
            phraseSeries.ChartType = SeriesChartType.Point;
            phraseSeries.ChartArea = "ChartArea1";
            phraseSeries.Legend = "Legend1";
            

            /*foreach (var phrase in result.Phrases)
            {
                var phraseValues = result.PitchValues.GetRange(phrase.Start, phrase.End - phrase.Start).ToArray();
                phraseSeries.Points.AddXY(phrase.Start, maxY);

                var dds = Analyzer.ComputeDDS(phraseValues);
                phraseSeries.Points.Add(new DataPoint() { XValue=phrase.Start, YValues=new[]{maxY}, Label=string.Format("{0}/{1}/{2}",dds.PitchDifference,dds.PitchDistance, dds.Slope)});
 
            }
            result.Phrases.ForEach(p => { });
            */

            this.Invoke(new Action(() =>
                {
                    chart1.ChartAreas[0].AxisX.Minimum = 0;
                    chart1.Series.Clear();
                    chart2.Series.Clear();
                    chart1.Series.Add(PitchSeries);
                    chart1.Series.Add(F1);
                    chart1.Series.Add(F2);
                    chart1.Series.Add(F3);
                    infoLabel.Text = sound.Centroid.ToString();

                    chart2.Series.Add(IntSeries);
                    chart1.Series.Add(phraseSeries);

                    chart1.DataBind();
                    chart2.DataBind();
                }));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            //dialog.Filter = "*.wav|Звуковые файлы";
            dialog.ShowDialog();
            if (string.IsNullOrEmpty(dialog.SelectedPath))
                return;

            Task.Run(() =>
            {
                int count = 0;
                var files=Directory.GetFiles(dialog.SelectedPath);
                int anger_count = 0;
                int happy_count = 0;
                int sadness_count = 0;
                int neutral_count = 0;

                foreach (var file in files)
                {
                    Extractor extractor = new Extractor();
                    var sound = extractor.ExtractValues(file);
                    var features=Analyzer.GetFeatures(sound,Emotions.Neutral);

                    var emotions_result=classifier.Classify(features);

                    var emotion=emotions_result.GetMax();
                    

                    if (emotion==Emotions.Anger)
                        anger_count++;
                    else if (emotion==Emotions.Happy)
                        happy_count++;
                    else if(emotion==Emotions.Sadness)
                        sadness_count++;
                    else
                        neutral_count++;
                    
                    this.Invoke(new Action(() =>
                            {
                                this.infoLabel.Text = string.Format("{0} processed",++count);
                            }));
                }


                double anger_precent=(anger_count*100)/files.Length;
                double happy_precent=(happy_count*100)/files.Length;
                double sadness_precent=(sadness_count*100)/files.Length;
                double neutral_precent=(neutral_count*100)/files.Length;

                Message_Result(string.Format("{0} anger{4} {1} happy{4} {2} sadness{4} {3} neutral",anger_precent,happy_precent,sadness_precent,neutral_precent,Environment.NewLine));
                
                
            });
        }

        private void LearnButton_Click(object sender, EventArgs e)
        {
            string path = "E:/praatfiles/training_set/";

            Dictionary<string,Emotions> dirsToAnalyze = new Dictionary<string,Emotions>();
            dirsToAnalyze.Add("anger",Emotions.Anger);
            dirsToAnalyze.Add("happiness",Emotions.Happy);
            dirsToAnalyze.Add("sadness",Emotions.Sadness);
            dirsToAnalyze.Add("neutral",Emotions.Neutral);

            int count = 0;

            Task.Factory.StartNew(() =>
                {
                    if (File.Exists("analyze_result.txt"))
                        File.Delete("analyze_result.txt");
                    File.AppendAllText("analyze_result.txt",
                    "filename\tddsPitchDif\tddsPitchDis\tddsIntDif\tddsIntDis\tddsF1Dif\tddsF1Dis\tF2dif\tF2Dis\tF3Dif\tF3Dis\tPitchRange\tIntRange\tPitchVariance\tIntVariance\tPhraseDuration\tSilenceDuration\tCentroid\tF3Variance\tF3Range\n");
                    
                    List<Features> featuresList = new List<Features>();
                    List<double> targets=new List<double>();


                    foreach (var dir in dirsToAnalyze.Keys)
                    {
                        var files = Directory.GetFiles(path + dir);

                        foreach (var file in files)
                        {
                            Extractor ex = new Extractor();
                            var sound = ex.ExtractValues(file);

                            var feature = Analyzer.GetFeatures(sound, dirsToAnalyze[dir]);
                            featuresList.Add(feature);

                            targets.Add(SVMClassifier.GetActivePassiveTarget(file));
                            count++;
                            Message_Classifier(count.ToString());
                        }
                    }
                    var scaled = SVMClassifier.ScaleFeatures(featuresList);

                    for(int i=0;i<targets.Count;i++)
                        File.AppendAllLines("analyze_result.txt", new string[] { scaled[i].GetSVMString(targets[i].ToString()) });

                    classifier = new Classifier();
                    classifier.BuildEngine(featuresList);
                    SaveFeatures(featuresList);
                    Message_Result("Complete!");
                });
        }
        private void Message_Classifier(string msg)
        {
            this.Invoke(new Action(()=>
                {
                    infoLabel2.Text = msg;
                }));
        }
        private void Message_Processing(string msg)
        {
            this.Invoke(new Action(() =>
            {
                infoLabel.Text = msg;
            }));
        }
        private void Message_Result(string msg)
        {
            this.Invoke(new Action(() =>
            {
                ResultsBox.Text = "";
                ResultsBox.Text = msg;
            }));
        }
        private DDS ProcessFile(string fileName)
        {
            //Extractor extractor = new Extractor();
            //var extraction_result = extractor.ExtractValues(fileName);

            //List<DDS> ddsList = new List<DDS>();
            //foreach (var phrase in extraction_result.Phrases)
            //{
            //    var pitchValues = extraction_result.PitchValues.GetRange(phrase.Start, phrase.End - phrase.Start).ToArray();
            //    var intValues = extraction_result.IntensityValues.GetRange(phrase.Start, phrase.End - phrase.Start).ToArray();

            //    ddsList.Add(Analyzer.ComputeDDS(pitchValues, intValues));
            //}
            //DDS result = new DDS() 
            //{ 
            //    PitchDifference=ddsList.Average(d=>d.PitchDifference),
            //    PitchDistance=ddsList.Average(d=>d.PitchDistance),
            //    PitchIntensityDifference=ddsList.Average(d=>d.PitchIntensityDifference),
            //    IntensityDistance=ddsList.Average(d=>d.IntensityDistance)
            //};
            //return result;
            return new DDS();
        }
        private void SaveFeatures(List<Features> features)
        {
            Stream FileStream = File.Create("Features.bin");
            BinaryFormatter serializer = new BinaryFormatter();
            serializer.Serialize(FileStream, features);
            FileStream.Close();
        }
        private List<Features> ReadFeatures()
        {
            if (File.Exists("Features.bin"))
            {
                try
                {
                    Stream FileStream = File.OpenRead("Features.bin");
                    BinaryFormatter deserializer = new BinaryFormatter();
                    List<Features> result = (List<Features>)deserializer.Deserialize(FileStream);
                    FileStream.Close();
                    return result;
                }
                catch
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            var features = ReadFeatures();
            if (features != null)
            {
                classifier = new Classifier();
                classifier.BuildEngine(features);
                classifier.EditFeatureWeights(new double[] /*{25,25,25,25,25,25,25,25,25,25,25,25,25,25,25}*/ {8,6,4,6,4,7,7,4,7,4,8,4,8,6,6,8 }); ;
                infoLabel2.Text = "Classifier is ready";
            }
        }
    }
}
