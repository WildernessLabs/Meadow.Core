using Meadow.Logging;
using System;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal class ChipInfo : IDisposable
    {
        private Logger Logger { get; }
        public IntPtr Handle { get; private set; }
        private Gpiod.Interop.gpiod_chip? Chip { get; }
        public string Name { get; } = string.Empty;
        public string Label { get; } = string.Empty;

        public LineCollection Lines { get; }

        public bool IsInvalid => Handle.ToInt64() <= 0;

        public static ChipInfo FromIntPtr(Logger logger, IntPtr p)
        {
            return new ChipInfo(logger, p);
        }

        private ChipInfo(Logger logger, IntPtr p)
        {
            Logger = logger;

            Handle = p;

            if (IsInvalid)
            {
                Logger.Debug($"Chip ptr is invalid - cannot get GPIOD chip details");
            }
            else
            {
                Chip = Marshal.PtrToStructure<Gpiod.Interop.gpiod_chip>(Handle);
                Name = Chip.Value.name;
                Label = Chip.Value.label;

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
            return $"{Name} [{Label}] ({Lines.Count} lines)";
        }
    }
}
