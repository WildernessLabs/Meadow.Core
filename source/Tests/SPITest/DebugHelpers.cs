using System;
using System.Text;

namespace SPITest
{
    public class DebugHelpers
    {
        /// <summary>
        ///     Convert a byte array to a series of hexadecimal numbers
        ///     separated by a minus sign.
        /// </summary>
        /// <param name="bytes">Array of bytes to convert.</param>
        /// <returns>series of hexadecimal bytes in the format xx-yy-zz</returns>
        public static string Hexadecimal(byte[] bytes)
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
        ///     Convert a byte into the hex representation of the value.
        /// </summary>
        /// <param name="b">Value to convert.</param>
        /// <returns>Two hexadecimal digits representing the byte.</returns>
        private static string HexadecimalDigits(byte b)
        {
            char[] digits = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b', 'c', 'd', 'e', 'f' };
            return "" + digits[b >> 4] + digits[b & 0xf];
        }

        /// <summary>
        ///     Convert a byte into hexadecimal including the "0x" prefix.
        /// </summary>
        /// <param name="b">Value to convert.</param>
        /// <returns>Hexadecimal string including the 0x prefix.</returns>
        public static string Hexadecimal(byte b)
        {
            return "0x" + HexadecimalDigits(b);
        }

        /// <summary>
        ///     Convert an unsigned short into hexadecimal.
        /// </summary>
        /// <param name="us">Unsigned short value to convert.</param>
        /// <returns>Hexadecimal representation of the unsigned short.</returns>
        public static string Hexadecimal(ushort us)
        {
            return "0x" + HexadecimalDigits((byte)((us >> 8) & 0xff)) + HexadecimalDigits((byte)(us & 0xff));
        }

        /// <summary>
        ///     Convert an integer into hexadecimal.
        /// </summary>
        /// <param name="i">Integer to convert to hexadecimal.</param>
        /// <returns>Hexadecimal representation of the unsigned short.</returns>
        public static string Hexadecimal(int i)
        {
            return "0x" + HexadecimalDigits((byte)((i >> 24) & 0xff)) + HexadecimalDigits((byte)((i >> 16) & 0xff)) +
                   HexadecimalDigits((byte)((i >> 8) & 0xff)) + HexadecimalDigits((byte)(i & 0xff));
        }

        /// <summary>
        ///     Dump the array of bytes to the debug output in hexadecimal.
        /// </summary>
        /// <param name="buffer">Byte array of the buffer to be converted to printable format.</param>
        public static string PrintableBuffer(byte[] buffer)
        {
            StringBuilder result = new StringBuilder();
            string line = string.Empty;
            result.AppendLine("             0    1    2    3    4    5    6    7    8    9    a    b    c    d    e    f");
            for (var index = 0; index < buffer.Length; index++)
            {
                if ((index % 16) == 0)
                {
                    if (line != string.Empty)
                    {
                        result.AppendLine(line);
                    }
                    line = Hexadecimal(index) + ": ";
                }
                line += Hexadecimal(buffer[index]) + " ";
            }
            if (line != string.Empty)
            {
                result.AppendLine(line);
            }
            return (result.ToString());
        }

        public DebugHelpers()
        {
        }
    }
}
