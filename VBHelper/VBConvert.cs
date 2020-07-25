using System;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.X86;

namespace VBHelper
{
    public static class VBConvert
    {
        private unsafe static int* _lpSource;

        public unsafe static void Pin(int[] source)
        {
            var handle = GCHandle.Alloc(source, GCHandleType.Pinned);
            try
            {
                _lpSource = (int*)handle.AddrOfPinnedObject().ToPointer();
            }
            finally
            {
                handle.Free();
            }
        }

        public unsafe static Vector128<int> Inc(int i)
            => Sse2.LoadVector128(_lpSource + i);
    }
}
