# Building a `Meadow.Windows` application

The Meadow.Core stack now includes support for running on desktop Windows hosts.

## Prerequisites

- An Adafruit FT232H (or similar)
- A Windows desktop machine with Visual Studio 2022

## Creating a sample `Meadow.Windows` application

- Create a .NET 7 Core Console Application
- Add NuGet references to the following
  - Meadow.Windows version 0.95.0
  - Meadow.Foundation.ICs.IOExpanders.Ft232h version 0.95.0
- Add the `libmpsse.dll` native driver to your project (for the appropriate architecture) and set it to `Copy Always`
- Replace the default `Program.cs` code with the following

```
using Meadow;
using Meadow.Foundation.ICs.IOExpanders;
using Meadow.Hardware;

public class MeadowApp : App<Windows>
{
    private Ft232h _expander = new Ft232h();
    private IDigitalOutputPort _c0;

    public static async Task Main(string[] _)
    {
        await MeadowOS.Start();
    }

    public override Task Initialize()
    {
        Console.WriteLine("Creating Outputs");

        _c0 = _expander.Pins.C0.CreateDigitalOutputPort();

        return Task.CompletedTask;
    }

    public override Task Run()
    {
        while (true)
        {
            _c0.State = !_c0.State;
            Thread.Sleep(1);
        }
    }
}
```

## Creating a `Meadow.Windows` application with UI support

`Meadow.Windows` also supports using a Meadow.Foundation `IDisplay` implemented on WinForms.  To create a simple HMI application, do the following:

- Create a .NET 7 Core Console Application
- Add NuGet references to the following
  - Meadow.Windows version 0.95.0
  - Meadow.Foundation.Displays.WinForms version 0.95.0
  - Meadow.Foundation.Graphics.MicroGraphics
  - Meadow.Foundation.Sensors.Hid.Keyboard
- Replace the default `Program.cs` code with the following

```
using Meadow;
using Meadow.Foundation;
using Meadow.Foundation.Displays;
using Meadow.Foundation.Graphics;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Foundation.Sensors.Hid;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

public class MeadowApp : App<Windows>
{
    public static async Task Main(string[] args)
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        ApplicationConfiguration.Initialize();

        await MeadowOS.Start();
    }

    private MicroGraphics _graphics = default!;
    private WinFormsDisplay _display = default!;
    private Keyboard _keyBoard = default!;
    private int _xDirection;
    private int _yDirection;
    private int _speed = 5;
    private int _x;
    private int _y;
    private int _radius = 10;

    public override async Task Run()
    {
        _ = Task.Run(() =>
        {
            Thread.Sleep(1000);
            DisplayLoop();
        });

        Application.Run(_display);
    }

    public override Task Initialize()
    {
        _display = new WinFormsDisplay();
        _graphics = new MicroGraphics(_display);
        _keyBoard = new Keyboard();

        var rightButton = new PushButton(_keyBoard.CreateDigitalInputPort(_keyBoard.Pins.Right));
        var leftButton = new PushButton(_keyBoard.CreateDigitalInputPort(_keyBoard.Pins.Left));
        var upButton = new PushButton(_keyBoard.CreateDigitalInputPort(_keyBoard.Pins.Up));
        var downButton = new PushButton(_keyBoard.CreateDigitalInputPort(_keyBoard.Pins.Down));

        rightButton.PressStarted += (s, e) => { _xDirection = 1; };
        leftButton.PressStarted += (s, e) => { _xDirection = -1; };
        upButton.PressStarted += (s, e) => { _yDirection = -1; };
        downButton.PressStarted += (s, e) => { _yDirection = 1; };

        rightButton.PressEnded += (s, e) => { _xDirection = _yDirection = 0; };
        leftButton.PressEnded += (s, e) => { _xDirection = _yDirection = 0; };
        upButton.PressEnded += (s, e) => { _xDirection = _yDirection = 0; };
        downButton.PressEnded += (s, e) => { _xDirection = _yDirection = 0; };

        _x = _display.Width / 2;
        _y = _display.Height / 2;

        return base.Initialize();
    }

    private void MoveAndDrawCircle()
    {
        var x = _x + (_speed * _xDirection);
        var y = _y + (_speed * _yDirection);

        // check for edge
        if (x - _radius > 0 && x + _radius < _display.Width && x != _x)
        {
            _x = x;
        }
        if (y - _radius > 0 && y + _radius < _display.Height && y != _y)
        {
            _y = y;
        }

        _graphics.DrawCircle(_x, _y, _radius, Color.Yellow, filled: true);
    }

    void DisplayLoop()
    {
        while (true)
        {

            _display.Invoke(() =>
            {
                // Do your drawing stuff here
                _graphics.Clear();

                MoveAndDrawCircle();

                _graphics.Show();
            });

            Thread.Sleep(33);
        }
    }
}

```

## State of Support

`Meadow.Windows` is currently in an alpha/beta state.  There are several things in the `Meadow.Core` stack that are implemented, but there are many that are not, so it is not unusual or unexpected to hit scenarios that throw `PlatformNotSupportedException`s.

The `FT232H` driver currently supports digital I/O and SPI, but *not* I2C.