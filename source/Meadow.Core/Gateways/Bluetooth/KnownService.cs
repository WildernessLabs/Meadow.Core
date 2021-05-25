using System;
namespace Meadow.Gateways.Bluetooth
{
    public struct KnownService
    {
        public string Name { get; private set; }
        public Guid Id { get; private set; }

        public KnownService(string name, Guid id)
        {
            Name = name;
            Id = id;
        }
    }
}
