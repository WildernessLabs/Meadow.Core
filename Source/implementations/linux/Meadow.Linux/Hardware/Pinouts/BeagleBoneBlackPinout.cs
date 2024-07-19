using Meadow.Hardware;

namespace Meadow.Pinouts;

/// <summary>
/// Defines the pinout configuration for a BeagleBone Black.
/// </summary>
public class BeagleBoneBlackPinout : PinDefinitionBase, IPinDefinitions
{
    private const string gpiochip0 = "gpiochip0";
    private const string gpiochip1 = "gpiochip1";
    private const string gpiochip2 = "gpiochip2";
    private const string gpiochip3 = "gpiochip3";

    internal BeagleBoneBlackPinout() { }

    /// <summary>
    /// Represents the GPIO_48 pin.
    /// </summary>
    public IPin GPIO_48 => new GpiodPin(Controller, "GPIO_48", "P9_15", gpiochip1, 16);
    /// <summary>
    /// Represents the GPIO_49 pin.
    /// </summary>
    public IPin GPIO_49 => new GpiodPin(Controller, "GPIO_49", "P9_23", gpiochip1, 17);
    /// <summary>
    /// Represents the GPIO_117 pin.
    /// </summary>
    public IPin GPIO_117 => new GpiodPin(Controller, "GPIO_117", "P9_25", gpiochip3, 21);
    /// <summary>
    /// Represents the GPIO_115 pin.
    /// </summary>
    public IPin GPIO_115 => new GpiodPin(Controller, "GPIO_115", "P9_27", gpiochip3, 19);
    /// <summary>
    /// Represents the GPIO_112 pin.
    /// </summary>
    public IPin GPIO_112 => new GpiodPin(Controller, "GPIO_112", "P9_30", gpiochip3, 16);
    /// <summary>
    /// Represents the GPIO_20 pin.
    /// </summary>
    public IPin GPIO_20 => new GpiodPin(Controller, "GPIO_20", "P9_41", gpiochip0, 20);
    /// <summary>
    /// Represents the GPIO_69 pin.
    /// </summary>
    public IPin GPIO_69 => new GpiodPin(Controller, "GPIO_69", "P8_9", gpiochip2, 5);
    /// <summary>
    /// Represents the GPIO_68 pin.
    /// </summary>
    public IPin GPIO_68 => new GpiodPin(Controller, "GPIO_68", "P8_10", gpiochip2, 4);
    /// <summary>
    /// Represents the GPIO_45 pin.
    /// </summary>
    public IPin GPIO_45 => new GpiodPin(Controller, "GPIO_45", "P8_11", gpiochip1, 13);
    /// <summary>
    /// Represents the GPIO_44 pin.
    /// </summary>
    public IPin GPIO_44 => new GpiodPin(Controller, "GPIO_44", "P8_12", gpiochip1, 12);
    /// <summary>
    /// Represents the GPIO_26 pin.
    /// </summary>
    public IPin GPIO_26 => new GpiodPin(Controller, "GPIO_26", "P8_14", gpiochip1, 18);
    /// <summary>
    /// Represents the GPIO_47 pin.
    /// </summary>
    public IPin GPIO_47 => new GpiodPin(Controller, "GPIO_47", "P8_15", gpiochip1, 16);
    /// <summary>
    /// Represents the GPIO_46 pin.
    /// </summary>
    public IPin GPIO_46 => new GpiodPin(Controller, "GPIO_46", "P8_16", gpiochip1, 14);
    /// <summary>
    /// Represents the GPIO_27 pin.
    /// </summary>
    public IPin GPIO_27 => new GpiodPin(Controller, "GPIO_27", "P8_17", gpiochip0, 27);
    /// <summary>
    /// Represents the GPIO_65 pin.
    /// </summary>
    public IPin GPIO_65 => new GpiodPin(Controller, "GPIO_65", "P8_18", gpiochip2, 1);
    /// <summary>
    /// Represents the GPIO_61 pin.
    /// </summary>
    public IPin GPIO_61 => new GpiodPin(Controller, "GPIO_61", "P8_26", gpiochip1, 29);
    /// <summary>
    /// Represents the GPIO_60 pin.
    /// </summary>
    public IPin GPIO_60 => new GpiodPin(Controller, "GPIO_60", "P9_12", gpiochip1, 28);
    /// <summary>
    /// Represents the GPIO_66 pin.
    /// </summary>
    public IPin GPIO_66 => new GpiodPin(Controller, "GPIO_66", "P8_7", gpiochip2, 2);
    /// <summary>
    /// Represents the GPIO_67 pin.
    /// </summary>
    public IPin GPIO_67 => new GpiodPin(Controller, "GPIO_67", "P8_8", gpiochip2, 3);

    /// <summary>
    /// Represents the AIN0 pin.
    /// </summary>
    public IPin AIN0 => new Pin(Controller, "AIN0", "P9_39", new[] { new AnalogChannelInfo("AIN0", 12, true, false) });
    /// <summary>
    /// Represents the AIN1 pin.
    /// </summary>
    public IPin AIN1 => new Pin(Controller, "AIN1", "P9_40", new[] { new AnalogChannelInfo("AIN1", 12, true, false) });
    /// <summary>
    /// Represents the AIN2 pin.
    /// </summary>
    public IPin AIN2 => new Pin(Controller, "AIN2", "P9_37", new[] { new AnalogChannelInfo("AIN2", 12, true, false) });
    /// <summary>
    /// Represents the AIN3 pin.
    /// </summary>
    public IPin AIN3 => new Pin(Controller, "AIN3", "P9_38", new[] { new AnalogChannelInfo("AIN3", 12, true, false) });
    /// <summary>
    /// Represents the AIN4 pin.
    /// </summary>
    public IPin AIN4 => new Pin(Controller, "AIN4", "P9_33", new[] { new AnalogChannelInfo("AIN4", 12, true, false) });
    /// <summary>
    /// Represents the AIN5 pin.
    /// </summary>
    public IPin AIN5 => new Pin(Controller, "AIN5", "P9_36", new[] { new AnalogChannelInfo("AIN5", 12, true, false) });
    /// <summary>
    /// Represents the AIN6 pin.
    /// </summary>
    public IPin AIN6 => new Pin(Controller, "AIN6", "P9_35", new[] { new AnalogChannelInfo("AIN6", 12, true, false) });

    /// <summary>
    /// Represents Pin P9_12, which corresponds to GPIO_60.
    /// </summary>
    public IPin P9_12 => GPIO_60;
    /// <summary>
    /// Represents Pin P9_15, which corresponds to GPIO_48.
    /// </summary>
    public IPin P9_15 => GPIO_48;
    /// <summary>
    /// Represents Pin P8_7, which corresponds to GPIO_66.
    /// </summary>
    public IPin P8_7 => GPIO_66;
    /// <summary>
    /// Represents Pin P8_8, which corresponds to GPIO_67
    /// </summary>
    public IPin P8_8 => GPIO_67;
    /// <summary>
    /// Represents Pin P9_23, which corresponds to GPIO_49
    /// </summary>
    public IPin P9_23 => GPIO_49;
    /// <summary>
    /// Represents Pin P9_25, which corresponds to GPIO_117
    /// </summary>
    public IPin P9_25 => GPIO_117;
    /// <summary>
    /// Represents Pin P9_27, which corresponds to GPIO_115
    /// </summary>
    public IPin P9_27 => GPIO_115;
    /// <summary>
    /// Represents Pin P9_30, which corresponds to GPIO_112
    /// </summary>
    public IPin P9_30 => GPIO_112;
    /// <summary>
    /// Represents Pin P9_41, which corresponds to GPIO_20
    /// </summary>
    public IPin P9_41 => GPIO_20;
    /// <summary>
    /// Represents Pin P8_9, which corresponds to GPIO_69
    /// </summary>
    public IPin P8_9 => GPIO_69;
    /// <summary>
    /// Represents Pin P8_10, which corresponds to GPIO_68
    /// </summary>
    public IPin P8_10 => GPIO_68;
    /// <summary>
    /// Represents Pin P8_11, which corresponds to GPIO_45
    /// </summary>
    public IPin P8_11 => GPIO_45;
    /// <summary>
    /// Represents Pin P8_12, which corresponds to GPIO_44
    /// </summary>
    public IPin P8_12 => GPIO_44;
    /// <summary>
    /// Represents Pin P8_14, which corresponds to GPIO_26
    /// </summary>
    public IPin P8_14 => GPIO_26;
    /// <summary>
    /// Represents Pin P8_15, which corresponds to GPIO_47
    /// </summary>
    public IPin P8_15 => GPIO_47;
    /// <summary>
    /// Represents Pin P8_16, which corresponds to GPIO_46
    /// </summary>
    public IPin P8_16 => GPIO_46;
    /// <summary>
    /// Represents Pin P8_17, which corresponds to GPIO_27
    /// </summary>
    public IPin P8_17 => GPIO_27;
    /// <summary>
    /// Represents Pin P8_18, which corresponds to GPIO_65
    /// </summary>
    public IPin P8_18 => GPIO_65;
    /// <summary>
    /// Represents Pin P8_26, which corresponds to GPIO_61
    /// </summary>
    public IPin P8_26 => GPIO_61;

    /// <summary>
    /// Represents Pin P9_33, which corresponds to AIN4
    /// </summary>
    public IPin P9_33 => AIN4;
    /// <summary>
    /// Represents Pin P9_35, which corresponds to AIN6
    /// </summary>
    public IPin P9_35 => AIN6;
    /// <summary>
    /// Represents Pin P9_36, which corresponds to AIN5
    /// </summary>
    public IPin P9_36 => AIN5;
    /// <summary>
    /// Represents Pin P9_37, which corresponds to AIN2
    /// </summary>
    public IPin P9_37 => AIN2;
    /// <summary>
    /// Represents Pin P9_38, which corresponds to AIN3
    /// </summary>
    public IPin P9_38 => AIN3;
    /// <summary>
    /// Represents Pin P9_39, which corresponds to AIN0
    /// </summary>
    public IPin P9_39 => AIN0;
    /// <summary>
    /// Represents Pin P9_40, which corresponds to AIN1
    /// </summary>
    public IPin P9_40 => AIN1;
}

/*
[application] gpiochip0 line 00:            "[mdio_data]"                  unused   input  active-high
[application] gpiochip0 line 01:             "[mdio_clk]"                  unused   input  active-high
[application] gpiochip0 line 02:      "P9_22 [spi0_sclk]"                 "P9_22"   input  active-high
[application] gpiochip0 line 03:        "P9_21 [spi0_d0]"                 "P9_21"   input  active-high
[application] gpiochip0 line 04:        "P9_18 [spi0_d1]"                 "P9_18"   input  active-high
[application] gpiochip0 line 05:       "P9_17 [spi0_cs0]"                 "P9_17"   input  active-high
[application] gpiochip0 line 06:              "[mmc0_cd]"                    "cd"   input  active-low
[application] gpiochip0 line 07:      "P8_42A [ecappwm0]"                 "P9_42"   input  active-high
[application] gpiochip0 line 08:        "P8_35 [lcd d12]"                 "P8_35"   input  active-high
[application] gpiochip0 line 09:        "P8_33 [lcd d13]"                 "P8_33"   input  active-high
[application] gpiochip0 line 10:        "P8_31 [lcd d14]"                 "P8_31"   input  active-high
[application] gpiochip0 line 11:        "P8_32 [lcd d15]"                 "P8_32"   input  active-high
[application] gpiochip0 line 12:       "P9_20 [i2c2_sda]"                 "P9_20"   input  active-high
[application] gpiochip0 line 13:       "P9_19 [i2c2_scl]"                 "P9_19"   input  active-high
[application] gpiochip0 line 14:      "P9_26 [uart1_rxd]"                 "P9_26"   input  active-high
[application] gpiochip0 line 15:      "P9_24 [uart1_txd]"                 "P9_24"   input  active-high
[application] gpiochip0 line 16:           "[rmii1_txd3]"                  unused   input  active-high
[application] gpiochip0 line 17:           "[rmii1_txd2]"                  unused   input  active-high
[application] gpiochip0 line 18:         "[usb0_drvvbus]"                  unused   input  active-high
[application] gpiochip0 line 19:             "[hdmi cec]"                  unused   input  active-high
[application] gpiochip0 line 20:                 "P9_41B"                 "P9_41"   input  active-high
[application] gpiochip0 line 21:           "[rmii1_txd1]"                  unused   input  active-high
[application] gpiochip0 line 22:       "P8_19 [ehrpwm2a]"                 "P8_19"   input  active-high
[application] gpiochip0 line 23:       "P8_13 [ehrpwm2b]"                 "P8_13"   input  active-high
[application] gpiochip0 line 24:                     "NC"                  unused   input  active-high
[application] gpiochip0 line 25:                     "NC"                  unused   input  active-high
[application] gpiochip0 line 26:                  "P8_14"                 "P8_14"   input  active-high
[application] gpiochip0 line 27:                  "P8_17"                 "P8_17"   input  active-high
[application] gpiochip0 line 28:           "[rmii1_txd0]"                  unused   input  active-high
[application] gpiochip0 line 29:         "[rmii1_refclk]"                  unused   input  active-high
[application] gpiochip0 line 30:      "P9_11 [uart4_rxd]"                 "P9_11"   input  active-high
[application] gpiochip0 line 31:      "P9_13 [uart4_txd]"                 "P9_13"   input  active-high
[application] gpiochip1 line 00:      "P8_25 [mmc1_dat0]"                 "P8_25"   input  active-high
[application] gpiochip1 line 01:            "[mmc1_dat1]"                 "P8_24"   input  active-high
[application] gpiochip1 line 02:       "P8_5 [mmc1_dat2]"                 "P8_05"   input  active-high
[application] gpiochip1 line 03:       "P8_6 [mmc1_dat3]"                 "P8_06"   input  active-high
[application] gpiochip1 line 04:      "P8_23 [mmc1_dat4]"                 "P8_23"   input  active-high
[application] gpiochip1 line 05:      "P8_22 [mmc1_dat5]"                 "P8_22"   input  active-high
[application] gpiochip1 line 06:       "P8_3 [mmc1_dat6]"                 "P8_03"   input  active-high
[application] gpiochip1 line 07:       "P8_4 [mmc1_dat7]"                 "P8_04"   input  active-high
[application] gpiochip1 line 08:                     "NC"                 "reset"  output  active-low
[application] gpiochip1 line 09:                     "NC"                  unused   input  active-high
[application] gpiochip1 line 10:                     "NC"                  unused   input  active-high
[application] gpiochip1 line 11:                     "NC"                  unused   input  active-high
[application] gpiochip1 line 12:                  "P8_12"                 "P8_12"   input  active-high
[application] gpiochip1 line 13:                  "P8_11"                 "P8_11"   input  active-high
[application] gpiochip1 line 14:                  "P8_16"                 "P8_16"   input  active-high
[application] gpiochip1 line 15:                  "P8_15"                 "P8_15"   input  active-high
[application] gpiochip1 line 16:                 "P9_15A"                 "P9_15"   input  active-high
[application] gpiochip1 line 17:                  "P9_23"                 "P9_23"   input  active-high
[application] gpiochip1 line 18:       "P9_14 [ehrpwm1a]"                 "P9_14"   input  active-high
[application] gpiochip1 line 19:       "P9_16 [ehrpwm1b]"                 "P9_16"   input  active-high
[application] gpiochip1 line 20:             "[emmc rst]"                  unused   input  active-high
[application] gpiochip1 line 21:             "[usr0 led]" "beaglebone:green:usr0"  output  active-high
[application] gpiochip1 line 22:             "[usr1 led]" "beaglebone:green:usr1"  output  active-high
[application] gpiochip1 line 23:             "[usr2 led]" "beaglebone:green:usr2"  output  active-high
[application] gpiochip1 line 24:             "[usr3 led]" "beaglebone:green:usr3"  output  active-high
[application] gpiochip1 line 25:             "[hdmi irq]"             "interrupt"   input  active-high
[application] gpiochip1 line 26:          "[usb vbus oc]"                  unused   input  active-high
[application] gpiochip1 line 27:           "[hdmi audio]"                "enable"  output  active-high
[application] gpiochip1 line 28:                  "P9_12"                 "P9_12"   input  active-high
[application] gpiochip1 line 29:                  "P8_26"                 "P8_26"   input  active-high
[application] gpiochip1 line 30:           "P8_21 [emmc]"                 "P8_21"   input  active-high
[application] gpiochip1 line 31:           "P8_20 [emmc]"                 "P8_20"   input  active-high
[application] gpiochip2 line 00:                 "P9_15B"                  unused   input  active-high
[application] gpiochip2 line 01:                  "P8_18"                 "P8_18"   input  active-high
[application] gpiochip2 line 02:                   "P8_7"                 "P8_07"   input  active-high
[application] gpiochip2 line 03:                   "P8_8"                 "P8_08"   input  active-high
[application] gpiochip2 line 04:                  "P8_10"                 "P8_10"   input  active-high
[application] gpiochip2 line 05:                   "P8_9"                 "P8_09"   input  active-high
[application] gpiochip2 line 06:           "P8_45 [hdmi]"                 "P8_45"   input  active-high
[application] gpiochip2 line 07:           "P8_46 [hdmi]"                 "P8_46"   input  active-high
[application] gpiochip2 line 08:           "P8_43 [hdmi]"                 "P8_43"   input  active-high
[application] gpiochip2 line 09:           "P8_44 [hdmi]"                 "P8_44"   input  active-high
[application] gpiochip2 line 10:           "P8_41 [hdmi]"                 "P8_41"   input  active-high
[application] gpiochip2 line 11:           "P8_42 [hdmi]"                 "P8_42"   input  active-high
[application] gpiochip2 line 12:           "P8_39 [hdmi]"                 "P8_39"   input  active-high
[application] gpiochip2 line 13:           "P8_40 [hdmi]"                 "P8_40"   input  active-high
[application] gpiochip2 line 14:           "P8_37 [hdmi]"                 "P8_37"   input  active-high
[application] gpiochip2 line 15:           "P8_38 [hdmi]"                 "P8_38"   input  active-high
[application] gpiochip2 line 16:           "P8_36 [hdmi]"                 "P8_36"   input  active-high
[application] gpiochip2 line 17:           "P8_34 [hdmi]"                 "P8_34"   input  active-high
[application] gpiochip2 line 18:           "[rmii1_rxd3]"                  unused   input  active-high
[application] gpiochip2 line 19:           "[rmii1_rxd2]"                  unused   input  active-high
[application] gpiochip2 line 20:           "[rmii1_rxd1]"                  unused   input  active-high
[application] gpiochip2 line 21:           "[rmii1_rxd0]"                  unused   input  active-high
[application] gpiochip2 line 22:           "P8_27 [hdmi]"                 "P8_27"   input  active-high
[application] gpiochip2 line 23:           "P8_29 [hdmi]"                 "P8_29"   input  active-high
[application] gpiochip2 line 24:           "P8_28 [hdmi]"                 "P8_28"   input  active-high
[application] gpiochip2 line 25:           "P8_30 [hdmi]"                 "P8_30"   input  active-high
[application] gpiochip2 line 26:            "[mmc0_dat3]"                  unused   input  active-high
[application] gpiochip2 line 27:            "[mmc0_dat2]"                  unused   input  active-high
[application] gpiochip2 line 28:            "[mmc0_dat1]"                  unused   input  active-high
[application] gpiochip2 line 29:            "[mmc0_dat0]"                  unused   input  active-high
[application] gpiochip2 line 30:             "[mmc0_clk]"                  unused   input  active-high
[application] gpiochip2 line 31:             "[mmc0_cmd]"                  unused   input  active-high
[application] gpiochip3 line 00:              "[mii col]"                  unused   input  active-high
[application] gpiochip3 line 01:              "[mii crs]"                  unused   input  active-high
[application] gpiochip3 line 02:           "[mii rx err]"                  unused   input  active-high
[application] gpiochip3 line 03:            "[mii tx en]"                  unused   input  active-high
[application] gpiochip3 line 04:            "[mii rx dv]"                  unused   input  active-high
[application] gpiochip3 line 05:             "[i2c0 sda]"                  unused   input  active-high
[application] gpiochip3 line 06:             "[i2c0 scl]"                  unused   input  active-high
[application] gpiochip3 line 07:            "[jtag emu0]"                  unused   input  active-high
[application] gpiochip3 line 08:            "[jtag emu1]"                  unused   input  active-high
[application] gpiochip3 line 09:           "[mii tx clk]"                  unused   input  active-high
[application] gpiochip3 line 10:           "[mii rx clk]"                  unused   input  active-high
[application] gpiochip3 line 11:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 12:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 13:          "[usb vbus en]"                  unused   input  active-high
[application] gpiochip3 line 14:      "P9_31 [spi1_sclk]"                 "P9_31"   input  active-high
[application] gpiochip3 line 15:        "P9_29 [spi1_d0]"                 "P9_29"   input  active-high
[application] gpiochip3 line 16:        "P9_30 [spi1_d1]"                 "P9_30"   input  active-high
[application] gpiochip3 line 17:       "P9_28 [spi1_cs0]"                 "P9_28"   input  active-high
[application] gpiochip3 line 18:      "P9_42B [ecappwm0]"                 "P9_92"   input  active-high
[application] gpiochip3 line 19:                  "P9_27"                 "P9_27"   input  active-high
[application] gpiochip3 line 20:                 "P9_41A"                 "P9_91"   input  active-high
[application] gpiochip3 line 21:                  "P9_25"                 "P9_25"   input  active-high
[application] gpiochip3 line 22:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 23:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 24:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 25:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 26:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 27:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 28:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 29:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 30:                     "NC"                  unused   input  active-high
[application] gpiochip3 line 31:                     "NC"                  unused   input  active-high
*/