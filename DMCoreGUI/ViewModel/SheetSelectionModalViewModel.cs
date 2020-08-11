using DMCoreGUI.Model;
using DMCoreLibrary.Connections;
using DMCoreLibrary.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Documents;

namespace DMCoreGUI.ViewModel
{
    class SheetSelectionModalViewModel : INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public List<ISheetHandler> SelectedSheets { get; set; } = new List<ISheetHandler>();

        private ObservableCollection<ISheetHandler> _sheets;
        public ObservableCollection<ISheetHandler> Sheets
        {
            get => _sheets;

            private set
            {
                _sheets = value;
                OnPropertyChanged();
            }
        }

        //private ISheetHandler _selectedSheet;
        //public ISheetHandler SelectedSheet 
        //{
        //    get => _selectedSheet;
        //    set
        //    {
        //        _selectedSheet = value;
        //        SelectedSheets.Add(value);
        //    } 
        //}

        private bool _isEnabled = false;
        public bool IsEnabled 
        { 
            get => _isEnabled;
            private set
            { 
                _isEnabled = value; 
                OnPropertyChanged();
            } 
        }

        private Visibility _visibility = Visibility.Hidden;
        public Visibility Visibility 
        { 
            get => _visibility;
            private set
            { 
                _visibility = value; 
                OnPropertyChanged();
            }
        }


        public void UpdateFileDetails(List<DataFileInfo> fileDetails)
        {
            Sheets = new ObservableCollection<ISheetHandler>(ConvertToSheetHandler(fileDetails));
        }

        private List<ISheetHandler> ConvertToSheetHandler(List<DataFileInfo> info)
        {
            var sheenames = new List<string>();
            List<ISheetHandler> results = new List<ISheetHandler>();

            foreach (var item in info)
            {
                sheenames.AddRange(item.Sheets);
            }

            foreach (var name in sheenames.Distinct())
            {
                var filesContained = info.Where(p => p.Sheets.Contains(name)).Select(p => p.SourceFile.Name).ToList();
                results.Add(new BasicSheetHandler(name, filesContained));
            }

            return results;
        }
    }
}
