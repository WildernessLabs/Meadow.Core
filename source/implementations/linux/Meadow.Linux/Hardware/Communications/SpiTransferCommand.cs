﻿using System.Runtime.InteropServices;

namespace Meadow
{
    public partial class SpiBus
    {
        [StructLayout(LayoutKind.Auto, Pack = 1)]
        private struct SpiTransferCommand
        {
            /*
            struct spi_ioc_transfer {
                __u64       tx_buf;
                __u64       rx_buf;

                __u32       len;
                __u32       speed_hz;

                __u16       delay_usecs;
                __u8        bits_per_word;
                __u8        cs_change;
                __u32       pad;
            };                
            */

            public ulong TransmitBuffer { get; set; } // 64-bit pointer to byte array
            public ulong ReceiveBuffer { get; set; } // 64-bit pointer to byte array
            public int Length { get; set; }
            public int SpeedHz { get; set; }
            public short DelayuSec { get; set; }
            public byte BitsPerWord { get; set; }
            public byte ChipSelectChange { get; set; }
            public int Pad { get; set; }
        }
    }
}
