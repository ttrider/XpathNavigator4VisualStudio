using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Utilities;

namespace TTRider.XPNPackage
{
    [Export(typeof(IWpfTextViewMarginProvider))]
    [Name(XPathTopMargin.MarginName)]
    [Order(After = PredefinedMarginNames.Bottom)] //Ensure that the margin occurs below the horizontal scrollbar
    [MarginContainer(PredefinedMarginNames.Bottom)] //Set the container to the bottom of the editor window
    [ContentType("xml")] //Show this margin for all text-based types
    [TextViewRole(PredefinedTextViewRoles.Interactive)]
    internal sealed class XPathTopMarginFactory : IWpfTextViewMarginProvider
    {
        public IWpfTextViewMargin CreateMargin(IWpfTextViewHost textViewHost, IWpfTextViewMargin containerMargin)
        {
            return new XPathTopMargin(textViewHost.TextView);
            
        }
    }

}
