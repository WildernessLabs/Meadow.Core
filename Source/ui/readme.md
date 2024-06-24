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

- Create a standard MAUI application
- Change the application to target .NET 7
- Add a reference to `Meadow.Maui`
- Modify App.xaml.cs
  - Change the class signature from
    ```
    public partial class App : Application
    ```
    to (use the hardware Platform of your choice.  Here it's Windows)
    ```
    public partial class App : MauiMeadowApplication<Meadow.Windows>
    ```
  - Change the class contructor from
    ```
    public App()
    {
        InitializeComponent();

        MainPage = new AppShell();
    }
    ```
    to
    ```
    public App()
    {
        InitializeComponent();
        LoadMeadowOS();

        MainPage = new AppShell();
    }
    ```
- Modify App.xaml
  Change
  ```
  <Application xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:MauiApp1"
             x:Class="MauiApp1.App">
   ```
   to (replacing the `Class` value with your project's class name)
   ```
   <ml:MauiMeadowApplication 
             x:TypeArguments="m:Windows"
             xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:local="clr-namespace:MauiApp1"
             xmlns:ml="clr-namespace:Meadow.UI;assembly=Meadow.Maui"
             xmlns:m="clr-namespace:Meadow;assembly=Meadow.Windows"
             x:Class="MauiApp1.App">
   ```
- Modify the application project file
  Inside the first `<PropertyGroup>` node, add the following:
  ```
  <ValidateExecutableReferencesMatchSelfContained>false</ValidateExecutableReferencesMatchSelfContained>
  ```
Your `App.xaml.cs` is now the analog to a standard Meadow `App` class with two exceptions.

1. `Meadow.IApp.Initialize()` is now wrapped in `MeadowInitialize()`
1. `Meadow.IApp.Run()` is now wrapped in `MeadowRun()`

## Meadow.WinUI

Support for WinUI3 is a work in progress