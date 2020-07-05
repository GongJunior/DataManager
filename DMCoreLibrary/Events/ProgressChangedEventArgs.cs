using System;

namespace DMCoreLibrary.Events
{
    public class ProgressChangedEventArgs : EventArgs
    {
        public string statusMessage;
        public int part;
        public ProgressChangedEventArgs(int part, string statusMessage)
        {
            this.statusMessage = statusMessage;
            this.part = part;
        }
    }
}
