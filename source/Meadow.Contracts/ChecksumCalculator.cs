using System;
using System.Text;

namespace Meadow.Utilities
{
    /// <summary>
    /// Provides CRC and checksum methods
    /// </summary>
    public static class ChecksumCalculator
    {
        /// <summary>
        ///     Lookup table for the polynomial CRC 8 method.
        /// </summary>
        private static byte[]? _lookupTable = null;

        /// <summary>
        ///     When the _lookupTable is not null then _polynomial will contain the 
        ///     value of the byte used to generate the lookup table for the PolynomialCRC8
        ///     method.
        /// </summary>
        private static byte _polynomial;


        /// <summary>
        ///     Calculate a checksum for the string by XORing the bytes in the string.
        /// </summary>
        /// <param name="data">String to calculate the checksum for.</param>
        /// <returns>XOR checksum for the string.</returns>
        public static byte XOR(string data/*, Encoding encoding = Encoding.UTF8*/)
        {
            // TODO: should we pass in the encoding?
            // tried, gives an error that its not a compile time constant. :(
            return XOR(Encoding.UTF8.GetBytes(data));
        }

        /// <summary>
        ///     Generate a checksum by XORing all of the data in the array.
        /// </summary>
        /// <param name="data">Data to calculate the checksum for.</param>
        /// <returns>XOR Checksum of the array of bytes.</returns>
        public static byte XOR(byte[] data)
        {
            byte checksum = 0;
            for (var index = 0; index < data.Length; index++) {
                checksum ^= data[index];
            }
            return checksum;
        }

        /// <summary>
        ///     Generte the lookup table for the PolynomialCRC method.
        /// </summary>
        private static void PopulateLookupTable(byte polynomial)
        {
            _lookupTable = new byte[256];

            for (int outer = 0; outer < 256; outer++) {
                int temp = outer;
                for (int inner = 0; inner < 8; inner++) {
                    if ((temp & 0x80) != 0) {
                        temp = (temp << 1) ^ polynomial;
                    } else {
                        temp <<= 1;
                    }
                }
                _lookupTable[outer] = (byte)temp;
            }
            _polynomial = polynomial;
        }

        /// <summary>
        ///     Calculate the 8-bit CRC using the specified polynomial.
        /// </summary>
        /// <param name="data">Data bytes to generate a CRC for.</param>
        /// <param name="polynomial">Polynomial byte to use in the CRC calculation.</param>
        public static byte PolynomialCRC(byte[] data, byte polynomial)
        {
            if ((data == null) || (data.Length == 0)) {
                throw new ArgumentException("data", "PolynomialCRC: Data to CRC is invalid.");
            }
            if ((_lookupTable == null) || ((_lookupTable != null) && (_polynomial != polynomial))) {
                PopulateLookupTable(polynomial);
            }
            if (_lookupTable != null) // shouldn't ever happen due to above, but prevents compiler warning
            {
                byte crc = 0;
                foreach (byte b in data)
                {
                    crc = _lookupTable[crc ^ b];
                }
                return crc;
            }
            return 0;
        }
    }
}
