using Gtk;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using System;

namespace Meadow.Graphics
{
    public class Display : IGraphicsDisplay
    {
        private Window _window;
        private SurfaceBuffer888 _pixelBuffer;

        static Display()
        {
            Application.Init();
        }

        public Display()
            : this(800, 600) // TODO: query screen size
        {
        }

        public Display(int width, int height)
        {
            _window = new Window(WindowType.Popup);
            _pixelBuffer = new SurfaceBuffer888(width, height);
            _window.WindowPosition = WindowPosition.Center;
            _window.DefaultSize = new Gdk.Size(width, height);

            _window.ShowAll();
            _window.Drawn += OnWindowDrawn;
        }

        public void Run()
        {
            Application.Run();
        }

        private void OnWindowDrawn(object o, DrawnArgs args)
        {
            args.Cr.SetSource(_pixelBuffer.Surface);
            args.Cr.Paint();

            if (args.Cr.GetTarget() is IDisposable d) d.Dispose();
            if (args.Cr is IDisposable cd) cd.Dispose();
            args.RetVal = true;
        }

        public IPixelBuffer PixelBuffer => _pixelBuffer;
        public ColorType ColorMode => _pixelBuffer.ColorMode;

        public int Width => _window.Window.Width;
        public int Height => _window.Window.Height;

        public void Show()
        {
            _window.QueueDraw();
        }

        public void Show(int left, int top, int right, int bottom)
        {
            _window.QueueDrawArea(left, top, right - left, bottom - top);
        }

        public void Clear(bool updateDisplay = false)
        {
            _pixelBuffer.Clear();
        }

        public void Fill(Foundation.Color fillColor, bool updateDisplay = false)
        {
            _pixelBuffer.Fill(fillColor);
        }

        public void Fill(int x, int y, int width, int height, Foundation.Color fillColor)
        {
            _pixelBuffer.Fill(x, y, width, height, fillColor);
        }

        public void DrawPixel(int x, int y, Foundation.Color color)
        {
            _pixelBuffer.SetPixel(x, y, color);
        }

        public void DrawPixel(int x, int y, bool colored)
        {
            DrawPixel(x, y, colored ? Foundation.Color.White : Foundation.Color.Black);
        }

        public void InvertPixel(int x, int y)
        {
            _pixelBuffer.InvertPixel(x, y);
        }

        public void WriteBuffer(int x, int y, IPixelBuffer displayBuffer)
        {
            _pixelBuffer.WriteBuffer(x, y, displayBuffer);
        }
    }
}
