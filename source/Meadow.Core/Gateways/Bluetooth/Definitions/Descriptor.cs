using System.Text;

namespace Meadow.Gateways.Bluetooth
{
    public class Descriptor
    {
        public ushort Handle { get; set; }

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
            Data = new byte[Length];
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
