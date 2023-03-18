using Meadow.Hardware;
using System.Collections;
using System.Collections.Generic;

namespace Meadow.Pinouts
{
    public class JetsonXavierAGX : IPinDefinitions
    {
        public IEnumerator<IPin> GetEnumerator() => AllPins.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        public IList<IPin> AllPins => new List<IPin> {
            I2C_GP5_DAT,
            I2C_GP5_CLK,
            MCLK05,
            UART1_RTS,
            I2S2_CLK,
            PWM01,
            GPIO27_PWM2,
            GPIO8_AO_DMIC_IN_DAT,
            GPIO35_PWM3,
            SPI1_MOSI,
            SPI1_MISO,
            GPIO17_40HEADER,
            SPI1_SCLK,
            SPI1_CS0,
            SPI1_CS1,
            CAN0_DIN,
            CAN0_DOUT,
            GPIO9_CAN1_GPIO0_DMIC_CLK,
            CAN1_DOUT,
            I2S_FS,
            UART1_CTS,
            CAN1_DIN,
            I2S_SDIN,
            I2S_SDOUT
        };

        public IPinController Controller { get; set; }

        public JetsonXavierAGX()
        {
        }

        public IPin I2C_GP5_DAT => new Pin(Controller, "I2C_GP5_DAT", "PIN03", null);
        public IPin I2C_GP5_CLK => new Pin(Controller, "I2C_GP5_CLK", "PIN05", null);
        public IPin MCLK05 => new SysFsPin(Controller, "MCLK05", "PIN07", 422);
        public IPin UART1_RTS => new SysFsPin(Controller, "UART1_RTS", "PIN11", 428);
        public IPin I2S2_CLK => new SysFsPin(Controller, "I2S2_CLK", "PIN12", 351);
        public IPin PWM01 => new SysFsPin(Controller, "PWM01", "PIN13", 424);
        public IPin GPIO27_PWM2 => new SysFsPin(Controller, "GPIO27_PWM2", "PIN15", 393);
        public IPin GPIO8_AO_DMIC_IN_DAT => new SysFsPin(Controller, "GPIO8_AO_DMIC_IN_DAT", "PIN16", 256);
        public IPin GPIO35_PWM3 => new SysFsPin(Controller, "GPIO35_PWM3", "PIN18", 344);
        public IPin SPI1_MOSI => new SysFsPin(Controller, "SPI1_MOSI", "PIN19", 493);
        public IPin SPI1_MISO => new SysFsPin(Controller, "SPI1_MISO", "PIN21", 492);
        public IPin GPIO17_40HEADER => new SysFsPin(Controller, "GPIO17_40HEADER", "PIN22", 417);
        public IPin SPI1_SCLK => new SysFsPin(Controller, "SPI1_SCLK", "PIN23", 491);
        public IPin SPI1_CS0 => new SysFsPin(Controller, "SPI1_CS0", "PIN24", 494);
        public IPin SPI1_CS1 => new SysFsPin(Controller, "SPI1_CS1", "PIN26", 495);
        public IPin I2C_GP2_DAT => new Pin(Controller, "I2C_GP2_DAT", "PIN27", null);
        public IPin I2C_GP2_CLK => new Pin(Controller, "I2C_GP2_CLK", "PIN28", null);
        public IPin CAN0_DIN => new SysFsPin(Controller, "CAN0_DIN", "PIN29", 251);
        public IPin CAN0_DOUT => new SysFsPin(Controller, "CAN0_DOUT", "PIN31", 250);
        public IPin GPIO9_CAN1_GPIO0_DMIC_CLK => new SysFsPin(Controller, "GPIO9_CAN1_GPIO0_DMIC_CLK", "PIN32", 257);
        public IPin CAN1_DOUT => new SysFsPin(Controller, "CAN1_DOUT", "PIN33", 248);
        public IPin I2S_FS => new SysFsPin(Controller, "I2S_FS", "PIN35", 354);
        public IPin UART1_CTS => new SysFsPin(Controller, "UART1_CTS", "PIN36", 429);
        public IPin CAN1_DIN => new SysFsPin(Controller, "CAN1_DIN", "PIN37", 249);
        public IPin I2S_SDIN => new SysFsPin(Controller, "I2S_SDIN", "PIN38", 353);
        public IPin I2S_SDOUT => new SysFsPin(Controller, "I2S_SDOUT", "PIN40", 352);

        // aliases for sanity
        public IPin Pin7 => MCLK05;
        public IPin Pin11 => UART1_RTS;
        public IPin Pin12 => I2S2_CLK;
        public IPin Pin13 => PWM01;
        public IPin Pin15 => GPIO27_PWM2;
        public IPin Pin16 => GPIO8_AO_DMIC_IN_DAT;
        public IPin Pin18 => GPIO35_PWM3;
        public IPin Pin19 => SPI1_MOSI;
        public IPin Pin21 => SPI1_MISO;
        public IPin Pin22 => GPIO17_40HEADER;
        public IPin Pin23 => SPI1_SCLK;
        public IPin Pin24 => SPI1_CS0;
        public IPin Pin26 => SPI1_CS1;
        public IPin Pin29 => CAN0_DIN;
        public IPin Pin31 => CAN0_DOUT;
        public IPin Pin32 => GPIO9_CAN1_GPIO0_DMIC_CLK;
        public IPin Pin33 => CAN1_DOUT;
        public IPin Pin35 => I2S_FS;
        public IPin Pin36 => UART1_CTS;
        public IPin Pin37 => CAN1_DIN;
        public IPin Pin38 => I2S_SDIN;
        public IPin Pin40 => I2S_SDOUT;
    }
}
