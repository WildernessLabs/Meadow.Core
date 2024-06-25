using System.Runtime.InteropServices;

namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// Represents a Bluetooth characteristic with a generic value of type <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">The type of the characteristic value.</typeparam>
    public abstract class Characteristic<T> : Characteristic
    {
        /// <summary>
        /// Serializes the value of type <typeparamref name="T"/> to a byte array.
        /// </summary>
        /// <param name="value">The value to be serialized.</param>
        /// <returns>The byte array representing the serialized value.</returns>
        protected abstract byte[] SerializeValue(T value);

        /// <summary>
        /// Initializes a new instance of the <see cref="Characteristic{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the characteristic.</param>
        /// <param name="uuid">The UUID of the characteristic.</param>
        /// <param name="permissions">The permissions of the characteristic.</param>
        /// <param name="properties">The properties of the characteristic.</param>
        /// <param name="maxLength">The maximum length of the characteristic value.</param>
        /// <param name="descriptors">The descriptors associated with the characteristic.</param>
        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, maxLength, descriptors)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Characteristic{T}"/> class with the specified parameters.
        /// </summary>
        /// <param name="name">The name of the characteristic.</param>
        /// <param name="uuid">The UUID of the characteristic.</param>
        /// <param name="permissions">The permissions of the characteristic.</param>
        /// <param name="properties">The properties of the characteristic.</param>
        /// <param name="descriptors">The descriptors associated with the characteristic.</param>
        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, Marshal.SizeOf(typeof(T)), descriptors)
        {
        }

        /// <summary>
        /// Sets the value of the characteristic.
        /// </summary>
        /// <param name="value">The value to be set.</param>
        public void SetValue(T value)
        {
            var bytes = SerializeValue(value);
            base.SendValueToAdapter(bytes);
        }

        /// <summary>
        /// Sets the value of the characteristic.
        /// </summary>
        /// <param name="value">The value to be set.</param>
        public override void SetValue(object value)
        {
            // TODO: is this a problem?  it will coerce things like short->int for us.  Do we want it to?
            SetValue((T)value);
        }
    }
}
