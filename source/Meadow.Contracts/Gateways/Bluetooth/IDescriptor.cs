using System;
namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// A descriptor for a GATT characteristic.
    /// </summary>
    public interface IDescriptor : IJsonSerializable
    {
        public ushort Handle { get; set; }
        public string Uuid { get; }
    }
}
