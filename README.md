[![NuGet Badge](https://buildstats.info/nuget/Meadow)](https://www.nuget.org/packages/Meadow)
[![Stable](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml)
[![License: MIT](https://img.shields.io/badge/License-Apache-green.svg)](license.txt)

<img src="design/banner.jpg" style="margin-bottom:10px" />

# Meadow.Core

Welcome to the Meadow Core library GitHub repository! This library is the foundation of the Meadow platform and provides a powerful set of tools for creating connected devices. The Meadow Core library includes APIs for common protocols such as SPI, I2C, and UART, as well as APIs for networking, storage, and more. The library is written in C# and is designed to work with the Meadow F7 Microcontroller. With Meadow Core, you can quickly and easily build connected devices that are powerful, reliable, and secure. Join our community and start building with Meadow Core today!

## Repositories

### Dependencies

If this repo is cloned, it will only build if the following repositories are cloned and stored at the same folder level:
1. [Meadow.Units](https://github.com/WildernessLabs/Meadow.Units)  contains a strong unitization into the entire stack of Meadow.
2. [Meadow.Logging](https://github.com/WildernessLabs/Meadow.Logging) a lightweight logging library for embedded hardware.
3. [Meadow.Contracts](https://github.com/WildernessLabs/Meadow.Contracts) contains the interfaces used by the entire Meadow stack.
4. [MQTTnet](https://github.com/WildernessLabs/MQTTnet) is a high performance .NET library for MQTT based communication.

### Samples

Additionally, you might want to check out our repos with tons of samples with different levels of complexity and hardware requirements:
1. [Meadow.Core.Samples](https://github.com/WildernessLabs/Meadow.Core.Samples) has all our sample projects that cover every feature Meadow has to offer with no extra peripherals required. 
1. [Meadow.Project.Samples](https://github.com/WildernessLabs/Meadow.Project.Samples) has en extensive collection of Meadow Projects using [Meadow.Foundation](https://github.com/WildernessLabs/Meadow.Foundation), our peripheral driver and hardware control libraries to make .NET IoT development plug-and-play.
1. [Meadow.ProjectLab.Samples](https://github.com/WildernessLabs/Meadow.ProjectLab.Samples) contains project samples for the breadboardless rapid prototyping board [Project Lab](https://github.com/WildernessLabs/Meadow.ProjectLab).

## Unified GPIO Architecture

### Peripherals Must Support Pin and Port

All peripherals must be able to be constructed with an `IPin` along with a device with the capabilities to configure a proper port (digital IO, analog, PWM, etc.) and a specific type of port such as `IDigitalOutputPort`, `IPwmPort`, `IAnalogInputPort`, etc. 

#### Analog Example

```
public class AnalogSensor 
{
   protected IAnalogInputPort AnalogInputPort  { get; set; }

   public AnalogSensor (IAnalogInputController device, IPin pin)
     : this (device.CreateAnalogInputPort(pin)) { }

   public AnalogSensor (IAnalogInputPort analogInputPort) 
   { 
      AnalogInputPort = analogInputPort;
   }
}
```
#### Digital Example

```
public class Led 
{
   protected IDigitalOutputPort DigitalOutputPort { get; set; }

   public Led (IDigitalOutputController device, IPin pin)
   : this(device.CreateDigitalOutputPort(pin)) { }

   public Led (IDigitalPort digitalOutputPort) 
   {
      DigitalOutputPort = digitalOutputPort;
   }
}
```
#### PWM Example

```
public class PwmLed 
{
   protected IPwmPort PwmPort { get; set; }

   public PwmLed (IPwmOutputController device, IPin pin) 
      : this (device.CreatePwmPort(pin)) {  }

   public PwmLed (IPwmPort pwmPort) 
   {  
      PwmPort = pwmPort;
   }
}
```
