using System;
using System.Collections.ObjectModel;

namespace Meadow.Gateways.Bluetooth
{
    /*
    /// <summary>
    /// A bluetooth LE GATT characteristic.
    /// </summary>
    public interface ICharacteristic
    {
        /// <summary>
        /// Event gets raised, when the davice notifies a value change on this characteristic.
        /// To start listening, call <see cref="StartUpdates"/>.
        /// </summary>
        event EventHandler<CharacteristicUpdatedEventArgs> ValueUpdated;

        /// <summary>
        /// Id of the characteristic.
        /// </summary>
        Guid ID { get; }

        ///// <summary>
        ///// TODO: review: do we need this in any case?
        ///// Uuid of the characteristic.
        ///// </summary>
        //string Uuid { get; set; }

        /// <summary>
        /// Name of the charakteristic.
        /// Returns the name if the <see cref="ID"/> is a id of a standard characteristic.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets the last known value of the characteristic.
        /// </summary>
        byte[] Value { get; set; }

        ///// <summary>
        ///// Gets <see cref="Value"/> as UTF8 encoded string representation.
        ///// </summary>
        //string StringValue { get; set; }

        /// <summary>
        /// Properties of the characteristic.
        /// </summary>
        CharacteristicPropertyType Properties { get; set; }

        /// <summary>
        /// Specifies how the <see cref="WriteAsync"/> function writes the value.
        /// </summary>
        CharacteristicWriteType WriteType { get; set; }

        /// <summary>
        /// Indicates wheter the characteristic can be read or not.
        /// </summary>
        bool CanRead { get; set; }

        /// <summary>
        /// Indicates wheter the characteristic can be written or not.
        /// </summary>
        bool CanWrite { get; set; }

        /// <summary>
        /// Indicates wheter the characteristic supports notify or not.
        /// </summary>
        bool CanUpdate { get; set; }

        /// <summary>
        /// Gets the descriptors of the characteristic.
        /// </summary>
        //ObservableCollection<IDescriptor> Descriptors { get; set; }
        ObservableDictionary<Guid, IDescriptor> Descriptors { get; }
    }
    */
}
