using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Collections.ObjectModel;
using Microsoft.Win32;
using System.Threading.Tasks;
using DMCoreLibrary.Connections;
using System.Linq;
using DMCoreLibrary.Models;
using System;
using System.Threading;
using DMCoreGUI.View;
using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace DMCoreGUI.ViewModel
{
    public class StandardViewModel : INotifyPropertyChanged
    {
        public string StartRow { get; set; }

        private string sheetName;
        public string SheetName 
        { 
            get => sheetName;
            set
            {
                sheetName = value;
                OnPropertyChanged();
            }
        }
        public List<string> Files { get; set; } = new List<string>();
        private ObservableCollection<string> _passwords = new ObservableCollection<string>();
        private bool _isButtonEnabled = true;

        private SpreadsheetCollectionOptions Options
        {
            get
            {
                return new SpreadsheetCollectionOptions()
                {
                    Files = Files,
                    StartRow = StartRow,
                    Sheets = SheetName,
                    Passwords = Passwords.ToList()
                };
            }
        }
        public bool InputIsValid
        {
            get => ValidateInput();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        public ObservableCollection<string> Passwords
        {
            get => _passwords;
            set
            {
                _passwords = value;
                OnPropertyChanged();
            }
        }
        public bool IsButtonEnabled
        {
            get => _isButtonEnabled;
            set
            {
                _isButtonEnabled = value;
                OnPropertyChanged();
            }
        }
        public OpenFileDialog ChooseFiles { get; } = new OpenFileDialog
        {
            Multiselect = true,
            Filter = "Excel Files|*.xlsx;*.xls|Crosstab|*.xlsx;*.xls;*.csv|CSV files (*.csv)|*.csv",
        };

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void MergeSpreadsheets(IProgress<SpreadsheetCollectionProgressModel> progress, CancellationToken cancelToken)
        {
            var spreadsheetCollection = new SpreadsheetCollection(Options);
            try
            {
                spreadsheetCollection.MergeDTfromFiles(progress, cancelToken);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
        }
        public void MergeSpreadsheetsTest(IProgress<SpreadsheetCollectionProgressModel> progress, CancellationToken cancelToken)
        {
            SpreadsheetCollectionProgressModel report = new SpreadsheetCollectionProgressModel();

            foreach (var file in Files)
            {
                Task.Delay(5000).Wait();
                report.steps.Add($"Loading: {file}\tRow: {StartRow}\tSheet:{SheetName}...");
                progress.Report(report);
                cancelToken.ThrowIfCancellationRequested();
            }
        }
        public async Task SelectSheetsAsync()
        {
            IsButtonEnabled = false;
            var spreadsheetCollection = new SpreadsheetCollection(new SpreadsheetCollectionOptions() { Files = Files });
            var filesTask = Task.Run(() => spreadsheetCollection.GetFileDetails());
            var fileDetails = await filesTask;
            var sheetSelector = new SheetSelectionModal(fileDetails);
            sheetSelector.SheetsSelected += SheetSelector_SheetsSelected;
            _ = sheetSelector.ShowDialog();
            IsButtonEnabled = true;
            sheetSelector.SheetsSelected -= SheetSelector_SheetsSelected;
        }

        private void SheetSelector_SheetsSelected(object sender, Model.SheetsSelectedEventArgs e)
        {
            if (e.SelectedSheets.Count > 0)
            {
                var names = e.SelectedSheets.Select(p => p.SheetName).ToList();
                SheetName = string.Join(",", names);
            }
        }

        private bool ValidateInput()
        {
            return IsSheetNameValid() & IsStartRowValid() & AreFilesSelected();
        }
        private bool IsStartRowValid()
        {
            if (string.IsNullOrEmpty(StartRow) || !Regex.IsMatch(StartRow, @"^\d+$"))
            {
                return false;
            }
            return true;
        }
        private bool IsSheetNameValid()
        {
            if (string.IsNullOrEmpty(SheetName))
            {
                return false;
            }
            return true;
        }
        private bool AreFilesSelected()
        {
            if (Files.Count < 1)
            {
                return false;
            }
            return true;
        }
    }
}
