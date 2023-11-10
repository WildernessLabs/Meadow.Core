using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            /*
            meadow_adc_configure() has 3 arguments:
                1. A list containing 1 to 16 analog input GPIOs. 
                   The list is populated with values such that PC7 would be 
                   entered as 0x27 and PA4 would be 0x04.
                2. The number of GPIOs in the list (1 to 16).
                3. A pointer to a block of memory which is at least gpioCount 
                   elements (doubles) in length where the 1 to 16 voltages can be copied.

            When the meadow_adc_read_values() function is called, the GPIOs from 
            gpioList will be converted to voltage values and copied to userDataBuf.

            Calling meadow_adc_read_temp_vbat() returns the current battery voltage 
            and temperature from the F7s internal hardware as doubles. No previous 
            configuration is needed for this to be used.

            To switch to a different configuration simple call meadow_adc_configure() 
            with the new arguments.

            In operation the call to meadow_adc_read_values() will not return until all 
            of the gpioList has been processed. I haven't timed this recently but 
            expect 15-30 usec per entry. So, probably 0.5 millisec or less for a list 
            of 16.
            */

            //int meadow_adc_configure(uint8_t gpioList[], uint32_t gpioCount, double* userDataBuf);
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int meadow_adc_configure(byte[] gpioList, int gpioCount, double[] userDataBuf);

            //int meadow_adc_read_values(void);
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int meadow_adc_read_values();

            //int meadow_adc_read_temp_vbat(double* batteryVoltage, double* temperatureValue);
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            public static extern int meadow_adc_read_temp_vbat(ref double batteryVoltage, ref double temperatureValue);
        }
    }
}
