using Meadow.Core;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Linq;
using static Meadow.Core.Interop;
using static Meadow.Core.Interop.Nuttx;

namespace Meadow.Devices;

internal class F7DigitalSignalAnalyzer : IDigitalSignalAnalyzer, IDisposable
{
    private Nuttx.AnalyzerConfig _analyzerConfig;
    private Nuttx.AnalyzerData _analyzerData = new();

    public bool IsDisposed { get; private set; }

    internal F7DigitalSignalAnalyzer(F7Pin pin, bool captureDutyCycle = true)
    {
        // these use the same pins as the PWMs, so we can just use the existing PWM info to extract port, pin, and timer info
        var pwmInfo = pin.SupportedChannels.FirstOrDefault(i => i is PwmChannelInfo) as PwmChannelInfo;

        if (pwmInfo == null)
        {
            Resolver.Log.Warn($"Pin {pin.Name} does not have Timer support.");
            throw new ArgumentException();
        }

        _analyzerConfig = new Interop.Nuttx.AnalyzerConfig
        {
            TimerNumber = (int)pwmInfo.Timer,
            ChannelNumber = (int)pwmInfo.TimerChannel,
            Port = (int)pin.ProcessorPort,
            Pin = pin.ProcessorPin,
            Type = captureDutyCycle ? Nuttx.AnalyzerType.WithDutyCycle : Nuttx.AnalyzerType.NoDutyCycle
        };

        var result = Nuttx.meadow_measure_freq_configure(ref _analyzerConfig);

        switch (result)
        {
            case Nuttx.AnalyzerCallStatus.ConfigureSuccess:
                break;
            default:
                Resolver.Log.Warn($"Configure analyzer pin returned {result}");
                throw new Exception();
        }
    }

    public double GetDutyCycle()
    {
        var data = ReadData();
        return data.DutyCycle1k / 1000d;
    }

    public Frequency GetFrequency()
    {
        var data = ReadData();
        return new Frequency(data.Frequency1k / 1000d, Frequency.UnitType.Hertz);
    }

    public Frequency GetMeanFrequency()
    {
        var data = ReadData();
        return new Frequency(data.AvgFrequency1k / 1000d, Frequency.UnitType.Hertz);
    }

    private AnalyzerData ReadData()
    {
        _analyzerData.TimerNumber = _analyzerConfig.TimerNumber;
        _analyzerData.ChannelNumber = _analyzerConfig.ChannelNumber;

        var result = Nuttx.meadow_measure_freq_return_freq_info(ref _analyzerData);

        switch (result)
        {
            case AnalyzerCallStatus.ReadSuccess:
                return _analyzerData;
            case AnalyzerCallStatus.FrequencyInputNotDetected:
                // no input signal - don't throw an exception, make sure data is just zero
                _analyzerData.Frequency1k = 0;
                _analyzerData.DutyCycle1k = 0;
                _analyzerData.AvgFrequency1k = 0;
                return _analyzerData;
            default:
                throw new Exception($"Analyzer read failed.  Returned {result} (T:{_analyzerData.TimerNumber} C:{_analyzerData.ChannelNumber} )");
        }
    }

    private void Unconfigure()
    {
        _analyzerConfig.Type = AnalyzerType.Unconfigure;
        var result = Nuttx.meadow_measure_freq_configure(ref _analyzerConfig);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            Unconfigure();
            IsDisposed = true;
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}