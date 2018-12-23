using System;
using System.Runtime.InteropServices;

namespace Meadow.Core.Interop
{
    internal static partial class Interop
    {   // /dev/wdt
        // from ioexpander
        // /dev/gpin%n
        // /dev/gpout%n
        // /dev/gpint%n
        // /dev/userleds
        public static partial class Nuttx
        {
            public const int _GPIOBASE = 0x2300;
            public const int GPIOC_WRITE = _GPIOBASE | 1;
            public const int GPIOC_READ = _GPIOBASE | 2;
            public const int GPIOC_PINTYPE = _GPIOBASE | 3;

            public static IntPtr INVALID_HANDLE = new IntPtr(-1);
            /*
            enum gpio_pintype_e
            {
              GPIO_INPUT_PIN = 0,
              GPIO_OUTPUT_PIN,
              GPIO_INTERRUPT_PIN,
              GPIO_NPINTYPES
            };
            */
            public enum GPIOPinType
            {
                Input = 0,
                Output = 1,
                Interrupt = 2
            }; 

            public struct GPIOPinState
            {
                public int PinNumber;
                public bool State;
            }
            //#define _TIOCBASE       (0x0100) /* Terminal I/O ioctl commands */
            //#define _WDIOCBASE      (0x0200) /* Watchdog driver ioctl commands */
            //#define _FIOCBASE       (0x0300) /* File system ioctl commands */
            //#define _DIOCBASE       (0x0400) /* Character driver ioctl commands */
            //#define _BIOCBASE       (0x0500) /* Block driver ioctl commands */
            //#define _MTDIOCBASE     (0x0600) /* MTD ioctl commands */
            //#define _SIOCBASE       (0x0700) /* Socket ioctl commands */
            //#define _ARPIOCBASE     (0x0800) /* ARP ioctl commands */
            //#define _TSIOCBASE      (0x0900) /* Touchscreen ioctl commands */
            //#define _SNIOCBASE      (0x0a00) /* Sensor ioctl commands */
            //#define _ANIOCBASE      (0x0b00) /* Analog (DAC/ADC) ioctl commands */
            //#define _PWMIOCBASE     (0x0c00) /* PWM ioctl commands */
            //#define _CAIOCBASE      (0x0d00) /* CDC/ACM ioctl commands */
            //#define _BATIOCBASE     (0x0e00) /* Battery driver ioctl commands */
            //#define _QEIOCBASE      (0x0f00) /* Quadrature encoder ioctl commands */
            //#define _AUDIOIOCBASE   (0x1000) /* Audio ioctl commands */
            //#define _SLCDIOCBASE    (0x1100) /* Segment LCD ioctl commands */
            //#define _WLIOCBASE      (0x1200) /* Wireless modules ioctl network commands */
            //#define _WLCIOCBASE     (0x1300) /* Wireless modules ioctl character driver commands */
            //#define _CFGDIOCBASE    (0x1400) /* Config Data device (app config) ioctl commands */
            //#define _TCIOCBASE      (0x1500) /* Timer ioctl commands */
            //#define _DJOYBASE       (0x1600) /* Discrete joystick ioctl commands */
            //#define _AJOYBASE       (0x1700) /* Analog joystick ioctl commands */
            //#define _PIPEBASE       (0x1800) /* FIFO/pipe ioctl commands */
            //#define _RTCBASE        (0x1900) /* RTC ioctl commands */
            //#define _RELAYBASE      (0x1a00) /* Relay devices ioctl commands */
            //#define _CANBASE        (0x1b00) /* CAN ioctl commands */
            //#define _BTNBASE        (0x1c00) /* Button ioctl commands */
            //#define _ULEDBASE       (0x1d00) /* User LED ioctl commands */
            //#define _ZCBASE         (0x1e00) /* Zero Cross ioctl commands */
            //#define _LOOPBASE       (0x1f00) /* Loop device commands */
            //#define _MODEMBASE      (0x2000) /* Modem ioctl commands */
            //#define _I2CBASE        (0x2100) /* I2C driver commands */
            //#define _SPIBASE        (0x2200) /* SPI driver commands */
            //#define _GPIOBASE       (0x2300) /* GPIO driver commands */
            //#define _CLIOCBASE      (0x2400) /* Contactless modules ioctl commands */
            //#define _USBCBASE       (0x2500) /* USB-C controller ioctl commands */
            //#define _MAC802154BASE  (0x2600) /* 802.15.4 MAC ioctl commands */
            //#define _PWRBASE        (0x2700) /* Power-related ioctl commands */
            //#define _FBIOCBASE      (0x2800) /* Frame buffer character driver ioctl commands */

            /* boardctl() commands share the same number space */

            //#define _BOARDBASE      (0xff00) /* boardctl commands */

            /* Macros used to manage ioctl commands */

            //#define _IOC_MASK       (0x00ff)
            //#define _IOC_TYPE(cmd)  ((cmd) & ~_IOC_MASK)
            //#define _IOC_NR(cmd)    ((cmd) & _IOC_MASK)

            //#define _IOC(type,nr)   ((type)|(nr))

            //#define _IOC(type,nr)   ((type)|(nr))
            //#define _GPIOC(nr)        _IOC(_GPIOBASE,nr)
            //#define GPIOC_WRITE      _GPIOC(1)
            //#define GPIOC_READ       _GPIOC(2)
            //#define GPIOC_PINTYPE    _GPIOC(3)
            //#define GPIOC_REGISTER   _GPIOC(4)
            //#define GPIOC_UNREGISTER _GPIOC(5)

        }
    }
}
