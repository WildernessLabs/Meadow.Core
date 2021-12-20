using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;

namespace Meadow
{
    internal class Gpiod : IDisposable
    {
        private class PinInfo
        {
            public int FileDescriptor { get; set; }
            public int ReferenceCount { get; set; }
        }

        public bool IsDisposed { get; private set; }
        private int DeviceHandle { get; set; }

        private const string DeviceName = "/dev/gpiochip0";

        public Gpiod()
        {
            DeviceHandle = Interop.open(DeviceName, Interop.DriverFlags.O_RDONLY);
            if (DeviceHandle < 0)
            {
                throw new NativeException($"Unable to open device '{DeviceName}'");
            }

            // get pin count, generate an array of Infos
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!IsDisposed)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                if (DeviceHandle != 0)
                {
                    Interop.close(DeviceHandle);
                    DeviceHandle = 0    ;
                }

                IsDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        /*
            // TODO:
            #define GPIO_GET_CHIPINFO_IOCTL _IOR(0xB4, 0x01, struct gpiochip_info)
            #define GPIO_GET_LINEINFO_IOCTL _IOWR(0xB4, 0x02, struct gpioline_info)
        */

        private unsafe int IoctlGetLineHandle(int fileDescriptor, GpioHandleRequest request)
        {
            // #define GPIO_GET_LINEEVENT_IOCTL _IOWR(0xB4, 0x04, struct gpioevent_request)    
            return Interop.ioctl(fileDescriptor, Interop._IOWR(0xb4, 0x04, Marshal.SizeOf(request)), (byte*)&request);
        }

        private unsafe int IoctlGetLineEvent(int fileDescriptor, GpioEventRequest request)
        {
            // #define GPIO_GET_LINEHANDLE_IOCTL _IOWR(0xB4, 0x03, struct gpiohandle_request)
            return Interop.ioctl(fileDescriptor, Interop._IOWR(0xb4, 0x03, Marshal.SizeOf(request)), (byte*)&request);
        }

        private unsafe int IoctlGetLineValues(int fileDescriptor, GpioHandleData data)
        {
            // #define GPIOHANDLE_GET_LINE_VALUES_IOCTL _IOWR(0xB4, 0x08, struct gpiohandle_data)
            return Interop.ioctl(fileDescriptor, Interop._IOWR(0xb4, 0x08, Marshal.SizeOf(data)), (byte*)&data);
        }

        private unsafe int IoctlSetLineValues(int fileDescriptor, GpioHandleData data)
        {
            // #define GPIOHANDLE_SET_LINE_VALUES_IOCTL _IOWR(0xB4, 0x09, struct gpiohandle_data)
            return Interop.ioctl(fileDescriptor, Interop._IOWR(0xb4, 0x09, Marshal.SizeOf(data)), (byte*)&data);
        }

        public unsafe void SetConfiguration(int gpio, LineConfiguration configuration)
        {
            // get a line handle
            var request = new GpioHandleRequest();
            request.LineOffsets[0] = gpio;
            request.Lines = 1;
            request.Flags = (int)configuration;

            var result = IoctlGetLineHandle(DeviceHandle, request);
            if (result == -1)
            {
                throw new NativeException($"Unable to get an IOCTL request handle");
            }
        }

        public unsafe void SetValue(int gpio, bool value)
        {
            // get a line handle, configured as output
            var request = new GpioHandleRequest();
            request.LineOffsets[0] = gpio;
            request.Lines = 1;
            request.Flags = (int)LineConfiguration.Output;

            var result = IoctlGetLineHandle(DeviceHandle, request);
            if (result == -1)
            {
                throw new NativeException($"Unable to get an IOCTL request handle");
            }

            var state = new GpioHandleData();
            state.Values[0] = (byte)(value ? 1 : 0);

            result = IoctlSetLineValues(request.FileDescriptor, state);
            Interop.close(request.FileDescriptor);
            if (result == -1)
            {
                throw new NativeException($"Unable to set IOCTL data");
            }
        }

        public unsafe bool GetValue(int gpio)
        {
            // get a line handle, configured as output
            var request = new GpioHandleRequest();
            request.LineOffsets[0] = gpio;
            request.Lines = 1;
            request.Flags = (int)LineConfiguration.Output;

            var result = IoctlGetLineHandle(DeviceHandle, request);
            if (result == -1)
            {
                throw new NativeException($"Unable to get an IOCTL request handle");
            }

            var state = new GpioHandleData();

            result = IoctlSetLineValues(request.FileDescriptor, state);
            Interop.close(request.FileDescriptor);
            if (result == -1)
            {
                throw new NativeException($"Unable to set IOCTL data");
            }
            return state.Values[0] != 0;
        }

        public unsafe void WaitForEdge(int gpio, GpioEdge edge)
        {
            var request = new GpioEventRequest
            {
                LineOffset = gpio,
                EventFlags = (int)edge
            };

            var result = IoctlGetLineEvent(DeviceHandle, request);
            if (result == -1)
            {
                throw new NativeException($"Unable to get an IOCTL event handle");
            }
            var pfd = new Interop.pollfd
            {
                fd = request.FileDescriptor,
                events = Interop.PollEvent.POLLIN
            };

            // TODO: we should probably *not* wait infinite (-1) here
            result = Interop.poll(ref pfd, 1, -1);
            if (result == -1)
            {
                throw new NativeException($"Unable to poll event handle");
            }
            if ((pfd.revents & Interop.PollEvent.POLLIN) != Interop.PollEvent.NONE)
            {
                // edge detected
            }

            Interop.close(request.FileDescriptor);
        }
    }
}
