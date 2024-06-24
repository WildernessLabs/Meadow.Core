using System;

namespace Meadow.Gateways.Bluetooth;

/// <summary>
/// Provides data for the event when a characteristic is updated.
/// </summary>
public class CharacteristicUpdatedEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the characteristic that was updated.
    /// </summary>
    public ICharacteristic Characteristic { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CharacteristicUpdatedEventArgs"/> class with the specified characteristic.
    /// </summary>
    /// <param name="characteristic">The updated characteristic.</param>
    public CharacteristicUpdatedEventArgs(ICharacteristic characteristic)
    {
        Characteristic = characteristic;
    }
}
