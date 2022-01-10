# Meadow For Linux

## Summary

Wilderness Labs Meadow for Linux is a .NET Framework for running IoT applications on Linux devices.  A Meadow for Linux application provides developers the ease of quickly creating applications for popular platforms that can use any of the peripheral drivers available in the [Meadow Foundation](https://github.com/WildernessLabs/Meadow.Foundation) library.

## Current State

![Build](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/build.yml/badge.svg)

## Supported Platforms and Distributions

Currently tested platforms and distributions:

| Hardware | Distro | Meadow.Core Version tested |
| :---: | :---: | :---: |
| Raspberry Pi 4 | Raspberry Pi OS | Beta 6 |
| Raspberry Pi Zero 2w | Raspberry Pi OS | Beta 6 |
| Jetson Nano | Ubuntu 20.04 | Beta 6 |

## License

Apache 2.0

See [LICENSE File](/LICENSE)

## Device Prerequisites

- Your device must have .NET 5.0 installed, unless you are running inside a container in which case the container image must have .NET 5.0 included. All [Wilderness Labs m4l Docker images](https://hub.docker.com/u/wildernesslabs) (samples and base) have the .NET 5.0 runtime included.
- You must have hardware drivers enabled (e.g. I2C on Raspberry Pi is disabled by default and is enabled by running `raspi-config`)

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
 
## Work in Progress

M4L is currently an *Alpha* product with several core features that are not yet implemented.  Details are available in the [Issues Tab](https://github.com/WildernessLabs/Meadow.Linux/issues) and the source.
