using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;
using System.Threading;

namespace voice
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public string[] FilesToProcess;

        public MainWindow()
        {
            
            InitializeComponent();

            //List<object> source = new List<object>();
            //source.Add(new {File="testFile.Wav", Joy="80%", Fear="10%", Disgust="2%", Anger="5%", Sadness="20%"});
            //dataGrid.ItemsSource = source;
            //dataGrid.ColumnWidth = 100;

            //SoundParametersExtractor listener = new SoundParametersExtractor();
            
            //int X0 = 0;
            //List<ProcessingResult> temp = new List<ProcessingResult>();

            //double lastY=300/10;
            //Point lastPoint=new Point();
            
            //var result = listener.Process("test.wav");

            //result.ToString();

        }

        private void SetFilesButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Multiselect = true;
            dialog.Filter = "Звуковые файлы (*.wav)|*.wav";
            dialog.ShowDialog();

            FilesToProcess = dialog.FileNames;
            SetFilesInfoLabbel.Content = "Выбрано: " + FilesToProcess.Length + " файлов";
            ProcessButton.IsEnabled = true;
        }

        private void ProcessButton_Click(object sender, RoutedEventArgs e)
        {
                SoundParametersExtractor extractor = new SoundParametersExtractor();
                Estimator estimator = new Estimator(Combobox.SelectedIndex==0);
                ProgressBar.Value = 0;
                ProgressBar.Maximum=FilesToProcess.Length-1;
                List<EmotionalEstimate> results = new List<EmotionalEstimate>();
                dataGrid.ItemsSource = results;
                
                Task.Factory.StartNew(() =>
                    {
                        foreach (var file in FilesToProcess)
                        {
                            var prcResult = extractor.Process(file);
                            
                            estimator.Add(prcResult);
                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                ProgressBar.Value++;
                            }));
                        }

                        foreach (var prc in estimator.ProcessingResults)
                        {
                            var result = estimator.Estimate(prc);
                            results.Add(result);


                            this.Dispatcher.BeginInvoke(new Action(() =>
                            {
                                try
                                {
                                    Thread.Sleep(50);
                                    
                                    dataGrid.ItemsSource = results;
                                    dataGrid.Items.Refresh();
                                }
                                catch
                                {
                                }
                            }));
                        }
                        this.Dispatcher.BeginInvoke(new Action(() =>
                        {
                            //var globalResult = new ProcessingResult() { Pitchs=results.Select(r=>r.AveragePitch).ToList(), Intens=results.Select(r=>r.AverageIntensity).ToList() };
                            //ProcessingLabel.Content = string.Format("Av.P: {0}, Av.In: {1}, Ran.P: {2}, Ran.In: {3}", globalResult.AveragePitch, globalResult.AverageIntensity, globalResult.RangePitch, globalResult.RangeIntensity);

                        }));
                    });
        }
    }
}
