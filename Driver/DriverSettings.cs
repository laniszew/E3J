using System;
using System.IO;
using System.IO.Ports;
using Newtonsoft.Json;

namespace Driver
{
    public class DriverSettings
    {
        #region Constants
        private const int DEFAULT_DATA_BITS = 8;
        private const int DEFAULT_BAUDRATE = 9600;
        private const Parity DEFAULT_PARITY = Parity.Even;
        private const StopBits DEFAULT_STOP_BITS = StopBits.Two;
        private const string DEFAULT_HANDSHAKE = "RTS/CTS";
        private const bool DEFAULT_RTS_ENABLE = true;
        private const int DEFAULT_READ_TIMEOUT = 5000;
        private const int DEFAULT_WRITE_TIMEOUT = 2000;

        private const string DEFAULT_SERIAL_SETTINGS_PATH = "SerialSettings.json";
        #endregion

        public int BaudRate { get; set; }
        public int DataBits { get; set; }
        public Parity Parity { get; set; }
        public StopBits StopBits { get; set; }
        public string Handshake { get; set; }
        public bool RtsEnable { get; set; }
        public int ReadTimeout { get; set; }
        public int WriteTimeout { get; set; }

        public string Path { get; protected set; }


        protected DriverSettings()
        {
        }

        public static DriverSettings CreateDefaultSettings()
        {
            var settings = new DriverSettings();
            settings.RestoreDefaults();
            settings.Path = string.Empty;
            return settings;
        }

        public static DriverSettings CreateFromSettingFile(string filePath = DEFAULT_SERIAL_SETTINGS_PATH)
        {
            var settings = new DriverSettings();
            try
            {
                var json = File.ReadAllText(filePath);
                dynamic deserialized = JsonConvert.DeserializeObject(json);
                settings.BaudRate = deserialized.BaudRate;
                settings.DataBits = deserialized.DataBits;
                settings.Parity = deserialized.Parity;
                settings.RtsEnable = deserialized.RtsEnable;
                settings.ReadTimeout = deserialized.ReadTimeout;
                settings.WriteTimeout = deserialized.WriteTimeout;
                settings.Path = filePath;
            }
            catch (FileNotFoundException)
            {
                Console.Error.WriteLine("Specified file does not exist. Restoring defaults.");
                settings.RestoreDefaults();
            }
            catch
            {
                Console.Error.WriteLine("Could not load settings from specified file. File may be corrupted. Restoring defaults.");
                settings.RestoreDefaults();
            }
            return settings;
        }

        public static SerialBuilder CreateCustom()
        {
            return new SerialBuilder();
        }

        public void SaveToFile(string path = DEFAULT_SERIAL_SETTINGS_PATH)
        {
            try
            {
                var json = JsonConvert.SerializeObject(this);
                File.WriteAllText(path, json);
            }
            catch(Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
            }
        }

        private void RestoreDefaults()
        {
            BaudRate = DEFAULT_BAUDRATE;
            DataBits = DEFAULT_DATA_BITS;
            StopBits = DEFAULT_STOP_BITS;
            Handshake = DEFAULT_HANDSHAKE;
            Parity = DEFAULT_PARITY;
            RtsEnable = DEFAULT_RTS_ENABLE;
            ReadTimeout = DEFAULT_READ_TIMEOUT;
            WriteTimeout = DEFAULT_WRITE_TIMEOUT;
        }
    }
}    


