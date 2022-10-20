using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Hardware;
using Meadow.Pinouts;
using System;
using System.Threading;

namespace PushButton_Sample
{
    public class MeadowApp : App<MeadowForLinux<RaspberryPi>>
    {
        private St7789 _display;
        private MicroGraphics _graphics;

        public MeadowApp()
        {
            Console.WriteLine("Initializing...");

            var display = ConfigureDisplay();

            _graphics = new MicroGraphics(display);
            _graphics.Rotation = RotationType._180Degrees;

            DoDrawing(_graphics);

            Console.WriteLine("PushButton(s) ready!!!");
        }

        private IGraphicsDisplay ConfigureDisplay()
        {
            var config = new SpiClockConfiguration(
                new Meadow.Units.Frequency(48, Meadow.Units.Frequency.UnitType.Megahertz),
                SpiClockConfiguration.Mode.Mode3);

            try
            {
                var spiBus = Device.CreateSpiBus(
                    Device.Pins.SPI0_SCLK,
                    Device.Pins.SPI0_MOSI,
                    Device.Pins.SPI0_MISO,
                    config);

                _display = new St7789(
                    device: Device,
                    spiBus: spiBus,
                    chipSelectPin: Device.Pins.GPIO25, // A03
                    dcPin: Device.Pins.GPIO27, // A04
                    resetPin: Device.Pins.GPIO22, // A05
                    width: 240,
                    height: 240);
            }
            catch (NativeException e)
            {
                Console.WriteLine($"NativeException: {e.Message} [{e.ErrorCode}]");
            }

            return _display;
        }

        private void DoDrawing(MicroGraphics graphics, int sleepDuration = 500)
        {
            Console.WriteLine("Clearing...");

            graphics.Clear(true);

            graphics.DrawRectangle(120, 0, 120, 220, Color.White, true);
            graphics.DrawRectangle(0, 0, 120, 20, Color.Red, true);
            graphics.DrawRectangle(0, 20, 120, 20, Color.Purple, true);
            graphics.DrawRectangle(0, 40, 120, 20, Color.Blue, true);
            graphics.DrawRectangle(0, 60, 120, 20, Color.Green, true);
            graphics.DrawRectangle(0, 80, 120, 20, Color.Yellow, true);
            graphics.DrawRectangle(0, 120, 120, 20, Color.Orange, true);

            Console.WriteLine("Show");

            graphics.Show();

            Thread.Sleep(sleepDuration);

            while (true)
            {
                PathTest();
                Thread.Sleep(sleepDuration);

                LineTest();
                Thread.Sleep(sleepDuration);

                PolarLineTest();
                Thread.Sleep(sleepDuration);

                RoundRectTest();
                Thread.Sleep(sleepDuration);

                QuadrantTest();
                Thread.Sleep(sleepDuration);

                StrokeTest();
                Thread.Sleep(sleepDuration);

                ShapeTest();
                Thread.Sleep(sleepDuration);

                FontScaleTest();
                Thread.Sleep(sleepDuration);

                FontAlignmentTest();
                Thread.Sleep(sleepDuration);

                ColorFontTest();
                Thread.Sleep(sleepDuration);

                CircleTest();
                Thread.Sleep(sleepDuration);

                InvertTest();
                Thread.Sleep(sleepDuration);
            }
        }

        void PathTest()
        {
            var pathSin = new GraphicsPath();
            var pathCos = new GraphicsPath();

            for (int i = 0; i < 48; i++)
            {
                if (i == 0)
                {
                    pathSin.MoveTo(0, 120 + (int)(Math.Sin(i * 10 * Math.PI / 180) * 100));
                    pathCos.MoveTo(0, 120 + (int)(Math.Cos(i * 10 * Math.PI / 180) * 100));
                    continue;
                }

                pathSin.LineTo(i * 5, 120 + (int)(Math.Sin(i * 10 * Math.PI / 180) * 100));
                pathCos.LineTo(i * 5, 120 + (int)(Math.Cos(i * 10 * Math.PI / 180) * 100));
            }

            _graphics.Clear();

            _graphics.Stroke = 3;
            _graphics.DrawLine(0, 120, 240, 120, Color.White);
            _graphics.DrawPath(pathSin, Color.Cyan);
            _graphics.DrawPath(pathCos, Color.LawnGreen);

            _graphics.Show();
        }

        void FontAlignmentTest()
        {
            _graphics.Clear();

            _graphics.DrawText(120, 0, "Left aligned", Color.Blue);
            _graphics.DrawText(120, 16, "Center aligned", Color.Green, ScaleFactor.X1, TextAlignment.Center);
            _graphics.DrawText(120, 32, "Right aligned", Color.Red, ScaleFactor.X1, TextAlignment.Right);

            _graphics.DrawText(120, 64, "Left aligned", Color.Blue, ScaleFactor.X2);
            _graphics.DrawText(120, 96, "Center aligned", Color.Green, ScaleFactor.X2, TextAlignment.Center);
            _graphics.DrawText(120, 128, "Right aligned", Color.Red, ScaleFactor.X2, TextAlignment.Right);

            _graphics.Show();
        }

        void InvertTest()
        {
            _graphics.CurrentFont = new Font12x16();
            _graphics.Clear();

            string msg = "Cursor test";
            string msg2 = "$123.456";

            _graphics.DrawText(0, 1, msg, WildernessLabsColors.AzureBlue);
            _graphics.DrawRectangle(0, 16, 12 * msg2.Length, 16, WildernessLabsColors.AzureBlueDark, true);
            _graphics.DrawText(0, 16, msg2, WildernessLabsColors.ChileanFire);

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    _graphics.InvertRectangle(i * 12, 0, 12, 16);

                    _graphics.Show();
                    Thread.Sleep(50);
                }
            }

            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    _graphics.InvertRectangle(i * 12, 16, 12, 16);

                    _graphics.Show();
                    Thread.Sleep(50);
                }
            }
        }

        void LineTest()
        {
            Console.WriteLine("Horizonal lines");

            _graphics.Clear();

            for (int i = 1; i < 10; i++)
            {
                _graphics.Stroke = i;
                _graphics.DrawHorizontalLine(5, 20 * i, (int)(_graphics.Width - 10), Color.Red);
            }
            _graphics.Show();
            Thread.Sleep(1500);

            _graphics.Clear();
            Console.WriteLine("Horizonal lines (negative)");
            for (int i = 1; i < 10; i++)
            {
                _graphics.Stroke = i;
                _graphics.DrawHorizontalLine((int)_graphics.Width - 5, 20 * i, (int)(10 - _graphics.Width), Color.Green);
            }
            _graphics.Show();
            Thread.Sleep(1500);
            _graphics.Clear();

            Console.WriteLine("Vertical lines");

            _graphics.Clear();

            for (int i = 1; i < 10; i++)
            {
                _graphics.Stroke = i;
                _graphics.DrawVerticalLine(20 * i, 5, (int)(_graphics.Height - 10), Color.Orange);
            }
            _graphics.Show();
            Thread.Sleep(1500);
            _graphics.Clear();

            Console.WriteLine("Vertical lines (negative)");
            for (int i = 1; i < 10; i++)
            {
                _graphics.Stroke = i;
                _graphics.DrawVerticalLine(20 * i, (int)(_graphics.Height - 5), (int)(10 - _graphics.Width), Color.Blue);
            }
            _graphics.Show();
            Thread.Sleep(1500);
        }

        void PolarLineTest()
        {
            _graphics.Clear();
            _graphics.Stroke = 3;

            for (int i = 0; i < 270; i += 12)
            {
                _graphics.DrawLine(120, 120, 80, (float)(i * Math.PI / 180), Color.White);
            }

            _graphics.Show();
        }

        void RoundRectTest()
        {
            _graphics.Clear();

            _graphics.Stroke = 1;

            _graphics.DrawRoundedRectangle(10, 10, 200, 200, 20, WildernessLabsColors.ChileanFire, false);

            _graphics.DrawRoundedRectangle(40, 40, 100, 60, 20, WildernessLabsColors.AzureBlue, true);

            _graphics.DrawRoundedRectangle(100, 70, 60, 60, 20, WildernessLabsColors.PearGreen, true);

            _graphics.Show();
        }

        void QuadrantTest()
        {
            _graphics.Clear();

            _graphics.DrawCircleQuadrant(120, 120, 110, 0, Color.Yellow, true);
            _graphics.DrawCircleQuadrant(120, 120, 110, 1, Color.Blue, true);
            _graphics.DrawCircleQuadrant(120, 120, 110, 2, Color.Cyan, true);
            _graphics.DrawCircleQuadrant(120, 120, 110, 3, Color.LawnGreen, true);

            _graphics.Show();
        }

        void CircleTest()
        {
            _graphics.Clear();

            _graphics.Stroke = 1;
            _graphics.DrawCircle(120, 120, 20, Color.Purple);

            _graphics.Stroke = 2;
            _graphics.DrawCircle(120, 120, 30, Color.Red);

            _graphics.Stroke = 3;
            _graphics.DrawCircle(120, 120, 40, Color.Orange);

            _graphics.Stroke = 4;
            _graphics.DrawCircle(120, 120, 50, Color.Yellow);

            _graphics.Stroke = 5;
            _graphics.DrawCircle(120, 120, 60, Color.LawnGreen);

            _graphics.Stroke = 6;
            _graphics.DrawCircle(120, 120, 70, Color.Cyan);

            _graphics.Stroke = 7;
            _graphics.DrawCircle(120, 120, 80, Color.Blue);

            _graphics.Show();
        }

        void ShapeTest()
        {
            _graphics.Clear();

            _graphics.DrawCircle(60, 60, 20, Color.Purple);
            _graphics.DrawRectangle(10, 10, 30, 60, Color.Red);
            _graphics.DrawTriangle(20, 20, 10, 70, 60, 60, Color.Green);

            _graphics.DrawCircle(90, 60, 20, Color.Cyan, true);
            _graphics.DrawRectangle(100, 100, 30, 10, Color.Yellow, true);
            _graphics.DrawTriangle(120, 20, 110, 70, 160, 60, Color.Pink, true);

            _graphics.DrawLine(10, 120, 110, 130, Color.SlateGray);

            _graphics.Show();
        }

        void StrokeTest()
        {
            _graphics.Clear();

            _graphics.Stroke = 1;
            _graphics.DrawLine(5, 5, 115, 5, Color.SteelBlue);
            _graphics.Stroke = 2;
            _graphics.DrawLine(5, 25, 115, 25, Color.SteelBlue);
            _graphics.Stroke = 3;
            _graphics.DrawLine(5, 45, 115, 45, Color.SteelBlue);
            _graphics.Stroke = 4;
            _graphics.DrawLine(5, 65, 115, 65, Color.SteelBlue);
            _graphics.Stroke = 5;
            _graphics.DrawLine(5, 85, 115, 85, Color.SteelBlue);

            _graphics.Stroke = 1;
            _graphics.DrawLine(135, 5, 135, 115, Color.SlateGray);
            _graphics.Stroke = 2;
            _graphics.DrawLine(155, 5, 155, 115, Color.SlateGray);
            _graphics.Stroke = 3;
            _graphics.DrawLine(175, 5, 175, 115, Color.SlateGray);
            _graphics.Stroke = 4;
            _graphics.DrawLine(195, 5, 195, 115, Color.SlateGray);
            _graphics.Stroke = 5;
            _graphics.DrawLine(215, 5, 215, 115, Color.SlateGray);

            _graphics.Stroke = 1;
            _graphics.DrawLine(5, 125, 115, 235, Color.Silver);
            _graphics.Stroke = 2;
            _graphics.DrawLine(25, 125, 135, 235, Color.Silver);
            _graphics.Stroke = 3;
            _graphics.DrawLine(45, 125, 155, 235, Color.Silver);
            _graphics.Stroke = 4;
            _graphics.DrawLine(65, 125, 175, 235, Color.Silver);
            _graphics.Stroke = 5;
            _graphics.DrawLine(85, 125, 195, 235, Color.Silver);

            _graphics.Stroke = 2;
            _graphics.DrawRectangle(2, 2, (int)(_graphics.Width - 4), (int)(_graphics.Height - 4), Color.DimGray, false);

            _graphics.Show();
        }

        void FontScaleTest()
        {
            _graphics.CurrentFont = new Font12x20();

            _graphics.Clear();

            _graphics.DrawText(0, 0, "2x Scale", Color.Blue, ScaleFactor.X2);

            _graphics.DrawText(0, 48, "12x20 Font", Color.Green, ScaleFactor.X2);

            _graphics.DrawText(0, 96, "0123456789", Color.Yellow, ScaleFactor.X2);

            _graphics.DrawText(0, 144, "!@#$%^&*()", Color.Orange, ScaleFactor.X2);

            _graphics.DrawText(0, 192, "3x!", Color.OrangeRed, ScaleFactor.X3);

            _graphics.DrawText(0, 240, "Meadow!", Color.Red, ScaleFactor.X2);

            _graphics.DrawText(0, 288, "B4.2", Color.Violet, ScaleFactor.X2);

            _graphics.Show();
        }

        void ColorFontTest()
        {
            _graphics.CurrentFont = new Font8x12();

            _graphics.Clear();

            _graphics.DrawTriangle(120, 20, 200, 100, 120, 100, Meadow.Foundation.Color.Red, false);

            _graphics.DrawRectangle(140, 30, 40, 90, Meadow.Foundation.Color.Yellow, false);

            _graphics.DrawCircle(160, 80, 40, Meadow.Foundation.Color.Cyan, false);

            int indent = 5;
            int spacing = 14;
            int y = indent;

            _graphics.DrawText(indent, y, "Meadow F7 SPI ST7789!!");

            _graphics.DrawText(indent, y += spacing, "Red", Meadow.Foundation.Color.Red);

            _graphics.DrawText(indent, y += spacing, "Purple", Meadow.Foundation.Color.Purple);

            _graphics.DrawText(indent, y += spacing, "BlueViolet", Meadow.Foundation.Color.BlueViolet);

            _graphics.DrawText(indent, y += spacing, "Blue", Meadow.Foundation.Color.Blue);

            _graphics.DrawText(indent, y += spacing, "Cyan", Meadow.Foundation.Color.Cyan);

            _graphics.DrawText(indent, y += spacing, "LawnGreen", Meadow.Foundation.Color.LawnGreen);

            _graphics.DrawText(indent, y += spacing, "GreenYellow", Meadow.Foundation.Color.GreenYellow);

            _graphics.DrawText(indent, y += spacing, "Yellow", Meadow.Foundation.Color.Yellow);

            _graphics.DrawText(indent, y += spacing, "Orange", Meadow.Foundation.Color.Orange);

            _graphics.DrawText(indent, y += spacing, "Brown", Meadow.Foundation.Color.Brown);

            _graphics.Show();

            Console.WriteLine("Show complete");
        }
    }
}
