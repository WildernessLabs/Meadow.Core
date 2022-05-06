using Meadow.Hardware;
using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Meadow
{
    internal partial class Gpiod : IDisposable
    {
        
        private class PinInfo
        {
            public int FileDescriptor { get; set; }
            public int ReferenceCount { get; set; }
        }

        public bool IsDisposed { get; private set; }

        private ChipCollection Chips { get; set; }
        private Logger Logger { get; }

        private string[] DeviceNames = new string[] { "gpiochip0", "gpiochip1" };

        public unsafe Gpiod(Logger logger)
        {
            Chips = new ChipCollection();
            Logger = logger;

            // TODO: query for list of chips
            foreach (var n in DeviceNames)
            {
                var info = ChipInfo.FromIntPtr(Interop.gpiod_chip_open_by_name(n));
                if (!info.IsInvalid)
                {
                    Chips.Add(info);

                    Logger.Debug(info.ToString());

                    foreach (var line in info.Lines)
                    {
                        Logger.Debug(line.ToString());
                    }
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                foreach(var chip in Chips)
                {
                    chip.Dispose();
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public LineInfo GetLine(GpiodPin pin)
        {
            var line = Chips[pin.Chip].Lines[pin.Offset];

            // TODO: check availability, check for other reservations

            return line;
        }
    }
}
