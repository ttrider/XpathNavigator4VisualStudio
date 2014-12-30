using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace TTRider.XPNPackage
{
    public class XPathAdornment
    {
        readonly Dictionary<int, XmlNodeInfo> markers = new Dictionary<int, XmlNodeInfo>();
        readonly IAdornmentLayer layer;
        readonly IWpfTextView view;
        readonly Brush brush;
        readonly Pen pen;


        public XPathAdornment(IWpfTextView view)
        {
            this.view = view;
            this.layer = view.GetAdornmentLayer("XPathAdornment");

            //Listen to any event that changes the layout (text changes, scrolling, etc)
            this.view.LayoutChanged += OnLayoutChanged;
            this.view.TextBuffer.Changed += OnTextBufferChanged;

            //Create the pen and brush to color the box behind the a's
            this.brush = new SolidColorBrush(Color.FromArgb(0x20, 0x00, 0x00, 0xff));
            this.brush.Freeze();
            
            var penBrush = new SolidColorBrush(Colors.Red);
            penBrush.Freeze();
            
            this.pen = new Pen(penBrush, 0.5);
            this.pen.Freeze();
        }

        void OnTextBufferChanged(object sender, TextContentChangedEventArgs e)
        {
            ClearMarkers();
        }

        /// <summary>
        /// On layout change add the adornment to any reformatted lines
        /// </summary>
        private void OnLayoutChanged(object sender, TextViewLayoutChangedEventArgs e)
        {
            foreach (ITextViewLine line in e.NewOrReformattedLines)
            {
                var lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position);
                XmlNodeInfo info;
                if (this.markers.TryGetValue(lineNumber, out info))
                {
                    AddAdornment(info, line.Start.Add(info.LinePosition - 1));
                }

                //this.CreateVisuals(line);
            }
        }


        internal void ClearMarkers()
        {
            this.layer.RemoveAllAdornments();
            markers.Clear();
        }

        internal void SetMarker(XmlNodeInfo info)
        {
            markers[info.LineNumber-1] = info;

            var line = this.view.TextSnapshot.GetLineFromLineNumber(info.LineNumber - 1);
            if (line != null)
            {
                AddAdornment(info, line.Start.Add(info.LinePosition - 1));
            }
        }

        private void AddAdornment(XmlNodeInfo info, SnapshotPoint startPoint)
        {
            var span = new SnapshotSpan(this.view.TextSnapshot, startPoint, info.Name.Length);

            var g = this.view.TextViewLines.GetMarkerGeometry(span);
            if (g != null)
            {
                var drawing = new GeometryDrawing(this.brush, this.pen, g);
                drawing.Freeze();

                var drawingImage = new DrawingImage(drawing);
                drawingImage.Freeze();

                var image = new Image { Source = drawingImage };

                //Align the image with the top of the bounds of the text geometry
                Canvas.SetLeft(image, g.Bounds.Left);
                Canvas.SetTop(image, g.Bounds.Top);

                this.layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, span, null, image, null);
            }
        }
    }
}
