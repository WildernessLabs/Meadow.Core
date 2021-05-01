using System;
using System.Collections.ObjectModel;

namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// A bluetooth LE GATT service.
    /// </summary>
	public interface IService : IDisposable
    {
        /// <summary>
        /// Id of the Service.
        /// </summary>
		Guid ID { get; }

        /// <summary>
        /// Name of the service.
        /// Returns the name if the <see cref="ID"/> is a standard Id. See <see cref="KnownServices"/>.
        /// </summary>
		string Name { get; set; }

        /// <summary>
        /// Indicates whether the type of service is primary or secondary.
        /// </summary>
		bool IsPrimary { get; set; }

        /// <summary>
        /// Gets the characteristics of the service.
        /// </summary>
        //ObservableCollection<ICharacteristic> Characteristics { get; }
        ObservableDictionary<Guid, ICharacteristic> Characteristics { get; }
    }
}
