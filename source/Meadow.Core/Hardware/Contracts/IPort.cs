namespace Meadow.Hardware
{
    //TODO: add IDisposable
    public interface IPort //: IDisposable
    {
        PortDirectionType Direction { get; }
        SignalType SignalType { get; } 
    }
}
