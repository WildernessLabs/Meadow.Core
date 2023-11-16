namespace Meadow.Gateways.Bluetooth
{
    /// <summary>
    /// Represents a Bluetooth characteristic.
    /// </summary>
    public abstract class Characteristic : ICharacteristic, IAttribute, IJsonSerializable
    {
        /// <summary>
        /// Occurs when the value of the characteristic is set.
        /// </summary>
        public event CharacteristicValueSetHandler ValueSet = default!;

        /// <summary>
        /// Occurs when the value of the server is set internally (used internally).
        /// </summary>
        public event ServerValueChangedHandler ServerValueSet = default!; // This is an internally used event.

        /// <summary>
        /// Gets or sets the handle of the characteristic definition.
        /// </summary>
        public ushort DefinitionHandle { get; set; }

        /// <summary>
        /// Gets or sets the handle of the characteristic value.
        /// </summary>
        public ushort ValueHandle { get; set; }

        /// <summary>
        /// Gets the name of the characteristic (for user reference).
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the UUID of the characteristic.
        /// </summary>
        public string Uuid { get; }

        /// <summary>
        /// Gets the permissions of the characteristic.
        /// </summary>
        public CharacteristicPermission Permissions { get; }

        /// <summary>
        /// Gets the properties of the characteristic.
        /// </summary>
        public CharacteristicProperty Properties { get; }

        /// <summary>
        /// Gets the maximum length of the characteristic value.
        /// </summary>
        public int MaxLength { get; }

        /// <summary>
        /// Gets the descriptors associated with the characteristic.
        /// </summary>
        public IDescriptor[] Descriptors { get; }

        internal Characteristic(string name, string uuid, CharacteristicPermission permissions, CharacteristicProperty properties, int maxLength, params Descriptor[] descriptors)
        {
            Name = name;
            Uuid = uuid;
            Permissions = permissions;
            Properties = properties;
            MaxLength = maxLength;
            Descriptors = descriptors;
        }

        /// <summary>
        /// Handles data write for the characteristic.
        /// </summary>
        /// <param name="data">The data to be written.</param>
        public abstract void HandleDataWrite(byte[] data);

        /// <summary>
        /// Sets the value of the characteristic.
        /// </summary>
        /// <param name="value">The value to be set.</param>
        public abstract void SetValue(object value);

        internal void SendValueToAdapter(byte[] data)
        {
            ServerValueSet?.Invoke(this, data);
        }

        /// <summary>
        /// Raises the <see cref="ValueSet"/> event, indicating that the value of the characteristic has been set.
        /// </summary>
        /// <param name="data">The data associated with the value set event.</param>
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
