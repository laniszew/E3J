using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;

namespace E3J.Utilities
{
    /// <summary>
    /// MissingFileManager class
    /// </summary>
    public static class MissingFileManager
    {
        #region Constants

        /// <summary>
        /// The default highlighting path
        /// </summary>
        public const string DEFAULT_HIGHLIGHTING_PATH = "HighlightingDefinition.xshd";
        /// <summary>
        /// The default commands path
        /// </summary>
        public const string DEFAULT_COMMANDS_PATH = "CommandsDefinition.xml";
        /// <summary>
        /// The session path
        /// </summary>
        public const string SESSION_PATH = "Session.xml";

        #endregion

        /// <summary>
        /// Creates Highlighting definition (.xshd) file.
        /// </summary>
        public static void CreateHighlightingDefinitionFile()
        {
            var document = new XmlDocument();
            document.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("E3J.Others.Resources.Highlighting.xshd"));
            document.Save(DEFAULT_HIGHLIGHTING_PATH);
        }

        /// <summary>
        /// Creates Command definition (.xml) file.
        /// </summary>
        public static void CreateCommandsFile()
        {
            var document = new XmlDocument();
            document.Load(Assembly.GetExecutingAssembly().GetManifestResourceStream("E3J.Others.Resources.Commands.xml"));
            document.Save(DEFAULT_COMMANDS_PATH);
        }

        /// <summary>
        /// Creates Highlighting (.xshd) and command (.xml) definition files.
        /// </summary>
        public static void CreateMissingFiles()
        {
            CreateCommandsFile();
            CreateHighlightingDefinitionFile();
        }

        /// <summary>
        /// Creates the session file.
        /// </summary>
        public static void CreateSessionFile()
        {
            var document = new XmlDocument();
            var root = document.CreateElement("Session");
            root.SetAttribute("CommandsMap", DEFAULT_COMMANDS_PATH);
            root.SetAttribute("HighlightingMap", DEFAULT_HIGHLIGHTING_PATH);
            document.AppendChild(root);
            document.Save("Session.xml");
        }

        /// <summary>
        /// Checks and creates missing files.
        /// </summary>
        public static void CheckForRequiredFiles()
        {
            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory(), "*", SearchOption.AllDirectories)
                .Where(file => file.Contains(DEFAULT_COMMANDS_PATH) || file.Contains(DEFAULT_HIGHLIGHTING_PATH) || file.Contains(SESSION_PATH))
                .Select(Path.GetFileName)
                .ToList();

            if (!files.Contains(DEFAULT_COMMANDS_PATH))
            {
                CreateCommandsFile();
            }
            if (!files.Contains(DEFAULT_HIGHLIGHTING_PATH))
            {
                CreateHighlightingDefinitionFile();
            }
            if (!files.Contains(SESSION_PATH))
            {
                CreateSessionFile();
            }
        }
    }
}
