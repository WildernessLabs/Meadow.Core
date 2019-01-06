using System;
using System.Collections.Generic;
using Meadow.Core.Interop;
using static Meadow.Core.Interop.Interop.Nuttx;

namespace Meadow.Hardware
{
    public static class DeviceChannelManager
    {
        /// <summary>
        /// Stores the current state of the various pin/channels on the device. 
        /// Used internally so that we can safely new up ports.
        /// </summary>
        private static IDictionary<IPin, ChannelConfig> _channelStates;
        private static readonly object _channelLock = new object();

        static DeviceChannelManager()
        {
            _channelStates = new Dictionary<IPin, ChannelConfig>();
        }

        /// <summary>
        /// Attempts to reserve a channel. Should be called during IPort constructors
        /// to see if a pin/channel is already in use.
        /// </summary>
        // NOTE: this method makes me long for F#. Oh pattern matching :D
        // TODO: should also check to see if that particular pin has the capapbility it's asking for? we can address later.
        internal static Tuple<bool, string> ReservePin(IPin pin, ChannelConfigurationType configType)
        {
            // thread sync
            lock (_channelLock)
            {
                // if the config exists in the collection
                if(_channelStates.ContainsKey(pin))
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

                // add the new config to the list
                var newConfig = new ChannelConfig() { Config = configType, State = ChannelState.InUse };
                _channelStates.Add(pin, newConfig);
                // successfully reserved the port
                return new Tuple<bool, string>(true, "Success");
            }
        }

        // TODO: do we want to message back any information?
        internal static bool ReleasePin(IPin pin)
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

    internal class ChannelConfig
    {
        public ChannelState State { get; set; }
        public ChannelConfigurationType Config { get; set; }
    }

    internal enum ChannelState
    {
        Unknown,
        NotInUse,
        InUse,
    }

    /// <summary>
    /// TODO: revisit this structure. Ultimately, it would be nice to know, specifically
    /// what a channel is cofigured for, i.e. DigitalInput, I2C TX, UART RX, etc.
    /// </summary>
    internal enum ChannelConfigurationType
    {
        None,
        Digital,
        Analog,
        SPI,
        I2C,
        CAN,
        UART
    }

}

