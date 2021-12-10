using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Displays.TftSpi;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using System;

namespace PushButton_Sample
{
    public class MeadowApp : App<MeadowForLinux<RaspberryPiPinout>, MeadowApp>
    {
        private St7789 _display;
        private MicroGraphics _graphics;

        public MeadowApp()
        {
            Console.WriteLine("Initializing...");

            var display = ConfigureDisplay();

            _graphics = new MicroGraphics(display);
            _graphics.Rotation = RotationType._180Degrees;

            _graphics.Clear(true);

            DoDrawing(_graphics);

            Console.WriteLine("PushButton(s) ready!!!");
        }

        private IGraphicsDisplay ConfigureDisplay()
        {
            var config = new SpiClockConfiguration(
                new Meadow.Units.Frequency(48, Meadow.Units.Frequency.UnitType.Megahertz), 
                SpiClockConfiguration.Mode.Mode3);

            var spiBus = Device.CreateSpiBus(
                Device.Pins.SPI0_SCLK, 
                Device.Pins.SPI0_MOSI, 
                Device.Pins.SPI0_MISO, 
                config);

            _display = new St7789(
                device: Device,
                spiBus: spiBus,
                chipSelectPin: Device.Pins.GPIO25,
                dcPin: Device.Pins.GPIO27,
                resetPin: Device.Pins.GPIO22,
                width: 240, 
                height: 240, 
                displayColorMode: ColorType.Format16bppRgb565);

            return _display;
        }

        private void DoDrawing(MicroGraphics graphics)
        {
            graphics.DrawRectangle(120, 0, 120, 220, Color.White, true);
            graphics.DrawRectangle(0, 0, 120, 20, Color.Red, true);
            graphics.DrawRectangle(0, 20, 120, 20, Color.Purple, true);
            graphics.DrawRectangle(0, 40, 120, 20, Color.Blue, true);
            graphics.DrawRectangle(0, 60, 120, 20, Color.Green, true);
            graphics.DrawRectangle(0, 80, 120, 20, Color.Yellow, true);
            graphics.DrawRectangle(0, 100, 120, 20, Color.Orange, true);

            graphics.Show();
        }
    }
}
