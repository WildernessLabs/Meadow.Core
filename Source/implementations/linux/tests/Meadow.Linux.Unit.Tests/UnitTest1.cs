using System;
using Xunit;

namespace M4L.Unit.Tests
{
    public class IoctlEncodingTests
    {
        // 1075866368 MSG     4020 6B00
        // 2147576577 RD_MODE 8001 6B01
        // 1073834753 WR_MODE 4001 6B01
        // 2147576578 RD_LSB  8001 6B02
        // 1073834754 WR_LSB  4001 6B02
        // 2147576579 RD_BITS 8001 6B03
        // 1073834755 WR_BITS 4001 6B03
        // 2147773188 RD_SPD  8004 6B04
        // 1074031364 WR_SPD  4004 6B04
        // 2147773189 RD_MD32 8004 6B05
        // 2147773189 WR_MD32 4004 6B05

        [Fact]
        public void Test1()
        {
            var modeIoctl = Meadow.Interop._IOW('k', 1, 8);

            var ioctl = Meadow.Interop._IOW('k', 3, 32);
        }
    }
}
