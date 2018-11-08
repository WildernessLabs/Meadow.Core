﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Meadow.Hardware
{
    public class SPI : IDisposable
    {
        public enum SPI_module
        {
            SPI1 = 0,
            SPI2 = 1,
            SPI3 = 2,
            SPI4 = 3
        }

        public SPI(Configuration config)
        {

        }

        ~SPI() { throw new NotImplementedException(); }

        public Configuration Config { get; set; }

        public void Dispose() { throw new NotImplementedException(); }
        public void Write(ushort[] writeBuffer) { throw new NotImplementedException(); }
        public void Write(byte[] writeBuffer) { throw new NotImplementedException(); }
        public void WriteRead(ushort[] writeBuffer, ushort[] readBuffer) { throw new NotImplementedException(); }
        public void WriteRead(byte[] writeBuffer, byte[] readBuffer) { throw new NotImplementedException(); }
        public void WriteRead(ushort[] writeBuffer, ushort[] readBuffer, int startReadOffset) { throw new NotImplementedException(); }
        public void WriteRead(byte[] writeBuffer, byte[] readBuffer, int startReadOffset) { throw new NotImplementedException(); }
        public void WriteRead(ushort[] writeBuffer, int writeOffset, int writeCount, ushort[] readBuffer, int readOffset, int readCount, int startReadOffset) { throw new NotImplementedException(); }
        public void WriteRead(byte[] writeBuffer, int writeOffset, int writeCount, byte[] readBuffer, int readOffset, int readCount, int startReadOffset) { throw new NotImplementedException(); }


        public class Configuration
        {
            public readonly Cpu.Pin BusyPin;
            public readonly bool BusyPin_ActiveState;
            public readonly bool ChipSelect_ActiveState;
            public readonly uint ChipSelect_HoldTime;
            public readonly Cpu.Pin ChipSelect_Port;
            public readonly uint ChipSelect_SetupTime;
            public readonly bool Clock_Edge;
            public readonly bool Clock_IdleState;
            public readonly uint Clock_RateKHz;
            public readonly SPI_module SPI_mod;

            public Configuration(Pins ChipSelect_Port, bool ChipSelect_ActiveState, uint ChipSelect_SetupTime, uint ChipSelect_HoldTime, bool Clock_IdleState, bool Clock_Edge, uint Clock_RateKHz, SPI_module SPI_mod)
            { throw new NotImplementedException(); }
            public Configuration(Pins ChipSelect_Port, bool ChipSelect_ActiveState, uint ChipSelect_SetupTime, uint ChipSelect_HoldTime, bool Clock_IdleState, bool Clock_Edge, uint Clock_RateKHz, SPI_module SPI_mod, Cpu.Pin BusyPin, bool BusyPin_ActiveState)
            { throw new NotImplementedException(); }
        }
    }
}
