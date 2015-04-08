using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmotionalEstimation
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            PraatManager.Start();
            PraatManager.Execute(string.Format("\"execute {0} {1}\"","words.praat","/praat/files"));
            PraatManager.Stop();
        }
    }
}
