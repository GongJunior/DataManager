using System;
using System.Collections.Generic;
using System.Text;

namespace DMCoreLibrary.Models
{
    public class SpreadsheetCollectionProgressModel
    {
        public int PercentCompleted { get; set; } = 0;
        public List<string> steps = new List<string>();
        public bool IsCancellable { get; set; } = true;
        public ProcessState State { get; set; } = ProcessState.NotStarted;

        public SpreadsheetCollectionProgressModel() { }
        internal SpreadsheetCollectionProgressModel(ProcessState state)
        {
            State = state;
        }
    }
}
