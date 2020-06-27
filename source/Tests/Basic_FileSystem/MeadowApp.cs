using System;
using System.IO;
using System.Threading;
using Meadow;
using Meadow.Devices;
using Meadow.Foundation;
using Meadow.Foundation.Leds;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        RgbPwmLed onboardLed;

        public MeadowApp()
        {
            string appRootDir = "meadow0";


            // This works
            //string[] files = Directory.GetFiles("/");
            string[] files = Directory.GetFiles(appRootDir);
            foreach (var file in files) {
                Console.WriteLine($"File: {file}");
            }

            // this fails with `Unhandled Exception: System.IO.FileNotFoundException: Could not find file '/dev'.`
            //DirectoryInfo directoryInfo = new DirectoryInfo("/");
            //FileInfo[] f = directoryInfo.GetFiles();
            //foreach (FileInfo file in f) {
            //    Console.WriteLine("File Name: {0} Size: {1}", file.Name, file.Length);
            //}


        }

    }
}