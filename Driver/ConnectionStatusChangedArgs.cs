using System;

namespace Driver
{
    public class ConnectionStatusChangedArgs : EventArgs
    {
        public bool OldStatus { get; }
        public bool NewStatus { get; }

        public ConnectionStatusChangedArgs(bool oldStatus, bool newStatus)
        {
            OldStatus = oldStatus;
            NewStatus = newStatus;
        }
    }
}
