<img src="design/banner.jpg" style="margin-bottom:10px" />

# Meadow.Core

## Repo Status

[![Stable Build](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml)

# Framework Design

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
