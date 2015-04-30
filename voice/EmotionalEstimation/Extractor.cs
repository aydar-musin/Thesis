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
            List<PhraseValues> phrases = new List<PhraseValues>();

            PraatManager.Start();
            PraatManager.Execute(string.Format("\"runScript: \\\"{0}\\\", {1}, {2}, \\\"{3}\\\", {4}\"", @"C:\praatfiles\words.praat", "50", "1", file, "0.3"));
            PraatManager.Stop();
            if (!File.Exists(@"C:\praatfiles\results.txt"))
            {
                return new SoundCharacteristics() { Phrases = new List<PhraseValues>() };
            }

            var resultLines = File.ReadAllLines(@"C:\praatfiles\results.txt");

            foreach (var line in resultLines)
            {
                PhraseValues phrase = new PhraseValues();

                var values = line.Trim().Split(' ');
                foreach (var value in values)
                {
                    var numbers = value.Split('/');
                    double pitch = Convert.ToDouble(numbers[0].Replace('.', ','));
                    double intensity = Convert.ToDouble(numbers[1].Replace('.', ','));

                    //if (pitch > 0)    //splitting to words
                    //{
                        phrase.Pitch.Add(pitch);
                        phrase.Intensity.Add(intensity);
                    //}
                }
                phrases.Add(phrase);
                
            }
            File.Delete(@"C:\praatfiles\results.txt");
            
            double sumInt = 0;
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
            }

            SoundCharacteristics result = new SoundCharacteristics();
            result.Phrases = phrases;

            result.AverageIntensity = sumInt / phrases.Count;
            result.AveragePitch = sumPitch / phrases.Count;
            result.RangeIntensity = maxInt - minInt;
            result.RangePitch = maxPitch - minPitch;

            return result;
        }
    }
    class SoundCharacteristics
    {
        public List<PhraseValues> Phrases;
        public double AveragePitch;
        public double AverageIntensity;
        public double RangePitch;
        public double RangeIntensity;
    }
    class PhraseValues
    {
        public List<double> Intensity;
        public List<double> Pitch;

        public PhraseValues()
        {
            Intensity = new List<double>();
            Pitch = new List<double>();
        }
    }
}
