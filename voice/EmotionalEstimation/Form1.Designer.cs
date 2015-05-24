using System.Drawing;
using System.Windows.Forms.DataVisualization.Charting;

namespace EmotionalEstimation
{
    partial class Form1
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend3 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Legend legend4 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.selectFileButton = new System.Windows.Forms.Button();
            this.infoLabel = new System.Windows.Forms.Label();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.chart2 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.button1 = new System.Windows.Forms.Button();
            this.LearnButton = new System.Windows.Forms.Button();
            this.infoLabel2 = new System.Windows.Forms.Label();
            this.ClassificatorGroupBox = new System.Windows.Forms.GroupBox();
            this.ProcessingGroupBox = new System.Windows.Forms.GroupBox();
            this.ResultsBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).BeginInit();
            this.ClassificatorGroupBox.SuspendLayout();
            this.ProcessingGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // selectFileButton
            // 
            this.selectFileButton.Location = new System.Drawing.Point(6, 19);
            this.selectFileButton.Name = "selectFileButton";
            this.selectFileButton.Size = new System.Drawing.Size(97, 23);
            this.selectFileButton.TabIndex = 0;
            this.selectFileButton.Text = "Process file";
            this.selectFileButton.UseVisualStyleBackColor = true;
            this.selectFileButton.Click += new System.EventHandler(this.selectFileButton_Click);
            // 
            // infoLabel
            // 
            this.infoLabel.Location = new System.Drawing.Point(6, 74);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(97, 141);
            this.infoLabel.TabIndex = 1;
            this.infoLabel.Text = "...";
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea1.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Enabled = false;
            legend1.Name = "Legend2";
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend1);
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(211, 17);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Legend = "Legend1";
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(659, 160);
            this.chart1.TabIndex = 2;
            this.chart1.Text = "chart1";
            // 
            // chart2
            // 
            this.chart2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            chartArea2.Name = "ChartArea1";
            this.chart2.ChartAreas.Add(chartArea2);
            this.chart2.Enabled = false;
            legend3.Name = "Legend2";
            legend4.Name = "Legend1";
            this.chart2.Legends.Add(legend3);
            this.chart2.Legends.Add(legend4);
            this.chart2.Location = new System.Drawing.Point(211, 183);
            this.chart2.Name = "chart2";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart2.Series.Add(series2);
            this.chart2.Size = new System.Drawing.Size(659, 160);
            this.chart2.TabIndex = 4;
            this.chart2.Text = "chart2";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 48);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(97, 23);
            this.button1.TabIndex = 5;
            this.button1.Text = "Process folder";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // LearnButton
            // 
            this.LearnButton.Location = new System.Drawing.Point(6, 19);
            this.LearnButton.Name = "LearnButton";
            this.LearnButton.Size = new System.Drawing.Size(97, 23);
            this.LearnButton.TabIndex = 6;
            this.LearnButton.Text = "Build new";
            this.LearnButton.UseVisualStyleBackColor = true;
            this.LearnButton.Click += new System.EventHandler(this.LearnButton_Click);
            // 
            // infoLabel2
            // 
            this.infoLabel2.AutoSize = true;
            this.infoLabel2.Location = new System.Drawing.Point(6, 84);
            this.infoLabel2.Name = "infoLabel2";
            this.infoLabel2.Size = new System.Drawing.Size(16, 13);
            this.infoLabel2.TabIndex = 7;
            this.infoLabel2.Text = "...";
            // 
            // ClassificatorGroupBox
            // 
            this.ClassificatorGroupBox.Controls.Add(this.infoLabel2);
            this.ClassificatorGroupBox.Controls.Add(this.LearnButton);
            this.ClassificatorGroupBox.Location = new System.Drawing.Point(12, 350);
            this.ClassificatorGroupBox.Name = "ClassificatorGroupBox";
            this.ClassificatorGroupBox.Size = new System.Drawing.Size(193, 100);
            this.ClassificatorGroupBox.TabIndex = 8;
            this.ClassificatorGroupBox.TabStop = false;
            this.ClassificatorGroupBox.Text = "Classificator Settings";
            // 
            // ProcessingGroupBox
            // 
            this.ProcessingGroupBox.Controls.Add(this.infoLabel);
            this.ProcessingGroupBox.Controls.Add(this.button1);
            this.ProcessingGroupBox.Controls.Add(this.selectFileButton);
            this.ProcessingGroupBox.Location = new System.Drawing.Point(5, 12);
            this.ProcessingGroupBox.Name = "ProcessingGroupBox";
            this.ProcessingGroupBox.Size = new System.Drawing.Size(200, 332);
            this.ProcessingGroupBox.TabIndex = 9;
            this.ProcessingGroupBox.TabStop = false;
            this.ProcessingGroupBox.Text = "Processing";
            // 
            // ResultsBox
            // 
            this.ResultsBox.Location = new System.Drawing.Point(211, 353);
            this.ResultsBox.Multiline = true;
            this.ResultsBox.Name = "ResultsBox";
            this.ResultsBox.ReadOnly = true;
            this.ResultsBox.Size = new System.Drawing.Size(659, 97);
            this.ResultsBox.TabIndex = 10;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(882, 462);
            this.Controls.Add(this.ResultsBox);
            this.Controls.Add(this.ProcessingGroupBox);
            this.Controls.Add(this.ClassificatorGroupBox);
            this.Controls.Add(this.chart2);
            this.Controls.Add(this.chart1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.chart2)).EndInit();
            this.ClassificatorGroupBox.ResumeLayout(false);
            this.ClassificatorGroupBox.PerformLayout();
            this.ProcessingGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button selectFileButton;
        private System.Windows.Forms.Label infoLabel;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button LearnButton;
        private System.Windows.Forms.Label infoLabel2;
        private System.Windows.Forms.GroupBox ClassificatorGroupBox;
        private System.Windows.Forms.GroupBox ProcessingGroupBox;
        private System.Windows.Forms.TextBox ResultsBox;
    }
}

