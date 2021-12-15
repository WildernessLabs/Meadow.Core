namespace Meadow
{
    internal unsafe struct GpioHandleData
    {
        /*
            #define GPIOHANDLES_MAX 64
            struct gpiohandle_data {
	            __u8 values[GPIOHANDLES_MAX];
            };
        */
        public fixed byte Values[64];
    }
}
