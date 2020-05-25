# Meadow Core Library

## Interrupt Notifications

There are two ways to obtain the state of a GPIO configured as input. One is to directly read the GPIO's state the other is to use interrupts to notify you when the GPIO's state changes. This document focuses on using interrupts, to notify your application when GPIO states change.

### Interrupt Basics

A GPIO, configured for input, can have only two valid states, High (AKA true) or Low (AKA false. *High* represents a voltage at or near the MCU's supply voltage, on the GPIO's pin. In the case of the F7 this is voltge is 3.3 VDC. *Low* is when the GPIO's input pin is at or near GND or zero volts. Any voltage in between these 2 cannot be determined by the MCU with certainty. The MCU will select either High or Low or may jump between the two randomly.

Assuming you want to be notified when the input changes state. First you create an input using Device.CreateDigitalInputPort and specify the following arguments. The entries marked (default) are the values that will be assigned if left unassigned by the caller.

1. IPin - Identifies the Meadow pin (e.g. Device.Pins.D03).
2. InterruptMode - *EdgeRising*, **EdgeFalling*, *EdgeBoth* or *None* (default).
3. ResistorMode - *PullUp*, *PullDown* or *Disabled* (default).
4. DebounceDuration - a numeric value 0 (default) or 0.1 - 1000 representing a number of milliseconds
5. GlitchDuration - a numeric value 0 (default) or 0.1 - 1000 representing a number of milliseconds

Of these, InterruptMode, DebounceDuration and GlitchDuration effect how interrupt notification behaves.
The Meadow implementation of *InterruptMode* is pretty much standard for industry. However, the understand of the terms *Debounce* and *Glitch* are not universally agreed upon. The following will explain the behavior and terminology used by Meadow.

#### Interrupt Mode

The chosen InterruptMode will establish the criteria for notifying the application. An *EdgeRising* InterruptMode will only notify when the input's voltage goes from *Low* to *High* (a rising voltage). Conversely, an *EdgeFalling* only notifies when the input's voltage goes from *High* to *Low*. The selection of *EdgeBoth* causes a transition in either direction (rising or falling) to notify the application. When the Interrupt Mode is *None*, no application notification occurs.

#### Debounce Filter

A *DebounceDuration* of zero will disable the Debounce filter. For a *DebounceDuration* greater then zero, when a GPIO input change occurs, Meadow will immediately read the state of the GPIO input and forward this state as a notification to the application. From the moment the interrupt occurs until the *DebounceDuration* expires no additional interrupts will be processed for this GPIO.

Note that not every change causes a notification. It depends on the selected *InterruptMode*. If the InterruptMode is *EdgeRising*, for example, then a *High* to *Low* (falling) transition will not cause a notification. When the transition is from *Low* to *High* (rising) then the application will be notified. If the Interrupt Mode is *EdgeBoth* then either rising or falling will cause a notification.

#### Glitch Filter

A *GlitchDuration* of zero will disable the Glitch filter. For values greater then zero, when an interrupt occurs the Meadow will immediately begin monitoring the state of the GPIO input. Only after the monitored GPIO has not changed (i.e. been stable) for the time specified by the *GlitchDuration* will the application be notified.

As with the Debounce filter, only changes that meet the InterruptMode criteria will notify the application.

#### Debounce and Glitch in Combinations

Both the Debounce and Glitch filters can be used in combination. When both *DebounceDuration* and *GlitchDuration* are greater than zero, the Glitch Filter will run first. After the GPIO has remained stable for the *GlitchDuration* and that the change satisfies the InterruptMode criteria, a notification will be sent to the application. After sending the change notificaton the Debounce filter will continue to inhibit GPIO notifications for the *DebounceDuration*. However, if no notification was sent, the Debounce filter process does not occur.

In the case where the Debounce and Glitch Filters are both zero (and the InterruptMode is other than *None*), Meadow will immedately read the state of the input GPIO and notify the application of the change (based on the InterruptMode criteria). As soon as the application has been notified, new changes will be processed.

#### Application Notes

##### Debounce Filter Applications

While a mechanical switch may appear to be on or off, to a high speed computer the transition from on-off or off-on is not a clean transition. Between these two final states (on or off) there often are a number of fast intermmediate transitions as the contacts *bounce*. This effect is called "switch bounce." If your curious, Google "switch bounce."

For this reason, the Debounce filter is not the best choice to use with mechanical switches connected to a Meadow. Why? Because the Debounce filter reads the GPIO input immedately after the first indication of a change, at this time the GPIO input may still be *bouncing*.

Where should the Debounce filter be used? When the source of the on-off or off-on transitions is know to be without unwanted momentary transitions, such as those from another electronic device, or GPIO configured for output.

##### Glitch Filter Applications

The Meadow Glitch Filter is designed for noise inputs, such as mechanical switches. Why? When GPIO input changes state, the Glitch Filter begins operation and only after a stable GPIO state has been monitored for the *GlitchDuration* will the Meadow, based on InterruptMode criteria, notify the application.

Or the Glitch filter can be used in a situation where an undesirable, very short transistion can occur. The Glitch filter can be set to as little as 0.1 millisecond (i.e. 100 microseconds). This would mean that all momentary transitions of less than 0.1 milliseconds would be ignored.

For a normal push button or toggle switch, *GlitchDebounce* values of 10 - 50 milliseconds are usually good enough.

##### Debounce and Glitch in Combination

Use both the Debounce and Glitch filters when you find a need to disable interrupts after the *GlitchDuration* has expired. The number of applications for feature is probably small.

When the input is from a source known to be free from momentary transitions, both *DebounceDuration* and *GlitchDuration* should be set to zero. This will immediately report the change to your application. However, there is a finite rate at which the notification can be processed. 

[ReadMe.md](https://github.com/WildernessLabs/Meadow.Core/blob/multi-interrupts/Readme.md)
