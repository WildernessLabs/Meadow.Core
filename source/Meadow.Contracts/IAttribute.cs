namespace Meadow.Gateways.Bluetooth
{
    public interface IAttribute
    {
        void HandleDataWrite(byte[] data);

        ushort DefinitionHandle { get; set; }
        ushort ValueHandle { get; set; }
    }
}
