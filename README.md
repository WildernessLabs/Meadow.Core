# Wilderness Labs Meadow.Linux

## Repo Status

[![Build](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/build.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/build.yml)
[![NuGet Package Creation](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/package.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/package.yml)

## Summary

Wilderness Labs Meadow.Linux is a .NET Framework for running IoT applications on Linux devices. A Meadow.Linux application provides developers the ease of quickly creating applications for popular platforms that can use any of the peripheral drivers available in the [Meadow.Foundation](https://github.com/WildernessLabs/Meadow.Foundation) library.

## Supported Platforms and Distributions

Currently tested platforms and distributions:

| Hardware | Distro | Meadow.Core Version tested |
| :---: | :---: | :---: |
| Raspberry Pi 4 | Raspberry Pi OS | Beta 6.2 |
| Raspberry Pi Zero 2 W | Raspberry Pi OS | Beta 6.2 |
| Jetson Nano | Ubuntu 20.04 | Beta 6.2 |
| Jetson Xavier AGX | Ubuntu 18.04 | Beta 6.2 |
| KRTKL Snickerdoodle Black | Ubuntu 20.04 | RC-1 |
| AMD64 Ubuntu 20.04 under WSL2  | Ubuntu 20.04 | RC-1 |

## License

Apache 2.0

See [LICENSE File](/LICENSE)

## Assumptions

For this documentation, we assume you are developing your application code on a machine *separate* from your target hardware.  You can certainly develop and compile your application directly on your target hardware, but the possible variations and permutations make documenting the process more difficult. If you are new to .NET development on IoT devices, we recommend starting with a separate development machine first.

## Prepare Your Target Hardware

`Meadow.Linux` runs applications using the .NET 6.0 Core Runtime, so you must install it on the target.

### Install .NET 6.0

Follow the instructions based on your distro:

https://docs.microsoft.com/en-us/dotnet/core/install/linux

```console
$ dotnet --list-runtimes
Microsoft.NETCore.App 6.0.1 [/opt/dotnet/shared/Microsoft.NETCore.App]
```

### Enable Hardware Access

Different platforms will have different rules for enabling application access to hardware devices such as GPIO, SPI and I2C. Consult your platform documentation for specifics, but as an example Raspberry Pi OS requires that you run `raspi-config` to enable the peripherals you want, and *additionally* add your user to specific groups for the peripherals.  If you get permissions errors while first running your application, this is the place to start looking.

## Develop your Application

- Create a .NET 6 Core Application Project
- Add NuGet references to Meadow.Linux and any other requirements (e.g Meadow.Foundation and peripheral drivers)
- Develop/Compile 

## Deploying

When you are ready to run your application you will need to copy it, and all dependencies, to the target device.

The simplest, most reliable way to collect all of those binaries is by using `dotnet publish`.

For example, the `Bme280_Sample` in this repo can be published to a local folder like this:

```console
C:\repos\wilderness\Meadow.Linux\src\samples\Bme280_Sample>dotnet publish -c Release -o publish
Microsoft (R) Build Engine version 17.2.0+41abc5629 for .NET
Copyright (C) Microsoft Corporation. All rights reserved.

  Determining projects to restore...
  All projects are up-to-date for restore.
  Meadow.Linux -> C:\repos\wilderness\Meadow.Linux\src\lib\bin\Release\netstandard2.1\Meadow.Linux.dll
  Bme280_Sample -> C:\repos\wilderness\Meadow.Linux\src\samples\Bme280_Sample\bin\Release\net6.0\App.dll
  Bme280_Sample -> C:\repos\wilderness\Meadow.Linux\src\samples\Bme280_Sample\publish\
``` 

The output folder contains all of the files and assemblies you need to copy to your target hardware.

Now use a tool such as SCP to copy all of these files to a folder on you target hardware.

[add example command / winscp sceen cap]

[todo: this is a note from my previous work, check if App.deps is a problem]
- Copy over your application binaries and `App.runtimeconfig.json`.  Do *not* copy over App.deps.json

Here's an example of all of the binaries for the Bme280_Sample aaplication after deployment to a target Raspberry Pi:

```console
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

Starting with the `RC1.0` release, Meadow now has a defined lifecycle.  This is imposed to allow automated application shutdown by the OtA update service. What this means, practically, is that your Meadow application is now a _library_ that gets loaded by the actual entry executable, which is `Meadow.dll`.

Use `dotnet` *on the target hardware* to execute your application by doing the following:

```console
$ dotnet Meadow.dll
```
 
## Work in Progress

`Meadow.Linux` is currently an *Beta* product with several core features that are not yet implemented. Details are available in the [Issues Tab](https://github.com/WildernessLabs/Meadow.Linux/issues) and the source.

## Running Meadow.Linux under Windows

`Meadow.Linux` can be run on Windows 11 using WSL2.  Since a PC has no GPIOs or hardware busses, it requires the `Meadow.Simulation` platform.
