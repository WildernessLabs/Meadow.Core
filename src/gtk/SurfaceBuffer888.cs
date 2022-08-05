using Cairo;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using System;

namespace Meadow.Graphics
{
    public class SurfaceBuffer888 : IPixelBuffer
    {
        private ImageSurface _surface;

        public Surface Surface => _surface;

        public int Width => _surface.Width;
        public int Height => _surface.Height;
        public ColorType ColorMode => ColorType.Format24bppRgb888;
        public int BitDepth => 24;
        public int ByteCount => _surface.Data.Length;
        public byte[] Buffer => _surface.Data;

        public SurfaceBuffer888(int width, int height)
        {
            _surface = new ImageSurface(Format.Rgb24, width, height);
        }

        public void Foo()
        {
            using (var cr = new Context(_surface))
            {
                var width = _surface.Width;
                var height = _surface.Height;

                // fill an area
                cr.Rectangle(0, 0, width, height);
                cr.SetSourceRGB(0, 1, 0);
                cr.Fill();

                // draw a pixel (can't yet find a better way)
                cr.SetSourceRGB(1, 0, 0);
                cr.Rectangle(400, 300, 1, 1);
                cr.Fill();
            }
        }

        public void Clear()
        {
            Fill(Foundation.Color.Black);
        }

        public void Fill(Foundation.Color color)
        {
            Fill(0, 0, Width, Height, color);
        }

        public void Fill(int originX, int originY, int width, int height, Foundation.Color color)
        {
            using (var cr = new Context(_surface))
            {
                cr.SetSourceRGB(color.R, color.G, color.B);
                cr.Rectangle(originX, originY, width, height);
                cr.Fill();
            }
        }

        public void SetPixel(int x, int y, Foundation.Color color)
        {
            using (var cr = new Context(_surface))
            {
                // draw a pixel (can't yet find a better way)
                cr.SetSourceRGB(color.R, color.G, color.B);
                cr.Rectangle(x, y, 1, 1);
                cr.Fill();
            }
        }

        public Foundation.Color GetPixel(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void InvertPixel(int x, int y)
        {
            throw new NotImplementedException();
        }

        public void WriteBuffer(int originX, int originY, IPixelBuffer buffer)
        {
            throw new NotImplementedException();
        }
    }
}
