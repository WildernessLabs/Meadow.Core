using Meadow.Hardware;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow
{
    // error codes:
    // 13 = permission denied
    // 16 = device busy
    internal class SysFsGpioDriver
    {
        private const string GpioFolder = "/sys/class/gpio";

        private readonly byte[] GPIO_IN = new byte[] { 0x69, 0x6e, 0x00 }; // the string "in"
        private readonly byte[] GPIO_OUT = new byte[] { 0x6f, 0x75, 0x74, 0x00 }; // the string "out"
        private readonly byte[] GPIO_HIGH = new byte[] { 0x31, 0x00 }; // the string "1"
        private readonly byte[] GPIO_LOW = new byte[] { 0x30, 0x00 }; // the string "0"

        public enum GpioDirection
        {
            Input,
            Output
        }

        public void Export(int gpio)
        {
            var path = $"{GpioFolder}/export";

            var handle = Interop.open(path, Interop.DriverFlags.O_WRONLY);
            if (handle < 0)
            {
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
            }

            try
            {
                // DEV NOTE: if a port was not properly disposed, it might still already be exported
                // Should we unexport it and retry?  It could be a bad thing if another app is using the port...

                var content = Encoding.ASCII.GetBytes($"{gpio}");
                var result = Interop.write(handle, content, content.Length);
                if (result < 0)
                {
                    // error 16 == DEVICE_BUSY, meaning it's already open
                    throw new NativeException($"Unable to export GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
                }
            }
            finally
            {
                Interop.close(handle);
            }
        }

        public void Unexport(int gpio)
        {
            var path = $"{GpioFolder}/unexport";

            var handle = Interop.open(path, Interop.DriverFlags.O_WRONLY);
            if (handle < 0)
            {
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
            }

            try
            {
                var content = Encoding.ASCII.GetBytes($"{gpio}");
                var result = Interop.write(handle, content, content.Length);
                if (result < 0)
                {
                    throw new NativeException($"Unable to export GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
                }
            }
            finally
            {
                Interop.close(handle);
            }
        }

        public void SetDirection(int gpio, GpioDirection direction)
        {
            var path = $"{GpioFolder}/gpio{gpio}/direction";

            var handle = Interop.open(path, Interop.DriverFlags.O_WRONLY);
            if(handle < 0)
            {
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
            }
            try
            {
                var content = direction == GpioDirection.Input ? GPIO_IN : GPIO_OUT;
                var result = Interop.write(handle, content, content.Length);
                if (result < 0)
                {
                    throw new NativeException($"Unable to write to GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
                }
            }
            finally
            {
                Interop.close(handle);
            }
        }

        public void SetValue(int gpio, bool value)
        {
            var path = $"{GpioFolder}/gpio{gpio}/value";

            var handle = Interop.open(path, Interop.DriverFlags.O_WRONLY);
            if (handle < 0)
            {
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
            }
            try
            {
                var content = value ? GPIO_HIGH : GPIO_LOW;
                var result = Interop.write(handle, content, content.Length);
                if (result < 0)
                {
                    throw new NativeException($"Unable to write to GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
                }
            }
            finally
            {
                Interop.close(handle);
            }
        }

        public unsafe bool GetValue(int gpio)
        {
            var path = $"{GpioFolder}/gpio{gpio}/value";

            var handle = Interop.open(path, Interop.DriverFlags.O_RDONLY);
            if (handle < 0)
            {
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
            }
            try
            {
                var buffer = stackalloc byte[1];
                var result = Interop.read(handle, buffer, 1);
                if (result < 0)
                {
                    throw new NativeException($"Unable to read from GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
                }
                return buffer[0] != '0';// 0x30;

            }
            finally
            {
                Interop.close(handle);
            }
        }

        // TODO: implement interrupts, which are really just a `poll` on the input handle
    }
}
