using System;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// Represents an NTP (Network Time Protocol) client for Linux.
/// </summary>
public class LinuxNtpClient : INtpClient
{
    /// <inheritdoc/>
    public bool Enabled => false;

    /// <inheritdoc/>
    public TimeSpan PollPeriod { get; set; }

    /// <inheritdoc/>
    public event TimeChangedEventHandler? TimeChanged;

    /// <inheritdoc/>
    public Task<bool> Synchronize(string? ntpServer = null)
    {
        TimeChanged?.Invoke(DateTime.UtcNow);

        throw new NotImplementedException();
    }
}
