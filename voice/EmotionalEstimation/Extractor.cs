using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;

namespace EmotionalEstimation
{
    class Extractor
    {
        public SoundCharacteristics ExtractValues(string file)
        {
            ProcessStartInfo si = new ProcessStartInfo();
            si.FileName = "praat/praatcon.exe";

            si.RedirectStandardOutput = true;
            si.WindowStyle = ProcessWindowStyle.Hidden;
            si.UseShellExecute = false;
            si.CreateNoWindow = true;

            si.Arguments = @"-a C:\praatfiles\words.praat " + file;

            Process process = new Process();
            process.StartInfo = si;
            process.Start();
            string resultLine = process.StandardOutput.ReadToEnd();

            SoundCharacteristics result=new SoundCharacteristics();

            if (resultLine.StartsWith("Error"))
                throw new Exception("Ошибка обработки файла!");
            else
            {

                var values = resultLine.Trim().Split(' ');

                int start=0;
                int end=0;

                foreach (var value in values)
                {
                    var numbers = value.Split('/');
                    double pitch = Convert.ToDouble(numbers[0].Replace('.', ','));
                    double intensity = Convert.ToDouble(numbers[1].Replace('.', ','));

                    if (pitch > 0)
                    {
                        result.PitchValues.Add(pitch);
                        result.IntensityValues.Add(intensity);
                        end++;
                    }
                    else
                    {
                        if(end>start)
                        {
                            result.Phrases.Add(new Phrase(){ Start=start, End=end});
                            start=end;
                        }
                        result.PitchValues.Add(0);
                        result.IntensityValues.Add(0);
                        start++;
                        end=start;
                    }
                }
                result.RangePitch = result.PitchValues.Max() - result.PitchValues.Where(p => p > 0).Min();
                result.RangeIntensity = result.IntensityValues.Max() - result.IntensityValues.Where(p => p > 0).Min();
                result.AverageIntensity = result.IntensityValues.Average();
                result.AveragePitch = result.PitchValues.Average();


                List<Phrase> phrases=new List<Phrase>();
                bool lastIsMerged=false;

                for (int i = 0; i < result.Phrases.Count - 1; i++)
                {
                    if ((result.Phrases[i + 1].Start-result.Phrases[i].End) < 5)
                    {
                        if (lastIsMerged)
                            phrases.Last().End = result.Phrases[i + 1].End;
                        else
                            phrases.Add(new Phrase() { Start = result.Phrases[i].Start, End = result.Phrases[i + 1].End });

                        lastIsMerged = true;
                    }
                    else
                    {
                        if (!lastIsMerged)
                            phrases.Add(result.Phrases[i]);
                        lastIsMerged = false;
                    }
                }
                result.Phrases = phrases;
            }
            
            /*double sumInt = 0;
            double sumPitch = 0;
            double minInt=10000;
            double maxInt=0;
            double minPitch=100000;
            double maxPitch=0;

            foreach (var phrase in phrases)
            {
                sumInt += phrase.Intensity.Average();
                sumPitch += phrase.Pitch.Average();

                double min=phrase.Intensity.Min();//Intensity ranges
                if(min<minInt)
                    minInt=min;
                double max=phrase.Intensity.Max();
                if(max>maxInt)
                    maxInt=max;

                min=phrase.Pitch.Min();//Pitch ranges
                if(min<minPitch)
                    minPitch=min;
                max=phrase.Pitch.Max();
                if(max>maxPitch)
                    maxPitch=max;
            }*/

            return result;
        }
    }
    class SoundCharacteristics
    {
        public List<Phrase> Phrases;

        public List<double> PitchValues;
        public List<double> IntensityValues;

        public double AveragePitch;
        public double AverageIntensity;
        public double RangePitch;
        public double RangeIntensity;

        public SoundCharacteristics()
        {
            Phrases = new List<Phrase>();
            PitchValues = new List<double>();
            IntensityValues = new List<double>();
        }
    }
    class Phrase
    {
        public int Start;
        public int End;

        public double AveragePitch;
        public double AverageIntensity;

    }
}
