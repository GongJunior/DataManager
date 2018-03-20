using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace DataManager
{
    public struct UserEventArgs
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
}
