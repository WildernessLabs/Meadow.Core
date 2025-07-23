using System;

namespace Meadow;

public class WindowsNtpClient : NtpClientBase
{
    /// <summary>
    /// Returns <c>true</c> if the NTP Client is enabled
    /// </summary>
    public override bool Enabled => true;

    internal WindowsNtpClient()
    { }

    /// <summary>
    /// Time period that the NTP client attempts to query the NTP time server(s)
    /// </summary>
    public override TimeSpan PollPeriod
    {
        get => TimeSpan.Zero; // currently only happens at startup
        set => throw new PlatformNotSupportedException("Changing NTP Poll Frequency not currently supported");
    }

    internal void RaiseTimeChanged()
    {
        RaiseTimeChanged(DateTime.UtcNow);
    }
}
