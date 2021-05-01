using System;
namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// A descriptor for a GATT characteristic.
    /// </summary>
	public interface IDescriptor
    {
        /// <summary>
        /// Id of the descriptor.
        /// </summary>
		Guid ID { get; }

        /// <summary>
        /// Name of the descriptor.
        /// Returns the name if the <see cref="ID"/> is a standard Id. See <see cref="KnownDescriptors"/>.
        /// </summary>
		string? Name { get; }

        /// <summary>
        /// The stored value of the descriptor. Call ReadAsync to update / write async to set it.
        /// </summary>
        byte[] Value { get; set; }
    }
}
