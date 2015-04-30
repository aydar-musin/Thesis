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

            Task.Run(() =>
                {
                    Extractor extractor = new Extractor();
                    result = extractor.ExtractValues(dialog.FileName);
                    this.Invoke(new Action(() =>
                        {
                            phraseslistBox.Items.Clear();
                            int i=0;
                            foreach (var phrase in result.Phrases)
                            {
                                phraseslistBox.Items.Add(i);
                                i++;
                            }
                            phraseslistBox.Enabled = true;
                            chart1.Enabled = true;
                        }));
                    
                });
        }
        private void ShowPhrase(PhraseValues phrase)
        {
            Chart chart = new Chart();
            Series PitchSeries = new Series("Pitch");
            PitchSeries.Color = Color.Blue;
            PitchSeries.IsVisibleInLegend = false;
            PitchSeries.ChartType = SeriesChartType.Line;
            PitchSeries.ChartArea = "ChartArea1";
            PitchSeries.Legend = "Legend1";

            phrase.Pitch.ForEach(p => { PitchSeries.Points.AddY(p); });
            chart.Series.Add(PitchSeries);

            Series IntSeries = new Series("Intensity");
            IntSeries.Color = Color.Red;
            IntSeries.IsVisibleInLegend = false;
            IntSeries.ChartType = SeriesChartType.Line;
            IntSeries.ChartArea = "ChartArea1";
            IntSeries.Legend = "Legend1";

            phrase.Intensity.ForEach(i => { IntSeries.Points.AddY(i); });

            chart1.Series.Clear();
            chart2.Series.Clear();
            chart1.Series.Add(PitchSeries);
            chart2.Series.Add(IntSeries);
            chart1.DataBind();
            chart2.DataBind();
        }

        private void phraseslistBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowPhrase(result.Phrases[phraseslistBox.SelectedIndex]);
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

            int line = 10;
            int column=1;
            int count = 0;

            Task.Run(() =>
            {
                foreach(var file in Directory.GetFiles(dialog.SelectedPath))
                {
                    Extractor extractor = new Extractor();
                    var result = extractor.ExtractValues(file);
                    foreach (var phrase in result.Phrases)
                    {
                        for (int i = 0; i < phrase.Intensity.Count; i++)
                        {
                            this.Invoke(new Action(() =>
                            {
                                ObjWorkSheet.Cells[line + i, column] = phrase.Pitch[i];
                                ObjWorkSheet.Cells[line + i, column + 1] = phrase.Intensity[i];
                            }));
                        }
                    }
                    column += 4;
                    line = 10;
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
