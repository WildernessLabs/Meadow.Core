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
            //            display.Fill(Color.Red);

            display.Run();
        }
    }
}
