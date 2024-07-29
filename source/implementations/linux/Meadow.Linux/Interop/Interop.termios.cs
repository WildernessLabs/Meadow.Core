using System.Runtime.InteropServices;

namespace Meadow
{
    internal static partial class Interop
    {
        [StructLayout(LayoutKind.Explicit)]
#pragma warning disable CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
        public struct termios
#pragma warning restore CS8981 // The type name only contains lower-cased ascii characters. Such names may become reserved for the language.
        {
            [FieldOffset(0)]
            public InputFlags c_iflag;
            [FieldOffset(4)]
            public OutputFlags c_oflag;
            [FieldOffset(8)]
            public ControlFlags c_cflag;
            [FieldOffset(12)]
            public LocalFlags c_lflag;
            [FieldOffset(16)]
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
            public byte[] c_cc;
            [FieldOffset(36)] // I *think* this is the right offset (20 bytes of control characters)
            public int c_ispeed;
            [FieldOffset(40)]
            public int c_ospeed;
        };
    }
}
