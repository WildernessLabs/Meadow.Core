[![Stable Build](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml)
<img src="design/banner.jpg" style="margin-bottom:10px" />

# Meadow.Core

[]

## Repositories

### Dependencies

If this repo is cloned, it will only build if the following repositories are cloned and stored at the same folder level:
1. [Meadow.Units](https://github.com/WildernessLabs/Meadow.Units)  contains a strong unitization into the entire stack of Meadow.
2. [Meadow.Logging](https://github.com/WildernessLabs/Meadow.Logging) a lightweight logging library for embedded hardware.
3. [Meadow.Contracts](https://github.com/WildernessLabs/Meadow.Contracts) contains the interfaces used by the entire Meadow stack.
4. [MQTTnet](https://github.com/WildernessLabs/MQTTnet) is a high performance .NET library for MQTT based communication.

Additionally, you might want to check out our [Meadow.Core.Samples](https://github.com/WildernessLabs/Meadow.Core.Samples) repo that has all our sample projects that cover every feature Meadow has to offer with no extra peripherals required. 

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
