using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Media;
using ICSharpCode.AvalonEdit.Document;

namespace E3J.Models.Syntax_Check
{
    /// <summary>
    /// SyntaxCheckVisualizer
    /// </summary>
    public class SyntaxCheckVisualizer
    {
        /// <summary>
        /// The text marker service
        /// </summary>
        private readonly TextMarkerService textMarkerService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SyntaxCheckVisualizer"/> class.
        /// </summary>
        /// <param name="textEditor">The text editor.</param>
        public SyntaxCheckVisualizer(ProgramEditor textEditor)
        { 
            textMarkerService = new TextMarkerService(textEditor.Document);
            textEditor.TextArea.TextView.BackgroundRenderers.Add(textMarkerService);
            textEditor.TextArea.TextView.LineTransformers.Add(textMarkerService);
            var services =
                (IServiceContainer) textEditor.Document.ServiceProvider.GetService(typeof(IServiceContainer));
            services?.AddService(typeof(TextMarkerService), textMarkerService);
        }

        /// <summary>
        /// Visualizes the specified is valid.
        /// </summary>
        /// <param name="isValid">if set to <c>true</c> [is valid].</param>
        /// <param name="line">The line.</param>
        public void Visualize(bool isValid, DocumentLine line)
        {
            if (!line.IsDeleted)
            {
                if (isValid || line.Length == 0)
                {
                    RemoveMarker(line);
                }
                else
                {
                    RemoveMarker(line);
                    AddMarker(line);
                }
            }
        }

        /// <summary>
        /// Removes the marker.
        /// </summary>
        /// <param name="line">The line.</param>
        private void RemoveMarker(DocumentLine line)
        {
            textMarkerService.RemoveAll(l => l.StartOffset == line.Offset);
        }

        /// <summary>
        /// Adds the marker.
        /// </summary>
        /// <param name="line">The line.</param>
        private void AddMarker(DocumentLine line)
        {
            var marker = textMarkerService.Create(line.Offset, line.Length);
            marker.MarkerTypes = TextMarkerTypes.SquigglyUnderline;
            marker.MarkerColor = Colors.Red;
        }
    }
}
