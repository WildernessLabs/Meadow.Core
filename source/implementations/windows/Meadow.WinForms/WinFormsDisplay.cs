using Meadow.Foundation.Graphics;
using Meadow.Foundation.Graphics.Buffers;
using Meadow.Hardware;

namespace Meadow.Graphics;

public class WinFormsDisplay : Form, IGraphicsDisplay, ITouchScreen
{
    public event TouchEventHandler TouchDown;
    public event TouchEventHandler TouchUp;
    public event TouchEventHandler TouchClick;

    private WinFormsPixelBuffer _buffer;

    public ColorType ColorMode => PixelBuffer.ColorMode;
    public IPixelBuffer PixelBuffer => _buffer;

    public WinFormsDisplay(int width = 800, int height = 600)
    {
        this.Width = width;
        this.Height = height;

        this.DoubleBuffered = true;

        _buffer = new WinFormsPixelBuffer(Width, Height);
    }

    void IGraphicsDisplay.Show()
    {
        this.Invalidate(true);
        this.Update();
    }

    void IGraphicsDisplay.Show(int left, int top, int right, int bottom)
    {
        this.Invalidate(new Rectangle(left, top, right - left, bottom - top), true);
    }

    void IGraphicsDisplay.Clear(bool updateDisplay)
    {
        _buffer.Clear();
        if (updateDisplay)
        {
            this.Show();
        }
    }

    void IGraphicsDisplay.Fill(Foundation.Color fillColor, bool updateDisplay)
    {
        _buffer.Fill(fillColor);
        if (updateDisplay)
        {
            this.Show();
        }
    }

    void IGraphicsDisplay.Fill(int x, int y, int width, int height, Foundation.Color fillColor)
    {
        _buffer.Fill(x, y, width, height, fillColor);
    }

    void IGraphicsDisplay.DrawPixel(int x, int y, Foundation.Color color)
    {
        _buffer.SetPixel(x, y, color);
    }

    void IGraphicsDisplay.DrawPixel(int x, int y, bool enabled)
    {
        _buffer.SetPixel(x, y, enabled ? Foundation.Color.White : Foundation.Color.Black);
    }

    void IGraphicsDisplay.InvertPixel(int x, int y)
    {
        _buffer.InvertPixel(x, y);
    }

    void IGraphicsDisplay.WriteBuffer(int x, int y, IPixelBuffer displayBuffer)
    {
        _buffer.WriteBuffer(x, y, displayBuffer);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.DrawImage(_buffer.Image, 0, 0);
        base.OnPaint(e);
    }
}