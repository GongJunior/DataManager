using System;

namespace DMCoreLibrary.Events
{
    public class CancelEventArgs : EventArgs
    {
        public bool Cancel { get; set; }
    }
}
