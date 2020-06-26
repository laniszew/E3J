using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Xml;
using Driver;
using FirstFloor.ModernUI.Presentation;

namespace E3J.Utilities
{
    /// <summary>
    /// Session class
    /// </summary>
    public class Session
    {
        #region Constants
        /// <summary>
        /// The session node
        /// </summary>
        private const string SESSION_NODE = "/Session";
        /// <summary>
        /// The commands parameter
        /// </summary>
        private const string COMMANDS_PARAM = "CommandsMap";
        /// <summary>
        /// The highlighting parameter
        /// </summary>
        private const string HIGHLIGHTING_PARAM = "HighlightingMap";

        private const string THEME_PARAM = "Theme";
        private const string ACCENT_COLOR_PARAM = "AccentColor";
        #endregion

        #region Settings
        /// <summary>
        /// Gets the commands.
        /// </summary>
        /// <value>
        /// The commands.
        /// </value>
        public Commands Commands { get; private set; }
        /// <summary>
        /// Gets the highlighting.
        /// </summary>
        /// <value>
        /// The highlighting.
        /// </value>
        public Highlighting Highlighting { get; private set; }
        #endregion

        /// <summary>
        /// The instance
        /// </summary>
        private static readonly Lazy<Session> instance = new Lazy<Session>(() => new Session());
        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static Session Instance => instance.Value;

        /// <summary>
        /// The document
        /// </summary>
        private readonly XmlDocument document = new XmlDocument();

        /// <summary>
        /// Prevents a default instance of the <see cref="Session"/> class from being created.
        /// </summary>
        private Session()
        {
            try
            {
                document.Load(MissingFileManager.SESSION_PATH);

                if (document.SelectSingleNode("/Session") == null)
                {
                    MissingFileManager.CreateSessionFile();
                }
            }
            catch(Exception e)
            {
                MissingFileManager.CreateSessionFile();
            }
        }

        /// <summary>
        /// Initializes this instance.
        /// </summary>
        public void Initialize()
        {
            document.Load(MissingFileManager.SESSION_PATH);
            var root = document.SelectSingleNode("/Session");

            // Loading commands path
            var commandsMapParam = root.Attributes["CommandsMap"];
            Commands = commandsMapParam != null ? new Commands(commandsMapParam.Value) : new Commands();

            // Loading highlighting
            var highlightingMapParam = root.Attributes["HighlightingMap"];
            Highlighting = highlightingMapParam != null ? new Highlighting(highlightingMapParam.Value) : new Highlighting();
        }

        public void InitializeColors()
        {
            document.Load(MissingFileManager.SESSION_PATH);
            var root = document.SelectSingleNode("/Session");

            // Loading last theme
            var themeParam = root.Attributes["Theme"];
          


            // Loading last accent color
            var accentColorParam = root.Attributes["AccentColor"];
       
        }

        /// <summary>
        /// Loads the programs.
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Program> LoadPrograms()
        {
            var list = new ObservableCollection<Program>();

            // Create session file if it does not exist
            MissingFileManager.CheckForRequiredFiles();
            
            try
            {
                document.Load(MissingFileManager.SESSION_PATH);
                var root = document.SelectSingleNode(SESSION_NODE);

                if (root == null)
                    return list;

                foreach (XmlNode child in root.ChildNodes)
                {
                    var path = child.InnerText;
                    try
                    {
                        if (!string.IsNullOrEmpty(path) && list.All(p => p.Path != path))
                        {
                            var program = new Program(Path.GetFileNameWithoutExtension(path))
                            {
                                Content = File.ReadAllText(path, Encoding.ASCII),
                                Path = path
                            };
                            list.Add(program);   
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.Error.WriteLine(ex.Message);
                    }
                }
                return list;
            }
            catch (Exception)
            {
                return list;
            }
        }

        /// <summary>
        /// Saves the programs.
        /// </summary>
        /// <param name="tabItems">The tab items.</param>
        public void SavePrograms(IEnumerable<TabItem> tabItems)
        {
            // Remove program nodes to override them
            document.Load(MissingFileManager.SESSION_PATH);
            var root = document.SelectSingleNode("Session");
            var programNodes = root.SelectNodes("Program");
            if (programNodes != null)
            {
                foreach (XmlNode programNode in programNodes)
                {
                    root.RemoveChild(programNode);
                }
            }
            
            // Add refreshed programs
            
        }

        /// <summary>
        /// Submits the highlighting.
        /// </summary>
        /// <param name="path">The path.</param>
        public void SubmitHighlighting(string path)
        {
            var root = document.SelectSingleNode("Session");
            var highligtingMapParam = root.Attributes[HIGHLIGHTING_PARAM];
            if (highligtingMapParam != null)
            {
                root.Attributes.Remove(highligtingMapParam);
            }
            highligtingMapParam = document.CreateAttribute(HIGHLIGHTING_PARAM);
            highligtingMapParam.Value = path;
            root.Attributes.Append(highligtingMapParam);
            document.Save(MissingFileManager.SESSION_PATH);
        }

        /// <summary>
        /// Submits the commands.
        /// </summary>
        /// <param name="path">The path.</param>
        public void SubmitCommands(string path)
        {
            var root = document.SelectSingleNode("Session");
            var commandsMapParam = root.Attributes[COMMANDS_PARAM];
            if (commandsMapParam != null)
            {
                root.Attributes.Remove(commandsMapParam);
            }
            commandsMapParam = document.CreateAttribute(COMMANDS_PARAM);
            commandsMapParam.Value = path;
            root.Attributes.Append(commandsMapParam);
            document.Save(MissingFileManager.SESSION_PATH);
        }

        public void SubmitAccentColor(Color accentColor)
        {
            var root = document.SelectSingleNode("Session");
            var themeParam = root.Attributes[ACCENT_COLOR_PARAM];
            if (themeParam != null)
            {
                root.Attributes.Remove(themeParam);
            }
            themeParam = document.CreateAttribute(ACCENT_COLOR_PARAM);
            themeParam.Value = accentColor.ToString();
            root.Attributes.Append(themeParam);
            document.Save(MissingFileManager.SESSION_PATH);
        }

        public void SubmitTheme(Link theme)
        {
            var root = document.SelectSingleNode("Session");
            var themeParam = root.Attributes[THEME_PARAM];
            if (themeParam != null)
            {
                root.Attributes.Remove(themeParam);
            }
            
            themeParam = document.CreateAttribute(THEME_PARAM);
            themeParam.Value = theme.DisplayName;
            root.Attributes.Append(themeParam);
            document.Save(MissingFileManager.SESSION_PATH);
        }
    }
}
