using System;
using System.Collections.ObjectModel;

namespace Meadow.Gateways.Bluetooth
{
    /*
    public class Characteristic : ICharacteristic
    {
        /// <summary>
        /// Gets the descriptors of the characteristic.
        /// </summary>
        //public ObservableCollection<IDescriptor> Descriptors { get; } = new ObservableCollection<IDescriptor>();
        public ObservableDictionary<Guid, IDescriptor> Descriptors { get; } = new ObservableDictionary<Guid, IDescriptor>();

        /// <summary>
        /// Event gets raised, when the device notifies a value change on this characteristic.
        /// </summary>
        public event EventHandler<CharacteristicUpdatedEventArgs> ValueUpdated = delegate { };

        /// <summary>
        /// Id of the characteristic.
        /// </summary>
        public Guid ID { get; }

        ///// <summary>
        ///// TODO: review: do we need this in any case?
        ///// Uuid of the characteristic.
        ///// </summary>
        //public string Uuid { get; set; }

        /// <summary>
        /// Name of the characteristic.
        /// </summary>
        public string? Name { get; set; }

        //TODO: do we want to try and pull the name from the known characteristics?
        // maybe if it's null? 
        //public string Name => this.Name ?? KnownCharacteristics.Lookup(Id).Name;

        /// <summary>
        /// Gets the last known value of the characteristic.
        /// </summary>
        public byte[]? Value { get; set; }

        // Gets <see cref="Value"/> as UTF8 encoded string representation.
        // TODO: should we have a `SetValue(string value)` method that converts
        // a string into the byte[] value?
        //public string? StringValue { get; set; }

        /// <summary>
        /// Properties of the characteristic.
        /// </summary>
        public CharacteristicPropertyType Properties { get; set; }

        /// <summary>
        /// Specifies how the <see cref="WriteAsync"/> function writes the value.
        /// </summary>
        public CharacteristicWriteType WriteType { get; set; }

        /// <summary>
        /// Indicates wheter the characteristic can be read or not.
        /// </summary>
        public bool CanRead { get; set; }

        /// <summary>
        /// Indicates wheter the characteristic can be written or not.
        /// </summary>
        public bool CanWrite { get; set; }

        /// <summary>
        /// Indicates wheter the characteristic supports notify or not.
        /// </summary>
        public bool CanUpdate { get; set; }

        public Characteristic(Guid id)
        {
            this.ID = id;
        }
    }
    */
}
