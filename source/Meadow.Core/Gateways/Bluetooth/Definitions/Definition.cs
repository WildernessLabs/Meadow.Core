using System.Runtime.InteropServices;

namespace Meadow.Gateways.Bluetooth
{
    public class Definition
    {
        public string DeviceName { get; }
        public ServiceCollection Services { get; }

        public Definition(string deviceName, params Service[] services)
        {
            DeviceName = deviceName;
            Services = new ServiceCollection();
            Services.AddRange(services);
        }

        internal string ToJson()
        {
            // serialize to JSON, but without pulling in a JSON lib dependency
            var json = $@"{{
                        ""deviceName"":""{DeviceName}""
                    ";

            if (Services != null && Services.Count > 0)
            {
                json += ", \"services\": [";
                for (int i = 0; i < Services.Count; i++)
                {
                    if (Services[i] != null)
                    {
                        json += Services[i]?.ToJson();
                        if (i < (Services.Count - 1))
                        {
                            json += ",";
                        }
                    }
                }
                json += "]";
            }

            json += "}";
            return json;
        }
    }
}
