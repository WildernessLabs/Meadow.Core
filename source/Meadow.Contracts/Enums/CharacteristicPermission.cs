namespace Meadow.Gateways.Bluetooth
{
    public enum CharacteristicPermission : short
    {
        Read = 1 << 0,
        ReadEncrypted = 1 << 1,
        ReadEncMITM = 1 << 2,
        // WHERE IS 1 << 3?
        Write = 1 << 4,
        WriteEncrypted = 1 << 5,
        WriteEncMITM = 1 << 6,
        WriteSigned = 1 << 7,
        WriteSignedMITM = 1 << 8
    }
}
