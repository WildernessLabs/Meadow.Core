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
public class BeagleBoneAnalogInputPort : AnalogInputPortBase
{
    private readonly string _devicePath;
    private Task? _updateTask;
    private bool _isUpdating = false;
    private CancellationTokenSource _cancellationTokenSource = new();
    private CircularBuffer<Voltage> _buffer;

    internal BeagleBoneAnalogInputPort(BeagleBoneBlack controller, IPin pin, int sampleCount, TimeSpan sampleInterval)
        : base(pin, pin.SupportedChannels!.OfType<IAnalogChannelInfo>().First(), sampleCount, sampleInterval, 1.8.Volts())
    {
        // pin name is in the form AINx where x is the device number
        var deviceNumber = pin.Name.Last();
        _devicePath = $"/sys/bus/iio/devices/iio:device0/in_voltage{deviceNumber}_raw";
        _buffer = new CircularBuffer<Voltage>(sampleCount);
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
    public override Task<Voltage> Read()
    {
        if (_isUpdating)
        {
            return Task.FromResult(_buffer.Average(e => e.Volts).Volts());
        }

        return Task.FromResult(GetChannelVoltage());
    }

    /// <inheritdoc/>
    public override void StartUpdating(TimeSpan? updateInterval)
    {
        if (_isUpdating) return;

        _updateTask = Task.Run(async () =>
        {
            _isUpdating = true;
            _cancellationTokenSource.TryReset();

            while (!_cancellationTokenSource.IsCancellationRequested)
            {
                _buffer.Append(GetChannelVoltage());
                await Task.Delay(UpdateInterval);
            }
        }, _cancellationTokenSource.Token);
    }

    /// <inheritdoc/>
    public override void StopUpdating()
    {
        if (!_isUpdating) return;

        _cancellationTokenSource.Cancel();
    }
}