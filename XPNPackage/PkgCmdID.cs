// PkgCmdID.cs
// MUST match PkgCmdID.h
using System;

namespace TTRider.XPNPackage
{
    static class PkgCmdIDList
    {
        public const uint cmdidMyCommand =        0x100;

        public const uint cmdidXPathCombo = 0x0102;
    };

    public class CommandGuids
    {
        

        public const string guidXmlGroupString = "061317b2-f992-435e-a23d-9ead4b972ed5";

        public static System.Guid guidXmlGroup = new System.Guid(guidXmlGroupString);
    }

}