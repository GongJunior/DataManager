using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataManager
{
    class UserEventArgs
    {
        // setup to handle args to do work
        public string startrow_Text, sheetname_Text;
        public List<string> file_List;

        public UserEventArgs(List<string> l = null, string sr = "", string sn = "")
        {
            if (l.Count > 0)
            {
                file_List = l;
            }
            else throw new System.ArgumentException("Must select multiple files!");

            if (!string.IsNullOrEmpty(sr))
            {
                if(Regex.IsMatch(sr, @"^\d+$"))
                {
                    startrow_Text = sr;
                }
                else throw new System.ArgumentException("Start row needs to be numeric");
            }
            else throw new System.ArgumentException("Start row number required!");

            if (!string.IsNullOrEmpty(sn))
            {
                sheetname_Text = sn;
            }
            else throw new System.ArgumentException("Sheet required!");
        }
    }

    class GuiReader
    {
        #region PROPERTIES
        private string startrow_text;
        private string sheetname_text;
        private List<string> file_list;
        private List<string> pw_list;

        public string Startrow_text
        {
            get => startrow_text;
            set => startrow_text = value;
        }
        public string Sheetname_text
        {
            get => sheetname_text;
            set => sheetname_text = value;
        }
        public List<string> File_list
        {
            get => file_list;
            set => file_list = value;
        }
        public List<string> PW_list
        {
            get => pw_list;
            set => pw_list = value;
        }

        public void ValidateInput()
        {

        }

        #endregion
    }
}
