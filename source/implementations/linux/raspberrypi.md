# Meadow Linux on Raspberry Pi

## Assumptions

For this documentation, we assume you are developing your application code on a machine *separate* from your target hardware.  You can certainly develop and compile your application directly on your target hardware, but the possible variations and permutations make documenting the process more difficult. If you are new to .NET development on IoT devices, we recommend starting with a separate development machine first.

## Prepare Your Target Hardware

`Meadow.Linux` runs applications using the .NET 8.0 Core Runtime, so you must install it on the target.

### Install .NET

Create and edit a new shell script
```
nano raspi-dotnet.sh
```

```
#!/bin/bash

add_to_bashrc() {
    local string="$1"
    if ! grep -q "$string" "$HOME/.bashrc"; then
        echo "$string" >> $HOME/.bashrc
    fi
}

wget --inet4-only https://dot.net/v1/dotnet-install.sh -O - | bash /dev/stdin --version latest
add_to_bashrc "export DOTNET_ROOT=\$HOME/.dotnet"
add_to_bashrc "export PATH=\$PATH:\$HOME/.dotnet"
```

<ctrl-s><ctrl-x>

List the local files

```
ls -l
```
Which gives an output like this:
```
total 4
-rw-r--r-- 1 pi pi 364 May 28 20:22 raspi-dotnet.sh
```

Make the shell script executable

```
chmod +x raspi-dotnet.sh
```
The file list will change
```
ls -l
total 4
-rwxr-xr-x 1 pi pi 364 May 28 20:22 raspi-dotnet.sh
```

Execute the newly-created script

```
./raspi-dotnet.sh
```

This takes a long time, like over 5 minutes long.  Be patient.  In the end you should see something like

```
dotnet-install: Installation finished successfully.
```

Now re-load your environment and verify the install

```
source ~/.bashrc
dotnet --version
```
This wll give the installed .NET version.
```
8.0.301
```

### Enable Hardware Things

Now you have to enable access to things like the hardware busses and GPIO

```
sudo raspi-config
```

### Enable Hardware Access

Use `raspi-config` to enable I2C and SPI on the platform.

## Develop your Application

- Create a .NET 8 Core Application Project
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
  Bme280_Sample -> C:\repos\wilderness\Meadow.Linux\src\samples\Bme280_Sample\bin\Release\net7.0\App.dll
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

Starting with the `RC1.0` release, Meadow now has a defined lifecycle.  This is imposed to allow automated application shutdown by the OtA update service. What this means, practically, is that your Meadow.Linux application must define an entry point in your `App<T>` implementation and and manually start the MeadowOS stack.

```console
public class MeadowApp : App<Linux>
{
    public static async Task Main(string[] _)
    {
        await MeadowOS.Start();
    }
    ...
}
```

Then launch the application using `dotnet`

```console
$ dotnet MyAppAssembly.dll
```
 