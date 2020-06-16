using System;
using System.Collections.Generic;
using System.IO;
using Meadow;
using Meadow.Devices;
using Meadow.Hardware;

namespace Benchmarks
{
    public class FileSystemApp : App<F7Micro, FileSystemApp>
    {
        public FileSystemApp()
        {
            Console.WriteLine("Meadow File System Tests");
            DirectoryListTest("/");
            DirectoryListTest("/meadow0");
            Console.WriteLine("Testing complete");
        }
        
        protected void DirectoryListTest(string path)
        {
            Console.WriteLine($"Enumerating path '{path}'");

            var dirs = Directory.GetDirectories(path);
            Console.WriteLine($" Found {dirs.Length} Directories {((dirs.Length > 0) ? ":" : string.Empty)}");
            foreach(var d in dirs)
            {
                Console.WriteLine($"   {d}");
            }
            var files = Directory.GetFiles(path);
            Console.WriteLine($" Found {files.Length} Files {((files.Length > 0) ? ":" : string.Empty)}");
            foreach (var f in files)
            {
                Console.WriteLine($"   {f}");
            }
        }

        protected void DirectoryListTest2()
        {
            Console.WriteLine("Enumerating logical drives...");

            var drives = Directory.GetLogicalDrives();

            Console.WriteLine($" Found {drives.Length} logical drives");

            foreach(var d in drives)
            {
                Console.WriteLine($"  DRIVE '{d}'");

                ShowFolder(d, 3);

                void ShowFolder(string path, int indent)
                {
                    var name = Path.GetDirectoryName(path);
                    name = string.IsNullOrEmpty(name) ? "/" : name;
                    Console.WriteLine($"{new string(' ', indent)}+ {name}");
                    
                    foreach(var fse in Directory.GetFileSystemEntries(path))
                    {
                        Console.WriteLine($"{new string(' ', indent + 3)}fse {fse}");
                    }

                    foreach (var dir in Directory.GetDirectories(d))
                    {
                        ShowFolder(dir, indent + 1);
                    }

                    foreach (var f in Directory.GetFiles(path))
                    {
                        Console.WriteLine($"{new string(' ', indent + 3)}f{f}");

                        var fi = new FileInfo(f);
                        if(fi.Exists)
                        {
                            Console.WriteLine($"{new string(' ', indent + 4)} Exists as file");
                        }
                        var di = new DirectoryInfo(f);
                        if (fi.Exists)
                        {
                            Console.WriteLine($"{new string(' ', indent + 4)} Exists as directory");
                        }
                    }
                }
            }

            Console.WriteLine("Opening file as a dir...");
            foreach (var f in Directory.GetFiles("/meadow0"))
            {
                Console.WriteLine($"f {f}");
            }
        }

    }
}
