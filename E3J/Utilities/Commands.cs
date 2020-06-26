using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using E3J.Models.ValueObjects;
using E3J.Utilities.Extensions;

namespace E3J.Utilities
{
    /// <summary>
    /// Commands class
    /// </summary>
    public class Commands
    {
        /// <summary>
        /// The file path
        /// </summary>
        private string filePath;
        /// <summary>
        /// Gets the commands map.
        /// </summary>
        /// <value>
        /// The commands map.
        /// </value>
        public ISet<Command> CommandsMap { get; }

        /// <summary>
        /// Gets the file path.
        /// </summary>
        /// <value>
        /// The file path.
        /// </value>
        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
                Session.Instance.SubmitCommands(filePath);
            }
        }

        // Utils
        /// <summary>
        /// The document
        /// </summary>
        private readonly XmlDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="Commands"/> class.
        /// </summary>
        /// <param name="path">The path.</param>
        public Commands(string path = MissingFileManager.DEFAULT_COMMANDS_PATH)
        {
            document = new XmlDocument();
            CommandsMap = new HashSet<Command>();
            FilePath = Uri.IsWellFormedUriString(path, UriKind.RelativeOrAbsolute) ? path : MissingFileManager.DEFAULT_COMMANDS_PATH;
            Load(FilePath);
        }

        /// <summary>
        /// Loads the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        public void Load(string path)
        {
            try
            {
                CommandsMap.Clear();
                document.RemoveAll();
                document.Load(path);

                var root = document.SelectSingleNode("/Commands");
                var commandNodes = root.ChildNodes;

                foreach (XmlNode commandNode in commandNodes)
                {
                    var name = commandNode.Attributes["name"].Value;
                    var content = commandNode.Attributes["content"].Value;
                    var regex = new Regex(commandNode.Attributes[2].Value);
                    var type = EnumExtensions
                        .GetValueFromDescription<Command.TypeE>(commandNode.Attributes["type"].Value);
                    var description = commandNode.FirstChild.InnerText;

                    CommandsMap.Add(Command.CreateCommand(name, content, description, regex, type));
                }
                FilePath = path;
            }
            catch
            {
                Console.Error.WriteLine("Could not load command list into memory");
                MissingFileManager.CreateCommandsFile();
                Load(MissingFileManager.DEFAULT_COMMANDS_PATH);
            }
        }

        /// <summary>
        /// Saves the specified path.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="System.InvalidOperationException">Commands were never loaded to memory.</exception>
        public void Save(string path)
        {
            if (null == CommandsMap)
            {
                throw new InvalidOperationException("Commands were never loaded to memory.");
            }
            try
            {
                document.RemoveAll();
                foreach (var command in CommandsMap)
                {
                    document.AppendChild(command.ToXML());
                }
                document.Save(path);
                FilePath = path;
            }
            catch
            {
                Console.Error.WriteLine("Could not save commands.");
            }
        }

        /// <summary>
        /// Adds the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void AddCommand(Command command)
        {
            CommandsMap.Add(command);
        }

        /// <summary>
        /// Removes the command.
        /// </summary>
        /// <param name="command">The command.</param>
        public void RemoveCommand(Command command)
        {
            CommandsMap.Remove(command);
        }
    }
}
