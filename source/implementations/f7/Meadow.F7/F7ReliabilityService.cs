using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.IO;
using static Meadow.Logging.Logger;

namespace Meadow.Devices;

/// <summary>
/// Provides F7-platform-specific implementations of common IReliabilityService functionality
/// </summary>
public class F7ReliabilityService : ReliabilityServiceBase
{
    /// <summary>
    /// The file name used for passing Platform OS messages to the Meadow stack
    /// </summary>
    public const string OsMessageFile = "meadow.log";

    internal F7ReliabilityService()
        : base()
    {
    }

    /// <inheritdoc/>
    protected override void ProcessSystemError(MeadowSystemErrorInfo errorInfo, out bool recommendReset)
    {
        var shouldReset = false;

        base.ProcessSystemError(errorInfo, out shouldReset);

        // some of these may necessitate restarting the device
        if (errorInfo is Esp32SystemErrorInfo espError)
        {
            if (!ErrorListenerIsAttached)
            {
                Resolver.Log.Warn($"The ESP32 has had an error ({espError.StatusCode}).", MessageGroup.Core);
            }

            switch (espError.StatusCode)
            {
                case StatusCodes.EspOutOfMemory:
                    shouldReset = true;
                    break;
                default:
                    // any ESP reset error code is also fatal
                    if (espError.StatusCode >= StatusCodes.EspReset && espError.StatusCode <= StatusCodes.EspResetSDIO)
                    {
                        shouldReset = true;
                    }
                    break;
            }
        }
        else if (errorInfo is MeadowCloudSystemErrorInfo cloudErrorInfo)
        {
            if (!ErrorListenerIsAttached)
            {
                Resolver.Log.Warn($"Meadow Cloud has had an error ({cloudErrorInfo.Exception}).", MessageGroup.Core);
            }

            shouldReset = true;
        }
        else
        {
            Resolver.Log.Info($"We've had a system error of type {errorInfo.Exception?.GetType().Name}: {errorInfo}", MessageGroup.Core);
        }

        recommendReset = shouldReset;
    }

    /// <summary>
    /// Called when the device boots after a crash
    /// </summary>
    public override void OnBootFromCrash()
    {
    }

    /// <inheritdoc/>
    public override ResetReason LastResetReason
    {
        get => (ResetReason)Core.Interop.Nuttx.meadow_os_reset_reason();
    }

    /// <inheritdoc/>
    public override int SystemResetCount
    {
        get => (int)Core.Interop.Nuttx.meadow_os_reset_cycle_count();
    }

    /// <inheritdoc/>
    public override int SystemPowerCycleCount
    {
        get => (int)Core.Interop.Nuttx.meadow_os_power_cycle_count();
    }

    /// <inheritdoc/>
    public override IEnumerable<PlatformOsMessage>? GetStartupMessages()
    {
        var list = new List<PlatformOsMessage>();

        // On the F7 platforms, Nuttx passes messages via a file named `meadow.log` which is tilde-delimited
        // sample data
        /*
        I~2023-02-21T09:05:29Z~Logging started
        E~2023-02-21T09:05:30Z~CONFIG: Load: libyaml: did not find expected key
        E~2023-02-21T09:05:30Z~CONFIG: Load: Backtrace:
        E~2023-02-21T09:05:30Z~CONFIG:   in mapping field: Interfaces
        E~2023-02-21T09:05:30Z~CONFIG:   in mapping field: Network
        E~2023-02-21T09:05:30Z~Error processing config file, using default config
        */

        var device = Resolver.Services.Get<IMeadowDevice>();
        if (device == null)
        {
            return base.GetStartupMessages();
        }

        var path = Path.Combine(device.PlatformOS.FileSystem.FileSystemRoot, OsMessageFile);

        if (File.Exists(path))
        {
            try
            {
                var contents = File.ReadAllText(path);
                var lines = contents.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                foreach (var line in lines)
                {
                    var fields = line.Split('~', StringSplitOptions.RemoveEmptyEntries);

                    if (fields.Length == 3)
                    {
                        var priority = fields[0] switch
                        {
                            "E" => MessagePriority.Error,
                            "W" => MessagePriority.Warning,
                            "I" => MessagePriority.Information,
                            _ => MessagePriority.Debug
                        };

                        var ts = DateTime.ParseExact(fields[1], "yyyy-MM-ddTHH:mm:ssZ", null);

                        list.Add(new PlatformOsMessage
                        {
                            Priority = priority,
                            Timestamp = ts,
                            Message = fields[2]
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Warn($"Unable to parse Platform OS messages from '{path}': {ex.Message}", MessageGroup.Core);
            }
        }

        return list;
    }
}
