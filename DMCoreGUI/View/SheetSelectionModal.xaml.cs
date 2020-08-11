using DMCoreGUI.Model;
using DMCoreGUI.ViewModel;
using DMCoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace DMCoreGUI.View
{
    /// <summary>
    /// Interaction logic for SheetSelectionModal.xaml
    /// </summary>
    public partial class SheetSelectionModal : Window
    {
        SheetSelectionModalViewModel vm = new SheetSelectionModalViewModel();
        internal event EventHandler<SheetsSelectedEventArgs> SheetsSelected;
        protected virtual void OnSheetsSelected(SheetsSelectedEventArgs sheets)
        {
            SheetsSelected?.Invoke(this, sheets);
        }

        public SheetSelectionModal()
        {
            InitializeComponent();
        }
        public SheetSelectionModal(List<DataFileInfo> files)
        {
            InitializeComponent();
            DataContext = vm;
            vm.UpdateFileDetails(files);
            Closing += SheetSelectionModal_Closing;
        }

        private void SheetSelectionModal_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            OnSheetsSelected(new SheetsSelectedEventArgs(vm.SelectedSheets));
        }

        private void ListBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            var selectedSheets = ((ListBox)sender).SelectedItems;
            vm.SelectedSheets = selectedSheets.Cast<ISheetHandler>().ToList();
        }
    }
}
