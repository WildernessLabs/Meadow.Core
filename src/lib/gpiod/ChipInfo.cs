using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal class ChipInfo : IDisposable
    {
        public IntPtr Handle { get; private set; }
        private Gpiod.Interop.gpiod_chip? Chip { get; }
        public string Name { get; } = string.Empty;
        public string Label { get; } = string.Empty;

        public LineCollection Lines { get; }

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
                Name = new string(Chip.Value.name).Trim('\0');
                Label = new string(Chip.Value.label).Trim('\0');

                // Init as an array of nulls.  We'll populate as they are accessed
                Lines = new LineCollection(this, (int)Chip.Value.num_lines);
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
