using System;
using System.IO;
using Meadow;
using Meadow.Devices;

namespace MeadowApp
{
    public class MeadowApp : App<F7Micro, MeadowApp>
    {
        public MeadowApp()
        {
            string appRootDir = "meadow0";
            string fileName = "text.txt";

            CreateFile(Path.Combine(appRootDir, fileName));

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


            Console.WriteLine($"File contents: {File.ReadAllText(Path.Combine(appRootDir, fileName))}");
        }

        private void CreateFile(string path)
        {
            Console.WriteLine($"Creating '{path}'...");

            try {
                using (var fs = File.CreateText(path)) {
                    fs.WriteLine("Hello Meadow File!");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

    }
}