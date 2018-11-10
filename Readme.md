# Meadow Core Library

# Framework Design

## Unified GPIO Architecture

### Peripherals Must Support Pin and Port

All peripherals must be able to be constructed with either a an `IPort` or an `IPin`:

```
protected IDigitalPort _port;

public Relay (IDigitalPin pin) {
   _port = new DigitalPort(pin);
  
}

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
