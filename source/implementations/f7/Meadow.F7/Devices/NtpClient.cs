﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using static Meadow.Logging.Logger;

namespace Meadow;

/// <summary>
/// Client for Network Time Protocol
/// </summary>
public class NtpClient : INtpClient
{
    /// <summary>
    /// Event raised when the device clock is adjusted by NTP
    /// </summary>
    public event TimeChangedEventHandler TimeChanged = default!;

    /// <summary>
    /// Returns <c>true</c> if the NTP Client is enabled
    /// </summary>
    public bool Enabled => F7PlatformOS.GetBoolean(IPlatformOS.ConfigurationValues.GetTimeAtStartup);

    internal NtpClient()
    { }

    /// <summary>
    /// Time period that the NTP client attempts to query the NTP time server(s)
    /// </summary>
    public TimeSpan PollPeriod
    {
        get => TimeSpan.Zero; // currently only happens at startup
        set => throw new PlatformNotSupportedException("Changing NTP Poll Frequency not currently supported");
    }

    internal void RaiseTimeChanged()
    {
        TimeChanged?.Invoke(DateTime.UtcNow);
    }

    private int _ntpLock = 1;

    /// <inheritdoc/>
    public Task<bool> Synchronize(string? ntpServer = null)
    {
        if (ntpServer == null)
        {
            if (Resolver.Device.PlatformOS.NtpServers.Length == 0)
            {
                ntpServer = "0.pool.ntp.org";
                Resolver.Log.Info($"No configured NTP servers. Defaulting to {ntpServer}", MessageGroup.Core);
            }
            else
            {
                ntpServer = Resolver.Device.PlatformOS.NtpServers[0];
            }
        }

        if (Interlocked.Exchange(ref _ntpLock, 0) == 1)
        {
            try
            {
                var m_ntpPacket = new byte[48];
                //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)
                m_ntpPacket[0] = 0x1B;

                UdpClient client = new UdpClient();
                client.Connect(ntpServer, 123);
                client.Send(m_ntpPacket, m_ntpPacket.Length);
                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = client.Receive(ref ep);

                // receive date data is at offset 32
                // Data is 64 bits - first 32 is seconds
                // it is not in an endian order, so we must rearrange
                byte[] endianSeconds = new byte[4];
                endianSeconds[0] = data[32 + 3];
                endianSeconds[1] = data[32 + 2];
                endianSeconds[2] = data[32 + 1];
                endianSeconds[3] = data[32 + 0];
                uint seconds = BitConverter.ToUInt32(endianSeconds, 0);

                // second 32 is fraction of a second
                endianSeconds[0] = data[32 + 7];
                endianSeconds[1] = data[32 + 6];
                endianSeconds[2] = data[32 + 5];
                endianSeconds[3] = data[32 + 4];

                uint fraction = BitConverter.ToUInt32(endianSeconds, 0);

                var s = double.Parse($"{seconds}.{fraction}");

                var dt = new DateTime(1900, 1, 1).AddSeconds(s);
                Resolver.Device.PlatformOS.SetClock(dt);
                TimeChanged?.Invoke(dt);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Failed to query NTP Server: '{ex.Message}'.", MessageGroup.Core);
                return Task.FromResult(false);
            }
            finally
            {
                Interlocked.Exchange(ref _ntpLock, 1);
            }

        }

        return Task.FromResult(false);
    }
}
