using Meadow.Units;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Meadow
{
    public class JetsonPlatformOS : LinuxPlatformOS
    {
        private Dictionary<string, string> _nameToPathLookup = new Dictionary<string, string>();

        private void PopulatePathLookup()
        {
            if (_nameToPathLookup.Count > 0) return;

            var root = new DirectoryInfo("/sys/class/thermal");
            if (root.Exists)
            {
                foreach (var zone in root.GetDirectories("thermal_zone*"))
                {
                    var name = File.ReadAllText(Path.Combine(zone.FullName, "type")).Trim();
                    _nameToPathLookup.Add(name, Path.Combine(zone.FullName, "temp"));
                }
            }
        }

        public string[] GetTemperatureNames()
        {
            PopulatePathLookup();

            return _nameToPathLookup.Keys.ToArray();
        }

        public Temperature GetTemperature(string name)
        {
            PopulatePathLookup();

            if (_nameToPathLookup.ContainsKey(name))
            {
                var raw = File.ReadAllText(_nameToPathLookup[name]).Trim();
                var c = double.Parse(raw);
                return new Temperature(c / 1000d, Temperature.UnitType.Celsius);
            }
            else
            {
                throw new ArgumentException($"Temperature name '{name}' not found");
            }
        }

        public override Temperature GetCpuTemperature()
        {
            return GetTemperature("CPU-therm");
        }
    }
}
