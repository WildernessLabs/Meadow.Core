# Meadow Core Library

## Repo Status

[![.NET Core CI Build](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml/badge.svg)](https://github.com/WildernessLabs/Meadow.Core/actions/workflows/ci-build.yml)

# Framework Design

## Unified GPIO Architecture

### Peripherals Must Support Pin and Port

All peripherals must be able to be constructed with either a an `IPort` or an `IPin`:

```
protected IDigitalPort _port;

public Relay (IDigitalPin pin) 
  : this (new DigitalPort(pin)) { }

public Relay (IDigitalPort port) {
   _port = port;
}
```


### Peripherals Must Only Accept Correct Port Type

e.g. `IDigitalPin` or `IPWMPin`

#### Analog Example

```
public class AnalogSensor {
   public AnalogSensor (IAnalogPin pin) { ... }
   public AnalogSensor (IAnalogPort port) { ... }
}
```
#### Digital Example

```
public class LED {
   public PwmLed (IDigitalPin pin) { ... }
   public PwmLed (IDigitalPort port { ... }
}
```
#### PWM Example

```
public class PwmLed {
   public PwmLed (IPwmPin pin) { ... }
   public PwmLed (IPwmPort port { ... }
}
```
# Publishing Nuget Packages

CI builds are [setup in Jenkins](http://jenkins.wildernesslabs.co/job/Meadow.Core/).  
To trigger a new build:  
- Go to project properties in VS 2017  
- in the `Package` tab, increment either the MAJOR or MINOR `Package version`.  

The CI job will pick up the changes, pack, and push the Nuget package. 

[![Build Status](http://jenkins.wildernesslabs.co/buildStatus/icon?job=Meadow.Core)](http://jenkins.wildernesslabs.co/job/Meadow.Core/)
