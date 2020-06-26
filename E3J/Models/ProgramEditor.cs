using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Xml;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using E3J.Models.Code_Completion;
using E3J.Models.Syntax_Check;
using E3J.Utilities;
using Microsoft.Win32;

namespace E3J.Models
{
    /// <summary>
    /// ProgramEditor class
    /// </summary>
    /// <seealso cref="ICSharpCode.AvalonEdit.TextEditor" />
    public class ProgramEditor : TextEditor
    {

        #region Fields

        /// <summary>
        /// The syntax checker mode
        /// </summary>
        private SyntaxCheckerModeE syntaxCheckerMode;
        /// <summary>
        /// The syntax checker
        /// </summary>
        private readonly SyntaxChecker syntaxChecker;
        /// <summary>
        /// The intellisense
        /// </summary>
        private readonly Intellisense intellisense;
        /// <summary>
        /// The syntax check visualizer
        /// </summary>
        private readonly SyntaxCheckVisualizer syntaxCheckVisualizer;
        /// <summary>
        /// The is highlighting enabled
        /// </summary>
        private bool isHighlightingEnabled;
        /// <summary>
        /// The is intellisense enabled
        /// </summary>
        private bool isIntellisenseEnabled;

        #endregion

        #region Enums

        /// <summary>
        /// Describes wheter syntax check will occcur on the real time or on demand.
        /// </summary>
        public enum SyntaxCheckerModeE
        {
            /// <summary>
            /// The real time
            /// </summary>
            RealTime,
            /// <summary>
            /// The on demand
            /// </summary>
            OnDemand
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgramEditor"/> class.
        /// </summary>
        public ProgramEditor()
        {
            syntaxChecker = new SyntaxChecker();
            intellisense = new Intellisense(TextArea);
            syntaxCheckVisualizer = new SyntaxCheckVisualizer(this);
            SyntaxCheckerMode = SyntaxCheckerModeE.OnDemand;

            // Events
            Session.Instance.Highlighting.HighlightingChanged += LoadHighligtingDefinition;
            DataObject.AddPastingHandler(this, OnPaste);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether this instance is highlighting enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is highlighting enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsHighlightingEnabled
        {
            get { return isHighlightingEnabled; }
            set
            {
                isHighlightingEnabled = value;
                LoadHighligtingDefinition();
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is intellisense enabled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is intellisense enabled; otherwise, <c>false</c>.
        /// </value>
        public bool IsIntellisenseEnabled
        {
            get { return isIntellisenseEnabled; }
            set
            {
                isIntellisenseEnabled = value;
                if (isIntellisenseEnabled)
                {
                    //Subscribes to intellisense events.
                    TextArea.TextEntering += OnIntellisensePreparation;
                    TextArea.TextEntered += OnIntellisenseShow;
                    TextArea.PreviewKeyDown += OnIntellisenseSubmition;
                }
                else
                {
                    //Unsubscribes from intellisense events.
                    TextArea.TextEntering -= OnIntellisensePreparation;
                    TextArea.TextEntered -= OnIntellisenseShow;
                    TextArea.PreviewKeyDown -= OnIntellisenseSubmition;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance is intellisense showing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is intellisense showing; otherwise, <c>false</c>.
        /// </value>
        public bool IsIntellisenseShowing => intellisense.IsShowing;

        /// <summary>
        /// The do syntax check property
        /// </summary>
        public static readonly DependencyProperty DoSyntaxCheckProperty =
             DependencyProperty.Register("DoSyntaxCheck", typeof(bool),
             typeof(ProgramEditor), new FrameworkPropertyMetadata(true));

        /// <summary>
        /// Gets or sets a value indicating whether [do syntax check].
        /// </summary>
        /// <value>
        ///   <c>true</c> if [do syntax check]; otherwise, <c>false</c>.
        /// </value>
        public bool DoSyntaxCheck
        {
            get { return (bool)GetValue(DoSyntaxCheckProperty); }
            set
            {
                SetValue(DoSyntaxCheckProperty, value);
                if (!value)
                {
                    foreach (var line in Document.Lines)
                    {
                        syntaxCheckVisualizer.Visualize(true, line);
                    }
                }
                else
                {
                    ValidateAllLines();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is one line.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is one line; otherwise, <c>false</c>.
        /// </value>
        public bool IsOneLine { get; set; }

        /// <summary>
        /// Gets or sets the syntax checker mode.
        /// </summary>
        /// <value>
        /// The syntax checker mode.
        /// </value>
        public SyntaxCheckerModeE SyntaxCheckerMode
        {
            get { return syntaxCheckerMode; }
            set
            {
                if (value == SyntaxCheckerModeE.RealTime)
                {
                    TextChanged += OnSyntaxCheck;
                }
                else
                {
                    TextChanged -= OnSyntaxCheck;
                }
                syntaxCheckerMode = value;
            }
        }

        #endregion

        #region Actions

        /// <summary>
        /// Fonts the enlarge.
        /// </summary>
        public void FontEnlarge()
        {
            FontSize++;
        }

        /// <summary>
        /// Fonts the reduce.
        /// </summary>
        public void FontReduce()
        {
            FontSize--;
        }

        /// <summary>
        /// Sets current font.
        /// </summary>
        /// <param name="fontName">Name of the font.</param>
        public void ChangeFont(string fontName)
        {
            var fontFamily = FontFamily;

            try
            {
                FontFamily = new FontFamily(fontName);
            }
            catch
            {
                FontFamily = fontFamily;
            }
        }

        /// <summary>
        /// Loads the highligting definition.
        /// </summary>
        private void LoadHighligtingDefinition()
        {
            if (IsHighlightingEnabled)
            {
                var filePath = Session.Instance.Highlighting.FilePath;
                using (var reader = new XmlTextReader(filePath))
                {
                    try
                    {
                        var definition = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                        SyntaxHighlighting = definition;
                    }
                    catch (FileNotFoundException)
                    {
                        MissingFileManager.CreateHighlightingDefinitionFile();
                        Session.Instance.SubmitHighlighting(MissingFileManager.DEFAULT_HIGHLIGHTING_PATH);
                        Session.Instance.Highlighting.Import(MissingFileManager.DEFAULT_HIGHLIGHTING_PATH);
                        Session.Instance.Highlighting.Apply(Session.Instance.Highlighting.Colors);
                    }
                }
            }
            else
            {
                SyntaxHighlighting = null;
            }

        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Called when [intellisense submition].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        private void OnIntellisenseSubmition(object sender, KeyEventArgs e)
        {
            /*SelectionStart = Text.Length;
            SelectionLength = 0;*/
            intellisense.Submit(e, IsOneLine);
        }


        /// <summary>
        /// Called when [intellisense preparation].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
        private void OnIntellisensePreparation(object sender, TextCompositionEventArgs e)
        {
            intellisense.Prepare(e);
        }

        /// <summary>
        /// Called when [intellisense show].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
        private void OnIntellisenseShow(object sender, TextCompositionEventArgs e)
        {
            var line = Document.GetLineByNumber(TextArea.Caret.Line);
            // show only if nothing valid was typed in current line yet
            if (!syntaxChecker.Validate(Document.GetText(line)))
            {
                intellisense.Show();
            }
            else
            {
                intellisense.Close();
            }
        }

        /// <summary>
        /// Called when [syntax check].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private async void OnSyntaxCheck(object sender, EventArgs e)
        {
            if (DoSyntaxCheck)
            {
                await ValidateLine(TextArea.Caret.Line);
            }
        }

        /// <summary>
        /// Called when [paste].
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="DataObjectPastingEventArgs"/> instance containing the event data.</param>
        private void OnPaste(object sender, DataObjectPastingEventArgs e)
        {
            var isText = e.SourceDataObject.GetDataPresent(DataFormats.UnicodeText, true);
            if (!isText)
                return;

            var text = e.SourceDataObject.GetData(DataFormats.UnicodeText) as string;

            if (!string.IsNullOrWhiteSpace(text))
            {
                ValidateAllLines();
            }
        }

        /// <summary>
        /// Validates all lines.
        /// </summary>
        public async void ValidateAllLines()
        {
            if (DoSyntaxCheck)
            {
                foreach (var line in TextArea.Document.Lines)
                {
                    var lineText = TextArea.Document.GetText(line);
                    var isValid = await syntaxChecker.ValidateAsync(lineText);
                    syntaxCheckVisualizer.Visualize(isValid, line);
                }
            }
        }

        /// <summary>
        /// Validates the line.
        /// </summary>
        /// <param name="lineNum">The line number.</param>
        /// <returns></returns>
        public async Task<bool> ValidateLine(int lineNum)
        {
            var line = TextArea.Document.GetLineByNumber(lineNum);
            var lineText = TextArea.Document.GetText(line);
            var isValid = await syntaxChecker.ValidateAsync(lineText);
            syntaxCheckVisualizer.Visualize(!DoSyntaxCheck || isValid, line);
            return isValid;
        }

        /// <summary>
        /// Exports the content.
        /// </summary>
        /// <param name="defaultFileName">Default name of the file.</param>
        /// <param name="extension">The extension.</param>
        public void ExportContent(string defaultFileName, string extension)
        {
            try
            {
                var dialog = new SaveFileDialog
                {
                    // Default file name
                    FileName = defaultFileName,
                    // Default file extension
                    DefaultExt = extension,
                    // Filter files by extension
                    Filter = $"{extension} files (.{extension}|*.{extension}"
                };

                // Process save file dialog box results
                if (dialog.ShowDialog() == false)
                {
                    return;
                }

                var lines = Text.Split(new[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);


                File.WriteAllLines($"{dialog.FileName}", lines);
            }
            catch (Exception)
            {
                MessageBox.Show("Something went very wrong here. Try again tommorow.",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion

    }
}
