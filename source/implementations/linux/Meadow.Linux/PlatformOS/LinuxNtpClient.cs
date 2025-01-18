using System;

namespace Meadow;

/// <summary>
/// Represents an NTP (Network Time Protocol) client for Linux.
/// </summary>
public class LinuxNtpClient : NtpClientBase
{
    internal LinuxNtpClient()
    {
    }

    /// <inheritdoc/>
    public override bool Enabled => false;

    /// <inheritdoc/>
    public override TimeSpan PollPeriod { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
}
