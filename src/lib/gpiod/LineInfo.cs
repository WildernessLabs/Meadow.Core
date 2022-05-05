using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;
using static Meadow.Gpiod.Interop;

namespace Meadow
{
    internal class LineInfo
    {
        private IntPtr Handle { get; set; }
        private gpiod_line? Line { get; set; }
        public string Name { get; private set; } = string.Empty;
        public string Consumer { get; private set; } = string.Empty;
        public int Offset { get; }
        public line_direction Direction { get; private set; }
        public line_active_state ActiveState { get; private set; }

        public bool IsInvalid => Handle.ToInt64() <= 0;

        public LineInfo(ChipInfo chip, int offset)
        {
            Offset = offset;
            Handle = gpiod_chip_get_line(chip.Handle, Offset);

            if (!IsInvalid)
            {
                UpdatePropsFromHandle();
            }
        }

        private void UpdatePropsFromHandle()
        {
            Line = Marshal.PtrToStructure<gpiod_line>(Handle);
            Line = Marshal.PtrToStructure<gpiod_line>(Handle);
            Name = new string(Line.Value.name).Trim('\0');
            Consumer = new string(Line.Value.consumer).Trim('\0');
            Direction = Line.Value.direction;
            ActiveState = Line.Value.active_state;
        }

        public void Update()
        {
            gpiod_line_update(Handle);
            UpdatePropsFromHandle();
        }

        private const string MeadowConsumer = "Meadow";

        public void Request(line_direction direction)
        {
            // TODO: check for free?
            int result;

            if (direction == line_direction.GPIOD_LINE_DIRECTION_INPUT)
            {
                result = gpiod_line_request_input(Handle, MeadowConsumer);
            }
            else
            {
                result = gpiod_line_request_output(Handle, MeadowConsumer);
            }

            if (result == -1)
            {
                throw new NativeException("Failed to request line", Marshal.GetLastWin32Error());
            }
        }

        public void SetValue(bool state)
        {
            var result = gpiod_line_set_value(Handle, state ? 1 : 0);

            if (result == -1)
            {
                throw new NativeException("Failed to set line value", Marshal.GetLastWin32Error());
            }
        }

        public bool GetValue()
        {
            var result = gpiod_line_get_value(Handle);

            if (result == -1)
            {
                throw new NativeException("Failed to set line value", Marshal.GetLastWin32Error());
            }

            return result == 1;
        }

        public override string ToString()
        {
            // same format as gpioinfo
            // gpiochip0 [pinctrl-bcm2711] (58 lines)
            var c = string.IsNullOrEmpty(Consumer) ? "unused" : $"\"{Consumer}\"";
            var n = $"\"{Name}\"";
            var d = Direction == line_direction.GPIOD_LINE_DIRECTION_INPUT ? " input" : "output";
            var s = ActiveState == line_active_state.GPIOD_LINE_ACTIVE_STATE_LOW ? "active-low" : "active-high";

            return $"line {Offset:00}: {n,16}{c,16}  {d}  {s}";
        }
    }
}
