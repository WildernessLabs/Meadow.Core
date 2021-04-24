
using System;

namespace Meadow.Gateways.Bluetooth
{
    public class CharacteristicBool : Characteristic<bool>
    {
        public CharacteristicBool(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, descriptors)
        {
        }

        public override void HandleDataWrite(byte[] data)
        {
            Console.WriteLine($"HandleDataWrite in {this.GetType().Name}");
            RaiseValueSet(data[0] != 0);
        }

        protected override byte[] SerializeValue(bool value)
        {
            return new byte[] { (byte)(value ? 1 : 0) };
        }
    }
}
