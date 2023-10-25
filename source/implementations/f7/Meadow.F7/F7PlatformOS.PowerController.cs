using Meadow.Devices;
using System;
using System.Collections.Generic;
using System.Threading;
using static Meadow.Core.Interop;
using static Meadow.Core.Interop.Nuttx;

namespace Meadow
{
    public partial class F7PlatformOS
    {
        private readonly List<ISleepAwarePeripheral> _sleepAwarePeripherals = new List<ISleepAwarePeripheral>();

        /// <summary>
        /// Event called before a software reset
        /// </summary>
        public event PowerTransitionHandler BeforeReset = delegate { };
        /// <summary>
        /// Event called before Sleep mode
        /// </summary>
        public event PowerTransitionHandler BeforeSleep = delegate { };
        /// <summary>
        /// Event called after returning from Sleep mode
        /// </summary>
        public event PowerTransitionHandler AfterWake = delegate { };

        /// <summary>
        /// Resets the device
        /// </summary>
        public void Reset()
        {
            Resolver.Log.Trace("Resetting device...");

            BeforeReset?.Invoke();

            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerReset);
        }

        /// <summary>
        /// Put the device into low-power (sleep) mode for the specified amount of time.
        /// </summary>
        /// <param name="duration">Amount of time to sleep</param>
        /// <remarks>duration has a resolution of 1 second and must be between 1 and 0xFFFF, inclusive.</remarks>
        public void Sleep(TimeSpan duration)
        {
            var seconds = (int)duration.TotalSeconds;

            if (seconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "duration must be > 0 seconds");
            }
            if (seconds > 0xffff)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "duration must be <= 0xffff seconds");
            }

            var cmd = new UpdSleepCommand
            {
                SecondsToSleep = seconds
            };

            Resolver.Log.Trace($"Device sleeping for {duration.TotalSeconds}...");

            lock (_sleepAwarePeripherals)
            {
                foreach (var p in _sleepAwarePeripherals)
                {
                    p.BeforeSleep(Resolver.App.CancellationToken).Wait();
                }
            }

            BeforeSleep?.Invoke();

            // This should suspend the processor and code should stop executing
            UPD.Ioctl(UpdIoctlFn.PowerSleep, cmd);

            // Stop execution while the device actually does it's thing
            Thread.Sleep(100);

            // TODO: see how long this actually takes

            Resolver.Log.Trace($"Device wake");

            AfterWake?.Invoke();

            lock (_sleepAwarePeripherals)
            {
                foreach (var p in _sleepAwarePeripherals)
                {
                    p.AfterWake(Resolver.App.CancellationToken).Wait();
                }
            }
        }

		/// <inheritdoc/>
		public void RegisterForSleep(ISleepAwarePeripheral peripheral)
        {
            lock (_sleepAwarePeripherals)
            {
                if (_sleepAwarePeripherals.Contains(peripheral))
                {
                    throw new ArgumentException("Peripheral already registered for sleep callbacks");
                }

                _sleepAwarePeripherals.Add(peripheral);
            }
        }
    }
}
