# Meadow UI Libraries

## Meadow.Avalonia

The `Meadow.Avalonia` library provides the ability for an Avalonia UI application to use the Meadow.Core stack.  This allows you to create rick, cross-platform HMIs that interact directly with hardware.

### Creating a `Meadow.Avalonia` Application

- Create a standard Avalonia or Avalonia MVVM application
- Change the application to target .NET 7
- Add a reference to `Meadow.Avalonia`
- Add a reference to the Meadow platform of your choice (i.e. `Meadow.Windows` or `Meadow.Linux`)
- Modify App.axaml.cs
  - Change the class signature from
    ```
    public partial class App : Application
    ```
    to (use the hardware Platform of your choice.  Here it's Windows)
    ```
    public partial class App : AvaloniaMeadowApplication<Windows>
    ```
  - Change `Initialize` from
    ```
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }
    ```
    to

    ```
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
        LoadMeadowOS();
    }
    ```

Your `App.axaml.cs` is now the analog to a standard Meadow `App` class with two exceptions.

1. `Meadow.IApp.Initialize()` is now wrapped in `MeadowInitialize()`
1. `Meadow.IApp.Run()` is now wrapped in `MeadowRun()`


## Meadow.MAUI

## Meadow.WinUI

## Meadow.Winforms

## Meadow.GTK 