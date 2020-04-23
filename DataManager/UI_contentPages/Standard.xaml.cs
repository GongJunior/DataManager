﻿using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.Win32;
using System.ComponentModel;
using System.Collections.ObjectModel;
using DataManager.ViewModels;

namespace DataManager.UI_contentPages
{
    /// <summary>
    /// Interaction logic for Standard.xaml
    /// </summary>
    public partial class Standard : Page
    {
        StandardViewModel vm = new StandardViewModel();
        ProgressBarTask alert;
        private BackgroundWorker worker;
        


        public Standard()
        {
            InitializeComponent();
            this.Title = "Data Manager";
            passwords.DataContext = vm;
        }

        private void SlctFiles_Click(object sender, RoutedEventArgs e)
        {
            vm.File_list.Clear();
            OpenFileDialog choosFiles = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "Excel Files|*.xlsx;*.xls|Crosstab|*.xlsx;*.xls;*.csv|CSV files (*.csv)|*.csv",
            };
            choosFiles.ShowDialog();
            vm.File_list.AddRange(choosFiles.FileNames);
            // updates UI to show files selected
            NumSelected.Text = $"{vm.File_list.Count} Files";
            NumSelected.Foreground = Brushes.Black;
        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                vm.Startrow_text = StartRow.Text;
                vm.Sheetname_text = SheetName.Text;
                worker = new BackgroundWorker
                {
                    WorkerReportsProgress = true,
                    WorkerSupportsCancellation = true

                };
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

                    worker.RunWorkerAsync(vm);
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
            }
            else
            {
                alert.Close();
                MessageBox.Show("Process Complete! Click OK to close...");
            }
            //add logic here clear any memory and/or close application
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {

            StandardViewModel unpackThem = (StandardViewModel)e.Argument;
            SurveyUtilitiesManager manager = new SurveyUtilitiesManager()
            {
                FilesList = unpackThem.File_list,
                Sheetnames = unpackThem.Sheetname_text,
                Startrow = unpackThem.Startrow_text
            };

            // Add feedback to show program is running
            manager.CheckCancel += (sender1, e1) => e1.Cancel = worker.CancellationPending;
            manager.ProgressChanged += (s, pe) => worker.ReportProgress(pe.part, pe.statusMessage);
            manager.MergeDTfromFiles();
        }

        private void addpw_Click(object sender, RoutedEventArgs e)
        {
            vm.PW_list.Add(pw.Text);
            pw.Text = null;
        }
    }
}
