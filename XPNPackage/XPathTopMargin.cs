using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;

namespace TTRider.XPNPackage
{
    //class XPathTopMargin : Canvas, IWpfTextViewMargin
    class XPathTopMargin : Grid, IWpfTextViewMargin
    {
        public const string MarginName = "XPathNavigator";
        private IWpfTextView _textView;
        private bool _isDisposed = false;


        /// <summary>
        /// Creates a <see cref="EditorMargin1"/> for a given <see cref="IWpfTextView"/>.
        /// </summary>
        /// <param name="textView">The <see cref="IWpfTextView"/> to attach the margin to.</param>
        public XPathTopMargin(IWpfTextView textView)
        {
            _textView = textView;

            this.Height = 25;
            this.ClipToBounds = true;

            this.SetResourceReference(Control.BackgroundProperty, EnvironmentColors.ScrollBarBackgroundBrushKey);

            this.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
            this.ColumnDefinitions.Add(new ColumnDefinition());
            this.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

            var label = new Label { Content = "XPath" };
            this.Children.Add(label);

            var xpath = new TextBox()
            {
                //Padding = new Thickness(0, 0.5, 0, 0.5)
            };

            this.Children.Add(xpath);


            var go = new Button() { Content = "→" };
            this.Children.Add(go);


            Grid.SetColumn(label, 0);
            Grid.SetColumn(xpath, 1);
            Grid.SetColumn(go, 2);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        #region IWpfTextViewMargin Members

        /// <summary>
        /// The <see cref="Sytem.Windows.FrameworkElement"/> that implements the visual representation
        /// of the margin.
        /// </summary>
        public System.Windows.FrameworkElement VisualElement
        {
            // Since this margin implements Canvas, this is the object which renders
            // the margin.
            get
            {
                ThrowIfDisposed();
                return this;
            }
        }

        #endregion

        #region ITextViewMargin Members

        public double MarginSize
        {
            // Since this is a horizontal margin, its width will be bound to the width of the text view.
            // Therefore, its size is its height.
            get
            {
                ThrowIfDisposed();
                return this.ActualHeight;
            }
        }

        public bool Enabled
        {
            // The margin should always be enabled
            get
            {
                ThrowIfDisposed();
                return true;
            }
        }

        /// <summary>
        /// Returns an instance of the margin if this is the margin that has been requested.
        /// </summary>
        /// <param name="marginName">The name of the margin requested</param>
        /// <returns>An instance of EditorMargin1 or null</returns>
        public ITextViewMargin GetTextViewMargin(string marginName)
        {
            return (marginName == XPathTopMargin.MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!_isDisposed)
            {
                GC.SuppressFinalize(this);
                _isDisposed = true;
            }
        }
        #endregion
    }
}
