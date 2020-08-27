using System;
namespace Meadow.Hardware
{
    public class DigitalChannelInfo : DigitalChannelInfoBase
    {
        public DigitalChannelInfo(
            string name,
            bool inputCapable = true,
            bool outputCapable = true,
            bool interruptCapable = true,
            bool pullDownCapable = true,
            bool pullUpCapable = true,
            bool inverseLogic = false,
            int interruptGroup = 0
        ) 
            : base(name, inputCapable, outputCapable, interruptCapable,
                pullDownCapable, pullUpCapable, inverseLogic, interruptGroup)
        {
        }
    }
}
