using Meadow.Core;
using Meadow.Hardware;
using Meadow.Units;
using System;
using System.Linq;
using static Meadow.Core.Interop;

namespace Meadow.Devices;

internal class F7DigitalSignalAnalyzer : IDigitalSignalAnalyzer
{
    internal F7DigitalSignalAnalyzer(F7Pin pin)
    {
        // these use the same pins as the PWMs, so we can just use the existing PWM info to extract port, pin, and timer info
        var pwmInfo = pin.SupportedChannels.FirstOrDefault(i => i is PwmChannelInfo) as PwmChannelInfo;

        if (pwmInfo == null)
        {
            Resolver.Log.Warn($"Pin {pin.Name} does not have Timer support.");
            throw new ArgumentException();
        }

        var config = new Interop.Nuttx.AnalyzerConfig
        {
            TimerNumber = (int)pwmInfo.Timer,
            ChannelNumber = (int)pwmInfo.TimerChannel,
            Port = (int)pin.ProcessorPort,
            Pin = pin.ProcessorPin,
            Type = Nuttx.AnalyzerType.WithDutyCycle
        };

        var result = Nuttx.meadow_measure_freq_configure(ref config);

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
        throw new NotImplementedException();
    }

    public Frequency GetFrequency()
    {
        throw new NotImplementedException();
    }

    public Frequency GetMeanFrequency()
    {
        throw new NotImplementedException();
    }
}