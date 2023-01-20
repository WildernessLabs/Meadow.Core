using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Hardware;

namespace Meadow.Graphics;

public class WinFormsDisplay : Form, IGraphicsDisplay, ITouchScreen
{
    public event TouchEventHandler TouchDown;
    public event TouchEventHandler TouchUp;
    public event TouchEventHandler TouchClick;

    public ColorType ColorMode => PixelBuffer.ColorMode;
    public IPixelBuffer PixelBuffer { get; private set; }

    public WinFormsDisplay()
    {

        this.Width = 800;
        this.Height = 600;
        PixelBuffer = new BufferRgb888(Width, Height);
    }

    void IGraphicsDisplay.Show()
    {
        var g = this.CreateGraphics();

        throw new NotImplementedException();
    }

    void IGraphicsDisplay.Show(int left, int top, int right, int bottom)
    {
        throw new NotImplementedException();
    }

    void IGraphicsDisplay.Clear(bool updateDisplay)
    {
        throw new NotImplementedException();
    }

    void IGraphicsDisplay.Fill(Foundation.Color fillColor, bool updateDisplay)
    {
        throw new NotImplementedException();
    }

    void IGraphicsDisplay.Fill(int x, int y, int width, int height, Foundation.Color fillColor)
    {
        throw new NotImplementedException();
    }

    void IGraphicsDisplay.DrawPixel(int x, int y, Foundation.Color color)
    {
        throw new NotImplementedException();
    }

    void IGraphicsDisplay.DrawPixel(int x, int y, bool enabled)
    {
        throw new NotImplementedException();
    }

    void IGraphicsDisplay.InvertPixel(int x, int y)
    {
        throw new NotImplementedException();
    }

    void IGraphicsDisplay.WriteBuffer(int x, int y, IPixelBuffer displayBuffer)
    {
        throw new NotImplementedException();
    }
}