using Gtk;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using System;

namespace Meadow.Graphics
{
    public class Display : Window, IGraphicsDisplay
    {
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
            : base(WindowType.Popup)
        {
            _pixelBuffer = new SurfaceBuffer888(width, height);
            WindowPosition = WindowPosition.Center;
            DefaultSize = new Gdk.Size(width, height);

            ShowAll();
        }

        public void Run()
        {
            Application.Run();
        }

        protected override bool OnDrawn(Cairo.Context cr)
        {
            cr.SetSource(_pixelBuffer.Surface);
            cr.Paint();

            if (cr.GetTarget() is IDisposable d) d.Dispose();
            if (cr is IDisposable cd) cd.Dispose();

            return true;
        }

        public IPixelBuffer PixelBuffer => _pixelBuffer;
        public ColorType ColorMode => ColorType.Format16bppRgb565;

        public int Width => base.Window.Width;
        public int Height => base.Window.Height;




        void IGraphicsDisplay.Show()
        {
            this.Show();
        }

        void IGraphicsDisplay.Show(int left, int top, int right, int bottom)
        {
            base.QueueDrawArea(left, top, right - left, bottom - top);
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
            throw new NotImplementedException();
        }

        public void InvertPixel(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void WriteBuffer(int x, int y, IPixelBuffer displayBuffer)
        {
            throw new NotImplementedException();
        }
    }
}
