using System;
namespace Meadow.Hardware
{
    public class SpiChannel : DigitalChannelBase, ISpiChannel
    {
        public SpiChannel(string name,
        bool pullDownCapable = false,
        bool pullUpCapable = false)
            : base (
                name,
                inputCapable: true,
                outputCapable: true,
                interruptCapable: false, // ?? i mean, technically, yes, but will we have events?
                pullDownCapable: pullDownCapable,
                pullUpCapable: pullUpCapable) //TODO: switch to C# 7.2+ to get rid of trailing names
        { 
        }
    }
}
