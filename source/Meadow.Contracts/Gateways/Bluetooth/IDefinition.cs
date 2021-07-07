namespace Meadow.Gateways.Bluetooth
{
    public interface IDefinition
    {
        string DeviceName { get; }
        ServiceCollection Services { get; }
        string ToJson();
    }
}
