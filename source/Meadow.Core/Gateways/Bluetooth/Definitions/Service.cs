namespace Meadow.Gateways.Bluetooth
{
    public class Service : IService
    {
        public ushort Handle { get; set; }
        public string Name { get; }
        public ushort Uuid { get; }
        public CharacteristicCollection Characteristics { get; }

        public Service(string name, ushort uuid, params ICharacteristic[] characteristics)
        {
            Name = name;
            Uuid = uuid;
            Characteristics = new CharacteristicCollection();
            Characteristics.AddRange(characteristics);
        }

        public string ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""name"":""{Name}"",
                        ""uuid"":{Uuid}
                    ";

            if (Characteristics != null && Characteristics.Count > 0)
            {
                json += ", \"characteristics\": [";
                for (int i = 0; i < Characteristics.Count; i++)
                {
                    json += (Characteristics[i] as IJsonSerializable)?.ToJson();
                    if (i < (Characteristics.Count - 1))
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
