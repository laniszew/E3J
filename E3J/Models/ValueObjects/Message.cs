using System;

namespace E3J.Models.ValueObjects
{
    public class Message
    {
        public enum Type
        {
            Send,
            Received
        }

        public Message(DateTime time, string message, Type myType)
        {
            MyTime = time;
            MyMessage = message;
            MyType = myType;
        }

        public DateTime MyTime { get; }

        public string MyMessage { get; }

        public Type MyType { get; }


        public string DisplayMessage()
        {
            if (MyMessage != null)
                return $"{MyTime:dd-MM-yyyy HH:mm:ss}" + ": " + MyMessage + Environment.NewLine;
            return null;
        }
    }
}
