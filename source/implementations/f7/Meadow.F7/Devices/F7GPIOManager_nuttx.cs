using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Linq;
using static Meadow.Core.Interop;

namespace Meadow.Devices
{
    public partial class F7GPIOManager
    {
        private class GpioConfig
        {
            public STM32.GpioPort Port { get; set; }
            public int Pin { get; set; }
            public STM32.GpioMode Mode { get; set; }
            public STM32.ResistorMode Resistor { get; set; }
            public STM32.GPIOSpeed Speed { get; set; }
            public STM32.OutputType Type { get; set; }
            public bool InitialState { get; set; }
            public InterruptMode InterruptMode { get; set; }
            public int AlternateFunctionNumber { get; set; }
        }

        private List<GpioConfig> _currentConfigs = new List<GpioConfig>();

        public void ReassertConfig(IPin pin, bool validateInterruptGroup = true)
        {
            var designator = GetPortAndPin(pin);
            lock (_currentConfigs)
            {
                var cfg = _currentConfigs.FirstOrDefault(c => c.Port == designator.port && c.Pin == designator.pin);
                if (cfg != null)
                {
                    ConfigureGpio(designator.port, designator.pin, cfg.Mode, cfg.Resistor, cfg.Speed, cfg.Type, cfg.InitialState, cfg.InterruptMode, 0, TimeSpan.Zero, TimeSpan.Zero, true);
                }
            }
        }

        void RegisterConfig(STM32.GpioPort port, int pin, STM32.GpioMode mode, STM32.ResistorMode resistor, STM32.GPIOSpeed speed, STM32.OutputType type, bool initialState, InterruptMode interruptMode, int alternateFunctionNumber)
        {
            Output.WriteLineIf((DebugFeatures & DebugFeature.GpioDetail) != 0, $" + RegisterConfig");

            lock (_currentConfigs)
            {
                var cfg = _currentConfigs.FirstOrDefault(c => c.Port == port && c.Pin == pin);
                if (cfg == null)
                {
                    cfg = new GpioConfig
                    {
                        Port = port,
                        Pin = pin,
                        Mode = mode,
                        Resistor = resistor,
                        Speed = speed,
                        Type = type,
                        InitialState = initialState,
                        InterruptMode = interruptMode,
                        AlternateFunctionNumber = alternateFunctionNumber
                    };
                    _currentConfigs.Add(cfg);
                }
                else
                {
                    cfg.Mode = mode;
                    cfg.Resistor = resistor;
                    cfg.Speed = speed;
                    cfg.Type = type;
                    cfg.InitialState = initialState;
                    cfg.InterruptMode = interruptMode;
                    cfg.AlternateFunctionNumber = alternateFunctionNumber;
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
