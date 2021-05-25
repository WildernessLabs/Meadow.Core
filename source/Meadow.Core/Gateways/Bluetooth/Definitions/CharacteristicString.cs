
using System;
using System.Text;

namespace Meadow.Gateways.Bluetooth
{
    public class CharacteristicString : Characteristic<string>
    {
        public CharacteristicString(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, maxLength, descriptors)
        {
        }

        public override void HandleDataWrite(byte[] data)
        {
            Console.WriteLine($"HandleDataWrite in {this.GetType().Name}");

            RaiseValueSet(Encoding.UTF8.GetString(data));
        }

        protected override byte[] SerializeValue(string value)
        {
            return Encoding.UTF8.GetBytes(value);
        }
    }
}
