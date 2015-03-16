using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace voice
{
    class Estimator
    {
        public List<ProcessingResult> ProcessingResults;
        private bool IsMale { get; set; }//Пол

        public Estimator(bool IsMale)
        {
            this.IsMale = IsMale;
            ProcessingResults = new List<ProcessingResult>();
        }
        public void Add(ProcessingResult processingResult)
        {
            ProcessingResults.Add(processingResult);
        }
        public EmotionalEstimate Estimate(ProcessingResult processingResult)
        {
            EmotionalEstimate result = new EmotionalEstimate();
            result.FileName = processingResult.FileName;

            double AveragePitch=ProcessingResults.Average(p=>p.AveragePitch);
            double AverageIntensity=ProcessingResults.Average(p=>p.AverageIntensity);
            double RangePitch=ProcessingResults.Average(p=>p.RangePitch);
            double RangeIntensity=ProcessingResults.Average(p=>p.RangeIntensity);

            double dif = processingResult.AveragePitch - AveragePitch;//обработка Pitch
            if (dif > 0)//больше среднего
            {
                    int procdif = (int)(dif / (AveragePitch / 100));

                    if (procdif > 20)
                    {
                        result.Fear = 35;
                        result.Anger = 35;
                    }
                    else
                    {
                        result.Joy = 35;
                        result.Fear = 20;
                    }
            }
            else//меньше среднего
            {
                result.Disgust = 35;
                result.Sadness = 35;
            }

            dif = processingResult.AverageIntensity - AverageIntensity;//обработка Intensity
            
            if (dif >= 0)//больше среднего
            {
                if (IsMale)
                    result.Anger += 35;  

                result.Fear += 35;
                result.Joy += 35;
            }
            else//меньше среднего
            {
                if(!IsMale)
                    result.Anger += 35;

                result.Disgust += 35;
                result.Sadness += 35;
            }


            dif = processingResult.RangePitch - RangePitch;//обработка RangePitch
            if (dif >= 0)//больше среднего
            {
                result.Anger += 15;
                result.Fear += 15;
                result.Joy += 35;

                if(IsMale)
                    result.Disgust += 15;
            }
            else//меньше среднего
            {
                if (!IsMale)
                    result.Disgust += 15;

                result.Sadness += 15;
            }

            dif = processingResult.RangeIntensity - RangeIntensity;//обработка RangeIntensity
            if (dif >= 0)//больше среднего
            {
                result.Anger +=15;
                result.Sadness += 15;
            }

            return result;
        }
    }

    class EmotionalEstimate//класс предоставления результатов оценки
    {
        public string FileName{ get; set;}

        public int Joy { get; set; }
        public int Fear { get; set; }
        public int Disgust { get; set; }
        public int Anger { get; set; }
        public int Sadness { get; set; }
    }
}
