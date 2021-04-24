using System;
using System.Runtime.InteropServices;

namespace Meadow.Gateways.Bluetooth
{
    public abstract class Characteristic<T> : Characteristic
    {
        protected abstract byte[] SerializeValue(T value);

        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, maxLength, descriptors)
        {
        }

        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, Marshal.SizeOf(typeof(T)), descriptors)
        {
        }

        public void SetValue(T value)
        {
            var bytes = SerializeValue(value);
            base.SendValueToAdapter(bytes);
        }

        public override void SetValue(object value)
        {
            // TODO: is this a problem?  it will coerce things like short->int for us.  Do we want it to?
            SetValue((T)value);
        }

    }
}
