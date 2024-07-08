using System;
namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// Represents the properties of a characteristic.
    /// It's a superset of all common platform specific properties.
    ///
    /// TODO: CTacke: we'll need to update these with whatever the ESP supports, i'm guessing.
    /// </summary>
    [Flags]
    public enum CharacteristicPropertyType
    {
        /// <summary>
        /// Characteristic value can be broadcast.
        /// </summary>
        Broadcast = 1,

        /// <summary>
        /// Characteristic value can be read.
        /// </summary>
        Read = 2,

        /// <summary>
        /// Characteristic value can be written without response.
        /// </summary>
        WriteWithoutResponse = 4,

        /// <summary>
        /// Characteristic can be written with response.
        /// </summary>
        Write = 8,

        /// <summary>
        /// Characteristic can notify value changes without acknowledgment.
        /// </summary>
        Notify = 16,

        /// <summary>
        ///Characteristic can indicate value changes with acknowledgment.
        /// </summary>
        Indicate = 32,

        /// <summary>
        /// Characteristic value can be written signed.
        /// </summary>
        AuthenticatedSignedWrites = 64,

        /// <summary>
        /// Indicates that more properties are set in the extended properties descriptor.
        /// </summary>
        ExtendedProperties = 128,

        // TODO: move these to separate enum
        /// <summary>
        /// Characteristic notifies of required encryption
        /// </summary>
        NotifyEncryptionRequired = 256, //0x100

        /// <summary>
        /// Indicates that encryption is required
        /// </summary>
        IndicateEncryptionRequired = 512, //0x200
    }
}
