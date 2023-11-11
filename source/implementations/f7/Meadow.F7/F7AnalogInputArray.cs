using Meadow.Devices;
using Meadow.Hardware;
using System;
using System.Runtime.InteropServices;
using static Meadow.Core.Interop;

namespace Meadow;

/// <summary>
/// A high-speed IAnalogInputArray specific to the Meadow F7 platforms
/// </summary>
public class F7AnalogInputArray : IAnalogInputArray, IDisposable
{
    private readonly double[] _dataBuffer;
    private GCHandle _bufferHandle;
    private IPin[] _pins;
    private IMeadowIOController _controller;

    /// <summary>
    /// Returns <b>True</b> if the array had been Disposed, otherwise false.
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <inheritdoc/>
    public double[] CurrentValues => _dataBuffer;

    internal F7AnalogInputArray(IMeadowIOController ioController, params IPin[] pins)
    {
        _pins = pins;
        _controller = ioController;

        foreach (var pin in _pins)
        {
            // validate the pins
            if (pin.Controller is not F7FeatherBase)
            {
                throw new ArgumentException("Pins must be on the F7");
            }

            if (!pin.Supports<IAnalogChannelInfo>(c => c.InputCapable))
            {
                throw new ArgumentException($"Pin {pin.Name} is not analog input capable");
            }

            // reserve the pins
            ioController.DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.AnalogInput);

            ioController.ConfigureAnalogInput(pin);
        }

        // initialize the ADC
        /*
        meadow_adc_configure() has 3 arguments:
            1. A list containing 1 to 16 analog input GPIOs. 
                The list is populated with values such that PC7 would be 
                entered as 0x27 and PA4 would be 0x04.
            2. The number of GPIOs in the list (1 to 16).
            3. A pointer to a block of memory which is at least gpioCount 
                elements (doubles) in length where the 1 to 16 voltages can be copied.
        */
        var pinNumbers = new byte[pins.Length];

        for (var i = 0; i < pins.Length; i++)
        {
            pinNumbers[i] = GetPinIdentifier(pins[i].Key.ToString());
        }

        Resolver.Log.Info($"pin numbers: {BitConverter.ToString(pinNumbers)}");

        _dataBuffer = new double[pins.Length];

        // pin the buffer
        _bufferHandle = GCHandle.Alloc(_dataBuffer, GCHandleType.Pinned);

        try
        {
            var result = Nuttx.meadow_adc_configure(pinNumbers, pins.Length, _bufferHandle.AddrOfPinnedObject());

            Resolver.Log.Info($"config returned {result}");
        }
        catch (EntryPointNotFoundException)
        {
            throw new PlatformNotSupportedException("The current OS version on this device does not support high-speed ADCs. You must use a newer version (1.5 or later)");
        }
    }

    private byte GetPinIdentifier(string key)
    {
        // the name will be in the format of Pxyy where 'x' is a letter from A-I or so, and 'yy' is a number
        var id = (byte)(((key[1] - 'A') << 4) | int.Parse(key[2..]));

        return id;
    }

    /// <inheritdoc/>
    public void Refresh()
    {
        var result = Nuttx.meadow_adc_read_values();

        if (result < 0)
        {
            Resolver.Log.Info($"read returned {result}");
        }
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            Nuttx.meadow_adc_configure(Array.Empty<byte>(), 0, IntPtr.Zero);

            if (_bufferHandle.IsAllocated)
            {
                _bufferHandle.Free();
            }

            // return pins to the available pool and reconfigure from analog to high-z inputs
            foreach (var pin in _pins)
            {
                _controller.ConfigureInput(pin, ResistorMode.Disabled, InterruptMode.None, TimeSpan.Zero, TimeSpan.Zero);
                _controller.DeviceChannelManager.ReleasePin(pin);
            }
        }
    }
    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
