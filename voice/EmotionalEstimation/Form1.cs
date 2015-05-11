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

namespace EmotionalEstimation
{
    public partial class Form1 : Form
    {
        private SoundCharacteristics result;

        public Form1()
        {
            InitializeComponent();
        }

        private void selectFileButton_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            //dialog.Filter = "*.wav|Звуковые файлы";
            dialog.ShowDialog();
            infoLabel.Text = "Идет обработка...";
            this.Text = dialog.FileName;

            Task.Run(() =>
                {
                    Extractor extractor = new Extractor();
                    result = extractor.ExtractValues(dialog.FileName);
                    this.Invoke(new Action(() =>
                        {
                            ShowGraphics();
                            infoLabel.Text = "Готово";
                        }));

                    List<Contour> contours = new List<Contour>();
                    List<Phrase> intphrases = new List<Phrase>();
                    foreach (var phrase in result.Phrases)
                    {
                        var phraseValues=result.PitchValues.GetRange(phrase.Start, phrase.End - phrase.Start).ToArray();
                        contours.Add(Analyzer.DetectContour(phraseValues));
                        var variance = Analyzer.GetVariance(phraseValues);

                        if (phraseValues.Average() > result.AveragePitch)
                            intphrases.Add(phrase);
                    }
                    contours.ToString();
                });
        }
        private void ShowGraphics()
        {
            Chart chart = new Chart();
            Series PitchSeries = new Series("Pitch");
            PitchSeries.Color = Color.Blue;
            PitchSeries.IsVisibleInLegend = false;
            PitchSeries.ChartType = SeriesChartType.Line;
            PitchSeries.ChartArea = "ChartArea1";
            PitchSeries.Legend = "Legend1";

            result.PitchValues.ForEach(p => { PitchSeries.Points.AddY(p); });
            chart.Series.Add(PitchSeries);

            Series IntSeries = new Series("Intensity");
            IntSeries.Color = Color.Red;
            IntSeries.IsVisibleInLegend = false;
            IntSeries.ChartType = SeriesChartType.Line;
            IntSeries.ChartArea = "ChartArea1";
            IntSeries.Legend = "Legend1";

            result.IntensityValues.ForEach(i => { IntSeries.Points.AddY(i); });

            double maxY = result.PitchValues.Max();

            Series phraseSeries = new Series("phrases");
            phraseSeries.Color = Color.Green;
            phraseSeries.IsVisibleInLegend = false;
            phraseSeries.ChartType = SeriesChartType.Point;
            phraseSeries.ChartArea = "ChartArea1";
            phraseSeries.Legend = "Legend1";
            result.Phrases.ForEach(p => { phraseSeries.Points.AddXY(p.Start, maxY); phraseSeries.Points.AddXY(p.End, maxY);});
            chart1.ChartAreas[0].AxisX.Minimum = 0;

            chart1.Series.Clear();
            chart2.Series.Clear();
            chart1.Series.Add(PitchSeries);
            chart2.Series.Add(IntSeries);
            chart1.Series.Add(phraseSeries);

            chart1.DataBind();
            chart2.DataBind();
        }

        private void phraseslistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowGraphics();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            //dialog.Filter = "*.wav|Звуковые файлы";
            dialog.ShowDialog();
            infoLabel.Text = "";
            Microsoft.Office.Interop.Excel.Application ObjExcel = new Microsoft.Office.Interop.Excel.Application();
            Microsoft.Office.Interop.Excel.Workbook ObjWorkBook;
            Microsoft.Office.Interop.Excel.Worksheet ObjWorkSheet;

            //Книга.
            ObjWorkBook = ObjExcel.Workbooks.Add(System.Reflection.Missing.Value);
            //Таблица.
            ObjWorkSheet = (Microsoft.Office.Interop.Excel.Worksheet)ObjWorkBook.Sheets[1];
            //ObjWorkSheet.Cells[1, 1] = "test";

            int line = 3;
            int column=1;
            int count = 0;

            Task.Run(() =>
            {
                foreach(var file in Directory.GetFiles(dialog.SelectedPath))
                {
                    Extractor extractor = new Extractor();
                    var result = extractor.ExtractValues(file);
                    var pitchVariance=Analyzer.GetVariance(result.PitchValues.ToArray());
                    var IntVariance=Analyzer.GetVariance(result.IntensityValues.ToArray());

                    ObjWorkSheet.Cells[line, column] = pitchVariance;
                    ObjWorkSheet.Cells[line, column + 1] = IntVariance;
                    ObjWorkSheet.Cells[line, column + 2] = result.RangePitch;
                    ObjWorkSheet.Cells[line, column + 3] = result.RangeIntensity;

                    List<Contour> contours = new List<Contour>();
                    foreach (var phrase in result.Phrases)
                    {
                        contours.Add(Analyzer.DetectContour(result.PitchValues.GetRange(phrase.Start, phrase.End - phrase.Start).ToArray()));
                    }
                    if(contours.Count(c => c == Contour.Rising)>(contours.Count/2))
                        ObjWorkSheet.Cells[line, column + 4] = "Rising";
                    else
                        ObjWorkSheet.Cells[line, column + 4] = "Falling";


                    column =1;
                    line++;
                    count++;

                    this.Invoke(new Action(() =>
                            {
                                this.infoLabel.Text = string.Format("{0} processed",count);
                            }));
                }

                this.Invoke(new Action(() =>
                {
                    ObjExcel.Visible = true;
                    ObjExcel.UserControl = true;
                }));
                
            });
        }
    }
}
