using Meadow.Devices;
using Meadow.Hardware;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Meadow;

public class F7FileSystemInfo : IPlatformOS.FileSystemInfo
{
    private readonly List<IExternalStorage> _externalStorage = new();
    private readonly bool _sdSupported;

    /// <inheritdoc/>
    public override IEnumerable<IExternalStorage> ExternalStorage => _externalStorage;

    /// <inheritdoc/>
    public override string FileSystemRoot => "/meadow0/";

    internal F7FileSystemInfo(StorageCapabilities capabilities, bool sdSupported)
    {
        _sdSupported = sdSupported;

        if (capabilities.HasSd && _sdSupported)
        {
            Resolver.Log.Trace("Device is SD Card Capable");

            if (F7ExternalStorage.TryMount("/dev/mmcsd0", "/sdcard", out F7ExternalStorage store))
            {
                _externalStorage.Add(store);
            }

            if (Resolver.Device is F7CoreComputeBase ccm)
            {
                // thread an not interrupt because we don't want to consume int group 6 for this and speed isn't critical
                new Thread(() => SdMonitorProc(ccm)).Start();
            }
        }
        else if (!_sdSupported)
        {
            Resolver.Log.Trace("SD Card is configured to 'off'");
        }
        else
        {
            Resolver.Log.Trace("Device is not SD Card Capable");
        }
    }

    private void HandleInserted()
    {
        if (_externalStorage.Count == 0)
        {
            if (F7ExternalStorage.TryMount("/dev/mmcsd0", "/sdcard", out F7ExternalStorage? store))
            {
                if (store != null)
                {
                    _externalStorage.Add(store);
                    ExternalStorageEvent?.Invoke(store, ExternalStorageState.Inserted);
                }
            }
        }
    }

    private void HandleRemoved()
    {
        if (_externalStorage.Count > 0)
        {
            var store = _externalStorage.First();

            _externalStorage.Clear();
            ExternalStorageEvent?.Invoke(store, ExternalStorageState.Ejected);
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
                Resolver.Log.Debug($"SD State changed to {input.State}");

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
