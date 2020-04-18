using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using Meadow.Core;
using Meadow.Hardware;
using static Meadow.Core.Interop;
using static Meadow.Core.Interop.STM32;

namespace Meadow.Devices
{

    public partial class F7GPIOManager : IIOController
    {
        public event InterruptHandler Interrupt;

        private Thread _ist;

        private void WireInterrupt(GpioPort port, int pin, InterruptMode interruptMode,
        // p-m SHOULD debounceDuration and glitchFilterCycleCount have a useful default?
                                    uint debounceDuration = 0, uint glitchFilterCycleCount = 0)
        {
            if (interruptMode != InterruptMode.None)
            {
                var cfg = new Interop.Nuttx.UpdGpioInterruptConfiguration()
                {
                    Enable = 1,
                    Port = (uint)port,
                    Pin = (uint)pin,
                    RisingEdge = (uint)(interruptMode == InterruptMode.EdgeRising || interruptMode == InterruptMode.EdgeBoth ? 1 : 0),
                    FallingEdge = (uint)(interruptMode == InterruptMode.EdgeFalling || interruptMode == InterruptMode.EdgeBoth ? 1 : 0),
                    Irq = ((uint)port << 4) | (uint)pin,
                    debounceDuration = debounceDuration,
                    glitchFilterCycleCount = glitchFilterCycleCount

                };

                if (_ist == null)
                {
                    _ist = new Thread(InterruptServiceThreadProc)
                    {
                        IsBackground = true
                    };

                    _ist.Start();
                }

                Output.WriteLineIf((DebugFeatures & (DebugFeature.GpioDetail | DebugFeature.Interrupts)) != 0,
                    $"Calling ioctl to enable interrupts Port:0x{cfg.Port:x02}::Pin:0x{cfg.Pin:x02}::GpioId:0x{cfg.Irq:x02}");

                var result = UPD.Ioctl(Nuttx.UpdIoctlFn.RegisterGpioIrq, ref cfg);

                if (result != 0)
                {
                    var err = UPD.GetLastError();

                    Output.WriteLineIf((DebugFeatures & (DebugFeature.GpioDetail | DebugFeature.Interrupts)) != 0,
                            $"failed to register interrupts: {err}");
                }
            }
            else
            {
                // TODO: disable interrupt if it was enabled
                /*
                 SOMETHING HERE IS BAD - CAUSES OS CRASHINESS!               
                var cfg = new Interop.Nuttx.UpdGpioInterruptConfiguration()
                {
                    Enable = false,
                    Port = (int)port,
                    Pin = pin,
                    RisingEdge = false,
                    FallingEdge = false,
                    Irq = ((int)port << 4) | pin
                };
                Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0,
                    "Calling ioctl to disable interrupts");
                var result = GPD.Ioctl(Nuttx.UpdIoctlFn.RegisterGpioIrq, ref cfg);
                */
            }
        }

        private void InterruptServiceThreadProc(object o)
        {
            IntPtr queue = Interop.Nuttx.mq_open(new StringBuilder("/mdw_int"), Nuttx.QueueOpenFlag.ReadOnly);
            Output.WriteLineIf((DebugFeatures & (DebugFeature.GpioDetail | DebugFeature.Interrupts)) != 0,
                $"IST Started reading queue {queue.ToInt32():X}");
            
            // We get 2 bytes from Nuttx. the first is the GPIOs port and pin the second
            // the debounced state of the input point
            var rx_buffer = new byte[2];

            while (true)
            {
                int priority = 0;
                var result = Interop.Nuttx.mq_receive(queue, rx_buffer, rx_buffer.Length, ref priority);

                Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0,
                    $"queue data arrived: {BitConverter.ToString(rx_buffer)}");

                // byte 1 contains the port and pin, byte 2 contains the stable state.
                if (result >= 0)
                {
                    var irq = rx_buffer[0];
                    bool state = rx_buffer[1] == 0 ? false : true;
                    var port = irq >> 4;
                    var pin = irq & 0xf;
                    var key = $"P{(char)(65 + port)}{pin}";

                    Output.WriteLineIf((DebugFeatures & DebugFeature.Interrupts) != 0,
                        $"Interrupt on {key} state:{state}");

                    lock (_interruptPins)
                    {
                        if (_interruptPins.ContainsKey(key))
                        {
                            var ipin = _interruptPins[key];
                            Interrupt?.Invoke(ipin, state);
                        }
                    }
                }
            }
        }
    }


    /* ===== MEADOW GPIO PIN MAP =====
        BOARD PIN   SCHEMATIC       CPU PIN   MDW NAME  ALT FN   IMPLEMENTED?
        J301-1      RESET                       
        J301-2      3.3                       
        J301-3      VREF                       
        J301-4      GND                       
        J301-5      DAC_OUT1        PA4         A0
        J301-6      DAC_OUT2        PA5         A1
        J301-7      ADC1_IN3        PA3         A2
        J301-8      ADC1_IN7        PA7         A3
        J301-9      ADC1_IN10       PC0         A4
        J301-10     ADC1_IN11       PC1         A5
        J301-11     SPI3_CLK        PC10        SCK
        J301-12     SPI3_MOSI       PB5         MOSI    AF6
        J301-13     SPI3_MISO       PC11        MISO    AF6
        J301-14     UART4_RX        PI9         D00     AF8
        J301-15     UART4_TX        PH13        D01     AF8
        J301-16     PC6             PC6         D02                 *
        J301-17     CAN1_RX         PB8         D03     AF9
        J301-18     CAN1_TX         PB9         D04     AF9

        J302-4      PE3             PE3         D15
        J302-5      PG3             PG3         D14
        J302-6      USART1_RX       PB15        D13     AF4
        J302-7      USART1_TX       PB14        D12     AF4
        J302-8      PC9             PC9         D11
        J302-9      PH10            PH10        D10
        J302-10     PB1             PB1         D09
        J302-11     I2C1_SCL        PB6         D08     AF4
        J302-12     I2C1_SDA        PB7         D07     AF4
        J302-13     PB0             PB0         D06
        J302-14     PC7             PC7         D05

        LED_B       PA0
        LED_G       PA1
        LED_R       PA2
    */
}
