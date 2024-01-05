using Meadow.Devices;
using Meadow.Hardware;
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
        public event PowerTransitionHandler BeforeReset = default!;
        /// <summary>
        /// Event called before Sleep mode
        /// </summary>
        public event PowerTransitionHandler BeforeSleep = default!;
        /// <summary>
        /// Event called after returning from Sleep mode
        /// </summary>
        public event PowerTransitionHandler AfterWake = default!;

        /// <summary>
        /// Resets the device
        /// </summary>
        public void Reset()
        {
            Resolver.Log.Trace("Resetting device...");

            BeforeReset?.Invoke();

            UPD.Ioctl(Nuttx.UpdIoctlFn.PowerReset);
        }

        internal const int MaxSleepSeconds = 0x24E9FF; // max of (28days - 1sec) == (28 * 24 * 60 * 60) - 1

        /// <summary>
        /// Put the device into low-power (sleep) mode for the specified amount of time.
        /// </summary>
        /// <param name="duration">Amount of time to sleep</param>
        /// <remarks>duration has a resolution of 1 second and must be between 1 and 0x24E9FF, inclusive.</remarks>
        public void Sleep(TimeSpan duration)
        {
            var seconds = (int)duration.TotalSeconds;

            if (seconds <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, "duration must be > 0 seconds");
            }
            if (seconds > MaxSleepSeconds)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, $"duration must be <= {MaxSleepSeconds} seconds");
            }

            var cmd = new UpdSleepCommand
            {
                SecondsToSleep = seconds
            };

            Resolver.Log.Trace($"Device sleeping for {duration.TotalSeconds}s...");

            DoSleepNotifications();

            // This should suspend the processor and code should stop executing
            UPD.Ioctl(UpdIoctlFn.PowerSleep, cmd);

            // Stop execution while the device actually does it's thing
            Thread.Sleep(100);

            // TODO: see how long this actually takes

            DoWakeNotifications(WakeSource.Timer);
        }

        private void DoSleepNotifications()
        {
            lock (_sleepAwarePeripherals)
            {
                foreach (var p in _sleepAwarePeripherals)
                {
                    p.BeforeSleep(Resolver.App.CancellationToken).Wait();
                }
            }

            BeforeSleep?.Invoke();
        }

        private void DoWakeNotifications(WakeSource source)
        {
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

        /// <inheritdoc/>
        public void Sleep(
            IPin interruptPin,
            InterruptMode interruptMode,
            ResistorMode resistorMode = ResistorMode.Disabled)
        {
            // TODO: if the device is already using the interrupt port, we need to record that to re-enable on wake
            var existingConfig = _ioController.GetConfiguredInterruptMode(interruptPin);

            if (existingConfig != null)
            {
                // disconnect any existing interrupt
                _ioController.WireInterrupt(interruptPin, InterruptMode.None, ResistorMode.Disabled, TimeSpan.Zero, TimeSpan.Zero);
            }

            // configure the wake interrupt
            _ioController.ConfigureWakeInterrupt(interruptPin, interruptMode, resistorMode);

            var cmd = new UpdSleepCommand
            {
                SecondsToSleep = MaxSleepSeconds
            };
            DoSleepNotifications();

            // This suspends the processor and code stops executing
            UPD.Ioctl(UpdIoctlFn.PowerSleep, cmd);

            // Stop app execution while the OS actually does it's thing
            // EXECUTION HALTS ON THIS SLEEP CALL UNTIL WAKE
            Thread.Sleep(100);

            if (existingConfig != null)
            {
                var existingInterruptMode = InterruptMode.None;

                if (existingConfig.Value.RisingEdge != 0)
                {
                    if (existingConfig.Value.FallingEdge != 0)
                    {
                        existingInterruptMode = InterruptMode.EdgeBoth;
                    }
                    else
                    {
                        existingInterruptMode = InterruptMode.EdgeRising;
                    }
                }
                else if (existingConfig.Value.FallingEdge != 0)
                {
                    existingInterruptMode = InterruptMode.EdgeFalling;
                }

                var existingResistorMode = existingConfig.Value.ResistorMode switch
                {
                    STM32.ResistorMode.PullUp => ResistorMode.InternalPullUp,
                    STM32.ResistorMode.PullDown => ResistorMode.InternalPullDown,
                    _ => ResistorMode.Disabled
                };

                // re-configure interrupt
                _ioController.WireInterrupt(
                    interruptPin,
                    existingInterruptMode,
                    existingResistorMode,
                    TimeSpan.FromMilliseconds(existingConfig.Value.DebounceDuration / 10d),
                    TimeSpan.FromMilliseconds(existingConfig.Value.GlitchDuration / 10d));
            }

            DoWakeNotifications(WakeSource.Interrupt); // this isn't necessarily true. TODO: determine why we woke
        }
    }
}
