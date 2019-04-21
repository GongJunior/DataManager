using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.ComponentModel;

namespace DataManager.UI_contentPages
{
    /// <summary>
    /// Interaction logic for Standard.xaml
    /// </summary>
    public partial class Standard : Page
    {
        List<string> files = new List<string>();
        ProgressBarTask alert;
        BackgroundWorker worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true

        };


        public Standard()
        {
            InitializeComponent();
            this.Title = "Data Manager";
        }

        private void SlctFiles_Click(object sender, RoutedEventArgs e)
        {
            files.Clear();
            OpenFileDialog choosFiles = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Excel Files|*.xlsx;*.xls|Crosstab|*.xlsx;*.xls;*.csv|CSV files (*.csv)|*.csv",
            };
            choosFiles.ShowDialog();
            files.AddRange(choosFiles.FileNames);
            // updates UI to show files selected
            NumSelected.Text = $"{files.Count.ToString()} Files";
            NumSelected.Foreground = Brushes.Black;
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                UserEventArgs holder = new UserEventArgs(files, StartRow.Text, SheetName.Text);
                worker.DoWork += Worker_DoWork;
                worker.ProgressChanged += Worker_ProgressChanged;
                worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

                // opens progress bar/cancel ui
                if (worker.IsBusy != true)
                {
                    alert = new ProgressBarTask();
                    // Event handler for cancelbutton
                    alert.Canceled += new EventHandler<EventArgs>(CancelButton_Click);
                    alert.Show();

                    worker.RunWorkerAsync(holder);
                }
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            if (worker.WorkerSupportsCancellation == true)
            {

                worker.CancelAsync();
                alert.Close();
            }

        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            alert.workStatus.Text = (string)e.UserState;
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                MessageBox.Show("Canceled by user, app closing");
            }
            if (e.Error != null)
            {
                alert.Close();
                MessageBox.Show("Error" + e.Error.Message);
                return;
            }
            else
            {
                alert.Close();
                MessageBox.Show("Process Complete! Click OK to close...");
            }
            //add logic here clear any memory and/or close
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            UserEventArgs unpackThem = (UserEventArgs)e.Argument;
            SurveyUtilitiesManager manager = new SurveyUtilitiesManager()
            {
                FilesList = unpackThem.file_List,
                Sheetnames = unpackThem.sheetname_Text,
                Startrow = unpackThem.startrow_Text
            };

            // Add feedback to show program is running
            manager.CheckCancel += (sender1, e1) => e1.Cancel = worker.CancellationPending;
            manager.ProgressChanged += (s, pe) => worker.ReportProgress(pe.part, pe.statusMessage);
            manager.MergeDTfromFiles();
        }
    }
}
