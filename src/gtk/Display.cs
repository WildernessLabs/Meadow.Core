using Gtk;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using System;

namespace Meadow.Graphics
{
    public class Display : Window, IGraphicsDisplay
    {
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
            cr.LineWidth = 9;
            cr.SetSourceRGB(0.7, 0.2, 0.0);

            int width, height;
            width = Allocation.Width;
            height = Allocation.Height;

            cr.Rectangle(0, 0, width, height);
            cr.SetSourceRGB(0, 0, 0);
            cr.Fill();

            cr.Translate(width / 2, height / 2);
            cr.Arc(0, 0, (width < height ? width : height) / 2 - 10, 0, 2 * Math.PI);
            cr.StrokePreserve();

            cr.SetSourceRGB(0.3, 0.4, 0.6);
            cr.Fill();

            if (cr.GetTarget() is IDisposable d) d.Dispose();
            if (cr is IDisposable cd) cd.Dispose();

            return true;
        }

        public IPixelBuffer PixelBuffer => throw new System.NotImplementedException();
        public ColorType ColorMode => ColorType.Format16bppRgb565;

        int IGraphicsDisplay.Width => base.Window.Width;
        int IGraphicsDisplay.Height => base.Window.Height;




        void IGraphicsDisplay.Show()
        {
            ((IGraphicsDisplay)this).Show(0, 0, ((IGraphicsDisplay)this).Width, ((IGraphicsDisplay)this).Height);
        }

        void IGraphicsDisplay.Show(int left, int top, int right, int bottom)
        {
            base.QueueDrawArea(left, top, right - left, bottom - top);
        }

        public void Clear(bool updateDisplay = false)
        {
            throw new NotImplementedException();
        }

        public void Fill(Foundation.Color fillColor, bool updateDisplay = false)
        {
            throw new NotImplementedException();
        }

        public void Fill(int x, int y, int width, int height, Foundation.Color fillColor)
        {
            throw new NotImplementedException();
        }

        public void DrawPixel(int x, int y, Foundation.Color color)
        {
            throw new NotImplementedException();
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
