using System.Collections.Generic;

namespace TTRider.XPNPackage
{
    internal class XmlElementInfo : XmlNodeInfo
    {
        Dictionary<string, XmlNodeInfo> attributes;

        public XmlElementInfo(int lineNumber, int linePosition, string name)
            : base(lineNumber, linePosition, name)
        {
        }

        public Dictionary<string, XmlNodeInfo> Attributes
        {
            get 
            {
                return this.attributes ?? (this.attributes = new Dictionary<string, XmlNodeInfo>());
            }
        }
    }
}