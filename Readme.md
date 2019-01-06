# Meadow Core Library

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

# DocFX Site

1. Install DocFx
  
  ```
  brew install docfx
  ```
  
2. Launch site:
  
  ```
  docfx docfx/docfx.json --serve
  ```
  
## Troubleshooting

If it fails on Mac/Linux with some `SQLitePCLRaw` nonsense, run this:

```
nuget install -OutputDirectory $TMPDIR SQLitePCLRaw.core -ExcludeVersion
for docfx in /usr/local/Cellar/docfx/*; do cp "$TMPDIR/SQLitePCLRaw.core/lib/net45/SQLitePCLRaw.core.dll" "$docfx/libexec"; done
```