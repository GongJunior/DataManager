using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using DMCoreGUI.ViewModel;

namespace DMCoreGUI.View
{
    /// <summary>
    /// Interaction logic for Standard.xaml
    /// </summary>
    public partial class Standard : Page
    {
        StandardViewModel vm = new StandardViewModel();


        public Standard()
        {
            InitializeComponent();
            passwords.DataContext = vm;
        }

        private void SlctFiles_Click(object sender, RoutedEventArgs e)
        {
            vm.File_list.Clear();
            vm.ChooseFiles.ShowDialog();
            vm.File_list.AddRange(vm.ChooseFiles.FileNames);

            NumSelected.Text = $"{vm.File_list.Count} Files";
            NumSelected.Foreground = Brushes.Black;

        }

        private void Run_Button_Click(object sender, RoutedEventArgs e)
        {
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
        }

        private void Addpw_Click(object sender, RoutedEventArgs e)
        {
            if (pw.Text != "")
            {
                vm.PW_list.Add(pw.Text);
                pw.Text = null;
            }
        }

        private void Pwrm_Click(object sender, RoutedEventArgs e)
        {
            vm.PW_list.Remove(passwords.SelectedItem.ToString());
        }

        private void RmAllPW_Click(object sender, RoutedEventArgs e)
        {
            vm.PW_list.Clear();
        }


    }
}
