using System;
namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// Describes the capabilities of the Bluetooth adapter.
    /// </summary>
    public class AdapterCapabilities
    {
        public bool HasClassicSupport { get; protected set; }
        public bool HasSecureClassicSupport { get; protected set; }
        public bool HasLowEnergySupport { get; protected set; }
        public bool HasLowEnergySecureSupport { get; protected set; }
        public bool HasLowEnergyCentralRoleSupport { get; protected set; }
        public bool HasLowEnergyPeripheralRoleSupport { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="T:Meadow.Gateways.Bluetooth.AdapterCapabilities"/>
        /// has support to offload advertising to secondary channels.
        /// </summary>
        /// <value><c>true</c> if has advertisement offload support; otherwise, <c>false</c>.</value>
        public bool HasAdvertisementOffloadSupport { get; set; }

        public AdapterCapabilities(
            bool hasClassicSupport, 
            bool hasSecureClassicSupport,
            bool hasLowEnergySupport,
            bool hasLowEnergySecureSupport,
            bool hasLowEnergyCentralRoleSupport,
            bool hasLowEnergyPeripheralRoleSupport
            )
        {
            this.HasClassicSupport = hasClassicSupport;
            this.HasSecureClassicSupport = hasSecureClassicSupport;
            this.HasLowEnergySupport = hasLowEnergySupport;
            this.HasLowEnergySecureSupport = hasLowEnergySecureSupport;
            this.HasLowEnergyCentralRoleSupport = hasLowEnergyCentralRoleSupport;
            this.HasLowEnergyPeripheralRoleSupport = hasLowEnergyPeripheralRoleSupport;
        }
    }
}
