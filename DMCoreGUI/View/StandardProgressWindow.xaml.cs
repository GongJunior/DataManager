using DMCoreGUI.ViewModel;
using DMCoreLibrary.Models;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace DMCoreGUI.View
{
    /// <summary>
    /// Interaction logic for StandardProgressWindow.xaml
    /// </summary>
    public partial class StandardProgressWindow : Window
    {
        private StandardProgressWindowViewModel vm;
        public StandardProgressWindow(Progress<SpreadsheetCollectionProgressModel> progress, CancellationTokenSource cxl)
        {
            InitializeComponent();
            vm = new StandardProgressWindowViewModel(progress, cxl);
            DataContext = vm;
            Show();
        }

        public void ProcessCancelled()
        {
            vm.ProgressOutput = "Process sucessfully cancelled";
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            vm.RequestCancellation();
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            vm.RequestCloseWindow();
            Close();
        }

        private void ScrollViewer_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var sv = sender as ScrollViewer;
            var autoscroll = true;
            if (e.ExtentHeightChange == 0)
            {
                if (sv.VerticalOffset == sv.ScrollableHeight)
                {
                    autoscroll = true;
                }
                else
                {
                    autoscroll = false;
                }
            }

            if (autoscroll && e.ExtentHeightChange != 0)
            {
                sv.ScrollToVerticalOffset(sv.ExtentHeight);
            }
        }
    }
}
