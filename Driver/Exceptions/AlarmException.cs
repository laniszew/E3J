using System;

namespace Driver.Exceptions
{
    public class AlarmException : Exception
    {
        public AlarmException(int errorCode) : base($"Action interrupted with error code: {errorCode}")
        {
            
        }
    }
}
