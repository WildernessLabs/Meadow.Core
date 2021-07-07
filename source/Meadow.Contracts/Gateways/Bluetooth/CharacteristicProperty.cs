namespace Meadow.Gateways.Bluetooth
{
    public enum CharacteristicProperty : byte
    {
        Broadcast = 1 << 0,
        Read = 1 << 1,
        WriteNoResponse = 1 << 2,
        Write = 1 << 3,
        Notify = 1 << 4,
        Indicate = 1 << 5,
        SignedWrite = 1 << 6,
        ExtendedProp = 1 << 7
    }
}
