using System;
using System.Collections.Generic;
using System.Text;

namespace DMCoreGUI.Model
{
    class BasicSheetHandler : ISheetHandler
    {
        public string SheetName { get; }

        public List<string> FilesFoundIn { get; }

        public BasicSheetHandler(string name, List<string> files)
        {
            SheetName = name;
            FilesFoundIn = files;
        }
    }
}
