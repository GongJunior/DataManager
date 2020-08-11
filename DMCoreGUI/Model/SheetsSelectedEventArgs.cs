using System;
using System.Collections.Generic;
using System.Windows.Documents;

namespace DMCoreGUI.Model
{
    public class SheetsSelectedEventArgs : EventArgs
    {
        public List<ISheetHandler> SelectedSheets { get; }
        public string Message { get; }

        public SheetsSelectedEventArgs(List<ISheetHandler> sheets )
        {
            SelectedSheets = sheets;
            Message = string.Empty;
        }
        public SheetsSelectedEventArgs(List<ISheetHandler> sheets, string message)
        {
            SelectedSheets = sheets;
            Message = message;
        }
    }
}