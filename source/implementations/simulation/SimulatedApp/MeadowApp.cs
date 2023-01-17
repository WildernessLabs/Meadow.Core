
using Meadow;
using Meadow.Hardware;
using Meadow.Modbus;
using Meadow.Simulation;

public class MeadowApp : App<SimulatedMeadow<SimulatedPinout>>
{
    private IDigitalOutputPort _out1;

    public MeadowApp()
    {
        Initialize();

        Run();
    }

    private void Run()
    {
        bool state = false;

        while (true)
        {
            Device.Logger.Info($"Setting {_out1.Pin.Name} to {state}");
            _out1.State = state;
            state = !state;
            Thread.Sleep(1000);
        }
    }

    void Initialize()
    {
        Device.Logger.Info("Initialize hardware...");

        _out1 = Device.CreateDigitalOutputPort(Device.Pins.D00);

        var port = Device.CreateSerialPort(new SerialPortName("foo", "COM10"), 19200);
        var client = new ModbusRtuClient(port);
        client.Connect();
        var register = client.ReadHoldingRegisters(1, 100, 1);
    }
}