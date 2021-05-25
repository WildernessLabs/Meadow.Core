namespace Meadow.Gateways.Bluetooth
{
    public delegate void CharacteristicValueSetHandler(ICharacteristic c, object data);
    public delegate void ServerValueChangedHandler(ICharacteristic c, byte[] valueBytes);

    public interface ICharacteristic : IAttribute
    {
        event CharacteristicValueSetHandler ValueSet;
        event ServerValueChangedHandler ServerValueSet;

        void SetValue(object value);

        string Name { get; } // only for user reference, not used in BLE anywhere
        string Uuid { get; }
        CharacteristicPermission Permissions { get; }
        CharacteristicProperty Properties { get; }
        int MaxLength { get; }
        Descriptor[] Descriptors { get; }
    }
}
