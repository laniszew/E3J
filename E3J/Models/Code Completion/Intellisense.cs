using System.Collections.Generic;
using System.Windows.Input;
using E3J.Models.ValueObjects;
using E3J.Utilities;
using E3J.Utilities.Extensions;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Editing;

namespace E3J.Models.Code_Completion
{
    /// <summary>
    /// Intellisense class
    /// </summary>
    public class Intellisense
    {
        /// <summary>
        /// The commands
        /// </summary>
        private readonly ISet<Command> commands;
        /// <summary>
        /// The text area
        /// </summary>
        private readonly TextArea textArea;
        /// <summary>
        /// The completion window
        /// </summary>
        private CompletionWindow completionWindow;

        /// <summary>
        /// Gets a value indicating whether this instance is showing.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is showing; otherwise, <c>false</c>.
        /// </value>
        public bool IsShowing => completionWindow != null && completionWindow.IsVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="Intellisense"/> class.
        /// </summary>
        /// <param name="textArea">The text area.</param>
        public Intellisense(TextArea textArea)
        {
            commands = Session.Instance.Commands.CommandsMap;
            this.textArea = textArea;
        }

        /// <summary>
        /// Prepares the specified e.
        /// </summary>
        /// <param name="e">The <see cref="TextCompositionEventArgs"/> instance containing the event data.</param>
        public void Prepare(TextCompositionEventArgs e)
        {
            if (completionWindow == null)
            {
                completionWindow = new CompletionWindow(textArea)
                {
                    CloseWhenCaretAtBeginning = true,
                };

                foreach (var command in commands)
                {
                    completionWindow.CompletionList.CompletionData.Add(new CompletionData(command.Content, command.Description, command.Type.Description()));
                }
                completionWindow.Closed += delegate
                {
                    completionWindow = null;
                };
            }

            if (e.Text.Length > 0 && completionWindow != null)
            {
                if (!char.IsLetterOrDigit(e.Text[0]))
                {
                    // Whenever a non-letter is typed while the completion window is open, insert the currently selected element.
                    completionWindow.CompletionList.RequestInsertion(e);
                }
            }
        }

        /// <summary>
        /// Submits the specified e.
        /// </summary>
        /// <param name="e">The <see cref="KeyEventArgs"/> instance containing the event data.</param>
        /// <param name="isOneLine">if set to <c>true</c> [is one line].</param>
        public void Submit(KeyEventArgs e, bool isOneLine)
        {
            if (e.Key == Key.Enter)
            {
                if (completionWindow != null)
                {
                    completionWindow?.Focus();
                    e.Handled = true;
                }

                if (isOneLine)
                {
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Shows this instance.
        /// </summary>
        public void Show()
        {
            var caretPosition = textArea.Caret.Line;
            var caretLine = textArea.Document.GetLineByNumber(caretPosition);
            var careLineText = textArea.Document.GetText(caretLine);

            if (completionWindow != null)
            {
                if (completionWindow.CompletionList.ListBox.HasItems && !caretLine.IsDeleted &&
                    !careLineText.Contains(" "))
                {
                    completionWindow.Show();
                }
                else
                {
                    completionWindow.Close();
                }
            }
        }

        /// <summary>
        /// Closes this instance.
        /// </summary>
        public void Close()
        {
            completionWindow?.Close();
        }
    }
}
