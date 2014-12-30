// Guids.cs
// MUST match guids.h
using System;

namespace TTRider.XPNPackage
{
    static class GuidList
    {
        public const string guidXPNPackagePkgString = "776f6dc9-8d69-403e-9145-cc86b96dd39c";
        public const string guidXPNPackageCmdSetString = "68533051-270b-4c41-bf90-79a1e1e64edd";

        public static readonly Guid guidXPNPackageCmdSet = new Guid(guidXPNPackageCmdSetString);
    };
}