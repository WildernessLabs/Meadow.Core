using System;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Basic_Tasks
{
    public class TasksApp : AppBase<F7Micro, TasksApp>
    {
        IDigitalOutputPort out1;
        IDigitalOutputPort out2;

        public TasksApp()
        {
            out1 = Device.CreateDigitalOutputPort(Device.Pins.D00);
            out2 = Device.CreateDigitalOutputPort(Device.Pins.D01);

            out1.State = true;

            StartATask();
        }

        public void StartATask()
        {
            Task t = new Task(() => {
                while (true) {
                    out2.State = true;
                    Task.Delay(250);
                    out2.State = false;
                    Task.Delay(250);
                }
            });
            t.Start();
        }
    }
}
