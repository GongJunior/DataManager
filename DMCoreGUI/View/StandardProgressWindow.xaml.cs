using DMCoreLibrary.Models;
using System;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace DMCoreGUI.View
{
    /// <summary>
    /// Interaction logic for StandardProgressWindow.xaml
    /// </summary>
    public partial class StandardProgressWindow : Window
    {
        public Progress<SpreadsheetCollectionProgressModel> Progress { get; set; }
        private CancellationTokenSource cancelSource { get; set; }
        private bool IsProcessCancellable { get; set; }
        public StandardProgressWindow(Progress<SpreadsheetCollectionProgressModel> progress, CancellationTokenSource cxl)
        {
            InitializeComponent();
            Progress = progress;
            cancelSource = cxl;
            Progress.ProgressChanged += ReportProgress;
            Show();
        }

        public void Unsubscribe()
        {
            Progress.ProgressChanged -= ReportProgress;
        }

        private void ReportProgress(object sender, SpreadsheetCollectionProgressModel e)
        {

            IsProcessCancellable = e.IsCancellable;
            output.Text = string.Empty;
            foreach (var step in e.steps)
            {
                output.Text += $"{step}\n";
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (IsProcessCancellable)
            {
                cancelSource.Cancel();
            }
            else
            {
                output.Text += "Cannot be cancelled, please wait...\n";
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (IsProcessCancellable)
            {
                cancelSource.Cancel();
                Close();
            }
            else
            {
                MessageBox.Show("Remaing process will be completed in the background");
                Close();
            }
        }
    }
}
