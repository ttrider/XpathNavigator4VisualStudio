using System.Xml.XPath;

namespace TTRider.XPNPackage
{
    internal class XmlNodeInfo
    {
        public XmlNodeInfo(int lineNumber, int linePosition, string name)
        {
            this.LineNumber = lineNumber;
            this.LinePosition = linePosition;
            this.Name = name;
        }

        public string Name { get; }
        public int LineNumber { get; }
        public int LinePosition { get; }
        public XPathNavigator Navigator { get; set; }
    }
}