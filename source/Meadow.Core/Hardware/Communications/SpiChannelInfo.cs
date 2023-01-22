namespace Meadow.Hardware
{
    public class SpiChannelInfo : DigitalChannelInfoBase, ISpiChannelInfo
    {
        public SpiLineType LineTypes { get; protected set; }

        public SpiChannelInfo(string name,
            SpiLineType lineTypes,
            bool pullDownCapable = false,
            bool pullUpCapable = false)
            : base(
                name,
                inputCapable: true,
                outputCapable: true,
                interruptCapable: false, // ?? i mean, technically, yes, but will we have events?
                pullDownCapable: pullDownCapable,
                pullUpCapable: pullUpCapable,
                inverseLogic: false) //TODO: switch to C# 7.2+ to get rid of trailing names
        {
            LineTypes = lineTypes;
        }
    }
}
