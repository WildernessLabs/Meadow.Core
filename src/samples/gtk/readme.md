# Meadow GTK Sample

## WSL2

To run the GTK Sample under WSL, you must ensure you have .NET 6.0 and GTK installed.

Check for .NET:

```
$ dotnet --list-runtimes
Microsoft.AspNetCore.App 6.0.10 [/usr/share/dotnet/shared/Microsoft.AspNetCore.App]
Microsoft.NETCore.App 6.0.10 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
```

If .NET is not installed, install it:

```
$ wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
$ sudo dpkg -i packages-microsoft-prod.deb
$ rm packages-microsoft-prod.deb
$ sudo apt update; \
    sudo apt install -y apt-transport-https && \
    sudo apt update && \
    sudo apt install -y dotnet-sdk-6.0
```

Install GTK# by installing the sample applications:

```
$ sudo apt install - y gtk-sharp3-examples
```

