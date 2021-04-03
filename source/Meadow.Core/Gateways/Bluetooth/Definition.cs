
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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

    public delegate void CharacteristicDataSetHandler<T>(Characteristic<T> c, T data) where T : struct;
    public delegate void CharacteristicDataSetHandler(ICharacteristic c, byte[] data);

    public interface ICharacteristic : IAttribute, IDataSetHandlerSource
    {
        event CharacteristicDataSetHandler OnDataSet;

        string Name { get; } // only for user reference, not used in BLE anywhere
        string Uuid { get; }
        CharacteristicPermission Permissions { get; }
        CharacteristicProperty Properties { get; }
        int MaxLength { get; }
        Descriptor[] Descriptors { get; }
    }

    public interface IAttribute
    {
        ushort DefinitionHandle { get; set; }
        ushort ValueHandle { get; set; }
    }

    internal interface IJsonSerializable
    {
        string ToJson();
    }

    // this really needs to be internal, but the interitance tree effs it up
    public interface IDataSetHandlerSource
    {
        void RaiseOnDataSet(byte[] data);
    }

    public abstract class Characteristic : ICharacteristic, IAttribute, IJsonSerializable
    {
        public event CharacteristicDataSetHandler OnDataSet;

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

            Console.WriteLine($"Characteristic {Name} is {maxLength} bytes.");
        }

        public virtual void RaiseOnDataSet(byte[] data)
        {
            // dev note: not a fan of much of this - but without specialization, not sure what the solution is
            Console.WriteLine($"Received {data.Length} bytes.  Copying to {MaxLength} handle");
            if (data.Length != MaxLength)
            {
                Console.WriteLine($"Houston, we have a problem!");
            }

            OnDataSet?.Invoke(this, data);
        }

        string IJsonSerializable.ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""uuid"":""{Uuid}"",
                        ""permission"":{(int)Permissions},
                        ""props"":{(int)Properties},
                        ""len"":{MaxLength}
                    ";

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

    public class Characteristic<T> : Characteristic
        where T : struct
    {
        public event CharacteristicDataSetHandler<T>? OnValueSet;


        public Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, params Descriptor[] descriptors)
            : base(name, uuid, permissions, properties, Marshal.SizeOf(typeof(T)), descriptors)
        {
        }

        public override void RaiseOnDataSet(byte[] data)
        {
            Console.WriteLine("Characteristic<T>:RaiseOnDataSet");

            base.RaiseOnDataSet(data);

            if (OnValueSet is { } vs)
            {
                Console.WriteLine("We have an OnValueSet");

                if (typeof(T).Equals(typeof(bool)))
                {
                    Console.WriteLine("We have a bool");
                    // yay managed code (len of bool == 4)!
                    // yay generics (no, you can't specialize for bool)!
                    vs.Invoke(this, (T)(object)(data[0] != 0));
                }
                else
                {
                    Console.WriteLine("We have a non-bool");
                    // well this is not very friendly, but I can't yet think of a better way.  ugh.
                    var result = default(T);
                    var handle = GCHandle.Alloc(result);
                    Marshal.Copy(data, 0, handle.AddrOfPinnedObject(), data.Length);
                    handle.Free();
                    vs.Invoke(this, result);
                }
            }
            else
            {
                Console.WriteLine("no OnValueSet");
            }
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
