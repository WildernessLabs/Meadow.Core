using System;
namespace Meadow.Hardware
{
    public class DigitalChannelnfo : IDigitalChannelInfo
    {
        public DigitalChannelnfo()
        {
        }

        public bool InterrruptCapable { get; set; } = false;
        public bool PullDownCapable { get; set; } = false;
        public bool PullUpCapable { get; set; } = false;
    }
}
