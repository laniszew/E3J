using System;
using System.Collections.ObjectModel;
using System.IO.Ports;
using System.Linq;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using Driver;
using E3J.Messages;
using E3J.Models;
using E3J.Models.ValueObjects;
using E3J.MO;


namespace E3J.ViewModels
{
    

    public class ConnectionSettingsViewModel : ViewModelBase
    {
        private readonly ProgramService _programService;

        private DialogHost dialogHost;
        public DriverSettings Settings { get; set; }
        public bool ConnectionToggleIsChecked { get; set; }
        public string SelectedCOMPort { get; set; }
        public ObservableCollection<string> AvailableCOMPorts { get; set; }
        public E3JManipulator Manipulator { get; set; }

        public ConnectionSettingsViewModel()
        {
            Settings = DriverSettings.CreateDefaultSettings();
            Manipulator = new E3JManipulator(DriverSettings.CreateDefaultSettings());
            MessageList = new MessageList();
        }

        private void Port_ConnectionStatusChanged(object sender, ConnectionStatusChangedArgs e)
        {
            if (e.OldStatus == true && e.NewStatus == false)
            {
                Manipulator.Port.DataReceived -= Port_DataReceived;
                ConnectionToggleIsChecked = false;
                SelectedCOMPort = null;
            }
        }

        private void Port_DataReceived(string data)
        {
            data = data.Replace("\r", string.Empty);
            MessageList.AddMessage(new Message(DateTime.Now, data, Message.Type.Received));

            var receivedMessages = MessageList.Messages.Where(i => i.MyType == Message.Type.Received).ToList();
           // CommandHistoryText += receivedMessages[receivedMessages.Count - 1].DisplayMessage();
           // commandHistory.Dispatcher.Invoke(() => commandHistory.ScrollToEnd());
        }

        public MessageList MessageList { get; }

        [Command]
        public void Connect(object obj)
        {
            if (null != obj)
            {
                var state = (bool)obj;
                if (!state)
                {
                    Manipulator?.Disconnect();
                }
                else
                {
                    if (string.IsNullOrEmpty(SelectedCOMPort))
                    {
                        ConnectionToggleIsChecked = false;
                        return;
                    }

                    ConnectionToggleIsChecked = true;
                    Manipulator = new E3JManipulator(Settings);
                    Manipulator.Port.ConnectionStatusChanged += Port_ConnectionStatusChanged;
                    Manipulator.Connect(SelectedCOMPort);
                    Manipulator.Port.DataReceived += Port_DataReceived;
                    Messenger.Default.Send(new NewManipulatorConnected(Manipulator));
                }
            }
        }

        [Command]
        public void RefreshCOMPorts()
        {
            AvailableCOMPorts = new ObservableCollection<string>(SerialPort.GetPortNames());
        }
    }
}
