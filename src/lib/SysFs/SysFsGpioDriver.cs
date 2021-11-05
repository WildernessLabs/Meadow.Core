using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
    internal class DeviceBusyException : Exception
    {
    }

    internal enum LinuxErrorCode
    {
        PermissionDenied = 13,
        DeviceBusy = 16
    }

    // error codes:
    // 13 = permission denied
    // 16 = device busy
    internal class SysFsGpioDriver
    {
        private const string GpioFolder = "/sys/class/gpio";
        private const int InterruptCheckPeriodMs = 1000;

        private readonly byte[] GPIO_IN = new byte[] { 0x69, 0x6e, 0x00 }; // the string "in"
        private readonly byte[] GPIO_OUT = new byte[] { 0x6f, 0x75, 0x74, 0x00 }; // the string "out"
        private readonly byte[] GPIO_HIGH = new byte[] { 0x31, 0x00 }; // the string "1"
        private readonly byte[] GPIO_LOW = new byte[] { 0x30, 0x00 }; // the string "0"

        private Dictionary<int, CancellationTokenSource> _cancelTokens = new();

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
                var ec = (LinuxErrorCode)Marshal.GetLastWin32Error();
                switch (ec)
                {
                    case LinuxErrorCode.PermissionDenied:
                        break;
                }
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {ec})");
            }

            try
            {
                var content = Encoding.ASCII.GetBytes($"{gpio}");
                var result = Interop.write(handle, content, content.Length);
                if (result < 0)
                {
                    var ec = (LinuxErrorCode)Marshal.GetLastWin32Error();
                    switch (ec)
                    {
                        case LinuxErrorCode.DeviceBusy:
                            throw new DeviceBusyException();
                            // DEV NOTE: if a port was not properly disposed, it might still already be exported
                            // Should we unexport it and retry?  It could be a bad thing if another app is using the port...
                    }
                    throw new NativeException($"Unable to export GPIO {gpio} (error code {ec})");
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

        private void SetEdge(int gpio, InterruptMode mode)
        {
            var path = $"{GpioFolder}/gpio{gpio}/edge";

            var handle = Interop.open(path, Interop.DriverFlags.O_WRONLY);
            if (handle < 0)
            {
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
            }
            try
            {
                byte[] content;
                switch(mode)
                {
                    case InterruptMode.EdgeBoth:
                        content = Encoding.ASCII.GetBytes("both\0");
                        break;
                    case InterruptMode.EdgeRising:
                        content = Encoding.ASCII.GetBytes("rising\0");
                        break;
                    case InterruptMode.EdgeFalling:
                        content = Encoding.ASCII.GetBytes("falling\0");
                        break;
                    default:
                        content = Encoding.ASCII.GetBytes("none\0");
                        break;
                }

                var result = Interop.write(handle, content, content.Length);
                if (result < 0)
                {
                    throw new NativeException($"Unable to set interrupt edge for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
                }
            }
            finally
            {
                Interop.close(handle);
            }
        }

        public void HookInterrupt(int gpio, InterruptMode mode, Action callback)
        {
            // TODO: we need to ensure the interrupt isn't hooked twice
            SetEdge(gpio, mode);

            var path = $"{GpioFolder}/gpio{gpio}/value";
            var handle = Interop.open(path, Interop.DriverFlags.O_RDONLY | Interop.DriverFlags.O_NONBLOCK);
            if (handle < 0)
            {
                throw new NativeException($"Unable to get handle for GPIO {gpio} (error code {Marshal.GetLastWin32Error()})");
            }

            var tokenSource = new CancellationTokenSource();

            _cancelTokens.Add(gpio, tokenSource);

            Task.Run(() =>
            {
                InterruptProc(gpio, handle, tokenSource.Token, callback);
            });
        }

        public void UnhookInterrupt(int gpio)
        {
            if (_cancelTokens.ContainsKey(gpio))
            {
                _cancelTokens[gpio].Cancel();
                _cancelTokens.Remove(gpio);
            }
        }

        private void InterruptProc(int gpio, int handle, CancellationToken cancelToken, Action callback)
        {
            var readBuffer = new byte[64];
            var fdset = new Interop.pollfd();

            fdset.fd = handle;
            fdset.events = Interop.PollEvent.POLLPRI;
            fdset.revents = Interop.PollEvent.NONE;

            // some platforms (looking at you Jetson!) require we clear the IRQ with a read before we do anything
            _ = Interop.read(handle, readBuffer, readBuffer.Length);

            while (true)
            {
                var result = Interop.poll(ref fdset, 1, InterruptCheckPeriodMs);

                if (cancelToken.IsCancellationRequested)
                {
                    break;
                }

                if (result < 0)
                {
                    throw new NativeException($"Failed to poll for interrupt: {Marshal.GetLastWin32Error()}");
                }
                else if(result == 0)
                {
                    // timeout, NOP
                }
                else
                {
                    // check the received event
                    if ((fdset.revents & Interop.PollEvent.POLLPRI) != Interop.PollEvent.NONE)
                    {
                        // Interrupt Occurred!
                        callback?.Invoke();

                        // clear the IRQ with a read
                        Interop.lseek(handle, 0, Interop.SeekWhence.SEEK_SET);
                        _ = Interop.read(handle, readBuffer, readBuffer.Length);
                    }
                }
            }

            Interop.close(handle);
        }
    }
}
