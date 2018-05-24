using Microsoft.Win32;
using System.Collections.Generic;
using System.Windows;
using System;
using System.ComponentModel;
using System.Windows.Media;

namespace DataManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        List<string> files = new List<string>();
        ProgressBarTask alert;
        BackgroundWorker worker = new BackgroundWorker
        {
            WorkerReportsProgress = true,
            WorkerSupportsCancellation = true

        };


        public MainWindow()
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
                Filter = "Crosstab|*.xlsx;*.xls;*.csv|Excel Files|*.xlsx;*.xls|CSV files (*.csv)|*.csv",
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
            if(e.Cancelled == true)
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
            this.Close();
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
            manager.ProgressChanged += (s, pe) => worker.ReportProgress(pe.part,pe.statusMessage);
            manager.MergeDTfromFiles();
        }

        
        
    }
}
