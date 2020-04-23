using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace DataManager.ViewModels
{
    class StandardViewModel : INotifyPropertyChanged
    {
        private string startrow_text;
        private string sheetname_text;
        private List<string> file_list = new List<string>();
        private ObservableCollection<string> pw_list = new ObservableCollection<string>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string Startrow_text
        {
            get => startrow_text;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    if (Regex.IsMatch(value, @"^\d+$"))
                    {
                        startrow_text = value;
                    }
                    else throw new System.ArgumentException("Start row needs to be numeric");
                }
                else throw new System.ArgumentException("Start row number required!");

            }
        }
        public string Sheetname_text
        {
            get => sheetname_text;
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    sheetname_text = value;
                }
                else throw new System.ArgumentException("Sheet required!");

            }
        }
        public List<string> File_list
        {
            get => file_list;
            set
            {
                if (value.Count > 0)
                {
                    file_list = value;
                }
                else throw new System.ArgumentException("Must select multiple files!");
            }

        }
        public ObservableCollection<string> PW_list
        {
            get => pw_list;
            set
            {
                pw_list = value;
                OnPropertyChanged();
            }
        }
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
