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
            case Nuttx.AnalyzerCallStatus.ConfigureSUCCESSFUL:
                break;
            default:
                Resolver.Log.Warn($"Configure analyzer pin returned {result}");
                throw new Exception();
        }
    }

    public double GetDutyCycle()
    {
        var result = Nuttx.meadow_measure_freq_return_freq_info(ref _analyzerData);

        if (result == AnalyzerCallStatus.ReadSUCCESSFUL)
        {
            return _analyzerData.DutyCycle1k / 1000d;
        }

        throw new Exception($"Analyzer read failed.  Returned {result}");
    }

    public Frequency GetFrequency()
    {
        var result = Nuttx.meadow_measure_freq_return_freq_info(ref _analyzerData);

        if (result == AnalyzerCallStatus.ReadSUCCESSFUL)
        {
            return new Frequency(_analyzerData.Frequency1k / 1000d, Frequency.UnitType.Hertz);
        }

        throw new Exception($"Analyzer read failed.  Returned {result}");
    }

    public Frequency GetMeanFrequency()
    {
        var result = Nuttx.meadow_measure_freq_return_freq_info(ref _analyzerData);

        if (result == AnalyzerCallStatus.ReadSUCCESSFUL)
        {
            return new Frequency(_analyzerData.AvgFrequency1k / 1000d, Frequency.UnitType.Hertz);
        }

        throw new Exception($"Analyzer read failed.  Returned {result}");
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