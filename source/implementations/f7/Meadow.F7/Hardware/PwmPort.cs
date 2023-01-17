using Meadow.Devices;
using Meadow.Units;
using System;
using System.Linq;

namespace Meadow.Hardware
{
    /// <summary>
    /// Represents a port that is capable of generating a Pulse-Width-Modulation
    /// signal; which approximates an analog output via digital pulses.
    /// </summary>
    public class PwmPort : PwmPortBase
    {
        private bool _isRunning = false;
        private Frequency _frequency;
        private float _dutyCycle;
        private bool _inverted;

        // dirty dirty hack
        internal bool IsOnboard { get; set; }

        protected IMeadowIOController IOController { get; set; }
        protected IPwmChannelInfo PwmChannelInfo { get; set; }

        protected PwmPort(
            IPin pin,
            IMeadowIOController ioController,
            IPwmChannelInfo channel,
            bool inverted = false,
            bool isOnboard = false)
            : base(pin, channel, new Frequency(100f, Units.Frequency.UnitType.Hertz))
        {
            this.IsOnboard = isOnboard;
            this.IOController = ioController;
            this.PwmChannelInfo = channel;
            this.Inverted = inverted;
        }

        internal static PwmPort From(
            IPin pin,
            IMeadowIOController ioController,
            Frequency frequency,
            float dutyCycle = 0.5f,
            bool inverted = false,
            bool isOnboard = false)
        {
            var channel = pin.SupportedChannels.OfType<IPwmChannelInfo>().FirstOrDefault();
            if (channel != null)
            {
                var success = ioController.DeviceChannelManager.ReservePwm(pin, channel, frequency);

                if (success.Item1)
                {
                    var port = new PwmPort(pin, ioController, channel, inverted, isOnboard);
                    port.TimeScale = TimeScale.Seconds;
                    port.Frequency = frequency;
                    port.DutyCycle = dutyCycle;
                    port.Inverted = inverted;
                    port.Channel = channel;
                    return port;
                }
                else
                {
                    throw new PortInUseException($"PwmPort: Pin {pin.Name} is already in use");
                }
            }
            else
            {
                throw new Exception("Unable to create an output port on the pin, because it doesn't have a PWM channel");
            }
        }

        /// <summary>
        /// When <b>true</b> Duty Cycle is "percentage of time spent low" rather than high.
        /// </summary>
        public override bool Inverted
        {
            get => _inverted;
            set
            {
                if (value == Inverted) return;
                _inverted = value;
                if (State)
                {
                    UpdateChannel();
                }
            }
        }

        /// <summary>
        /// The frequency, in Hz (cycles per second) of the PWM square wave.
        /// </summary>
        public override Frequency Frequency
        {
            get => _frequency;
            set
            {
                // clamp
                if (value.Hertz < 0) { value = new Frequency(0); }

                // TODO: add upper bound.

                // shortcut
                if (value == Frequency) return;

                _frequency = value;
                if (State)
                {
                    UpdateChannel();
                }
            }
        }

        /// <summary>
        /// The percentage of time the PWM pulse is high (in the range of 0.0 to 1.0)
        /// </summary>
        public override float DutyCycle
        {
            get => _dutyCycle;
            set
            {
                // clamp
                if (value < 0) { value = 0; }
                if (value > 1) { value = 1; }
                if (value == DutyCycle) return;

                // dirty dirty hack
                // Onboard LED flatlines at PWM > 0.85ish.
                if (IsOnboard && (value > 0.85f))
                {
                    value = 0.85f;
                }

                _dutyCycle = value;
                if (State)
                {
                    UpdateChannel();
                }
            }
        }

        /// <summary>
        /// The amount of time, in seconds, that the a PWM pulse is high.  This will always be less than or equal to the Period
        /// </summary>
        public override float Duration
        {
            get => DutyCycle * Period;
            set
            {
                if (value > Period) throw new ArgumentOutOfRangeException("Duration must be less than Period");
                // clamp
                if (value < 0) { value = 0; }

                DutyCycle = value / Period;
            }
        }

        /// <summary>
        /// The reciprocal of the PWM frequency - in seconds.
        /// </summary>
        public override float Period
        {
            get => 1.0f / (float)Frequency.Hertz * (float)TimeScale;
            set
            {
                Frequency = new Frequency(1.0f * (float)TimeScale / value);
            }
        }

        private void UpdateChannel()
        {
            UPD.PWM.Start(PwmChannelInfo, (uint)Frequency.Hertz, Inverted ? (1.0f - DutyCycle) : DutyCycle);
        }


        /// <summary>
        /// Returns <b>true</b> if the PWM is currently running, otherwise <b>false</b>
        /// </summary>
        public override bool State
        {
            get => _isRunning;
        }

        /// <summary>
        /// Starts the PWM output
        /// </summary>
        public override void Start()
        {
            this.IOController.DeviceChannelManager.BeforeStartPwm(this.PwmChannelInfo);
            UpdateChannel();
            this.IOController.DeviceChannelManager.AfterStartPwm(this.PwmChannelInfo, this.IOController);
            _isRunning = true;
        }

        /// <summary>
        /// Stops the PWM output
        /// </summary>
        public override void Stop()
        {
            UPD.PWM.Stop(PwmChannelInfo);
            _isRunning = false;
        }

        protected override void Dispose(bool disposing)
        {
            Stop();
            UPD.PWM.Shutdown(PwmChannelInfo.Timer);
        }
    }
}