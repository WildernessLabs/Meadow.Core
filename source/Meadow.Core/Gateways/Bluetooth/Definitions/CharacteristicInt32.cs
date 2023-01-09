
using System;

namespace Meadow.Gateways.Bluetooth
{
    public class CharacteristicInt32 : Characteristic<int>
    {
        public CharacteristicInt32(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, descriptors)
        {
        }

        public override void HandleDataWrite(byte[] data)
        {
            Resolver.Log.Info($"HandleDataWrite in {this.GetType().Name}");

            // TODO: if the written data isn't 4 bytes, then what??
            // for now I'll right-pad with zeros
            if (data.Length < 4)
            {
                Resolver.Log.Info($"HandleDataWrite only got {data.Length} bytes - padding");
                var temp = new byte[4];
                Array.Copy(data, temp, data.Length);
                RaiseValueSet(BitConverter.ToInt32(temp));
            }
            else
            {
                if (data.Length > 4)
                {
                    Resolver.Log.Info($"HandleDataWrite got {data.Length} bytes - using only the first 4");
                }
                RaiseValueSet(BitConverter.ToInt32(data));
            }
        }

        protected override byte[] SerializeValue(int value)
        {
            return BitConverter.GetBytes(value);
        }
    }
}
