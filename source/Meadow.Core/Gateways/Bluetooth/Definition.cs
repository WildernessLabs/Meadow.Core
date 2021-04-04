
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

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
        public ICharacteristic[] Characteristics { get; }

        public Service(string name, ushort uuid, params ICharacteristic[] characteristics)
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
                    json += (Characteristics[i] as IJsonSerializable)?.ToJson();
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

    public delegate void CharacteristicValueSetHandler(ICharacteristic c, object data);
    public delegate byte[] CharacteristicValueGetHandler(ICharacteristic c);

    public interface ICharacteristic : IAttribute
    {
        event CharacteristicValueSetHandler ValueSet;
        Func<byte[]>? HandleDataRead { get; } // TODO: should we use an event instead?  A bit odd since multiple delegates can be attached to an event

        string Name { get; } // only for user reference, not used in BLE anywhere
        string Uuid { get; }
        CharacteristicPermission Permissions { get; }
        CharacteristicProperty Properties { get; }
        int MaxLength { get; }
        Descriptor[] Descriptors { get; }
    }

    public interface IAttribute
    {
        void HandleDataWrite(byte[] data);

        ushort DefinitionHandle { get; set; }
        ushort ValueHandle { get; set; }
    }

    internal interface IJsonSerializable
    {
        string ToJson();
    }

    public class CharacteristicBool : Characteristic<bool>
    {
        public CharacteristicBool(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, descriptors)
        {
        }

        public CharacteristicBool(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, Func<byte[]>? readFunction, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, readFunction, descriptors)
        {
        }

        public override void HandleDataWrite(byte[] data)
        {
            Console.WriteLine($"HandleDataWrite in {this.GetType().Name}");
            RaiseValueSet(data[0] != 0);
        }
    }

    public class CharacteristicInt32 : Characteristic<int>
    {
        public CharacteristicInt32(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, descriptors)
        {
        }

        public override void HandleDataWrite(byte[] data)
        {
            Console.WriteLine($"HandleDataWrite in {this.GetType().Name}");

            // TODO: if the written data isn't 4 bytes, then what??
            // for now I'll right-pad with zeros
            if (data.Length < 4)
            {
                Console.WriteLine($"HandleDataWrite only got {data.Length} bytes - padding");
                var temp = new byte[4];
                Array.Copy(data, temp, data.Length);
                RaiseValueSet(BitConverter.ToInt32(temp));
            }
            else
            {
                if(data.Length > 4)
                {
                    Console.WriteLine($"HandleDataWrite got {data.Length} bytes - using only the first 4");
                }
                RaiseValueSet(BitConverter.ToInt32(data));
            }
        }
    }

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
    }

    public abstract class Characteristic<T> : Characteristic
    {

        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, maxLength, descriptors)
        {
        }

        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, Marshal.SizeOf(typeof(T)), descriptors)
        {
        }


        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, Func<byte[]>? readFunction, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, maxLength, readFunction, descriptors)
        {
        }

        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, Func<byte[]>? readFunction, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, Marshal.SizeOf(typeof(T)), readFunction, descriptors)
        {
        }
    }

    public abstract class Characteristic : ICharacteristic, IAttribute, IJsonSerializable
    {
        public event CharacteristicValueSetHandler ValueSet;

        public abstract void HandleDataWrite(byte[] data);
        public Func<byte[]>? HandleDataRead { get; }

        public ushort DefinitionHandle { get; set; }
        public ushort ValueHandle { get; set; }

        public string Name { get; } // only for user reference, not used in BLE anywhere
        public string Uuid { get; }
        public CharacteristicPermission Permissions { get; }
        public CharacteristicProperty Properties { get; }
        public int MaxLength { get; }
        public Descriptor[] Descriptors { get; }

        internal Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
            : this(name, uuid, permissions, properties, maxLength, null, descriptors)
        {

        }

        internal Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, Func<byte[]>? readFunction, params Descriptor[] descriptors)
        {
            Name = name;
            Uuid = uuid;
            Permissions = permissions;
            Properties = properties;
            MaxLength = maxLength;
            Descriptors = descriptors;
            HandleDataRead = readFunction;

            Console.WriteLine($"Characteristic {Name} is {maxLength} bytes.");
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

    public class Descriptor
    {
        internal ushort Handle { get; set; }

        private CharacteristicPermission Permissions { get; }
        private CharacteristicProperty Properties { get; }
        private int Length { get; }
        private byte[] Data { get; }

        public string Uuid { get; }

        public Descriptor(string uuid, int value)
        {
            Uuid = uuid;

            // TODO: open these up
            Permissions = CharacteristicPermission.Read | CharacteristicPermission.Write;

            Length = sizeof(int);
            Data = new byte [Length];
        }

        public Descriptor(string uuid, string value)
        {
            Uuid = uuid;

            // TODO: open these up
            Permissions = CharacteristicPermission.Read | CharacteristicPermission.Write;

            Data = Encoding.UTF8.GetBytes(value);
            Length = Data.Length;
        }

        internal string ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""uuid"":10498,
                        ""permission"":{(int)Permissions},
                        ""len"":2,
                        ""value"":0
                        }}";

            return json;
        }
    }
}
