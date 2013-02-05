using System;
using System.ComponentModel;
using System.Windows;

namespace City
{
    public partial class ProgressWindow
    {
        public BackgroundWorker Worker { get; set; }
        public DoWorkEventArgs Args { get; set; }

        public int Current { get; set; }

        public string ProgressPercentDisplay
        {
            set { progressLabel.Content = value; }
        }

        public int ProgressBarValue
        {
            set { progressBar.Value = value; }
        }

        public string ProgressDesc
        {
            set { progressDescLabel.Content = value; }
        }

        public ProgressWindow()
        {
            InitializeComponent();
            Current = 0;
            Cancel += delegate { Worker.CancelAsync(); };
        }

        private void CancelBtnClick(object sender, RoutedEventArgs e)
        {
            Cancel(sender, e);
        }

        public void InvokeUpdate(int total, string desc)
        {
            decimal totalDecimal = (decimal)total != 0 ? total : Current != 0 ? Current : 1;
            var percent = Convert.ToInt32((Current / totalDecimal) * 100);
            Dispatcher.BeginInvoke((UpdateProgressDelegate) UpdateProgressText, percent, total, desc);
        }

        public delegate void UpdateProgressDelegate(int percent, int total, string desc);

        public void UpdateProgressText(int percent, int total, string desc)
        {
            ProgressPercentDisplay = string.Format("{0}% ({1}/{2} total elements)", percent, Current, total);
            ProgressBarValue = percent;
            ProgressDesc = desc;
        }

        public event EventHandler Cancel = delegate { };
    }
}