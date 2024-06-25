using System.Diagnostics;
using System.Text;

namespace Meadow.Devices
{
    /// <summary>
    /// Provides utility methods for outputting debug information.
    /// </summary>
    public static class Output
    {
        /// <summary>
        /// Header string for a 16-byte buffer plus address prefix.
        /// </summary>
        private const string HEXADECIMAL_BUFFER_HEADER = "             0    1    2    3    4    5    6    7    8    9    a    b    c    d    e    f";

        /// <summary>
        /// Writes the specified string to the log if the test condition is true (conditional debug output).
        /// </summary>
        /// <param name="test">The condition to test.</param>
        /// <param name="value">The value to write to the log.</param>
        [Conditional("DEBUG")]
        public static void WriteIf(bool test, string value)
        {
            if (test)
            {
                Resolver.Log.Info(value);
            }
        }

        /// <summary>
        /// Writes the specified string followed by a line terminator to the log if the test condition is true (conditional debug output).
        /// </summary>
        /// <param name="test">The condition to test.</param>
        /// <param name="value">The value to write to the log.</param>
        [Conditional("DEBUG")]
        public static void WriteLineIf(bool test, string value)
        {
            if (test)
            {
                Resolver.Log.Info(value);
            }
        }

        /// <summary>
        /// Writes the specified string to the log (conditional debug output).
        /// </summary>
        /// <param name="value">The value to write to the log.</param>
        [Conditional("DEBUG")]
        public static void Write(string value)
        {
            Resolver.Log.Info(value);
        }

        /// <summary>
        /// Writes the specified string followed by a line terminator to the log (conditional debug output).
        /// </summary>
        /// <param name="value">The value to write to the log.</param>
        [Conditional("DEBUG")]
        public static void WriteLine(string value)
        {
            Resolver.Log.Info(value);
        }

        /// <summary>
        /// Convert a byte array to a series of hexadecimal numbers
        /// separated by a minus sign.
        /// </summary>
        /// <param name="bytes">Array of bytes to convert.</param>
        /// <returns>series of hexadecimal bytes in the format xx-yy-zz</returns>
        private static string Hexadecimal(byte[] bytes)
        {
            string result = string.Empty;

            for (byte index = 0; index < bytes.Length; index++)
            {
                if (index > 0)
                {
                    result += "-";
                }
                result += HexadecimalDigits(bytes[index]);
            }

            return (result);
        }

        /// <summary>
        /// Convert a byte into the hex representation of the value.
        /// </summary>
        /// <param name="b">Value to convert.</param>
        /// <returns>Two hexadecimal digits representing the byte.</returns>
        private static string HexadecimalDigits(byte b)
        {
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
            return "" + digits[b >> 4] + digits[b & 0xf];
        }

        /// <summary>
        /// Convert a byte into hexadecimal including the "0x" prefix.
        /// </summary>
        /// <param name="b">Value to convert.</param>
        /// <returns>Hexadecimal string including the 0x prefix.</returns>
        private static string Hexadecimal(byte b)
        {
            return "0x" + HexadecimalDigits(b);
        }

        /// <summary>
        /// Convert an unsigned short into hexadecimal.
        /// </summary>
        /// <param name="us">Unsigned short value to convert.</param>
        /// <returns>Hexadecimal representation of the unsigned short.</returns>
        private static string Hexadecimal(ushort us)
        {
            return "0x" + HexadecimalDigits((byte)((us >> 8) & 0xff)) + HexadecimalDigits((byte)(us & 0xff));
        }

        /// <summary>
        /// Convert an integer into hexadecimal.
        /// </summary>
        /// <param name="i">Integer to convert to hexadecimal.</param>
        /// <returns>Hexadecimal representation of the unsigned short.</returns>
        private static string Hexadecimal(int i)
        {
            return Hexadecimal((uint)i);
        }

        /// <summary>
        /// Convert an unsigned integer into hexadecimal.
        /// </summary>
        /// <param name="i">Unsigned integer to convert to hexadecimal.</param>
        /// <returns>Hexadecimal representation of the unsigned short.</returns>
        private static string Hexadecimal(uint i)
        {
            return "0x" + HexadecimalDigits((byte)((i >> 24) & 0xff)) + HexadecimalDigits((byte)((i >> 16) & 0xff)) +
                   HexadecimalDigits((byte)((i >> 8) & 0xff)) + HexadecimalDigits((byte)(i & 0xff));
        }

        /// <summary>
        /// Generate a single line or printable output from a buffer.
        /// </summary>
        /// <param name="address">Offset into the buffer to start processing.</param>
        /// <param name="buffer">Buffer to be used as a source of data.</param>
        /// <returns>String containing the hex address and up to 16 bytes of data from the buffer.</returns>
        private static string BufferLine(int address, byte[] buffer)
        {
            int end = address + 16;
            StringBuilder result = new StringBuilder(HEXADECIMAL_BUFFER_HEADER.Length);

            if (buffer.Length <= end)
            {
                end = buffer.Length;
            }
            result.Append(Hexadecimal(address));
            result.Append(": ");
            for (var index = address; index < end; index++)
            {
                result.Append(Hexadecimal(buffer[index]));
                result.Append(" ");
            }

            return (result.ToString());
        }

        /// <summary>
        /// Output the buffer in hexadecimal if the condition is met.
        /// </summary>
        /// <param name="test">Determine if the method should generate any output.</param>
        /// <param name="buffer">Byte array of the buffer to be converted to printable format.</param>
        public static void BufferIf(bool test, byte[] buffer)
        {
            BufferIf(test, buffer, 0, buffer.Length);
        }

        /// <summary>
        /// Output the buffer in hexadecimal if the condition is met.
        /// </summary>
        /// <param name="test">Determine if the method should generate any output.</param>
        /// <param name="buffer">Byte array of the buffer to be converted to printable format.</param>
        /// <param name="offset">Offset into the buffer to start the data display.</param>
        /// <param name="length">Amount of data to display.</param>
        public static void BufferIf(bool test, byte[] buffer, int offset, int length)
        {
            if (test)
            {
                WriteLine(HEXADECIMAL_BUFFER_HEADER);
                for (var index = offset; index < length; index += 16)
                {
                    WriteLine(BufferLine(index, buffer));
                }
            }
        }

        /// <summary>
        /// Output the buffer in hexadecimal.
        /// </summary>
        /// <param name="buffer">Byte array of the buffer to be converted to printable format.</param>
        public static void Buffer(byte[] buffer)
        {
            BufferIf(true, buffer);
        }
    }

    /* ===== MEADOW GPIO PIN MAP =====
        BOARD PIN   SCHEMATIC       CPU PIN   MDW NAME  ALT FN   IMPLEMENTED?
        J301-1      RESET
        J301-2      3.3
        J301-3      VREF
        J301-4      GND
        J301-5      DAC_OUT1        PA4         A0
        J301-6      DAC_OUT2        PA5         A1
        J301-7      ADC1_IN3        PA3         A2
        J301-8      ADC1_IN7        PA7         A3
        J301-9      ADC1_IN10       PC0         A4
        J301-10     ADC1_IN11       PC1         A5
        J301-11     SPI3_CLK        PC10        SCK
        J301-12     SPI3_MOSI       PB5         MOSI    AF6
        J301-13     SPI3_MISO       PC11        MISO    AF6
        J301-14     UART4_RX        PI9         D00     AF8
        J301-15     UART4_TX        PH13        D01     AF8
        J301-16     PC6             PC6         D02                 *
        J301-17     CAN1_RX         PB8         D03     AF9
        J301-18     CAN1_TX         PB9         D04     AF9

        J302-4      PE3             PE3         D15
        J302-5      PG3             PG3         D14
        J302-6      USART1_RX       PB15        D13     AF4
        J302-7      USART1_TX       PB14        D12     AF4
        J302-8      PC9             PC9         D11
        J302-9      PH10            PH10        D10
        J302-10     PB1             PB1         D09
        J302-11     I2C1_SCL        PB6         D08     AF4
        J302-12     I2C1_SDA        PB7         D07     AF4
        J302-13     PB0             PB0         D06
        J302-14     PC7             PC7         D05

        LED_B       PA0
        LED_G       PA1
        LED_R       PA2
    */
}
