using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Editing;
using System.Windows.Media.Imaging;

namespace E3J.Models.Code_Completion
{
    /// <summary>
    /// CompletionData class
    /// </summary>
    /// <seealso cref="ICSharpCode.AvalonEdit.CodeCompletion.ICompletionData" />
    public class CompletionData : ICompletionData
    {
        /// <summary>
        /// The type
        /// </summary>
        private readonly string type;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompletionData"/> class.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="description">The description.</param>
        /// <param name="type">The type.</param>
        public CompletionData(string text, string description, string type)
        {
            Text = text;
            Description = description;
            this.type = type;
        }

        /// <summary>
        /// Gets the image.
        /// </summary>
        public System.Windows.Media.ImageSource Image
        {
            get
            {
                BitmapImage bitmapImage;
                switch (type)
                {
                    case "Comment":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Comment);
                        break;

                    case "Movement":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Movement);
                        break;

                    case "Grip":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Grip);
                        break;

                    case "TimersCounters":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.TimersCounters);
                        break;

                    case "Programming":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Programming);
                        break;

                    case "Information":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Information);
                        break;

                    case "Macro":
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Macros);
                        break;

                    default:
                        bitmapImage = Bitmap2BitmapImage(Properties.Resources.Invalid);
                        break;
                }
                return bitmapImage;
            }
        }

        /// <summary>
        /// Gets the text. This property is used to filter the list of visible elements.
        /// </summary>
        public string Text { get; private set; }

        // Use this property if you want to show a fancy UIElement in the list.
        /// <summary>
        /// The displayed content. This can be the same as 'Text', or a WPF UIElement if
        /// you want to display rich content.
        /// </summary>
        public object Content => this.Text;

        /// <summary>
        /// Gets the description.
        /// </summary>
        public object Description { private set; get; }

        /// <summary>
        /// Gets the priority. This property is used in the selection logic. You can use it to prefer selecting those items
        /// which the user is accessing most frequently.
        /// </summary>
        public double Priority => 0;

        /// <summary>
        /// Perform the completion.
        /// </summary>
        /// <param name="textArea">The text area on which completion is performed.</param>
        /// <param name="completionSegment">The text segment that was used by the completion window if
        /// the user types (segment between CompletionWindow.StartOffset and CompletionWindow.EndOffset).</param>
        /// <param name="insertionRequestEventArgs">The EventArgs used for the insertion request.
        /// These can be TextCompositionEventArgs, KeyEventArgs, MouseEventArgs, depending on how
        /// the insertion was triggered.</param>
        public void Complete(TextArea textArea, ISegment completionSegment,
            EventArgs insertionRequestEventArgs)
        {
            textArea.Document.Replace(completionSegment, this.Text);
        }

        /// <summary>
        /// Bitmap2s the bitmap image.
        /// </summary>
        /// <param name="bitmap">The bitmap.</param>
        /// <returns></returns>
        public BitmapImage Bitmap2BitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Png);
                memory.Position = 0;

                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }
    }
}
