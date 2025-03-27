using Meadow.Hardware;
using Meadow.Units;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Pinouts;

/// <summary>
/// Represents an analog input port for BeagleBone
/// </summary>
public class BeagleBoneAnalogInputPort : IAnalogInputPort
{
    private readonly string _devicePath;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    /// <inheritdoc/>
    public Voltage ReferenceVoltage => 1.8.Volts();

    /// <inheritdoc/>
    public IAnalogChannelInfo Channel { get; }

    /// <inheritdoc/>
    public IPin Pin { get; }

    internal BeagleBoneAnalogInputPort(IPin pin, IAnalogChannelInfo channelInfo)
    {
        Pin = pin;
        Channel = channelInfo;

        // pin name is in the form AINx where x is the device number
        var deviceNumber = pin.Name.Last();
        _devicePath = $"/sys/bus/iio/devices/iio:device0/in_voltage{deviceNumber}_raw";
    }

    private Voltage GetChannelVoltage()
    {
        // cat /sys/bus/iio/devices/iio\:device0/in_voltage0_raw

        var rawText = File.ReadAllText(_devicePath);
        if (int.TryParse(rawText, out var raw))
        {
            return ((raw / 4096d) * ReferenceVoltage.Volts).Volts();
        }

        throw new Exception($"Unable to parse {rawText} to a voltage");
    }

    /// <inheritdoc/>
    public Task<Voltage> Read()
    {
        return Task.FromResult(GetChannelVoltage());
    }

    /// <inheritdoc/>
    public void Dispose()
    {
    }
}