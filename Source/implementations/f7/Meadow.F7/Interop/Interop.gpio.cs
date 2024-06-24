using System;

namespace Meadow.Core
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
                public PinDesignator PinDesignator;
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

            private const int GPIO_MODE_SHIFT = 18;     /* Bits 18-19: GPIO port mode */
            private const int GPIO_PUPD_SHIFT = 16;     /* Bits 16-17: Pull-up/pull down */
            private const int GPIO_AF_SHIFT = 12;       /* Bits 12-15: Alternate function */
            private const int GPIO_SPEED_SHIFT = 10;    /* Bits 10-11: GPIO frequency selection */
            private const int GPIO_PORT_SHIFT = 4;      /* Bit 4-7:  Port number */
            private const int GPIO_PIN_SHIFT = 0;       /* Bits 0-3: GPIO number: 0-15 */

            [Flags]
            public enum GPIOConfigFlags
            {
                ModeInput = 0 << GPIO_MODE_SHIFT,
                ModeOutput = 1 << GPIO_MODE_SHIFT,
                ModeAlternate = 2 << GPIO_MODE_SHIFT,
                ModeAnalog = 3 << GPIO_MODE_SHIFT,

                ResistorFloat = 0 << GPIO_PUPD_SHIFT,
                ResistorPullUp = 1 << GPIO_PUPD_SHIFT,
                ResistorPullDown = 2 << GPIO_PUPD_SHIFT,

                AlternateFunction0 = 0 << GPIO_AF_SHIFT,
                AlternateFunction1 = 1 << GPIO_AF_SHIFT,
                AlternateFunction2 = 2 << GPIO_AF_SHIFT,
                AlternateFunction3 = 3 << GPIO_AF_SHIFT,
                AlternateFunction4 = 4 << GPIO_AF_SHIFT,
                AlternateFunction5 = 5 << GPIO_AF_SHIFT,
                AlternateFunction6 = 6 << GPIO_AF_SHIFT,
                AlternateFunction7 = 7 << GPIO_AF_SHIFT,
                AlternateFunction8 = 8 << GPIO_AF_SHIFT,
                AlternateFunction9 = 9 << GPIO_AF_SHIFT,
                AlternateFunction10 = 10 << GPIO_AF_SHIFT,
                AlternateFunction11 = 11 << GPIO_AF_SHIFT,
                AlternateFunction12 = 12 << GPIO_AF_SHIFT,
                AlternateFunction13 = 13 << GPIO_AF_SHIFT,
                AlternateFunction14 = 14 << GPIO_AF_SHIFT,
                AlternateFunction15 = 15 << GPIO_AF_SHIFT,
                AlternateFunction16 = 16 << GPIO_AF_SHIFT,

                Speed2MHz = 0 << GPIO_SPEED_SHIFT,
                Speed25MHz = 1 << GPIO_SPEED_SHIFT,
                Speed50MHz = 2 << GPIO_SPEED_SHIFT,
                Speed100MHz = 3 << GPIO_SPEED_SHIFT,

                OutputOpenDrain = 1 << 9,

                OutputInitialValueHigh = 1 << 8,
                OutputInitialValueLow = 0 << 8,

                InputInterruptEnable = 1 << 8,

                // *** IMPORTANT DEV NOTE ***
                // These should never change, but if they do, you *must* modify the 
                // PinDesignator enum to match
                PortA = 0 << GPIO_PORT_SHIFT,
                PortB = 1 << GPIO_PORT_SHIFT,
                PortC = 2 << GPIO_PORT_SHIFT,
                PortD = 3 << GPIO_PORT_SHIFT,
                PortE = 4 << GPIO_PORT_SHIFT,
                PortF = 5 << GPIO_PORT_SHIFT,
                PortG = 6 << GPIO_PORT_SHIFT,
                PortH = 7 << GPIO_PORT_SHIFT,
                PortI = 8 << GPIO_PORT_SHIFT,
                PortJ = 9 << GPIO_PORT_SHIFT,
                PortK = 10 << GPIO_PORT_SHIFT,

                Pin0 = 0 << GPIO_PIN_SHIFT,
                Pin1 = 1 << GPIO_PIN_SHIFT,
                Pin2 = 2 << GPIO_PIN_SHIFT,
                Pin3 = 3 << GPIO_PIN_SHIFT,
                Pin4 = 4 << GPIO_PIN_SHIFT,
                Pin5 = 5 << GPIO_PIN_SHIFT,
                Pin6 = 6 << GPIO_PIN_SHIFT,
                Pin7 = 7 << GPIO_PIN_SHIFT,
                Pin8 = 8 << GPIO_PIN_SHIFT,
                Pin9 = 9 << GPIO_PIN_SHIFT,
                Pin10 = 10 << GPIO_PIN_SHIFT,
                Pin11 = 11 << GPIO_PIN_SHIFT,
                Pin12 = 12 << GPIO_PIN_SHIFT,
                Pin13 = 13 << GPIO_PIN_SHIFT,
                Pin14 = 14 << GPIO_PIN_SHIFT,
                Pin15 = 15 << GPIO_PIN_SHIFT
            }

            [Flags]
            public enum PinDesignator
            {
                PortA = GPIOConfigFlags.PortA,
                PortB = GPIOConfigFlags.PortB,
                PortC = GPIOConfigFlags.PortC,
                PortD = GPIOConfigFlags.PortD,
                PortE = GPIOConfigFlags.PortE,
                PortF = GPIOConfigFlags.PortF,
                PortG = GPIOConfigFlags.PortG,
                PortH = GPIOConfigFlags.PortH,
                PortI = GPIOConfigFlags.PortI,
                PortJ = GPIOConfigFlags.PortJ,
                PortK = GPIOConfigFlags.PortK,

                Pin0 = GPIOConfigFlags.Pin0,
                Pin1 = GPIOConfigFlags.Pin1,
                Pin2 = GPIOConfigFlags.Pin2,
                Pin3 = GPIOConfigFlags.Pin3,
                Pin4 = GPIOConfigFlags.Pin4,
                Pin5 = GPIOConfigFlags.Pin5,
                Pin6 = GPIOConfigFlags.Pin6,
                Pin7 = GPIOConfigFlags.Pin7,
                Pin8 = GPIOConfigFlags.Pin8,
                Pin9 = GPIOConfigFlags.Pin9,
                Pin10 = GPIOConfigFlags.Pin10,
                Pin11 = GPIOConfigFlags.Pin11,
                Pin12 = GPIOConfigFlags.Pin12,
                Pin13 = GPIOConfigFlags.Pin13,
                Pin14 = GPIOConfigFlags.Pin14,
                Pin15 = GPIOConfigFlags.Pin15,
            }
        }
    }
}