using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace TTRider.XPNPackage
{
    internal class XPathTextManager
    {
        readonly Dictionary<XPathNavigator, XmlNodeInfo> navigators = new Dictionary<XPathNavigator, XmlNodeInfo>(new XPathNavComparer());
        readonly XPathNavigator root;
        readonly XmlNamespaceManager namespaceManager;

        public XPathTextManager(TextReader reader, string prefixForDefault)
        {
            var lir = new XmlLineInfoReader(reader);
            var document = new XPathDocument(lir);
            this.root = document.CreateNavigator();
            var xmlNameTable = this.root.NameTable;
            if (xmlNameTable != null)
            {
                this.namespaceManager = new XmlNamespaceManager(xmlNameTable);
            }

            var iterator = this.root.SelectDescendants(XPathNodeType.Element, true);

            var i = 0;
            foreach (XPathNavigator item in iterator)
            {
                if (item != null)
                {
                    var namespacesInScope = item.GetNamespacesInScope(XmlNamespaceScope.All);
                    if (namespacesInScope != null)
                    {
                        foreach (var de in namespacesInScope)
                        {
                            if (string.IsNullOrEmpty(de.Key))
                            {
                                this.namespaceManager.AddNamespace(prefixForDefault, de.Value);
                                this.HasDefaultNamespace = true;
                            }
                            else
                            {
                                this.namespaceManager.AddNamespace(de.Key, de.Value);
                            }
                        }
                    }

                    var info = lir.Elements[i++];
                    info.Navigator = item;
                    this.navigators[item] = info;
                }
                if (i >= lir.Elements.Count)
                {
                    break;
                }
            }
        }

        T GetNodeInfo<T>(XPathNavigator item) where T : XmlNodeInfo
        {
            XmlNodeInfo info;
            if (this.navigators.TryGetValue(item, out info))
            {
                info.Navigator = item;
                return info as T;
            }
            return default(T);
        }

        internal IEnumerable<XmlNodeInfo> Select(string query)
        {
            XPathExpression exp = XPathExpression.Compile(query, this.namespaceManager);

            //var qb = exp.GetType().Assembly.GetType("MS.Internal.Xml.XPath.QueryBuilder");

            //var xpParserType = exp.GetType().Assembly.GetType("MS.Internal.Xml.XPath.XPathParser");

            //var mt = xpParserType.GetMethod("ParseXPathExpresion", BindingFlags.Public | BindingFlags.Static);
            //dynamic ast = mt.Invoke(null, new object[]{query});


            foreach (XPathNavigator item in this.root.Select(exp))
            {
                switch (item.NodeType)
                {
                    case XPathNodeType.Attribute:
                        {
                            var nav = item.CreateNavigator();
                            if (nav.MoveToParent())
                            {
                                var info = GetNodeInfo<XmlElementInfo>(nav);
                                if (info != null)
                                {
                                    XmlNodeInfo ainfo;
                                    if (info.Attributes.TryGetValue(item.Name, out ainfo))
                                    {
                                        ainfo.Navigator = item;
                                        yield return ainfo;
                                    }
                                }
                            }
                        }
                        break;

                    case XPathNodeType.Element:
                        {
                            var info = GetNodeInfo<XmlElementInfo>(item);
                            if (info != null)
                            {
                                info.Navigator = item;
                                yield return info;
                            }
                            break;
                        }
                }
            }
        }

        public bool HasDefaultNamespace { get; }
    }
}