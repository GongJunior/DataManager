using System;
using System.Collections.Generic;
using System.Text;

namespace DMCoreGUI.Model
{
    public interface ISheetHandler
    {
        public string SheetName { get; }
        public List<string> FilesFoundIn { get; }
    }
}
