using System;
using Meadow.Hardware;

namespace Meadow.Devices
{
    public abstract partial class F7MicroBase
    {
        public IPwmPort CreatePwmPort(
            IPin pin,
            float frequency = IPwmOutputController.DefaultPwmFrequency,
            float dutyCycle = IPwmOutputController.DefaultPwmDutyCycle,
            bool inverted = false)
        {
            bool isOnboard = IsOnboardLed(pin);
            return PwmPort.From(pin, this.IoController, frequency, dutyCycle, inverted, isOnboard);
        }

        /// <summary>
        /// Tests whether or not the pin passed in belongs to an onboard LED
        /// component. Used for a dirty dirty hack.
        /// </summary>
        /// <param name="pin"></param>
        /// <returns>whether or no the pin belons to the onboard LED</returns>
        protected bool IsOnboardLed(IPin pin)
        {
            // HACK NOTE: can't compare directly here, so we're comparing the name.
            // might be able to cast and compare?
            return (
                pin.Name == Pins.OnboardLedBlue.Name ||
                pin.Name == Pins.OnboardLedGreen.Name ||
                pin.Name == Pins.OnboardLedRed.Name
                );
        }

    }
}