using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Meadow
{
    public partial class I2CBus
    {
        private class PeripheralMap : IDisposable
        {
            // the I2C block driver is...interesting.  You open it multiple times, once per peripheral address per bus
            // this is a container of bus, address, info
            private Dictionary<byte, I2CPeripheralInfo> _infoMap = new Dictionary<byte, I2CPeripheralInfo>();
            private bool _disposedValue;
            private string _driverName;

            public PeripheralMap(int busNumber)
            {
                _driverName = $"/dev/i2c-{busNumber}";
            }

            public bool SupportsIoctlExchange { get; private set; }

            public int GetAddressHandle(int busNumber, byte busAddress)
            {
                I2CPeripheralInfo info;

                if (_infoMap.ContainsKey(busAddress))
                {
                    info = _infoMap[busAddress];
                }
                else
                {
                    // open the i2c block driver
                    var handle = Interop.open(_driverName, Interop.DriverFlags.O_RDWR);
                    if (handle < 0)
                    {
                        // TODO: maybe try modprobe here?
                        throw new Exception($"Unable to open driver {_driverName}.  Is it enabled on your platform?");
                    }
                    info = new I2CPeripheralInfo
                    {
                        DriverHandle = handle,
                        BusAddress = busAddress
                    };
                    _infoMap.Add(busAddress, info);

                    // get the capabilities of the platform
                    uint caps = 0;
                    var result = Interop.ioctl(info.DriverHandle, (int)I2CIoctl.FUNCS, ref caps);
                    if (result < 0)
                    {
                        SupportsIoctlExchange = false;
                    }
                    else
                    {
                        SupportsIoctlExchange = (caps & 0x01) != 0;
                    }
                }

                if (!info.IsOpen)
                {
                    // call the ioctl to set the address for this handle
                    var result = Interop.ioctl(info.DriverHandle, (int)I2CIoctl.SLAVE, info.BusAddress);
                    if (result < 0)
                    {
                        Console.WriteLine($"ERROR: {Marshal.GetLastWin32Error()}");
                    }
                }

                return info.DriverHandle;
            }

            protected virtual void Dispose(bool disposing)
            {
                if (!_disposedValue)
                {
                    if (disposing)
                    {
                        foreach (var address in _infoMap.Values)
                        {
                            Interop.close(address.DriverHandle);
                        }
                    }

                    _disposedValue = true;
                }
            }

            public void Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }

        private class I2CPeripheralInfo
        {
            public byte BusAddress { get; set; }
            public int DriverHandle { get; set; }
            public bool IsOpen { get; set; }
        }
    }
}
