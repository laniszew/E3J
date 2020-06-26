// Copyright (c) 2014 AlphaSierraPapa for the SharpDevelop Team
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this
// software and associated documentation files (the "Software"), to deal in the Software
// without restriction, including without limitation the rights to use, copy, modify, merge,
// publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons
// to whom the Software is furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED,
// INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR
// PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
// OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace E3J.Models.Syntax_Check
{
    /// <summary>
    /// Handles the text markers for a code editor.
    /// </summary>
    /// <seealso cref="ICSharpCode.AvalonEdit.Rendering.DocumentColorizingTransformer" />
    /// <seealso cref="ICSharpCode.AvalonEdit.Rendering.IBackgroundRenderer" />
    /// <seealso cref="IDE.Common.Models.Syntax_Check.ITextMarkerService" />
    /// <seealso cref="ICSharpCode.AvalonEdit.Rendering.ITextViewConnect" />
    public sealed class TextMarkerService : DocumentColorizingTransformer, IBackgroundRenderer, ITextMarkerService, ITextViewConnect
	{
        /// <summary>
        /// The markers
        /// </summary>
        TextSegmentCollection<TextMarker> markers;
        /// <summary>
        /// The document
        /// </summary>
        TextDocument document;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMarkerService"/> class.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <exception cref="System.ArgumentNullException">document</exception>
        public TextMarkerService(TextDocument document)
		{
			if (document == null)
				throw new ArgumentNullException("document");
			this.document = document;
			this.markers = new TextSegmentCollection<TextMarker>(document);
		}

        #region ITextMarkerService
        /// <summary>
        /// Creates a new text marker. The text marker will be invisible at first,
        /// you need to set one of the Color properties to make it visible.
        /// </summary>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="length">The length.</param>
        /// <returns></returns>
        /// <exception cref="System.InvalidOperationException">Cannot create a marker when not attached to a document</exception>
        /// <exception cref="System.ArgumentOutOfRangeException">
        /// startOffset - Value must be between 0 and " + textLength
        /// or
        /// length - length must not be negative and startOffset+length must not be after the end of the document
        /// </exception>
        public ITextMarker Create(int startOffset, int length)
		{
			if (markers == null)
				throw new InvalidOperationException("Cannot create a marker when not attached to a document");
			
			int textLength = document.TextLength;
			if (startOffset < 0 || startOffset > textLength)
				throw new ArgumentOutOfRangeException("startOffset", startOffset, "Value must be between 0 and " + textLength);
			if (length < 0 || startOffset + length > textLength)
				throw new ArgumentOutOfRangeException("length", length, "length must not be negative and startOffset+length must not be after the end of the document");
			
			TextMarker m = new TextMarker(this, startOffset, length);
			markers.Add(m);
			// no need to mark segment for redraw: the text marker is invisible until a property is set
			return m;
		}

        /// <summary>
        /// Finds all text markers at the specified offset.
        /// </summary>
        /// <param name="offset">The offset.</param>
        /// <returns></returns>
        public IEnumerable<ITextMarker> GetMarkersAtOffset(int offset)
		{
			if (markers == null)
				return Enumerable.Empty<ITextMarker>();
			else
				return markers.FindSegmentsContaining(offset);
		}

        /// <summary>
        /// Gets the list of text markers.
        /// </summary>
        /// <value>
        /// The text markers.
        /// </value>
        public IEnumerable<ITextMarker> TextMarkers {
			get { return markers ?? Enumerable.Empty<ITextMarker>(); }
		}

        /// <summary>
        /// Removes all text markers that match the condition.
        /// </summary>
        /// <param name="predicate">The predicate.</param>
        /// <exception cref="System.ArgumentNullException">predicate</exception>
        public void RemoveAll(Predicate<ITextMarker> predicate)
		{
			if (predicate == null)
				throw new ArgumentNullException("predicate");
			if (markers != null) {
				foreach (TextMarker m in markers.ToArray()) {
					if (predicate(m))
						Remove(m);
				}
			}
		}

        /// <summary>
        /// Removes the specified text marker.
        /// </summary>
        /// <param name="marker">The marker.</param>
        /// <exception cref="System.ArgumentNullException">marker</exception>
        public void Remove(ITextMarker marker)
		{
			if (marker == null)
				throw new ArgumentNullException("marker");
			TextMarker m = marker as TextMarker;
			if (markers != null && markers.Remove(m)) {
				Redraw(m);
				m.OnDeleted();
			}
		}

        /// <summary>
        /// Redraws the specified text segment.
        /// </summary>
        /// <param name="segment">The segment.</param>
        internal void Redraw(ISegment segment)
		{
			foreach (var view in textViews) {
				view.Redraw(segment, DispatcherPriority.Normal);
			}
			if (RedrawRequested != null)
				RedrawRequested(this, EventArgs.Empty);
		}

        /// <summary>
        /// Occurs when [redraw requested].
        /// </summary>
        public event EventHandler RedrawRequested;
        #endregion

        #region DocumentColorizingTransformer
        /// <summary>
        /// Override this method to colorize an individual document line.
        /// </summary>
        /// <param name="line"></param>
        protected override void ColorizeLine(DocumentLine line)
		{
			if (markers == null)
				return;
			int lineStart = line.Offset;
			int lineEnd = lineStart + line.Length;
			foreach (TextMarker marker in markers.FindOverlappingSegments(lineStart, line.Length)) {
				Brush foregroundBrush = null;
				if (marker.ForegroundColor != null) {
					foregroundBrush = new SolidColorBrush(marker.ForegroundColor.Value);
					foregroundBrush.Freeze();
				}
				ChangeLinePart(
					Math.Max(marker.StartOffset, lineStart),
					Math.Min(marker.EndOffset, lineEnd),
					element => {
						if (foregroundBrush != null) {
							element.TextRunProperties.SetForegroundBrush(foregroundBrush);
						}
						Typeface tf = element.TextRunProperties.Typeface;
						element.TextRunProperties.SetTypeface(new Typeface(
							tf.FontFamily,
							marker.FontStyle ?? tf.Style,
							marker.FontWeight ?? tf.Weight,
							tf.Stretch
						));
					}
				);
			}
		}
        #endregion

        #region IBackgroundRenderer
        /// <summary>
        /// Gets the layer on which this background renderer should draw.
        /// </summary>
        public KnownLayer Layer {
			get {
				// draw behind selection
				return KnownLayer.Selection;
			}
		}

        /// <summary>
        /// Causes the background renderer to draw.
        /// </summary>
        /// <param name="textView"></param>
        /// <param name="drawingContext"></param>
        /// <exception cref="System.ArgumentNullException">
        /// textView
        /// or
        /// drawingContext
        /// </exception>
        public void Draw(TextView textView, DrawingContext drawingContext)
		{
			if (textView == null)
				throw new ArgumentNullException("textView");
			if (drawingContext == null)
				throw new ArgumentNullException("drawingContext");
			if (markers == null || !textView.VisualLinesValid)
				return;
			var visualLines = textView.VisualLines;
			if (visualLines.Count == 0)
				return;
			int viewStart = visualLines.First().FirstDocumentLine.Offset;
			int viewEnd = visualLines.Last().LastDocumentLine.EndOffset;
			foreach (TextMarker marker in markers.FindOverlappingSegments(viewStart, viewEnd - viewStart)) {
				if (marker.BackgroundColor != null) {
					BackgroundGeometryBuilder geoBuilder = new BackgroundGeometryBuilder();
					geoBuilder.AlignToWholePixels = true;
					geoBuilder.CornerRadius = 3;
					geoBuilder.AddSegment(textView, marker);
					Geometry geometry = geoBuilder.CreateGeometry();
					if (geometry != null) {
						Color color = marker.BackgroundColor.Value;
						SolidColorBrush brush = new SolidColorBrush(color);
						brush.Freeze();
						drawingContext.DrawGeometry(brush, null, geometry);
					}
				}
				var underlineMarkerTypes = TextMarkerTypes.SquigglyUnderline | TextMarkerTypes.NormalUnderline | TextMarkerTypes.DottedUnderline;
				if ((marker.MarkerTypes & underlineMarkerTypes) != 0) {
					foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(textView, marker)) {
						Point startPoint = r.BottomLeft;
						Point endPoint = r.BottomRight;
						
						Brush usedBrush = new SolidColorBrush(marker.MarkerColor);
						usedBrush.Freeze();
						if ((marker.MarkerTypes & TextMarkerTypes.SquigglyUnderline) != 0) {
							double offset = 2.5;
							
							int count = Math.Max((int)((endPoint.X - startPoint.X) / offset) + 1, 4);
							
							StreamGeometry geometry = new StreamGeometry();
							
							using (StreamGeometryContext ctx = geometry.Open()) {
								ctx.BeginFigure(startPoint, false, false);
								ctx.PolyLineTo(CreatePoints(startPoint, endPoint, offset, count).ToArray(), true, false);
							}
							
							geometry.Freeze();
							
							Pen usedPen = new Pen(usedBrush, 1);
							usedPen.Freeze();
							drawingContext.DrawGeometry(Brushes.Transparent, usedPen, geometry);
						}
						if ((marker.MarkerTypes & TextMarkerTypes.NormalUnderline) != 0) {
							Pen usedPen = new Pen(usedBrush, 1);
							usedPen.Freeze();
							drawingContext.DrawLine(usedPen, startPoint, endPoint);
						}
						if ((marker.MarkerTypes & TextMarkerTypes.DottedUnderline) != 0) {
							Pen usedPen = new Pen(usedBrush, 1);
							usedPen.DashStyle = DashStyles.Dot;
							usedPen.Freeze();
							drawingContext.DrawLine(usedPen, startPoint, endPoint);
						}
					}
				}
			}
		}

        /// <summary>
        /// Creates the points.
        /// </summary>
        /// <param name="start">The start.</param>
        /// <param name="end">The end.</param>
        /// <param name="offset">The offset.</param>
        /// <param name="count">The count.</param>
        /// <returns></returns>
        IEnumerable<Point> CreatePoints(Point start, Point end, double offset, int count)
		{
			for (int i = 0; i < count; i++)
				yield return new Point(start.X + i * offset, start.Y - ((i + 1) % 2 == 0 ? offset : 0));
		}
        #endregion

        #region ITextViewConnect
        /// <summary>
        /// The text views
        /// </summary>
        readonly List<TextView> textViews = new List<TextView>();

        /// <summary>
        /// Called when added to a text view.
        /// </summary>
        /// <param name="textView"></param>
        void ITextViewConnect.AddToTextView(TextView textView)
		{
			if (textView != null && !textViews.Contains(textView)) {
				Debug.Assert(textView.Document == document);
				textViews.Add(textView);
			}
		}

        /// <summary>
        /// Called when removed from a text view.
        /// </summary>
        /// <param name="textView"></param>
        void ITextViewConnect.RemoveFromTextView(TextView textView)
		{
			if (textView != null) {
				Debug.Assert(textView.Document == document);
				textViews.Remove(textView);
			}
		}
		#endregion
	}

    /// <summary>
    /// 
    /// </summary>
    /// <seealso cref="ICSharpCode.AvalonEdit.Document.TextSegment" />
    /// <seealso cref="IDE.Common.Models.Syntax_Check.ITextMarker" />
    public sealed class TextMarker : TextSegment, ITextMarker
	{
        /// <summary>
        /// The service
        /// </summary>
        readonly TextMarkerService service;

        /// <summary>
        /// Initializes a new instance of the <see cref="TextMarker"/> class.
        /// </summary>
        /// <param name="service">The service.</param>
        /// <param name="startOffset">The start offset.</param>
        /// <param name="length">The length.</param>
        /// <exception cref="System.ArgumentNullException">service</exception>
        public TextMarker(TextMarkerService service, int startOffset, int length)
		{
			if (service == null)
				throw new ArgumentNullException("service");
			this.service = service;
			this.StartOffset = startOffset;
			this.Length = length;
			this.markerTypes = TextMarkerTypes.None;
		}

        /// <summary>
        /// Event that occurs when the text marker is deleted.
        /// </summary>
        public event EventHandler Deleted;

        /// <summary>
        /// Gets whether the text marker was deleted.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is deleted; otherwise, <c>false</c>.
        /// </value>
        public bool IsDeleted {
			get { return !this.IsConnectedToCollection; }
		}

        /// <summary>
        /// Deletes the text marker.
        /// </summary>
        public void Delete()
		{
			service.Remove(this);
		}

        /// <summary>
        /// Called when [deleted].
        /// </summary>
        internal void OnDeleted()
		{
			if (Deleted != null)
				Deleted(this, EventArgs.Empty);
		}

        /// <summary>
        /// Redraws this instance.
        /// </summary>
        void Redraw()
		{
			service.Redraw(this);
		}

        /// <summary>
        /// The background color
        /// </summary>
        Color? backgroundColor;

        /// <summary>
        /// Gets/Sets the background color.
        /// </summary>
        /// <value>
        /// The color of the background.
        /// </value>
        public Color? BackgroundColor {
			get { return backgroundColor; }
			set {
				if (backgroundColor != value) {
					backgroundColor = value;
					Redraw();
				}
			}
		}

        /// <summary>
        /// The foreground color
        /// </summary>
        Color? foregroundColor;

        /// <summary>
        /// Gets/Sets the foreground color.
        /// </summary>
        /// <value>
        /// The color of the foreground.
        /// </value>
        public Color? ForegroundColor {
			get { return foregroundColor; }
			set {
				if (foregroundColor != value) {
					foregroundColor = value;
					Redraw();
				}
			}
		}

        /// <summary>
        /// The font weight
        /// </summary>
        FontWeight? fontWeight;

        /// <summary>
        /// Gets/Sets the font weight.
        /// </summary>
        /// <value>
        /// The font weight.
        /// </value>
        public FontWeight? FontWeight {
			get { return fontWeight; }
			set {
				if (fontWeight != value) {
					fontWeight = value;
					Redraw();
				}
			}
		}

        /// <summary>
        /// The font style
        /// </summary>
        FontStyle? fontStyle;

        /// <summary>
        /// Gets/Sets the font style.
        /// </summary>
        /// <value>
        /// The font style.
        /// </value>
        public FontStyle? FontStyle {
			get { return fontStyle; }
			set {
				if (fontStyle != value) {
					fontStyle = value;
					Redraw();
				}
			}
		}

        /// <summary>
        /// Gets/Sets an object with additional data for this text marker.
        /// </summary>
        /// <value>
        /// The tag.
        /// </value>
        public object Tag { get; set; }

        /// <summary>
        /// The marker types
        /// </summary>
        TextMarkerTypes markerTypes;

        /// <summary>
        /// Gets/Sets the type of the marker. Use TextMarkerType.None for normal markers.
        /// </summary>
        /// <value>
        /// The marker types.
        /// </value>
        public TextMarkerTypes MarkerTypes {
			get { return markerTypes; }
			set {
				if (markerTypes != value) {
					markerTypes = value;
					Redraw();
				}
			}
		}

        /// <summary>
        /// The marker color
        /// </summary>
        Color markerColor;

        /// <summary>
        /// Gets/Sets the color of the marker.
        /// </summary>
        /// <value>
        /// The color of the marker.
        /// </value>
        public Color MarkerColor {
			get { return markerColor; }
			set {
				if (markerColor != value) {
					markerColor = value;
					Redraw();
				}
			}
		}

        /// <summary>
        /// Gets/Sets an object that will be displayed as tooltip in the text editor.
        /// </summary>
        /// <value>
        /// The tool tip.
        /// </value>
        /// <remarks>
        /// Not supported in this sample!
        /// </remarks>
        public object ToolTip { get; set; }
	}
}
