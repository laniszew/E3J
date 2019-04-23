using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace Driver
{
   public class SerialCOM
    {
        private readonly SerialPort port;
        private readonly Queue<string> bufferedMessages;
        private readonly Thread heartBeatThread;

        #region Enums and data structures
        public enum Terminator
        {
            NONE,
            CR,     // Carriage return
            LF,     // Line feed
            CRLF    // Both
        };
        private const Terminator DEFAULT_FRAME_TERMINATOR = SerialCOM.Terminator.CR;
        #endregion

        #region Events

        public delegate void ReceiveData(string data);
        public delegate void StatusChangedDelegate(object sender, ConnectionStatusChangedArgs e);

        public event ReceiveData DataReceived;
        public event StatusChangedDelegate ConnectionStatusChanged;

        #endregion

        #region Properties
        public int BaudRate
        {
            get { return port.BaudRate; }
            set { port.BaudRate = value; }
        }

        public int DataBits
        {
            get { return port.DataBits; }
            set { port.DataBits = value; }
        }

        public Parity Parity
        {
            get { return port.Parity; }
            set { port.Parity = value; }
        }

        public StopBits StopBits
        {
            get { return port.StopBits; }
            set { port.StopBits = value; }
        }

        public bool RtsEnable
        {
            get { return port.RtsEnable; }
            set { port.RtsEnable = value; }
        }

        public Terminator FrameTerminator { get; set; }

        public bool Opened => port.IsOpen;

        #endregion

        public SerialCOM(DriverSettings settings)
        {
            port = new SerialPort();
            bufferedMessages = new Queue<string>();
            BaudRate = settings.BaudRate;
            DataBits = settings.DataBits;
            Parity = settings.Parity;
            StopBits = settings.StopBits;
            RtsEnable = settings.RtsEnable;
            port.WriteTimeout = settings.WriteTimeout;
            port.ReadTimeout = settings.ReadTimeout;

            FrameTerminator = DEFAULT_FRAME_TERMINATOR;
            port.DataReceived += Port_DataReceived;

            heartBeatThread = new Thread(HeartBeat) {IsBackground = true};
            heartBeatThread.Start();
        }

        private void Port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            var read = "";
            while(!read.Contains("\r"))
            {
                read += port.ReadExisting();
            }
            bufferedMessages.Enqueue(read);
            DataReceived?.Invoke(read);
        }

        public async void OpenPort(string portName)
        {
            try
            {
                if (port.IsOpen) return;
                port.PortName = portName;
                port.Open();
                // delay for initialization purposes
                await Task.Delay(1000);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        public void ClosePort()
        {
            if (!port.IsOpen) return;
            port.Close();
        }

        public void Write(string data)
        {
            try
            {
                port.Write(data + GetTerminator());
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        public string Read()
        {
            return bufferedMessages.Count != 0 ? bufferedMessages.Dequeue() : string.Empty;
        }

        public async Task WaitForMessageAsync()
        {
            var counter = 0;
            await Task.Run(async () =>
            {
                while (bufferedMessages.Count == 0 || counter == 4000)
                {
                    counter++;
                    await Task.Delay(1);
                }
            });

        }

        private string GetTerminator()
        {
            var terminator = "";
            switch (FrameTerminator)
            {
                case Terminator.CR:
                    terminator = "\r";
                    break;

                case Terminator.LF:
                    terminator = "\n";
                    break;

                case Terminator.CRLF:
                    terminator = "\r\n";
                    break;
            }
            return terminator;
        }

        private void HeartBeat()
        {
            var oldStatus = Opened;
            
            while (true)
            {
                Thread.Sleep(500);
                var newStatus = Opened;

                if (oldStatus != newStatus)
                {
                    ConnectionStatusChanged?.Invoke(this, new ConnectionStatusChangedArgs(oldStatus, newStatus));
                }
                oldStatus = newStatus;
            }
        }
    }
}
