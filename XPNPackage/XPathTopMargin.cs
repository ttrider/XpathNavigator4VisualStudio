using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using Microsoft.VisualStudio.PlatformUI;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace TTRider.XPNPackage
{
    class XPNForm : Grid, IWpfTextViewMargin
    {
        public const string MarginName = "XPNForm";
        private XPNDocument document;

        private IWpfTextView textView;
        private bool isDisposed;

        public XPNForm(IWpfTextView textView)
        {
            this.textView = textView;
            this.document = new XPNDocument(textView);

            this.textView.TextBuffer.Changed += TextBuffer_Changed;

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

            this.DataContext = this.document;
            this.SetBinding(Label.IsEnabledProperty, new Binding("Error") { IsAsync = true });
        }

        private void TextBuffer_Changed(object sender, Microsoft.VisualStudio.Text.TextContentChangedEventArgs e)
        {
            //throw new NotImplementedException();
        }

        private void ThrowIfDisposed()
        {
            if (isDisposed)
                throw new ObjectDisposedException(MarginName);
        }

        #region IWpfTextViewMargin Members


        public FrameworkElement VisualElement
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
            return (marginName == XPNForm.MarginName) ? (IWpfTextViewMargin)this : null;
        }

        public void Dispose()
        {
            if (!isDisposed)
            {
                GC.SuppressFinalize(this);
                isDisposed = true;
            }
        }
        #endregion
    }

    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(XPNForm.MarginName)]
    [Order(After = PredefinedMarginNames.Bottom)] //Ensure that the margin occurs below the horizontal scrollbar
    [MarginContainer(PredefinedMarginNames.Bottom)] //Set the container to the bottom of the editor window
    [ContentType("xml")] //Show this margin for all text-based types
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class XPathTopMarginFactory : IWpfTextViewMarginProvider
    {
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            return new XPNForm(textViewHost.TextView);
        }
    }
}
