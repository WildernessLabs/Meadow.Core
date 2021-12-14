using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    public interface ISpiBus
    {
        /// <summary>
        /// Gets an array of all of the speeds that the SPI bus supports.
        /// </summary>
        Units.Frequency[] SupportedSpeeds { get; }

        SpiClockConfiguration Configuration { get; }

        //==== new hotness
        /// <summary>
        /// Reads data from the SPI bus into the buffer.
        /// </summary>
        /// <param name="chipSelect">Port to use as the chip select to activate the bus.</param>
        /// <param name="readBuffer">Data to write</param>
        /// <param name="csMode">Describes which level on the chip select activates the peripheral.</param>
        void Read(IDigitalOutputPort? chipSelect,
            Span<byte> readBuffer,
            ChipSelectMode csMode = ChipSelectMode.ActiveLow);

        /// <summary>
        /// Writes data to the SPI bus
        /// </summary>
        /// <param name="chipSelect">Port to use as the chip select to activate the peripheral.</param>
        /// <param name="writeBuffer">Data to write</param>
        /// <param name="csMode">Describes which level on the chip select activates the peripheral.</param>
        void Write(
            IDigitalOutputPort? chipSelect,
            Span<byte> writeBuffer,
            ChipSelectMode csMode = ChipSelectMode.ActiveLow);

        /// <summary>
        /// Writes data from the write buffer to a peripheral on the bus while
        /// at the same time reading return data into the read buffer.
        /// </summary>
        /// <param name="chipSelect">Port to use as the chip select to activate the peripheral.</param>
        /// <param name="writeBuffer">Buffer to read data from.</param>
        /// <param name="readBuffer">Buffer to read returning data into.</param>
        /// <param name="csMode">Describes which level on the chip select activates the peripheral.</param>
        void Exchange(
            IDigitalOutputPort? chipSelect,
            Span<byte> writeBuffer, Span<byte> readBuffer,
            ChipSelectMode csMode = ChipSelectMode.ActiveLow);

        //==== OLD AND BUSTED //TODO: Delete after M.Foundation update

        [Obsolete("Use the `Span<byte>` overload instead.", true)]
        void SendData(IDigitalOutputPort chipSelect, params byte[] data);
        [Obsolete("Use the `Span<byte>` overload instead.", true)]
        void SendData(IDigitalOutputPort chipSelect, IEnumerable<byte> data);
        [Obsolete("Use the `Span<byte>` overload instead.", true)]
        byte[] ReceiveData(IDigitalOutputPort chipSelect, int numberOfBytes);

        [Obsolete("Use the `Span<byte>` overload instead.", true)]
        void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] data);
        [Obsolete("Use the `Span<byte>` overload instead.", true)]
        void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, IEnumerable<byte> data);

        [Obsolete("Use the `Span<byte>` overload instead.", true)]
        byte[] ReceiveData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, int numberOfBytes);

        [Obsolete("Use the `Span<byte>` overload instead.", true)]
        void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer);
        [Obsolete("Use the `Span<byte>` overload instead.")]
        void ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, byte[] sendBuffer, byte[] receiveBuffer, int bytesToExchange);
    }
}
