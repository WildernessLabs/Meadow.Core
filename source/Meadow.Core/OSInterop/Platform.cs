using System;
using System.Runtime.InteropServices;

namespace Meadow.OSInterop
{
    internal partial class Platform
    {
        private const string NUTTX = "NuttXOrWhatevers";

        internal partial class Nuttx
        {
            //    [DllImport(ConstValueForLibraryName, SetLastError = true)]
            //    internal static extern void Foo();

            //stm32_configgpio
            [DllImport(NUTTX, SetLastError = true)]
            internal static extern void Stm32_configgpio(uint configSet);

        }


    }
}
