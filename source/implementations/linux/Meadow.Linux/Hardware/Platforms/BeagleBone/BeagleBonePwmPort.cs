using Meadow.Hardware;
using Meadow.Units;
using System;
using System.IO;

namespace Meadow.Pinouts;

/// <summary>
/// Represents a PWM output port for BeagleBone
/// </summary>
public class BeagleBonePwmPort : PwmPortBase
{
    // https://www.kernel.org/doc/html/v5.10/driver-api/pwm.html

    private readonly string _devicePath;
    private readonly string _enablePath;
    private readonly string _periodPath;
    private readonly string _polarityPath;
    private readonly string _dutyCyclePath;

    private const string PolarityNormal = "normal";
    private const string PolarityInverted = "inversed";

    /// <inheritdoc/>
    public override TimePeriod Duration { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    /// <inheritdoc/>
    public override bool State => ReadFileValue(_enablePath) != 0;

    internal BeagleBonePwmPort(IPin pin, IPwmChannelInfo channel, Frequency frequency, float dutyCycle, bool inverted)
        : base(pin, channel)
    {
        _devicePath = $"/sys/class/pwm/pwmchip{channel.Timer}/pwm{channel.TimerChannel}";
        _enablePath = Path.Combine(_devicePath, "enable");
        _periodPath = Path.Combine(_devicePath, "period");
        _polarityPath = Path.Combine(_devicePath, "polarity");
        _dutyCyclePath = Path.Combine(_devicePath, "duty_cycle");

        Frequency = frequency;
        DutyCycle = dutyCycle;
        Inverted = inverted;
    }

    /// <inheritdoc/>
    public override bool Inverted
    {
        get => File.ReadAllText(_polarityPath) == PolarityInverted;
        set
        {
            if (_polarityPath == null) return;
            File.WriteAllText(_polarityPath, value ? PolarityInverted : PolarityNormal);
        }
    }

    /// <inheritdoc/>
    public override Frequency Frequency
    {
        get => (1.0 / Period.Seconds).Hertz();
        set => Period = TimePeriod.FromSeconds(1 / value.Hertz);
    }

    /// <inheritdoc/>
    public override double DutyCycle
    {
        get => ReadFileValue(_dutyCyclePath) / (Period.Nanoseconds);
        set
        {
            SetPeriodAndDuty(Period, value);
        }
    }

    /// <inheritdoc/>
    public override TimePeriod Period
    {
        get => TimePeriod.FromMicroseconds(ReadFileValue(_periodPath) / 1000d);
        set
        {
            SetPeriodAndDuty(value, DutyCycle);
        }
    }

    private void SetPeriodAndDuty(TimePeriod period, double duty)
    {
        if (_periodPath == null || _dutyCyclePath == null) return;

        if (duty < 0) duty = 0;
        if (duty > 1.0) duty = 1.0;

        var dutyNs = (long)(period.Nanoseconds * duty);

        File.WriteAllText(_periodPath, ((long)period.Nanoseconds).ToString());
        File.WriteAllText(_dutyCyclePath, dutyNs.ToString());
    }

    private int ReadFileValue(string path)
    {
        return int.Parse(File.ReadAllText(path));
    }

    /// <inheritdoc/>
    public override void Start()
    {
        File.WriteAllText(_enablePath, "1");
    }

    /// <inheritdoc/>
    public override void Stop()
    {
        File.WriteAllText(_enablePath, "0");
    }
}

// 0 => 1
// 1 => 1
// 2 => 1
// 3 => 2
// 5 => 2
// 7 => 2
/*
ADDRESS         CONTROLLER      SYSFS           POSSIBLE PINS
-------         ----------      -----           -------------
0x48302200      EHRPWM1A        pwmchip0-0      P9_14, P8_36
0x48302200      EHRPWM1B        pwmchip0-1      P9_16, P8_34
0x48300200      EHRPWM0A        pwmchip1-0      P9_22, P9_31
0x48300200      EHRPWM0B        pwmchip1-1      P9_21, P9_29
0x48300100      ECAPPWM0        pwmchip3-0      P9_42
0x48304180      ECAPPWM2        pwmchip3-1      P9_28
0x48304180      EHRPWM2A        pwmchip6-0      P8_19, P8_45
0x48304180      EHRPWM2B        pwmchip6-1      P8_13, P8_46
*/
