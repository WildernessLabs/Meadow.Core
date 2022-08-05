using Meadow.Foundation;
using System.Threading.Tasks;

namespace Meadow
{
    public class MeadowApp : App<Simulation.SimulatedMeadow<Simulation.SimulatedPinout>>
    {
        public MeadowApp()
        {
        }

        public override async Task Run()
        {
            var display = new Meadow.Graphics.Display();
            display.Fill(Color.Red);
            display.Fill(100, 100, display.Width - 200, display.Height - 200, Color.Blue);
            for (int x = 0; x < display.Width; x++)
            {
                display.DrawPixel(x, display.Height / 2, Color.Green);
            }

            display.Run();
        }
    }
}
