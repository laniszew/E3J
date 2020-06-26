using System.Collections.ObjectModel;
using E3J.Models.ValueObjects;

namespace E3J.Models

{
    /// <summary>
    /// MessageList class
    /// </summary>
    public class MessageList
    {

        public MessageList()
        {
            Messages = new ObservableCollection<Message>();
        }

        public ObservableCollection<Message> Messages { get; }

        public void AddMessage(Message message)
        {
            Messages.Add(message);
        }
        public void RemoveMessage(Message message)
        {
            Messages.Remove(message);
        }
    }
}
