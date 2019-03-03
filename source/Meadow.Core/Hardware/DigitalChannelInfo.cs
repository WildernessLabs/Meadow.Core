using System;
namespace Meadow.Hardware
{
    public class DigitalChannelInfo : DigitalChannelIInfoBase
    {
        public DigitalChannelInfo(
            string name,
            bool inputCapable = true,
            bool outputCapable = true,
            bool interruptCapable = true,
            bool pullDownCapable = true,
            bool pullUpCapable = true) 
            : base(name, inputCapable, outputCapable, interruptCapable,
                pullDownCapable, pullUpCapable)
        {
        }
    }
}
