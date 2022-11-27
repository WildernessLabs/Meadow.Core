using Meadow.Devices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Meadow
{
    public partial class F7PlatformOS : IPlatformOS
    {
        public event ExternalStorageEventHandler ExternalStorageEvent;

        private List<IExternalStorage> _externalStorage = new();

        public IEnumerable<IExternalStorage> ExternalStorage => _externalStorage;
        public string FileSystemRoot => "/meadow0/";

        private void InitializeStorage()
        {
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

        private void HandleInserted()
        {
            if (_externalStorage.Count == 0)
            {
                if (F7ExternalStorage.TryMount("/dev/mmcsd0", "/sdcard", out F7ExternalStorage? store))
                {
                    _externalStorage.Add(store);
                    ExternalStorageEvent?.Invoke(store, ExternalStorageState.Inserted);
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
            var input = Resolver.Device.CreateDigitalInputPort(ccm.Pins.SD_IN_L);
            var lastState = input.State;
            var firstRun = true;

            while (true)
            {
                if (firstRun || input.State != lastState)
                {
                    lastState = input.State;

                    if (lastState)
                    {
                        // inserted
                        HandleInserted();
                    }
                    else if (!firstRun)
                    {
                        // removed
                        HandleRemoved();
                    }
                }

                Thread.Sleep(1000); // slow - TODO: make configurable, maybe?
            }
        }
    }
}
