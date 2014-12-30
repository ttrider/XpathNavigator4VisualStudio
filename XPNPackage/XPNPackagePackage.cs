using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using System.Windows.Forms;
using EnvDTE;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;

namespace TTRider.XPNPackage
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the information needed to show this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    [Guid(GuidList.guidXPNPackagePkgString)]
    public sealed class XpnPackagePackage : Package
    {
        private OleMenuCommand xpathCombobox;
        private Guid outputGuid = new Guid("{AD72592B-06BC-4e75-8149-B35901767820}");
        private const string DefaultNamespacePrefix = "ns";

        /// <summary>
        /// Default constructor of the package.
        /// Inside this method you can place any initialization code that does not require 
        /// any Visual Studio service because at this point the package object is created but 
        /// not sited yet inside Visual Studio environment. The place to do all the other 
        /// initialization is the Initialize method.
        /// </summary>
        public XpnPackagePackage()
        {
            Debug.WriteLine("Entering constructor for: {0}", this.ToString());
        }



        /////////////////////////////////////////////////////////////////////////////
        // Overridden Package Implementation

        #region Package Members

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initialization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            Debug.WriteLine("Entering Initialize() of: {0}", this.ToString());
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            var mcs = GetService(typeof (IMenuCommandService)) as OleMenuCommandService;
            if (null != mcs)
            {
                xpathCombobox = new OleMenuCommand(
                    OnXPathCommand,
                    new CommandID(new Guid("{68533051-270b-4c41-bf90-79a1e1e64edd}"), (int) PkgCmdIDList.cmdidXPathCombo))
                {
                    ParametersDescription = "$"
                };

                xpathCombobox.BeforeQueryStatus += delegate
                {
                    xpathCombobox.Enabled = GetActiveTextDocument() != null;
                };

                mcs.AddCommand(xpathCombobox);
            }
        }

        #endregion

        public string GetSelectedText()
        {
            IVsTextManager txtMgr = (IVsTextManager)GetService(typeof(SVsTextManager));
            IVsTextView txtView = null;
            string selectedText = string.Empty;

            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out txtView);

            
            txtView.GetSelectedText(out selectedText);
            return selectedText;
        }

        private IWpfTextView GetActiveTextView()
        {
            IWpfTextView view = null;
            IVsTextView vTextView = null;

            IVsTextManager txtMgr =
            (IVsTextManager)GetService(typeof(SVsTextManager));

            int mustHaveFocus = 1;
            txtMgr.GetActiveView(mustHaveFocus, null, out vTextView);

            IVsUserData userData = vTextView as IVsUserData;
            if (null != userData)
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost)holder;
                view = viewHost.TextView;
            }

            return view;
        }


        private TextDocument GetActiveTextDocument()
        {
            var dte = (_DTE) GetService(typeof (_DTE));
            if ((dte != null) && (dte.ActiveWindow != null) && (dte.ActiveWindow.Document != null) &&
                (string.Equals(dte.ActiveWindow.Document.Language, "XML", StringComparison.OrdinalIgnoreCase)))
            {
                var doc = dte.ActiveDocument.Object() as TextDocument;
                return doc;
            }
            return null;
        }

        private IVsOutputWindowPane GetOutputPane()
        {
            IVsOutputWindowPane ret = null;
            var wnd = (IVsOutputWindow) GetService(typeof (IVsOutputWindow));
            if (wnd != null)
            {
                wnd.GetPane(ref outputGuid, out ret);

                if (ret == null)
                {
                    wnd.CreatePane(ref outputGuid, Resources.XPathNavigator, 0, 1);
                    wnd.GetPane(ref outputGuid, out ret);
                }
            }

            return ret;
        }

        private string GetXPathQuery(EventArgs e)
        {
            var eventArgs = e as OleMenuCmdEventArgs;
            if (eventArgs != null)
            {
                object input = eventArgs.InValue;

                if (input != null)
                {
                    return input.ToString();
                }
            }
            return null;
        }


        private void OnXPathCommand(object sender, EventArgs e)
        {
            try
            {
                var view = GetActiveTextView();
                var ad = view.Properties.GetProperty<XPathAdornment>("XPathAdornmentObject");
                ad.ClearMarkers();


                string xpathQuery = GetXPathQuery(e);
                if (!string.IsNullOrEmpty(xpathQuery))
                {
                    TextDocument td = GetActiveTextDocument();
                    
                    if (td != null)
                    {
                        EditPoint sp = td.CreateEditPoint(td.StartPoint);
                        EditPoint ep = td.CreateEditPoint(td.EndPoint);

                        var manager = new XPathTextManager(new StringReader(sp.GetText(ep)),
                            DefaultNamespacePrefix);



                        IVsOutputWindowPane pane = GetOutputPane();
                        if (pane != null)
                        {
                            pane.Clear();
                            pane.Activate();
                            pane.OutputString(string.Format(Resources.Query, xpathQuery));

                            int count = 0;
                            foreach (XmlNodeInfo info in manager.Select(xpathQuery))
                            {
                                string xmlSample = info.Navigator.OuterXml;
                                if (xmlSample.Length > 1024)
                                {
                                    xmlSample = xmlSample.Substring(0, 1024) + " ...";
                                }

                                xmlSample = xmlSample.Replace("\r", "").Replace("\n", " ");

                                ad.SetMarker(info);

                                string output = string.Format(Resources.OutputString, info.LineNumber, info.LinePosition,
                                    xmlSample);


                                var pane2 = pane as IVsOutputWindowPane2;
                                if (pane2 == null)
                                {
                                    pane.OutputTaskItemString(output, VSTASKPRIORITY.TP_NORMAL,
                                        VSTASKCATEGORY.CAT_SHORTCUTS, Resources.XPathNavigator, 0,
                                        td.DTE.ActiveDocument.FullName, (uint) info.LineNumber - 1, output);
                                }
                                else
                                {
                                    pane2.OutputTaskItemStringEx2(output, VSTASKPRIORITY.TP_NORMAL,
                                        VSTASKCATEGORY.CAT_SHORTCUTS, Resources.XPathNavigator,
                                        (int) _vstaskbitmap.BMP_SHORTCUT, td.DTE.ActiveDocument.FullName,
                                        (uint) info.LineNumber - 1, (uint) info.LinePosition - 1, null, output, null);
                                }
                                count++;
                            }

                            pane.OutputString(string.Format(
                                (count == 1) ? Resources.ResultSingle : Resources.ResultMult, count));
                            pane.FlushToTaskList();

                            if ((count == 0) && (manager.HasDefaultNamespace) &&
                                (xpathQuery.IndexOf(DefaultNamespacePrefix + ":", StringComparison.Ordinal) == -1))
                            {
                                pane.OutputString(Resources.ResultZero);
                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                VsShellUtilities.ShowMessageBox(this, ex.Message, Resources.Error, OLEMSGICON.OLEMSGICON_CRITICAL,
                    OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
            }
        }










    }
}
