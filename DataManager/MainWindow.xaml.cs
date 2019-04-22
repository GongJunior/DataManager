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
            ChangePageIfNew("UI_contentPages/Settings.xaml", Settings.ToolTip.ToString());
        }

        private void Misc_Click(object sender, RoutedEventArgs e)
        {
            ChangePageIfNew("UI_contentPages/MiscFunctions.xaml", MiscPage.ToolTip.ToString());
        }

        private void Instructions_Click(object sender, RoutedEventArgs e)
        {
            ChangePageIfNew("UI_contentPages/Instructions.xaml", InstructionsPage.ToolTip.ToString());
        }

        private void StandardPage_Click(object sender, RoutedEventArgs e)
        {
            ChangePageIfNew("UI_contentPages/Standard.xaml", StandardPage.ToolTip.ToString());
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
