using System;

namespace Meadow;

/// <summary>
/// Client for Network Time Protocol
/// </summary>
public class NtpClient : NtpClientBase
{
    /// <summary>
    /// Returns <c>true</c> if the NTP Client is enabled
    /// </summary>
    public override bool Enabled => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.GetTimeAtStartup);

    internal NtpClient()
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
