using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace TTRider.XPNPackage
{
    internal class XmlLineInfoReader : XmlTextReader
    {
        readonly List<XmlNodeInfo> elements = new List<XmlNodeInfo>();

        public XmlLineInfoReader(TextReader s)
            : base(s)
        {
        }

        public List<XmlNodeInfo> Elements
        {
            get { return this.elements; }
        }

        public override bool Read()
        {
            bool retVal = base.Read();

            if (retVal)
            {
                if (this.NodeType == XmlNodeType.Element)
                {
                    var elInfo = new XmlElementInfo(this.LineNumber, this.LinePosition, this.Name);
                    elements.Add(elInfo);

                    // let's get information about attributes
                    if (this.HasAttributes && this.MoveToFirstAttribute())
                    {
                        var aInfo = new XmlNodeInfo(this.LineNumber, this.LinePosition, this.Name);
                        elInfo.Attributes[this.Name] = aInfo;

                        while (this.MoveToNextAttribute())
                        {
                            aInfo = new XmlNodeInfo(this.LineNumber, this.LinePosition, this.Name);
                            elInfo.Attributes[this.Name] = aInfo;
                        }

                        this.MoveToElement();
                    }
                }
            }
            return retVal;
        }
    }
}