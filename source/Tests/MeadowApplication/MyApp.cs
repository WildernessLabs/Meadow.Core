using System;
using System.Threading.Tasks;
using Meadow;
using Meadow.Devices;

namespace MeadowApplication
{
    public class MyApp : AppBase<F7Micro, MyApp>
    {
        protected bool _running;

        public MyApp()
        {
            Console.WriteLine("Got here.");

            // can access anywhere via:
            var goo = MyApp.Current;
        }

        public override void Run()
        {
            StartDoingSomeStuff();
        }

        public void StartDoingSomeStuff()
        {
            _running = true;

            Task stuff = new Task (async () => {
                while(_running){
                    Console.WriteLine("I'm an app, and I'm OK.");
                    Console.WriteLine("I sleep all night, and I work all day!");
                    await Task.Delay(5000);
                }
            });
            stuff.Start();
        }
    }
}
