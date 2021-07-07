using System;
using System.Collections.ObjectModel;

namespace Meadow.Gateways.Bluetooth
{
    public interface IService : IJsonSerializable
    {
        ushort Handle { get; set; }
        string Name { get; }
        ushort Uuid { get; }
        CharacteristicCollection Characteristics { get; }
    }
}
