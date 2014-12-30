using System.Xml.XPath;

namespace TTRider.XPNPackage
{
    internal class XmlNodeInfo
    {
        int lineNumber;
        int linePosition;
        string name;

        public XmlNodeInfo(int lineNumber, int linePosition, string name)
        {
            this.lineNumber = lineNumber;
            this.linePosition = linePosition;
            this.name = name;
        }

        public string Name { get { return this.name; } }
        public int LineNumber { get { return this.lineNumber; } }
        public int LinePosition { get { return this.linePosition; } }
        public XPathNavigator Navigator { get; set; }
    }
}