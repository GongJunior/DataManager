using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DMCoreGUI.ViewModel;
using DMCoreLibrary.Models;

namespace DMCoreGUI.View
{
    /// <summary>
    /// Interaction logic for Standard.xaml
    /// </summary>
    public partial class Standard : Page
    {
        private StandardViewModel vm = new StandardViewModel();
        private StandardProgressWindow progressWindow;


        public Standard()
        {
            InitializeComponent();
            DataContext = vm;
        }

        private void SlctFiles_Click(object sender, RoutedEventArgs e)
        {
            vm.Files.Clear();
            vm.ChooseFiles.ShowDialog();
            vm.Files.AddRange(vm.ChooseFiles.FileNames);

            NumSelected.Text = $"{vm.Files.Count} Files";
            NumSelected.Foreground = Brushes.Black;
        }

        private async void Run_Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (vm.InputIsValid)
                {
                    vm.IsButtonEnabled = false;
                    progressWindow?.Close();
                    var cancelSource = new CancellationTokenSource();
                    var progressReporter = new Progress<SpreadsheetCollectionProgressModel>();
                    progressWindow = new StandardProgressWindow(progressReporter, cancelSource);

                    await Task.Run(() => vm.MergeSpreadsheets(progressReporter, cancelSource.Token));

                    vm.IsButtonEnabled = true;
                }
                else
                {
                    _ = MessageBox.Show($"Double check your input!");
                }
            }
            catch (OperationCanceledException)
            {
                progressWindow.ProcessCancelled();
            }
            catch (Exception)
            {
                MessageBox.Show("Something went really wrong...");
            }
        }
        private void Addpw_Click(object sender, RoutedEventArgs e)
        {
            if (pw.Text != "")
            {
                vm.Passwords.Add(pw.Text);
                pw.Text = null;
            }
        }

        private void Pwrm_Click(object sender, RoutedEventArgs e)
        {
            vm.Passwords.Remove(passwords.SelectedItem.ToString());
        }

        private void RmAllPW_Click(object sender, RoutedEventArgs e)
        {
            vm.Passwords.Clear();
        }
    }
}
