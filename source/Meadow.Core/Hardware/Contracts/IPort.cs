namespace Meadow.Hardware
{
    //TODO: add IDisposable
    public interface IPort<C> where C : IChannelInfo //: IDisposable
    {
        PortDirectionType Direction { get; }
        SignalType SignalType { get; }
        C Channel { get; }
        IPin Pin { get; }
    }
}
