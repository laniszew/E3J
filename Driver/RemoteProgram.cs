using System;
using System.Globalization;

namespace Driver
{
    public sealed class RemoteProgram
    {
        public string Name { get; }
        public int Size { get; }
        public string Timestamp { get; }

        public RemoteProgram(string name, int size, string timestamp)
        {
            Name = name;
            Size = size;
            Timestamp = timestamp;
        }

        public static RemoteProgram Create(string info)
        {
            var splittedInfo = info.Split(';');
            //if (splittedInfo.Length != 6)
            //    throw new ArgumentException();
            if (!splittedInfo[0].EndsWith("RE2"))
                return null;
            
            int startIndex = splittedInfo[0].IndexOf("QoK") + "QoK".Length;
            int endIndex = splittedInfo[0].IndexOf(".RE2", startIndex);
            var name = splittedInfo[0].Substring(startIndex, endIndex - startIndex);

            var size = Convert.ToInt32(splittedInfo[1]);
            if (size == 0)
                return null;

            //var timestamp = splittedInfo[2];
            var date = splittedInfo[2].Substring(0, 8);
            var time = splittedInfo[2].Substring(8, splittedInfo.Length);
            var splittedData = date.Split('-');
            var temp = splittedData[0];
            splittedData[0] = splittedData[2];
            splittedData[2] = "20" + temp;
            splittedData[1] = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(int.Parse(splittedData[1]));

            var timestamp = $"{string.Join(" ", splittedData)} {date.Replace('-', ':')}";

            return new RemoteProgram(name, size, timestamp);
        }
    }
}
