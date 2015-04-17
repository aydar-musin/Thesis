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
            if(!File.Exists(@"C:\praatfiles\results.txt"))
                throw new Exception("Не удалось выполнить анализ файла");

            var resultLines = File.ReadAllLines(@"C:\praatfiles\results.txt");

            foreach (var line in resultLines)
            {
                PhraseValues phrase = new PhraseValues();

                var values = line.Trim().Split(' ');
                foreach (var value in values)
                {
                    var numbers = value.Split('/');
                    float pitch = (float)Convert.ToDouble(numbers[0].Replace('.', ','));
                    float intensity = (float)Convert.ToDouble(numbers[1].Replace('.', ','));

                    if (pitch > 0)
                    {
                        phrase.Pitch.Add(pitch);
                        phrase.Intensity.Add(intensity);
                    }
                }
                phrases.Add(phrase);
                
            }
            File.Delete(@"C:\praatfiles\results.txt");
            
            float sumInt = 0;
            float sumPitch = 0;
            float minInt=10000;
            float maxInt=0;
            float minPitch=100000;
            float maxPitch=0;

            foreach (var phrase in phrases)
            {
                sumInt += phrase.Intensity.Average();
                sumPitch += phrase.Pitch.Average();

                float min=phrase.Intensity.Min();//Intensity ranges
                if(min<minInt)
                    minInt=min;
                float max=phrase.Intensity.Max();
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
        public float AveragePitch;
        public float AverageIntensity;
        public float RangePitch;
        public float RangeIntensity;
    }
    class PhraseValues
    {
        public List<float> Intensity;
        public List<float> Pitch;

        public PhraseValues()
        {
            Intensity = new List<float>();
            Pitch = new List<float>();
        }
    }
}
