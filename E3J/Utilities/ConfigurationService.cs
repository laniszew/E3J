using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;
using E3J.Models.ValueObjects;
using E3J.Utilities.Extensions;

namespace E3J.Utilities
{
    // TODO: Useless class - dispose
    /// <summary>
    /// ConfigurationService class
    /// </summary>
    public class ConfigurationService
    {
        /// <summary>
        /// The default commands path
        /// </summary>
        private const string DEFAULT_COMMANDS_PATH = "Commands.xml";

        /// <summary>
        /// The instance
        /// </summary>
        private static readonly Lazy<ConfigurationService> instance = new Lazy<ConfigurationService>(() => new ConfigurationService());
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static ConfigurationService Instance => instance.Value;

        // Configuration related objects
        /// <summary>
        /// The commands
        /// </summary>
        private ISet<Command> commands;

        // Utils
        /// <summary>
        /// The document
        /// </summary>
        private readonly XmlDocument document;

        /// <summary>
        /// Prevents a default instance of the <see cref="ConfigurationService"/> class from being created.
        /// </summary>
        private ConfigurationService()
        {
            document = new XmlDocument();
        }

        // TODO: Move to session
        /// <summary>
        /// Loads the commands.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        public ISet<Command> LoadCommands(string path = DEFAULT_COMMANDS_PATH)
        {
            if (null == commands)
            {
                commands = new HashSet<Command>();
            }
            try
            {
                commands.Clear();
                document.RemoveAll();
                document.Load(DEFAULT_COMMANDS_PATH);

                var root = document.SelectSingleNode("/Commands");
                var commandNodes = root.ChildNodes;

                foreach (XmlNode commandNode in commandNodes)
                {
                    var name = commandNode.Attributes[0].Value;
                    var content = commandNode.Attributes[1].Value;
                    var regex = new Regex(commandNode.Attributes[2].Value);
                    var type = EnumExtensions
                        .GetValueFromDescription<Command.TypeE>(commandNode.Attributes[3].Value);
                    var description = commandNode.FirstChild.InnerText;

                    commands.Add(Command.CreateCommand(name, content, description, regex, type));
                }
            }
            catch
            {
                Console.Error.WriteLine("Could not load command list into memory");
            }
            return commands;
        }

        // TODO: Move to command manager
        /// <summary>
        /// Saves the commands.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <exception cref="System.InvalidOperationException">Commands were never loaded to memory.</exception>
        public void SaveCommands(string path = DEFAULT_COMMANDS_PATH)
        {
            if (null == commands)
            {
                throw new InvalidOperationException("Commands were never loaded to memory.");
            }

            document.RemoveAll();
            foreach (var command in commands)
            {
                document.AppendChild(command.ToXML());
            }
            document.Save(path);
        }
    }
}
