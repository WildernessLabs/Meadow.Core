using System;
using System.Threading.Tasks;

namespace Meadow
{
    public interface IPWMPort : IPort
    {
        void Start();
        void Stop();

        // TODO: correct type? should be UInt?
        double Duration { get; set; }
        double Period { get; set; }

        // TODO: correct type?
        double DutyCycle { get; set; }
        double Frequency { get; set; }

        double Scale { get; set; }

    }
}
