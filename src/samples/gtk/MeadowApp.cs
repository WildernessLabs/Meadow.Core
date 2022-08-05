using Meadow.Foundation;
using Meadow.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
    public class MeadowApp : App<Simulation.SimulatedMeadow<Simulation.SimulatedPinout>>
    {
        private Display _display;
        private ExecutionContext? _ctx;

        public MeadowApp()
        {
        }

        public override async Task Run()
        {
            _display = new Meadow.Graphics.Display();
            _ = Task.Run(() => Updater());
            _display.Run();
        }

        private void DrawStuff(object? o)
        {
            _display.Fill(RandomColor());
            _display.Fill(100, 100, _display.Width - 200, _display.Height - 200, RandomColor());
            var linecolor = RandomColor();
            for (int x = 0; x < _display.Width; x++)
            {
                _display.DrawPixel(x, _display.Height / 2, linecolor);
            }
            _display.Show();
        }

        public void Updater()
        {
            while (true)
            {
                InvokeOnMainThread(DrawStuff);
                Thread.Sleep(1000);
            }
        }

        public Color RandomColor()
        {
            return new Color(Random.Shared.NextDouble(), Random.Shared.NextDouble(), Random.Shared.NextDouble());
        }
    }
}
