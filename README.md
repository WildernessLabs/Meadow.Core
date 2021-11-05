# Meadow For Linux

## Summary

## License

## Supported Platforms and Distributions

## Device Prerequisites

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
- Publish for `arm-linux` target
- Copy over your application binaries and `App.runtimeconfig.json`.  Do *not* copy over App.deps.json

```
$ ls
App.dll                 Meadow.Contracts.dll  Meadow.F7.dll          Meadow.Linux.dll
App.runtimeconfig.json  Meadow.dll            Meadow.Foundation.dll  Meadow.Units.dll
```

> Productivity Note:
> Once you deploy all of the `publish` binaries, you can then copy over just the changed files incrementally from the build output folder (i.e. if you only change App.dll, only copy it)


## Running


 