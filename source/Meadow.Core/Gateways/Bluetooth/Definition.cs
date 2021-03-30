
using System;
using System.Collections.Generic;

namespace Meadow.Gateways.Bluetooth
{
    public enum CharacteristicPermission : short
    {
        Read = 1 << 0,
        ReadEncrypted = 1 << 1,
        ReadEncMITM = 1 << 2,
        // WHERE IS 1 << 3?
        Write = 1 << 4,
        WriteEncrypted = 1 << 5,
        WriteEncMITM = 1 << 6,
        WriteSigned = 1 << 7,
        WriteSignedMITM = 1 << 8
    }

    public enum CharacteristicProperty : byte
    {
        Broadcast = 1 << 0,
        Read = 1 << 1,
        WriteNoResponse = 1 << 2,
        Write = 1 << 3,
        Notify = 1 << 4,
        Indicate = 1 << 5,
        SignedWrite = 1 << 6,
        ExtendedProp = 1 << 7
    }

    public class Definition
    {
        public string DeviceName { get; }
        public Service[] Services { get; }

        public Definition(string deviceName, params Service[] services)
        {
            DeviceName = deviceName;
            Services = services;
        }

        internal string ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""deviceName"":""{DeviceName}""
                    ";

            if (Services != null && Services.Length > 0)
            {
                json += ", \"services\": [";
                for (int i = 0; i < Services.Length; i++)
                {
                    json += Services[i].ToJson();
                    if (i < (Services.Length - 1))
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

    public class Service
    {
        internal ushort Handle { get; set; }
        public string Name { get; }
        public ushort Uuid { get; }
        public Characteristic[] Characteristics { get; }

        public Service(string name, ushort uuid, params Characteristic[] characteristics)
        {
            Name = name;
            Uuid = uuid;
            Characteristics = characteristics;
        }

        internal string ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""name"":""{Name}"",
                        ""uuid"":{Uuid}
                    ";

            if (Characteristics != null && Characteristics.Length > 0)
            {
                json += ", \"characteristics\": [";
                for (int i = 0; i < Characteristics.Length; i++)
                {
                    json += Characteristics[i].ToJson();
                    if (i < (Characteristics.Length - 1))
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

    public delegate void CharacteristicDataSetHandler(Characteristic c, byte[] data);

    public class Characteristic
    {
        public event CharacteristicDataSetHandler OnDataSet;

        internal ushort DefinitionHandle { get; set; }
        internal ushort ValueHandle { get; set; }

        public string Name { get; } // only for user reference, not used in BLE anywhere
        public string Uuid { get; }
        public CharacteristicPermission Permissions { get; }
        public CharacteristicProperty Properties { get; }
        public int MaxLength { get; }
        public Descriptor[] Descriptors { get; }

        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
        {
            Name = name;
            Uuid = uuid;
            Permissions = permissions;
            Properties = properties;
            MaxLength = maxLength;
            Descriptors = descriptors;
        }

        internal void RaiseOnDataSet(byte[] data)
        {
            OnDataSet?.Invoke(this, data);
        }

        internal string ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""uuid"":""{Uuid}"",
                        ""permission"":{(int)Permissions},
                        ""props"":{(int)Properties},
                        ""len"":{MaxLength}
                    ";

            if(Descriptors != null && Descriptors.Length > 0)
            {
                json += ", \"descriptors\": [";
                for(int i = 0; i < Descriptors.Length; i++)
                {
                    json += Descriptors[i].ToJson();
                    if(i < (Descriptors.Length - 1))
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

    public class Descriptor
    {
        internal ushort Handle { get; set; }

        internal string ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency

            return base.ToString();
        }
    }
}