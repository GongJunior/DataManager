using DMCoreLibrary.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Windows;

namespace DMCoreGUI.ViewModel
{
    class StandardProgressWindowViewModel : INotifyPropertyChanged
    {
        public Progress<SpreadsheetCollectionProgressModel> CurrentProgress { get; set; }
        public CancellationTokenSource CancelSource { get; set; }
        private bool IsProcessCancellable { get; set; }
        private ProcessState State { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private string _progressOutput;
        public string ProgressOutput
        {
            get 
            { 
                return _progressOutput; 
            }
            set 
            {
                if (_progressOutput == null)
                {
                    _progressOutput = value;
                }
                else
                {
                    _progressOutput += $"\n{value}";
                }
                OnPropertyChanged();
            }
        }

        public StandardProgressWindowViewModel(Progress<SpreadsheetCollectionProgressModel> progress, CancellationTokenSource cxl)
        {
            CurrentProgress = progress;
            CancelSource = cxl;
            CurrentProgress.ProgressChanged += ReportProgress;
        }

        private void ReportProgress(object sender, SpreadsheetCollectionProgressModel e)
        {
            IsProcessCancellable = e.IsCancellable;
            State = e.State;

            if (e.steps.Count > 0)
            {
                var newStep = e.steps[^1];
                ProgressOutput = newStep;
            }

            if (e.State == ProcessState.Completed)
            {
                CurrentProgress.ProgressChanged -= ReportProgress;
            }
        }

        public void RequestCancellation()
        {
            if (IsProcessCancellable && State == ProcessState.IsRunning)
            {
                CancelSource.Cancel();
            }
            else
            {
                ProgressOutput = "Cannot be cancelled, please wait until process is complete";
            }

        }

        public void RequestCloseWindow()
        {
            if (State == ProcessState.Completed)
            {
                return;
            }
            else if (IsProcessCancellable)
            {
                CancelSource.Cancel();
                ProgressOutput = "Cancelling process";
            }
            else
            {
                MessageBox.Show("Remaing process will be completed in the background");
            }
        }
    }
}
