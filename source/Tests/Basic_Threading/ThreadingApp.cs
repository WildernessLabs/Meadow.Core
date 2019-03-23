using System;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Basic_Threading
{
    public class ThreadingApp : AppBase<F7Micro, ThreadingApp>
    {
        IDigitalOutputPort out1;
        IDigitalOutputPort out2;

        public ThreadingApp()
        {
            out1 = Device.CreateDigitalOutputPort(Device.Pins.D00);
            out2 = Device.CreateDigitalOutputPort(Device.Pins.D01);

            out1.State = true;

            StartAThread();

            Thread.Sleep(Timeout.Infinite);
        }

        public void StartAThread()
        {
            Thread th = new Thread(() => {
                while (true) {
                    out2.State = true;
                    Thread.Sleep(250);
                    out2.State = false;
                    Thread.Sleep(250);
                }
            });
            th.Start();
        }
    }
}
