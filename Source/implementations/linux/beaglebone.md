# Meadow Linux on BeagleBone Black

The BeagleBone Black does not have a large on-board eMMC (4GB) so the necessary bits will not fit once the OS is installed.  You must boot from an SD card.

## Download the BeagleBone OS

https://www.beagleboard.org/distros

Use the following file:

```
AM335x 11.8 2023-10-07 4GB microSD IoT
```


Use Balena Etcher to write it to an microSD card

Plug the board into wired ethernet.

Insert the SD card into an UNPOWERED device

Press and hold the BOOT button (onthe obverse side of the board, right next to the position of SD slot)

Power the device.  It will boot from the SD Card.

After a minute or so, you can SSH into the device

```
ssh debian@beaglebone.local
```

default username: debian
default password: temppwd


To prevent the device from booting to the smaller eMMC device, we must invalidate it.

Write zeros over the first 1MB of the eMMC
```
sudo dd if=/dev/zero of=/dev/mmcblk1 bs=1M count=1
run sync
run sudo shutdown now
```

Disconnect and then reconnect power to boot again from SD without the need of the `BOOT` button.

## Install .NET

Create and edit a new shell script
```
nano raspi-dotnet.sh
```

```
#!/bin/bash

add_to_bashrc() {
    local string="$1"
    if ! grep -q "$string" "$HOME/.bashrc"; then
        echo "$string" >> $HOME/.bashrc
    fi
}

wget --inet4-only https://dot.net/v1/dotnet-install.sh -O - | bash /dev/stdin --version latest
add_to_bashrc "export DOTNET_ROOT=\$HOME/.dotnet"
add_to_bashrc "export PATH=\$PATH:\$HOME/.dotnet"
source ~/.bashrc
```

<ctrl-s><ctrl-x>

List the local files

```
ls -l
```
Which gives an output like this:
```
total 4
-rw-r--r-- 1 pi pi 364 May 28 20:22 raspi-dotnet.sh
```

Make the shell script executable

```
chmod +x raspi-dotnet.sh
```
The file list will change
```
ls -l
total 4
-rwxr-xr-x 1 pi pi 364 May 28 20:22 raspi-dotnet.sh
```

Execute the newly-created script

```
./raspi-dotnet.sh
```

This takes a long time, like over 5 minutes long.  Be patient.  In the end you should see something like

```
dotnet-install: Installation finished successfully.
```

Now re-load your environment and verify the install

```
source ~/.bashrc
dotnet --version
```
This wll give the installed .NET version.
```
8.0.301
```

## Configuring Pins

If you want to use a pin on the BeagleBone for a function other than its default configuration, you must change its behavior.  In example of this is PWM. None of the BeagleBone pins are configured for PWM by default, they are GPIO.  If you create a `PwmOutputPort` in your application, you will get no errors, but you also won't see any PWM signal.

You must reconfigure the pin to be used for PWM.  For example, to change `P9_42` from `GPIO7` to `ECAPPWM0` you must run this:

```
sudo config-pin P9.42 pwm
```

## Configuring for SPI

`SPI0` takes a little extra work to access

First, the pins must be configured for SPI:

```
sudo config-pin P9.18 spi
sudo config-pin P9.21 spi
sudo config-pin P9.22 spi_sclk_
```

The the `spi` group must be created, applied to the driver, and the use radded to the group:

```
sudo groupadd spi
sudo adduser debian spi
sudo chgrp spi /dev/spidev0.0
sudo chmod 660 /dev/spidev0.0
newgrp spi
```