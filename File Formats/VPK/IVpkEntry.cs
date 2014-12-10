using System;
namespace Doto_Unlocker.VPK
{
    interface IVpkEntry
    {
        string Ext { get; }
        string Name { get; }
        string Path { get; }
    }
}
