using Meadow.Foundation;
using Meadow.Foundation.Graphics;
using Meadow.Graphics;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow
{
    public class MeadowApp : App<Simulation.SimulatedMeadow<Simulation.SimulatedPinout>>
    {
        private MicroGraphics _graphics;
        private Display _display;
        private bool _useGraphics = true;

        public MeadowApp()
        {
        }

        public override async Task Run()
        {
            _display = new Meadow.Graphics.Display();
            _graphics = new MicroGraphics(_display);
            _ = Task.Run(() => Updater());
            _display.Run();
        }

        private void DrawStuff(object? o)
        {
            if (_useGraphics)
            {
                // use MicroGraphics
                _graphics.Clear(RandomColor());
                _graphics.DrawRectangle(100, 100, _display.Width - 200, _display.Height - 200, RandomColor(), true);
                _graphics.DrawLine(0, _display.Height / 3, _display.Width, _display.Height / 3, RandomColor());
                _graphics.Show();
            }
            else
            {
                // use only display primitives
                _display.Fill(RandomColor());
                _display.Fill(100, 100, _display.Width - 200, _display.Height - 200, RandomColor());

                var linecolor = RandomColor();

                for (int x = 0; x < _display.Width; x++)
                {
                    _display.DrawPixel(x, _display.Height / 2, linecolor);
                }
                _display.Show();
            }
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
