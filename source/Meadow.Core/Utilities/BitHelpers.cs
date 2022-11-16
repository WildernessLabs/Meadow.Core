namespace Meadow.Utilities
{
    public static class BitHelpers
    {
        /// <summary>
        /// Returns a new byte mask based on the input mask, with a single 
        /// bit set. To the passed in value.
        /// </summary>
        /// <param name="mask">The original byte mask value.</param>
        /// <param name="bitIndex">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit. Should be 0 or 1.</param>
        public static byte SetBit(byte mask, byte bitIndex, byte value)
        {
            return SetBit(mask, bitIndex, (value == 0) ? false : true);
        }

        /// <summary>
        /// Returns a new byte mask based on the input mask, with a single 
        /// bit set. To the passed in value.
        /// </summary>
        /// <param name="mask">The original byte mask value.</param>
        /// <param name="bitIndex">The index of the bit to set.</param>
        /// <param name="value">The value to set the bit. true for 1, false for 0.</param>
        /// <returns></returns>
        public static byte SetBit(byte mask, byte bitIndex, bool value)
        {
            byte newMask = mask;

            if (value)
            {
                newMask |= (byte)(1 << bitIndex);
            }
            else
            {
                newMask &= (byte)~(1 << bitIndex); // tricky to zero
            }

            return newMask;
        }

        /// <summary>
        /// Returns a new 16-bit short with the single bit set or cleared
        /// </summary>
        /// <param name="mask">The original value</param>
        /// <param name="bitIndex">The index of the bit to affect</param>
        /// <param name="value">True to set, False to clear</param>
        /// <returns></returns>
        public static short SetBit(short mask, byte bitIndex, bool value)
        {
            short b = mask;
            if (value)
            {
                return (short)(b | (short)(1 << bitIndex));
            }
            return (short)(b & (short)(~(1 << bitIndex)));
        }

        /// <summary>
        /// Returns the value of the mask at the given ordinal.
        /// </summary>
        /// <param name="mask"></param>
        /// <param name="bitIndex"></param>
        /// <returns></returns>
        public static bool GetBitValue(byte mask, byte bitIndex)
        {

            return ((mask & (byte)(1 << bitIndex)) != 0) ? true : false;
        }

        /// <summary>
        /// Determines if a specified bit in a 16-bit short is set
        /// </summary>
        /// <param name="mask">The value to check</param>
        /// <param name="bitIndex">The index of the bit to check</param>
        /// <returns></returns>
        public static bool GetBitValue(short mask, byte bitIndex)
        {
            if ((mask & (short)(1 << bitIndex)) == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Converts the first 2 bytes of an array to a little-endian 16-bit short
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static short ToInt16(byte[] data)
        {
            return (short)(data[0] | (data[1] << 8));
        }
    }
}
