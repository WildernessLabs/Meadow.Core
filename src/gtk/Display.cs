using Cairo;
using Gtk;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using System;
using System.Buffers.Binary;

namespace Meadow.Graphics
{
    public class Display : IGraphicsDisplay
    {
        private Window _window;
        private IPixelBuffer _pixelBuffer;
        private Cairo.Format _format;
        private int _stride;

        static Display()
        {
            Application.Init();
        }

        public Display(ColorType mode = ColorType.Format24bppRgb888)
        {
            Initialize(800, 600, mode); // TODO: query screen size and caps
        }

        public Display(int width, int height, ColorType mode = ColorType.Format24bppRgb888)
        {
            Initialize(width, height, mode);
        }

        private void Initialize(int width, int height, ColorType mode)
        {
            _window = new Window(WindowType.Popup);

            switch (mode)
            {
                case ColorType.Format24bppRgb888:
                    _pixelBuffer = new BufferRgb888(width, height);
                    _format = Cairo.Format.Rgb24;
                    break;
                case ColorType.Format16bppRgb565:
                    _pixelBuffer = new BufferRgb565(width, height);
                    _format = Cairo.Format.Rgb16565;
                    break;
                case ColorType.Format32bppRgba8888:
                    _pixelBuffer = new BufferRgb8888(width, height);
                    _format = Cairo.Format.Argb32;
                    break;
                default:
                    throw new Exception($"Mode {mode} not supported");
            }
            _stride = GetStrideForBitDepth(width, _pixelBuffer.BitDepth);

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
            ConvertRGBBufferToBGRBuffer(_pixelBuffer.Buffer);

            using (var surface = new ImageSurface(_pixelBuffer.Buffer, _format, Width, Height, _stride))
            {
                args.Cr.SetSource(surface);
                args.Cr.Paint();

                if (args.Cr.GetTarget() is IDisposable d) d.Dispose();
                if (args.Cr is IDisposable cd) cd.Dispose();
                args.RetVal = true;
            }
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


        // This attempts to copy the cairo method for getting stride length
        // as defined here:
        // https://github.com/freedesktop/cairo/blob/a934fa66dba2b880723f4e5c3fdea92cbe0207e7/src/cairoint.h#L1553
        // #define CAIRO_STRIDE_FOR_WIDTH_BPP(w,bpp) \
        // ((((bpp)*(w)+7)/8 + CAIRO_STRIDE_ALIGNMENT-1) & -CAIRO_STRIDE_ALIGNMENT)
        private int GetStrideForBitDepth(int width, int bpp)
        {
            var cairo_stride_alignment = sizeof(UInt32);
            var stride = ((((bpp) * (width) + 7) / 8 + cairo_stride_alignment - 1) & -cairo_stride_alignment);
            return stride;
        }



        private void ConvertRGBBufferToBGRBuffer(byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i += 2)
            {
                // convert two bytes into a short
                ushort pixel = (ushort)(buffer[i] << 8 | buffer[i + 1]);

                // reverse the endianness because that's what cairo expects
                BinaryPrimitives.ReverseEndianness(pixel);

                // separate back into two bytes
                buffer[i] = (byte)pixel;
                buffer[i + 1] = (byte)(pixel >> 8);
            }
        }
    }
}
