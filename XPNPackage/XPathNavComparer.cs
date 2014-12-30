using System.Collections.Generic;

namespace TTRider.XPNPackage
{
    class XPathNavComparer : IEqualityComparer<System.Xml.XPath.XPathNavigator>
    {

        #region IEqualityComparer<XPathNavigator> Members

        public bool Equals(System.Xml.XPath.XPathNavigator x, System.Xml.XPath.XPathNavigator y)
        {
            return System.Xml.XPath.XPathNavigator.NavigatorComparer.Equals(x, y);
        }

        public int GetHashCode(System.Xml.XPath.XPathNavigator obj)
        {
            return System.Xml.XPath.XPathNavigator.NavigatorComparer.GetHashCode(obj);
        }

        #endregion
    }
}