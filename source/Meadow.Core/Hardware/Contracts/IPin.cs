using System;
namespace Meadow.Hardware
{
    public interface IPin
    {
        string Name { get; }
        uint Address { get; }
    }
}
