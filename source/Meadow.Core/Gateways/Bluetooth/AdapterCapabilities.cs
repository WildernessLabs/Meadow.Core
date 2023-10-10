namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Describes the capabilities of the Bluetooth adapter.
/// </summary>
public class AdapterCapabilities
{
    /// <summary>
    /// Gets a value indicating whether the adapter has support for classic Bluetooth.
    /// </summary>
    public bool HasClassicSupport { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the adapter has secure support for classic Bluetooth.
    /// </summary>
    public bool HasSecureClassicSupport { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the adapter has support for Bluetooth Low Energy (BLE).
    /// </summary>
    public bool HasLowEnergySupport { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the adapter has secure support for Bluetooth Low Energy (BLE).
    /// </summary>
    public bool HasLowEnergySecureSupport { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the adapter has support for the central role in BLE.
    /// </summary>
    public bool HasLowEnergyCentralRoleSupport { get; protected set; }

    /// <summary>
    /// Gets a value indicating whether the adapter has support for the peripheral role in BLE.
    /// </summary>
    public bool HasLowEnergyPeripheralRoleSupport { get; protected set; }

    /// <summary>
    /// Gets or sets a value indicating whether the adapter has support to offload advertising to secondary channels.
    /// </summary>
    public bool HasAdvertisementOffloadSupport { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdapterCapabilities"/> class.
    /// </summary>
    /// <param name="hasClassicSupport">Indicates whether the adapter has support for classic Bluetooth.</param>
    /// <param name="hasSecureClassicSupport">Indicates whether the adapter has secure support for classic Bluetooth.</param>
    /// <param name="hasLowEnergySupport">Indicates whether the adapter has support for Bluetooth Low Energy (BLE).</param>
    /// <param name="hasLowEnergySecureSupport">Indicates whether the adapter has secure support for Bluetooth Low Energy (BLE).</param>
    /// <param name="hasLowEnergyCentralRoleSupport">Indicates whether the adapter has support for the central role in BLE.</param>
    /// <param name="hasLowEnergyPeripheralRoleSupport">Indicates whether the adapter has support for the peripheral role in BLE.</param>
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
