using Meadow.Units;
using System;

namespace Meadow
{
    public partial class F7PlatformOS : IPlatformOS
    {
        public F7PlatformOS()
        {
        }

        public Temperature GetCpuTemperature()
        {
            throw new NotSupportedException();
        }

        public void Initialize()
        {

        }
    }
}
