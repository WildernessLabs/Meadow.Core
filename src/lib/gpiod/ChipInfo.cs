using System;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal class LineInfo
    {
    }

    internal class ChipInfo : IDisposable
    {
        private IntPtr Handle { get; set; }
        private Gpiod.Interop.gpiod_chip? Chip { get; }
        public string Name { get; } = string.Empty;
        public string Label { get; } = string.Empty;

        public int Lines { get; } = 0;

        public bool IsInvalid => Handle.ToInt64() <= 0;

        public static ChipInfo FromIntPtr(IntPtr p)
        {
            return new ChipInfo(p);
        }

        private ChipInfo(IntPtr p)
        {
            Handle = p;

            if (!IsInvalid)
            {
                Chip = Marshal.PtrToStructure<Gpiod.Interop.gpiod_chip>(Handle);
                Name = new string(Chip.Value.name);
                Label = new string(Chip.Value.label);
                Lines = (int)Chip.Value.num_lines;
            }
        }

        public void Dispose()
        {
            if (IsInvalid) return;

            Gpiod.Interop.gpiod_chip_close(Handle);
            Handle = IntPtr.Zero;
        }

        public override string ToString()
        {
            // same format as gpiodetect
            // gpiochip0 [pinctrl-bcm2711] (58 lines)
            return $"{Name} [{Label}] ({Lines} lines)";
        }
    }
}
