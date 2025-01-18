using Meadow.Devices;
using System.Collections.Generic;
using System.Threading;
using static Meadow.F7PlatformOS;
using static Meadow.Logging.Logger;

namespace Meadow;

/// <summary>
/// Provides a STM32F7-specific file system implementation for the Meadow
/// </summary>
public class F7FileSystemInfo : IPlatformOS.FileSystemInfo
{
    private readonly List<IStorageInformation> _drives = new();
    private F7ExternalStorage? _sdCard = default;
    private bool _isMounted = false;
    private readonly bool _sdSupported;

    /// <inheritdoc/>
    public override IEnumerable<IStorageInformation> Drives => _drives;

    /// <inheritdoc/>
    public override string FileSystemRoot => "/meadow0/";

    internal F7FileSystemInfo(StorageCapabilities capabilities, bool sdSupported)
    {
        _drives.Add(F7StorageInformation.Create(Resolver.Device));

        _sdSupported = sdSupported;

        if (capabilities.HasSd && _sdSupported)
        {
            Resolver.Log.Trace("Device is SD Card Capable", MessageGroup.Core);

            if (F7ExternalStorage.TryMount("/dev/mmcsd0", "/sdcard", out _sdCard))
            {
                _drives.Add(_sdCard);
                _isMounted = true;
            }
            else
            {
                _isMounted = false;
            }
            if (Resolver.Device is F7CoreComputeBase ccm)
            {
                // thread an not interrupt because we don't want to consume int group 6 for this and speed isn't critical
                new Thread(() => SdMonitorProc(ccm)).Start();
            }
        }
        else if (!_sdSupported)
        {
            Resolver.Log.Trace("SD Card is configured to 'off'", MessageGroup.Core);
        }
        else
        {
            Resolver.Log.Trace("Device is not SD Card Capable", MessageGroup.Core);
        }
    }

    private void HandleInserted()
    {
        if (!_isMounted)
        {
            if (F7ExternalStorage.TryMount("/dev/mmcsd0", "/sdcard", out _sdCard))
            {
                _drives.Add(_sdCard);
                RaiseExternalStorageEvent(_sdCard, ExternalStorageState.Inserted);
                _isMounted = true;
            }
        }
    }

    private void HandleRemoved()
    {
        if (_sdCard != null)
        {
            RaiseExternalStorageEvent(_sdCard, ExternalStorageState.Ejected);
            _drives.Remove(_sdCard);
            _isMounted = false;
            _sdCard = null;
        }
    }

    private void SdMonitorProc(F7CoreComputeBase ccm)
    {
        var input = ccm.Pins.PG6_SDMMC_IN_L.CreateDigitalInputPort();
        var lastState = input.State;
        var firstRun = true;

        while (true)
        {
            var currentState = input.State;

            if (firstRun || currentState != lastState)
            {
                Resolver.Log.Debug($"SD State changed to {input.State}", MessageGroup.Core);

                // DEV NOTE: The CCM SD Module uses inverse logic for card detect (high == no card, low == card)
                if (!currentState)
                {
                    // inserted
                    HandleInserted();
                }
                else if (!firstRun)
                {
                    // removed
                    HandleRemoved();
                }

                lastState = input.State;
            }

            firstRun = false;

            Thread.Sleep(1000); // slow - TODO: make configurable, maybe?
        }
    }
}
