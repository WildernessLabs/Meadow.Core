
using System;

namespace Meadow.Gateways.Bluetooth
{
    public abstract class Characteristic : ICharacteristic, IAttribute, IJsonSerializable
    {
        public event CharacteristicValueSetHandler ValueSet;

        public event ServerValueChangedHandler ServerValueSet; // this is an internally used event.  

        public abstract void HandleDataWrite(byte[] data);

        public abstract void SetValue(object value);

        public ushort DefinitionHandle { get; set; }
        public ushort ValueHandle { get; set; }

        public string Name { get; } // only for user reference, not used in BLE anywhere
        public string Uuid { get; }
        public CharacteristicPermission Permissions { get; }
        public CharacteristicProperty Properties { get; }
        public int MaxLength { get; }
        public Descriptor[] Descriptors { get; }

        internal Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
        {
            Name = name;
            Uuid = uuid;
            Permissions = permissions;
            Properties = properties;
            MaxLength = maxLength;
            Descriptors = descriptors;
        }

        internal void SendValueToAdapter(byte[] data)
        {
            ServerValueSet?.Invoke(this, data);
        }

        protected void RaiseValueSet(object data)
        {
            ValueSet?.Invoke(this, data);
        }

        string IJsonSerializable.ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""uuid"":""{Uuid}"",
                        ""permission"":{(int)Permissions},
                        ""props"":{(int)Properties},
                        ""len"":{MaxLength}";

            if (Descriptors != null && Descriptors.Length > 0)
            {
                json += ", \"descriptors\": [";
                for (int i = 0; i < Descriptors.Length; i++)
                {
                    json += Descriptors[i].ToJson();
                    if (i < (Descriptors.Length - 1))
                    {
                        json += ",";
                    }
                }
                json += "]";
            }

            json += "}";

            return json;
        }
    }
}
