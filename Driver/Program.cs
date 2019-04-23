using System;
using System.Collections.Generic;
using System.Linq;

namespace Driver
{
    public class Program
    {
        #region Properties

        public string Name { get; }
        public string Path { get; set; }
        public string Content { get; set; }
        public string Timestamp { get; private set; }
        public int Size { get; private set; }

        #endregion

        public Program(string name)
        {
            Name = name;
            Content = string.Empty;
        }

        public static Program CreateFromRemoteProgram(RemoteProgram program, string content)
        {
            return new Program(program.Name)
            {
                Size = program.Size,
                Timestamp = program.Timestamp,
                Content = content
            };
        }

        public List<string> GetLines()
        {
            var lines = Content.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            return lines;
        }

        /// <summary>
        /// Provides a shallow copy of program
        /// </summary>
        /// <returns>Shallow copy of program</returns>
        public Program Clone()
        {
            return (Program)MemberwiseClone();
        }
    }
}
