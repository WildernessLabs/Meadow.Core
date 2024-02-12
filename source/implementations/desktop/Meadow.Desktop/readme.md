# `Meadow.Desktop`

`Meadow.Desktop` is a cross-platform abstraction for using the Meadow software stack on Desktop operating systems (Windows, Mac and Linux).

## Buses and Peripherals

Since most desktop machines do not have GPIO or typical embedded communication buses, in order to communicate with SPI or I2C devices you must use an IO Expander such as the FT232H or CP2112.

For example, creating an I2C periperhal using an expander might look like this:

```
var expanders = new FT232Collection();
var i2cBus = expanders[0].CreateI2cBus();
var peripheral = new Bme280(i2cBus)
```

## Prototyping embedded user itnerfaces

If you are prototyping a user interface that uses a Meadow.Foundation `IPixelDisplay`, then `Meadow.Desktop` provides a cross-platform display simulation.  The simulated display is available through the `Device.Display` property.

For example, if you are using MicroLayout as your layout engine, you can initialize a screen with the following:

```
var screen = new DisplayScreen(Device.Display);
```

In all cases, the display requires an underlying rendering loop to be run to keep the display updated, and your application must call that rendering loop.  The simplest way to do this is:

```
public override Task Run()
{
    // NOTE: this will not return until the display is closed
    ExecutePlatformDisplayRunner();

    return Task.CompletedTask;
}

```

### Windows

On Windows machines, the `IPixelDisplay` uses WinForms under the hood.  This requires a few housekeeping items.

First, you must make adjustment to the application project file.  You muust add `net8.0-windows` to your `TargetFrameworks` and you must add `UseWindowsForms` to that target framework.  Below are the relevent project file sections.

```
<Project Sdk="Microsoft.NET.Sdk">
    <PropertyGroup>
        <OutputType>Exe</OutputType>
        <TargetFrameworks>net8.0;net8.0-windows</TargetFrameworks>
    </PropertyGroup>

    <PropertyGroup Condition=" '$(TargetFramework)' == 'net8.0-windows'">
        <UseWindowsForms>true</UseWindowsForms>
    </PropertyGroup>
</Project>
```

Second, WinForms needs some initialization.  Do this in the application's `Main` entry point.

```
public static async Task Main(string[] args)
{
#if WINDOWS
    System.Windows.Forms.Application.EnableVisualStyles();
    System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
    ApplicationConfiguration.Initialize();
#endif
    await MeadowOS.Start(args);
}

```


### Mac

`Meadow.Desktop`'s display on Macintosh relies on GTK, so GTK must be installed on the machine.

#### Install Brew

If Brew is not already installed, install it:

```
$ /usr/bin/ruby -e "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/master/install)"
```

#### Install GTK

To install gtk+3 on macOS using brew, run the following command:

```
$ brew install gtk+3
$ brew install gtksourceview4
```

### Linux

To install gtk+3 on Linux, run the following command:

```
$ sudo apt-get install libgtk-3-dev
```

