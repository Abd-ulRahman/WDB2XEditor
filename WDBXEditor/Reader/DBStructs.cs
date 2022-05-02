using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

#pragma warning disable CS0649
#pragma warning disable IDE0044

namespace WDBXEditor.Commons
{
    public enum CompressionType
    {
        None = 0,
        Immediate = 1,
        Sparse = 2,
        Pallet = 3,
        PalletArray = 4,
        SignedImmediate = 5
    }
}
