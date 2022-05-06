using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
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

        private bool _istIsRunning = false;
        private bool _istShouldStop = false;
        public event EventHandler InterruptOccurred = delegate { };

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

        public void RequestInput(line_request_flags flags)
        {
            // TODO: check for free?
            var result = gpiod_line_request_input_flags(Handle, MeadowConsumer, flags);

            if (result == -1)
            {
                throw new NativeException("Failed to request line", Marshal.GetLastWin32Error());
            }
        }

        public void SpawnIST(InterruptMode mode, line_request_flags flags)
        {
            if (_istIsRunning) return;

            _istShouldStop = false;

            switch (mode)
            {
                case InterruptMode.EdgeRising:
                    gpiod_line_request_rising_edge_events_flags(Handle, MeadowConsumer, flags);
                    break;
                case InterruptMode.EdgeFalling:
                    gpiod_line_request_falling_edge_events(Handle, MeadowConsumer);
                    break;
                case InterruptMode.EdgeBoth:
                    gpiod_line_request_both_edges_events(Handle, MeadowConsumer);
                    break;
                default:
                    return;
            }

            Task.Run(() => IST());
        }

        private void IST()
        {
            _istIsRunning = true;
            timespec timeout = new timespec { tv_sec = 1 }; // 1-second timeout
            gpiod_line_event evnt = new gpiod_line_event();

            while (!_istShouldStop)
            {
                var result = gpiod_line_event_wait(Handle, ref timeout);

                switch (result)
                {
                    case 0: // timeout
                        // nop, just wait again
                        break;
                    case 1: // event
                        Console.WriteLine($"INTERRUPT");
                        /*
                        result = gpiod_line_event_read(Handle, ref evnt);

                        if (result == 0)
                        {
                            Console.WriteLine($"INTERRUPT {evnt.event_type}");
                        }
                        else
                        {
                            Console.WriteLine($"READ INTERRUPT ERR {Marshal.GetLastWin32Error()}");
                        }
                        Thread.Sleep(500);
                        */
                        InterruptOccurred?.Invoke(this, EventArgs.Empty);
                        break;
                    case -1: // error
                    default: //undefined
                        throw new NativeException("Waiting for interrupt event failed", Marshal.GetLastWin32Error());
                        break;
                }

            }

            _istIsRunning = false;
        }

        public void Release()
        {
            _istShouldStop = true;
            gpiod_line_release(Handle);
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
