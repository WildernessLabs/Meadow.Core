using Meadow.Hardware;
using Meadow.Logging;
using Meadow.Units;
using System;

namespace Meadow.Simulation;

internal class SimulationEngine<TPinDefinitions> : SimulationEnvironment, IMeadowIOController
    where TPinDefinitions : IPinDefinitions
{
    private ISimulatedDevice<TPinDefinitions> _device;
    private WebSocketServer _wsServer;

    public SimulationEngine(ISimulatedDevice<TPinDefinitions> device, Logger logger)
    {
        _device = device;

        _wsServer = new WebSocketServer(logger);
        _wsServer.MessageReceived += OnWebSocketMessageReceived;

        Initialize(null);
    }

    //        private Dictionary<IPin, bool> _discreteStates = new Dictionary<IPin, bool>();
    //        private Dictionary<IPin, double> _analogStates = new Dictionary<IPin, double>();

    public IDeviceChannelManager DeviceChannelManager => throw new NotImplementedException();

    public string OSVersion => throw new NotImplementedException();

    public string OSBuildDate => throw new NotImplementedException();

    public string MonoVersion => throw new NotImplementedException();

    public event InterruptHandler Interrupt;

    public void Initialize(string[]? reservedPins)
    {
        /*
        foreach (var pin in _device.Pins)
        {
            // discretes
            if (pin.Supports<IDigitalChannelInfo>())
            {
                _discreteStates.Add(pin, false);
            }

            // analog inputs
            if (pin.Supports<IDigitalChannelInfo>())
            {
                _analogStates.Add(pin, 0d);
            }
        }
        */

        _wsServer.Start();

        PublishState();
    }

    private void PublishState()
    {
        var state = new SimulationState();

        foreach (SimulatedPin pin in _device.Pins)
        {
            state.PinStates.Add(pin.Name, pin.Voltage.Volts);
        }

        var j = System.Text.Json.JsonSerializer.Serialize(state);
        _wsServer.SendMessage(j);
    }

    private void OnWebSocketMessageReceived(WebSocketServer source, string message)
    {
        switch (message)
        {
            case "get_state":
                // publish full state
                PublishState();
                break;
        }
    }

    void IMeadowIOController.Initialize(string[]? reservedPins)
    {
    }

    public void SetDiscrete(IPin pin, bool state)
    {
        var voltage = state ? SimulationEnvironment.ActiveVoltage : SimulationEnvironment.InactiveVoltage;
        SetPinVoltage(pin, voltage);
    }

    public bool GetDiscrete(IPin pin)
    {
        return GetPinVoltage(pin) == SimulationEnvironment.ActiveVoltage; // TODO: do we want an active threshold below 3.3V?
    }

    public void SetPinVoltage(IPin pin, Voltage voltage)
    {
        if (pin is SimulatedPin { } sp)
        {
            if (voltage.Volts < 0) throw new ArgumentOutOfRangeException();

            var rising = voltage > sp.Voltage;

            sp.Voltage = voltage;

            _wsServer.SendMessage($"{pin.Name}={voltage.Volts}V");

            Interrupt?.Invoke(pin, rising);
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public Voltage GetPinVoltage(IPin pin)
    {
        if (pin is SimulatedPin { } sp)
        {
            return sp.Voltage;
        }
        else
        {
            throw new NotSupportedException();
        }
    }

    public void SetResistorMode(IPin pin, ResistorMode mode)
    {
        throw new NotImplementedException();
    }

    public void ConfigureOutput(IPin pin, bool initialState, OutputType outputType)
    {
        throw new NotImplementedException();
    }

    public void ConfigureInput(IPin pin, ResistorMode resistorMode, InterruptMode interruptMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotImplementedException();
    }

    public bool UnconfigureGpio(IPin pin)
    {
        throw new NotImplementedException();
    }

    public void ConfigureAnalogInput(IPin pin)
    {
        throw new NotImplementedException();
    }

    public int GetAnalogValue(IPin pin)
    {
        throw new NotImplementedException();
    }

    public Temperature GetTemperature()
    {
        throw new NotImplementedException();
    }

    public void WireInterrupt(IPin pin, InterruptMode interruptMode, ResistorMode resistorMode, TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        throw new NotImplementedException();
    }

    public void ReassertConfig(IPin pin, bool validateInterruptGroup = true)
    {
        throw new NotImplementedException();
    }

    public T GetConfigurationValue<T>(IPlatformOS.ConfigurationValues item) where T : struct
    {
        throw new NotImplementedException();
    }

    public void SetConfigurationValue<T>(IPlatformOS.ConfigurationValues item, T value) where T : struct
    {
        throw new NotImplementedException();
    }
}
