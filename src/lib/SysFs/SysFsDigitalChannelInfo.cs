using Meadow.Hardware;

namespace Meadow
{
    public class SysFsDigitalChannelInfo : DigitalChannelInfoBase
    {
        public SysFsDigitalChannelInfo(
            string name)
            : base(name, true, true, true, false, false, false, null)
        {
        }
    }
}
