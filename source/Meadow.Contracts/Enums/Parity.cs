namespace Meadow.Hardware
{
    /// <summary>
    /// Specifies the parity bit for a SerialPort object.
    /// </summary>
    public enum Parity
    {
        /// <summary>
        /// No parity check occurs.
        /// </summary>
        None,
        /// <summary>
        /// Sets the parity bit so that the count of bits set is an odd number.
        /// </summary>
        Odd,
        /// <summary>
        /// Sets the parity bit so that the count of bits set is an even number.
        /// </summary>
        Even,
    }

}
