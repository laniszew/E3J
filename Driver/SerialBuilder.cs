using System.IO.Ports;

namespace Driver
{
    public class SerialBuilder
    {
        private readonly DriverSettings settings;

        public SerialBuilder()
        {
            settings = DriverSettings.CreateDefaultSettings();
        }

        public DriverSettings Build()
        {
            return settings;
        }

        public DriverSettings BuildAndSave(string filePath)
        {
            if(string.IsNullOrWhiteSpace(filePath))
                settings.SaveToFile();
            else
                settings.SaveToFile(filePath);

            return Build();
        }

        public SerialBuilder BaudRate(int baudrate)
        {
            settings.BaudRate = baudrate;
            return this;
        }

        public SerialBuilder Parity(Parity parity)
        {
            settings.Parity = parity;
            return this;
        }

        public SerialBuilder StopBits(StopBits stopBits)
        {
            settings.StopBits = stopBits;
            return this;
        }

        public SerialBuilder RtsEnable(bool state)
        {
            settings.RtsEnable = state;
            return this;
        }

        public SerialBuilder DataBits(int databits)
        {
            settings.DataBits = databits;
            return this;
        }

        public SerialBuilder ReadTimeout(int readtimeout)
        {
            settings.ReadTimeout = readtimeout;
            return this;
        }

        public SerialBuilder WriteTimeout(int writetimeout)
        {
            settings.WriteTimeout = writetimeout;
            return this;
        }
    }
}
