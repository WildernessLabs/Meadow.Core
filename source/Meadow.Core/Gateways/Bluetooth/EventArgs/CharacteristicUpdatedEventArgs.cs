using System;
namespace Meadow.Gateways.Bluetooth
{
    public class CharacteristicUpdatedEventArgs : EventArgs
    {
        public ICharacteristic Characteristic { get; set; }

        public CharacteristicUpdatedEventArgs(ICharacteristic characteristic)
        {
            Characteristic = characteristic;
        }
    }
}
