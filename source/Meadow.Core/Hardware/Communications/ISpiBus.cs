using System;
using System.Collections.Generic;

namespace Meadow.Hardware
{
    public enum ChipSelectMode
    {
        ActiveLow,
        ActiveHigh
    }

    public interface ISpiBus
    {
        long[] SupportedSpeeds { get; }

        SpiClockConfiguration Configuration { get; }

        void SendData(IDigitalOutputPort chipSelect, params byte[] data);
        void SendData(IDigitalOutputPort chipSelect, IEnumerable<byte> data);
        byte[] ReceiveData(IDigitalOutputPort chipSelect, int numberOfBytes);
        byte[] ExchangeData(IDigitalOutputPort chipSelect, params byte[] dataToWrite);

        void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] data);
        void SendData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, IEnumerable<byte> data);
        byte[] ReceiveData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, int numberOfBytes);
        byte[] ExchangeData(IDigitalOutputPort chipSelect, ChipSelectMode csMode, params byte[] dataToWrite);
    }
}
