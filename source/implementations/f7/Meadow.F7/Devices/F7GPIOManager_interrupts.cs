using Meadow.Core;
using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static Meadow.Core.Interop;

namespace Meadow.Devices;

public partial class F7GPIOManager : IMeadowIOController
{
    /// <summary>
    /// An event raised when an interrupt occurs
    /// </summary>
    public event InterruptHandler Interrupt = default!;

    private Thread? _ist;
    private readonly List<int> _interruptGroupsInUse = new();
    private bool _firstInterrupt = true;

    private readonly IPin _nullPin = new NullPin();

    /// <summary>
    /// Hooks up the provided pin to the underlying OS interrupt handling
    /// </summary>
    /// <param name="pin">The pin to connect</param>
    /// <param name="interruptMode">The interrupt mode to use</param>
    /// <param name="resistorMode">The resistor mode to use</param>
    /// <param name="debounceDuration">The debounce duration (or TimeSpan.Zero for none)</param>
    /// <param name="glitchDuration">The glitch filter duration(or TimeSpan.Zero for none)</param>
    public void WireInterrupt(IPin pin,
        InterruptMode interruptMode,
        ResistorMode resistorMode,
        TimeSpan debounceDuration,
        TimeSpan glitchDuration)
    {
        var designator = GetPortAndPin(pin);

        Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0, $" + Wire Interrupt {interruptMode}");

        if (interruptMode != InterruptMode.None)
        {
            ConnectInterrupt(pin, (int)designator.port, designator.pin, interruptMode, resistorMode, debounceDuration, glitchDuration);
        }
        else
        {
            DisconnectInterrupt((int)designator.port, designator.pin);
        }
    }

    private void ConnectInterrupt(IPin pin, int portNumber, int pinNumber, InterruptMode interruptMode, ResistorMode resistorMode,
        TimeSpan debounceDuration, TimeSpan glitchDuration)
    {
        lock (_interruptGroupsInUse)
        {
            Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0, $" ConnectInterrupt {portNumber}:{pinNumber}");

            // interrupt group is effectively the F7 pin number (not the Meadow pin number)
            if (_interruptGroupsInUse.Contains(pinNumber))
            {
                Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0, $" interrupt group {pinNumber} in use");

                throw new InterruptGroupInUseException(pinNumber);
            }
            else
            {
                Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0, $" interrupt group {pinNumber} not in use");

                if (_interruptGroupsInUse.Contains(pinNumber) == false)
                {
                    _interruptGroupsInUse.Add(pinNumber);
                }
            }
        }

        // STM resistor mode and Meadow resistor mode are inverted
        STM32.ResistorMode rm;
        switch (resistorMode)
        {
            case ResistorMode.InternalPullUp:
                rm = STM32.ResistorMode.PullUp;
                break;
            case ResistorMode.InternalPullDown:
                rm = STM32.ResistorMode.PullDown;
                break;
            default:
                rm = STM32.ResistorMode.Float;
                break;
        }

        var cfg = new Nuttx.UpdGpioInterruptConfiguration()
        {
            Enable = 1,
            Port = (uint)portNumber,
            Pin = (uint)pinNumber,
            RisingEdge = (uint)(interruptMode == InterruptMode.EdgeRising || interruptMode == InterruptMode.EdgeBoth ? 1 : 0),
            FallingEdge = (uint)(interruptMode == InterruptMode.EdgeFalling || interruptMode == InterruptMode.EdgeBoth ? 1 : 0),
            ResistorMode = rm,

            // Nuttx side expects 1 - 10000 to represent .1 - 1000 milliseconds
            DebounceDuration = (uint)(debounceDuration.TotalMilliseconds * 10),
            GlitchDuration = (uint)(glitchDuration.TotalMilliseconds * 10)
        };

        if (_ist == null)
        {
            _ist = new Thread(InterruptServiceThreadProc)
            {
                Priority = ThreadPriority.Highest
            };

            _ist.Start();
        }

        //Not sure why but Ioctl fails without this after reasserting interrupt groups
        Thread.Sleep(10);

        Output.WriteLineIf((DebugFeatures & (DebugFeature.GpioDetail | DebugFeature.Interrupts)) != 0,
            $"Calling ioctl from WireInterrupt() enable Input: {portNumber}{pinNumber}, ResistorMode:{rm}, debounce:{debounceDuration}, glitch:{glitchDuration}");

        var result = UPD.Ioctl(Nuttx.UpdIoctlFn.RegisterGpioIrq, ref cfg);

        if (result != 0)
        {
            var err = UPD.GetLastError();

            Output.WriteLineIf((DebugFeatures & (DebugFeature.GpioDetail | DebugFeature.Interrupts)) != 0,
                    $"failed to register interrupts: {err}");
        }
        else
        {
            _interruptPins[portNumber, pinNumber] = pin;
        }
    }

    private void DisconnectInterrupt(int portNumber, int pinNumber)
    {
        // only disable if it was previously enabled!
        if (_interruptPins[portNumber, pinNumber] != null)
        {

            var cfg = new Interop.Nuttx.UpdGpioInterruptConfiguration()
            {
                Enable = 0,   // Disable
                Port = (uint)portNumber,
                Pin = (uint)pinNumber,
                RisingEdge = 0,
                FallingEdge = 0,
                ResistorMode = 0,
                DebounceDuration = 0,
                GlitchDuration = 0
            };

            Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0,
                $"Calling ioctl to disable interrupts for Input: {portNumber}{pinNumber}");

            UPD.Ioctl(Nuttx.UpdIoctlFn.RegisterGpioIrq, ref cfg);

            lock (_interruptGroupsInUse)
            {
                if (_interruptGroupsInUse.Contains(pinNumber))
                {
                    _interruptGroupsInUse.Remove(pinNumber);
                }
                else
                {
                    Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0,
                        $"Int group: {pinNumber} not in use");
                }
            }

            _interruptPins[portNumber, pinNumber] = null;

        }
    }

    private void InterruptServiceThreadProc(object o)
    {
        IntPtr queue = Interop.Nuttx.mq_open(new StringBuilder("/mdw_int"), Nuttx.QueueOpenFlag.ReadOnly);

        // We get 2 bytes from Nuttx. the first is the GPIOs port and pin the second
        // the debounced state of the GPIO
        var rx_buffer = new byte[2];

        while (true)
        {
            if (_firstInterrupt)
            {
                // DEV NOTE: this is to force the interp pipeline top build at least some of the call stack and improve the response of first-interrupt results
                Interrupt?.Invoke(_nullPin, false);
                _firstInterrupt = false;
            }

            int priority = 0;
            int result;

            do {
                result = Interop.Nuttx.mq_receive(queue, rx_buffer, rx_buffer.Length, ref priority);
            } while (result < 0 && UPD.GetLastError() == Nuttx.ErrorCode.InterruptedSystemCall);

            // byte 1 contains the port and pin, byte 2 contains the stable state.
            if (result >= 0)
            {
                var irq = rx_buffer[0];
                bool state = rx_buffer[1] != 0;
                var port = irq >> 4;
                var pin = irq & 0xf;
                IPin ipin;


                lock (_interruptPins)   //FIXME: If this is supposed to keep modifications from
                                        //happening on the array, lock()'s are needed at those points also
                {
                    ipin = _interruptPins[port, pin];
                    if (ipin == null)
                        continue;
                }

                try
                {
                    Interrupt?.Invoke(ipin, state);
                }
                catch
                {
                    Thread.Sleep(5000);
                }
            }
        }
    }
}


/* ===== MEADOW GPIO PIN MAP =====
    BOARD PIN   SCHEMATIC       CPU PIN   MDW NAME  ALT FN   INT GROUP
    J301-1      RESET                                           - 
    J301-2      3.3                                             - 
    J301-3      VREF                                            - 
    J301-4      GND                                             - 
    J301-5      DAC_OUT1        PA4         A0                  4
    J301-6      DAC_OUT2        PA5         A1                  5
    J301-7      ADC1_IN3        PA3         A2                  3
    J301-8      ADC1_IN7        PA7         A3                  7
    J301-9      ADC1_IN10       PC0         A4                  0
    J301-10     ADC1_IN11       PC1         A5                  1
    J301-11     SPI3_CLK        PC10        SCK                 10
    J301-12     SPI3_MOSI       PB5         MOSI    AF6         5
    J301-13     SPI3_MISO       PC11        MISO    AF6         11
    J301-14     UART4_RX        PI9         D00     AF8         9
    J301-15     UART4_TX        PH13        D01     AF8         13
    J301-16     PC6             PC6         D02                 6
    J301-17     CAN1_RX         PB8         D03     AF9         8
    J301-18     CAN1_TX         PB9         D04     AF9         9

    J302-4      PE3             PE3         D15                 3
    J302-5      PG3             PG3         D14                 3
    J302-6      USART1_RX       PB15        D13     AF4         15
    J302-7      USART1_TX       PB14        D12     AF4         14
    J302-8      PC9             PC9         D11                 9
    J302-9      PH10            PH10        D10                 10
    J302-10     PB1             PB1         D09                 1
    J302-11     I2C1_SCL        PB6         D08     AF4         6
    J302-12     I2C1_SDA        PB7         D07     AF4         7
    J302-13     PB0             PB0         D06                 0
    J302-14     PC7             PC7         D05                 7

    LED_B       PA0
    LED_G       PA1
    LED_R       PA2
*/
