using Meadow;
using Meadow.Foundation.Sensors.Buttons;
using Meadow.Hardware;
using Meadow.Pinouts;
using System;
using System.Collections.Generic;
using System.Threading;

namespace PushButton_Sample
{
    public class MeadowApp : App<MeadowForLinux<RaspberryPi>>
    {
        private List<PushButton> _pushButtons;

        public MeadowApp()
        {
            Console.WriteLine("Initializing...");

            ConfigureButtons();

            Console.WriteLine("PushButton(s) ready!!!");
        }

        void ConfigureButtons()
        {
            _pushButtons = new List<PushButton>();

            var inputExternalPullUp = Device.CreateDigitalInputPort(
                pin: Device.Pins.GPIO21,
                InterruptMode.EdgeBoth,
                resistorMode: ResistorMode.ExternalPullUp);
            var buttonExternalPullUp = new PushButton(inputExternalPullUp);

            _pushButtons.Add(buttonExternalPullUp);

            var inputExternalPullDown = Device.CreateDigitalInputPort(
                pin: Device.Pins.GPIO20,
                InterruptMode.EdgeBoth,
                resistorMode: ResistorMode.ExternalPullDown);
            var buttonExternalPullDown = new PushButton(inputExternalPullDown);

            _pushButtons.Add(buttonExternalPullDown);

            foreach (var pushButton in _pushButtons)
            {
                pushButton.LongClickedThreshold = new TimeSpan(0, 0, 1);

                pushButton.Clicked += PushButtonClicked;
                pushButton.PressStarted += PushButtonPressStarted;
                pushButton.PressEnded += PushButtonPressEnded;
                pushButton.LongClicked += PushButtonLongClicked;
            }
        }

        void PushButtonClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton Clicked!");
            // TODO: set some output/LED
            Thread.Sleep(500);
            // TODO: set some output/LED
        }

        void PushButtonPressStarted(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton PressStarted!");
            // TODO: set some output/LED
        }

        void PushButtonPressEnded(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton PressEnded!");
            // TODO: set some output/LED
        }

        void PushButtonLongClicked(object sender, EventArgs e)
        {
            Console.WriteLine($"PushButton LongClicked!");
            // TODO: set some output/LED
            Thread.Sleep(500);
            // TODO: set some output/LED
        }
    }
}
