using System;
namespace Meadow.Hardware
{
    public abstract class PinBase : IPin
    {
        public string Name { get; protected set; }
        public uint Address { get; protected set; }

        protected PinBase(string name, uint address)
        {
            this.Name = name;
            this.Address = address;
        }
    }
}
