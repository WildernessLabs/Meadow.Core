using Meadow.Devices;
using Meadow.Units;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Meadow.Hardware
{
    internal class DeviceChannelManager : IDeviceChannelManager
    {
        /// <summary>
        /// Stores the current state of the various pin/channels on the device. 
        /// Used internally so that we can safely new up ports.
        /// </summary>
        private IDictionary<IPin, ChannelConfig> _channelStates;
        private readonly object _channelLock = new object();
        private IDictionary<uint, double> _pwmTimerFrequencies;
        private List<uint> _pwmTimersInitialized = new List<uint>();
        private KeyValuePair<IPin, ChannelConfig>[]? _pinsToReasssertForPwm = null;
        private string[]? _reservedPins = null;

        internal bool ShowDebug { get; set; } = false;

        internal DeviceChannelManager()
        {
            _channelStates = new Dictionary<IPin, ChannelConfig>();
            _pwmTimerFrequencies = new Dictionary<uint, double>();
        }

        public string[]? SystemReservedPins
        {
            get => _reservedPins;
            set
            {
                _reservedPins = value;
                if (_reservedPins != null && _reservedPins.Length > 0)
                {
                    Resolver.Log.Debug($"System reserved pins: {string.Join(';', SystemReservedPins)}");
                }
            }
        }

        /// <summary>
        /// Attempts to reserve a channel. Should be called during IPort constructors
        /// to see if a pin/channel is already in use.
        /// </summary>
        // NOTE: this method makes me long for F#. Oh pattern matching :D
        // TODO: should also check to see if that particular pin has the capability it's asking for? we can address later.
        public Tuple<bool, string> ReservePin(IPin pin, ChannelConfigurationType configType)
        {
            Output.WriteLineIf(ShowDebug, "+ReservePin");

            if (SystemReservedPins != null)
            {
                if (SystemReservedPins.Contains(pin.Key.ToString().Substring(1)))
                {
                    return new Tuple<bool, string>(false, "Pin is defined as System Reserved.");
                }
            }

            // thread sync
            try
            {
                lock (_channelLock)
                {
                    // if the config exists in the collection
                    if (_channelStates.ContainsKey(pin))
                    {
                        // if the channel is already in use
                        if (_channelStates[pin].State == ChannelState.InUse)
                        {
                            // bail out
                            return new Tuple<bool, string>(false, $"Pin {pin.Name} is already in use");
                        }
                        else
                        { // if it's not, probably need to do some cleanup
                            _channelStates.Remove(pin);
                        }
                    }

                    // add the new config to the list
                    var newConfig = new ChannelConfig() { Config = configType, State = ChannelState.InUse };
                    _channelStates.Add(pin, newConfig);
                    // successfully reserved the port
                    return new Tuple<bool, string>(true, "Success");
                }
            }
            finally
            {
                Output.WriteLineIf(ShowDebug, "-ReservePin");
            }
        }

        public void BeforeStartPwm(IPwmChannelInfo info)
        {
            // HACK HACK HACK
            // In Nuttx, the first time a PWM timer is started, it sets the AF bit for all pins in the timer
            // this is not desired behavior.  We record the types of all pins that are not the target pin, and re-assert that after starting
            var pinsToReassert = new int[0];
            if (!_pwmTimersInitialized.Contains(info.Timer))
            {
                string[]? c = null;

                // first time this timer has been touched
                switch (info.Timer)
                {
                    case 2:
                        c = new string[] { "OnboardLedBlue", "OnboardLedGreen", "OnboardLedRed" };
                        break;
                    case 3:
                        c = new string[] { "D05", "D06", "D09" };
                        break;
                    case 4:
                        c = new string[] { "D08", "D07", "D03", "D04" };
                        break;
                    case 8:
                        c = new string[] { "D02", "D11" };
                        break;
                    case 12:
                        c = new string[] { "D12", "D13" };
                        break;
                }

                if (c != null)
                {
                    _pinsToReasssertForPwm = _channelStates.Where(s => c.Contains(s.Key.Name)).ToArray();
                }
            }
        }

        public void AfterStartPwm(IPwmChannelInfo info, IMeadowIOController ioController)
        {
            // HACK HACK HACK
            // In Nuttx, the first time a PWM timer is started, it sets the AF bit for all pins in the timer
            // this is not desired behavior.  We record the types of all pins that are not the target pin, and re-assert that after starting
            if (!_pwmTimersInitialized.Contains(info.Timer))
            {
                // TODO: re-assert AF
                if (_pinsToReasssertForPwm != null)
                {
                    foreach (var p in _pinsToReasssertForPwm)
                    {
                        ioController.ReassertConfig(p.Key, false);
                    }
                }
                _pinsToReasssertForPwm = null;
                _pwmTimersInitialized.Add(info.Timer);
            }
        }

        public Tuple<bool, string> ReservePwm(IPin pin, IPwmChannelInfo channelInfo, Frequency frequency)
        {
            lock (_channelLock)
            {
                // if the config exists in the collection
                if (_channelStates.ContainsKey(pin))
                {
                    // if the channel is already in use
                    if (_channelStates[pin].State == ChannelState.InUse)
                    {
                        // bail out
                        return new Tuple<bool, string>(false, "Channel already in use.");
                    }
                    else
                    { // if it's not, probably need to do some cleanup
                        _channelStates.Remove(pin);
                    }
                }

                // PWMs on a single timer must have the same frequency (but can have different duty cycles)
                if (_pwmTimerFrequencies.ContainsKey(channelInfo.Timer))
                {
                    var currentFrequency = new Frequency(_pwmTimerFrequencies[channelInfo.Timer], Frequency.UnitType.Hertz);
                    if (currentFrequency != frequency)
                    {
                        return new Tuple<bool, string>(false, $"PWM Timer frequency cannot be different between separate channels. PWM Timer {channelInfo.Timer} is already set to {currentFrequency} Hz");
                    }
                }
                else
                {
                    _pwmTimerFrequencies.Add(channelInfo.Timer, frequency.Hertz);
                }

                // due to nuttx - any PWM created on a timer will initialize all channels on that timer to alternate function - killing any existing IO
                // we need to deal with that - re-assert the GPIO type after

                return new Tuple<bool, string>(true, "Success");
            }
        }
        // TODO: do we want to message back any information?
        public bool ReleasePin(IPin pin)
        {
            // thread sync
            lock (_channelLock)
            {
                // if the config exists in the collection
                ChannelConfig config = _channelStates[pin];
                if (config != null)
                {
                    _channelStates.Remove(pin);
                }
            }
            return true;
        }
    }
}

