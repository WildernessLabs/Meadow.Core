# Meadow For Linux

## Summary

[TBD]

## License

Apache 2.0

See [LICENSE File](/LICENSE)

## Supported Platforms and Distributions

Currently tested platforms and distributions:

| Hardware | Distro | M4L Version tested |
| - | - |
| Raspberry Pi 4 | Raspberry Pi OS | pre-release |
| Jetson Nano | Ubuntu 20.04 | pre-release |

## Device Prerequisites

- Your device must have .NET 5.0 installed.
- You must have hardware drivers enabled (e.g. I2C on Raspberry Pi is disabled by default)

### Install .NET 5.0
```
$ curl -SL -o dotnet.tar.gz https://download.visualstudio.microsoft.com/download/pr/09a24e9f-0096-454a-b761-70cdf9504775/eafe9578bbedd15c9319b7580d5a20d9/dotnet-runtime-5.0.7-linux-arm.tar.gz
$ sudo mkdir -p /usr/share/dotnet
$ sudo tar -zxf dotnet.tar.gz -C /usr/share/dotnet
$ sudo ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet
$ $ dotnet --list-runtimes
Microsoft.NETCore.App 5.0.7 [/usr/share/dotnet/shared/Microsoft.NETCore.App]
```

## Deploying

- Create the application as a .NET 5.0 App
- Copy over your application binaries and `App.runtimeconfig.json`.  Do *not* copy over App.deps.json

Here's an example of all of the binaries for simple application using a BME280 and Meadow Foundation

```
$ ls -al
total 472
drwxr-xr-x  2 pi pi   4096 Nov  7 17:56 .
drwxr-xr-x 16 pi pi   4096 Nov  7 17:55 ..
-rw-r--r--  1 pi pi   7680 Nov  7 18:09 App.dll
-rw-r--r--  1 pi pi    147 Nov  7 17:55 App.runtimeconfig.json
-rw-r--r--  1 pi pi  17920 Nov  7 17:58 Bme280.dll
-rw-r--r--  1 pi pi  69632 Nov  7 17:23 Meadow.Contracts.dll
-rw-r--r--  1 pi pi 127488 Nov  7 17:23 Meadow.dll
-rw-r--r--  1 pi pi 111104 Nov  7 17:23 Meadow.Foundation.dll
-rw-r--r--  1 pi pi  38400 Nov  7 17:23 Meadow.Linux.dll
-rw-r--r--  1 pi pi  83968 Nov  7 17:23 Meadow.Units.dll
```

## Running

Use `dotnet` to execute your application

```
$ dotnet App.exe
```
 