# Wilderness Labs Meadow.Linux

## Repo Status

[![Build](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/build.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/build.yml)
[![NuGet Package Creation](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/package.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Linux/actions/workflows/package.yml)

## Summary

Wilderness Labs Meadow.Linux is a .NET Framework for running IoT applications on Linux devices. A Meadow.Linux application provides developers the ease of quickly creating applications for popular platforms that can use any of the peripheral drivers available in the [Meadow.Foundation](https://github.com/WildernessLabs/Meadow.Foundation) library.

## Supported Platforms and Distributions

Linux Embedded Hardware is a broad space and we've made efforts to test support on a broad number of devices.  because Meadow is modular and interface-based, adding new platform support is generally simple, requiring only a few days to port and test with hardware.

The table below shows the platforms we have done at least some work on and to what degree we have tested subsystems.

| Platform | OS Tested | GPIO | I2C | SPI | ADC | UART | PWM |
| --- | --- | --- | --- | --- | --- | --- |
| Raspberry Pi 3b+ | Rasberry Pi OS xxx 64-bit Lite | X | X | X | - | U | O |
| Raspberry Pi 4 | Rasberry Pi OS xxx 64-bit Lite | X | X | X | - | U | O |
| Raspberry Pi 5 | Rasberry Pi OS xxx 64-bit Lite | X | X | X | - | X | O |
| Raspberry Pi Zero 2 W | Rasberry Pi OS Lite 64 (kernel 6.6.20+rpt-rpi-v8, Debian Bookworm 1:6.6.20-1+rpt1)  | X | X | X | - | X | O |
| BeagleBone Black | Debian Bullseye (kernel 5.10.168-ti-r72) | X | X | X | X | O | X |
| NVIDIA Jetson Xavier Nano | Ubuntu 18.04 | X | O | O | - | O | O |
| NVIDIA Jetson Xavier AGX | Ubuntu 18.04 | X | X | U | - | O | O |
| krtkl Snickerdoodle Black | Ubuntu 20.04 | X | O | O | - | O | O |
| AMD64 Ubuntu 20.04 under WSL2  | Ubuntu 20.04 | - | - | - | - | U | - |

X: Implemented and tested
U: Implemented but untested
O: Not implemented/Not Tested
-: Not supported by hardware

## Platform-Specific Instructions and Details

### [BeagleBone Black](beaglebone.md)
### [Raspberry Pi](raspberrypi.md)

## License

Apache 2.0

See [LICENSE File](/LICENSE)

