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
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ThirdPage_Click(object sender, RoutedEventArgs e)
        {
            //ChangePageIfNew("ThirdPage.xaml", "Third Page");
            //Page note implemented yet
        }

        private void SecondPage_Click(object sender, RoutedEventArgs e)
        {
            //ChangePageIfNew("SecondPage.xaml", "Second Page");
            //Page note implemented yet
        }

        private void StandardPage_Click(object sender, RoutedEventArgs e)
        {
            ChangePageIfNew("UI_contentPages/Standard.xaml", "Standard Page");
        }

        private void ChangePageIfNew(string pageRequest, string newTitle)
        {

            if (!ContentIsland.Source.ToString().EndsWith(pageRequest))
            {
                PageTitle.Content = newTitle;
                ContentIsland.Navigate(new Uri(pageRequest, UriKind.Relative));
            }
        }
    }
}
