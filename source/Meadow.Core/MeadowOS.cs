using System;
using Meadow.Hardware;

namespace Meadow
{
    public static class MeadowOS//<D> where D : IIODevice
    {
        static MeadowOS()
        {
        }

        internal static void Init(IIODevice device)
        {
            CurrentDevice = device;
        }

        public static IIODevice CurrentDevice { get; private set; }


        public static void Sleep(DateTime until) {
            throw new NotImplementedException();
        }

        public static void Sleep(TimeSpan duration) {
            throw new NotImplementedException();
        }

        public static void Sleep(WakeUpOptions wakeUp) {
            throw new NotImplementedException();
        }

    }
}
