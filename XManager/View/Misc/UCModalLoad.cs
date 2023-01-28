using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace XManager
{
    public partial class UCModalLoad : UserControl
    {
        internal int TotalFile;
        internal UCModalLoad()
        {
            ProgressBar ProgressBarPercentage = new ProgressBar();
            ProgressBarPercentage.Visible = true;
            ProgressBarPercentage.Minimum = 1;
            ProgressBarPercentage.Maximum = TotalFile;
            ProgressBarPercentage.Value = 1;
            ProgressBarPercentage.Step = 1;
            InitializeComponent();
        }

        internal UCModalLoad(int totalFile)
        {
            TotalFile = totalFile;
            ProgressBarPercentage.Visible = true;
            ProgressBarPercentage.Minimum = 1;
            ProgressBarPercentage.Maximum = TotalFile;
            ProgressBarPercentage.Value = 1;
            ProgressBarPercentage.Step = 1;
            InitializeComponent();
        }
        internal void ProgressIncrement(bool bSuccess)
        {
            if (bSuccess)
            {
                ProgressBarPercentage.PerformStep();
            }
        }

        internal void RunAsync(Action action)
        {
            Task.Run(action);
        }
    }
}
