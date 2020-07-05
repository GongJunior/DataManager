using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace DMCoreLibrary.Connections
{
    class SpreadsheetCollectionOptions
    {
        public List<string> Files { get; set; } = new List<string>();
        public string StartRow { get; set; } = "1";
        public string Sheets { get; set; } = "allsheets";

        public List<string>? Passwords { get; set; }

    }
}
