using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;
using NAudio.Dsp;
using NAudio.CoreAudioApi;

namespace voice
{
    class SoundParametersExtractor
    {
        private static int fftLength = 4096; // размер выборки для БПФ(степень двойки)

        // класс для обработки БПФ
        private SampleAggregator sampleAggregator;
        private WaveFileReader reader;

        ProcessingResult Results;//здесь будем хранить данные до их возвращения

        public SoundParametersExtractor()
        {
            sampleAggregator = new SampleAggregator(fftLength);
            sampleAggregator.FftCalculated += new EventHandler<FftEventArgs>(FftCalculated);
            sampleAggregator.PerformFFT = true;
        }
        public ProcessingResult Process(string fileName)
        {
            reader = new WaveFileReader(fileName);
            Results = new ProcessingResult();
            Results.FileName = fileName;

            byte[] buffer = new byte[35000];
            int readed = reader.Read(buffer, 0, 35000);

            while (readed != 0)
            {
                WaveInEventArgs args = new WaveInEventArgs(buffer, readed);
                OnDataAvailable(reader.WaveFormat.BlockAlign, args);
                readed = reader.Read(buffer, 0, 35000);
            }

            return Results;
        }

        void OnDataAvailable(object sender, WaveInEventArgs e)//обработка в каждый момент времени
        {
            byte[] buffer = e.Buffer;
            int bytesRecorded = e.BytesRecorded;
            int bufferIncrement = reader.WaveFormat.BlockAlign;

            for (int index = 0; index < bytesRecorded; index += bufferIncrement)
            {
                float sample32 = BitConverter.ToSingle(buffer, index);
                sampleAggregator.Add(sample32);
            }
        }


        void FftCalculated(object sender, FftEventArgs e)
        {
            float binSize = reader.WaveFormat.SampleRate / fftLength;
            int minBin = Freq2Index(300, reader.WaveFormat.SampleRate, fftLength);
            int maxBin = Freq2Index(4000, reader.WaveFormat.SampleRate, fftLength);
            float maxIntensity = 0;
            int maxBinIndex = 0;

            List<int> freqs = new List<int>();

            for (int bin = minBin; bin <= maxBin; bin++)
            {
                float real = e.Result[bin].X;
                float imaginary = e.Result[bin].Y;
                float intensity = Math.Abs(real * real + imaginary * imaginary);

                if (intensity > maxIntensity)
                {
                    maxIntensity = intensity;
                    maxBinIndex = bin;
                }
            }
            //WaveDats.Add(new ProcessingResult() { Freq = Index2Freq(maxBinIndex, reader.WaveFormat.SampleRate, fftLength), Intensity = maxIntensity });

            //сохраняем данные для этого момента времени
            if (maxIntensity > 0.000001)
            {
                Results.Pitchs.Add(Index2Freq(maxBinIndex, reader.WaveFormat.SampleRate, fftLength));
                Results.Intens.Add(maxIntensity);
            }

            //OnTick(new ProcessingResult() { Freq = Index2Freq(maxBinIndex, reader.WaveFormat.SampleRate, fftLength), Intensity = maxIntensity });
        }

        public static double Index2Freq(int i, double samples, int nFFT)
        {
            return (double)i * (samples / nFFT);
        }

        public static int Freq2Index(double freq, double samples, int nFFT)
        {
            return (int)(freq / (samples / nFFT));
        }
    }

    class ProcessingResult
    {
        public string FileName { get; set; }

        public List<double> Pitchs;
        public List<double> Intens;

        public double AveragePitch
        {
            get
            {
                if (Pitchs.Count>0)
                    return Pitchs.Average(p => p);
                else
                    return -1;
            }
        }
        public double AverageIntensity
        {
            get
            {
                if (Intens.Count>0)
                    return Intens.Average(p => p);
                else
                    return -1;
            }
        }
        public double RangePitch
        {
            get
            {
                if (Pitchs.Count > 0)
                    return Pitchs.Max() - Pitchs.Min();
                else
                    return -1;
            }
        }
        public double RangeIntensity
        {
            get
            {
                if (Intens.Count > 0)
                    return Intens.Max() - Intens.Min();
                else
                    return -1;
            }
        }

        public ProcessingResult()
        {
            Pitchs = new List<double>();
            Intens = new List<double>();
        }
    }
}
