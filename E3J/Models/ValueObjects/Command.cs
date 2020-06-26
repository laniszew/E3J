using System;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Xml;
using E3J.Utilities;
using E3J.Utilities.Extensions;

namespace E3J.Models.ValueObjects
{
   
    public class Command : IXMLObject
    {

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="Command"/> class.
        /// </summary>
        protected Command()
        {

        }

        #endregion

        #region enums

        /// <summary>
        /// 
        /// </summary>
        public enum TypeE
        {
            /// <summary>
            /// The other
            /// </summary>
            [Description("Other")]
            Other,
            /// <summary>
            /// The comment
            /// </summary>
            [Description("Comment")]
            Comment,
            /// <summary>
            /// The movement
            /// </summary>
            [Description("Movement")]
            Movement,
            /// <summary>
            /// The grip
            /// </summary>
            [Description("Grip")]
            Grip,
            /// <summary>
            /// The timers counters
            /// </summary>
            [Description("TimersCounters")]
            TimersCounters,
            /// <summary>
            /// The programming
            /// </summary>
            [Description("Programming")]
            Programming,
            /// <summary>
            /// The information
            /// </summary>
            [Description("Information")]
            Information,
            /// <summary>
            /// The macro
            /// </summary>
            [Description("Macro")]
            Macro
        };

        #endregion

        #region Properties

        /// <summary>
        /// Gets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name { get; private set; }
        /// <summary>
        /// Gets the content.
        /// </summary>
        /// <value>
        /// The content.
        /// </value>
        public string Content { get; private set; }
        /// <summary>
        /// Gets the description.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; private set; }
        /// <summary>
        /// Gets the regex.
        /// </summary>
        /// <value>
        /// The regex.
        /// </value>
        public Regex Regex { get; private set; }
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public TypeE Type { get; private set; }

        #endregion

        #region Actions

        /// <summary>
        /// Creates new command with specified parameters
        /// </summary>
        /// <param name="name">Defnies the name of command</param>
        /// <param name="content">Defines command keyword in movemaster language</param>
        /// <param name="description">Describes command [not mandatory]</param>
        /// <param name="regex">Defines valid syntax of command [not mandatory]</param>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentException">Invalid arguments were passed</exception>
        public static Command CreateCommand(string name, string content, string description, Regex regex, TypeE type)
        {
            if (ValidateData(name, content, description, regex))
            {
                var command = new Command
                {
                    Name = name,
                    Content = content,
                    Description = description,
                    Regex = regex,
                    Type = type
                };
                return command;
            }
            throw new ArgumentException("Invalid arguments were passed");
        }

        /// <summary>
        /// Validates the data.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="content">The content.</param>
        /// <param name="description">The description.</param>
        /// <param name="regex">The regex.</param>
        /// <returns></returns>
        private static bool ValidateData(string name, string content, string description, Regex regex)
        {
            return !string.IsNullOrWhiteSpace(name) && !string.IsNullOrEmpty(content) &&
                   null != description && null != regex;
        }

        /// <summary>
        /// To the XML.
        /// </summary>
        /// <returns></returns>
        public XmlElement ToXML()
        {
            var document = new XmlDocument();
            var root = document.CreateElement("Command");
            root.SetAttribute("name", Name);
            root.SetAttribute("content", Content);
            root.SetAttribute("regex", Convert.ToString(Regex));
            root.SetAttribute("type", Type.Description());
            var descriptioNode = root.AppendChild(document.CreateElement("Description"));
            descriptioNode.InnerText = Description;

            return root;
        }

        #endregion

    }
}
