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
            Stat("/meadow0/App.exe");
            CreateFile("/meadow0/hello.txt");
            Tree("/", true);
            Console.WriteLine("Testing complete");
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

        protected void Stat(string path)
        {
            Console.WriteLine($"File: {Path.GetFileName(path)}");

            try {
                using (var stream = File.Open(path, FileMode.Open, FileAccess.Read)) {
                    Console.WriteLine($"Size: {stream.Length,-8}");
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.Message);
            }
        }

        protected void Tree(string root, bool showSize = false)
        {
            var fileCount = 0;
            var folderCount = 0;

            ShowFolder(root, 0);
            Console.WriteLine(string.Empty);
            Console.WriteLine($"{folderCount} directories");
            Console.WriteLine($"{fileCount} files");

            void ShowFolder(string folder, int depth, bool last = false)
            {
                string[] files = null;

                try {
                    files = Directory.GetFiles(folder);
                    Console.WriteLine($"{GetPrefix(depth, last && files.Length == 0)}{Path.GetFileName(folder)}");
                } catch {
                    Console.WriteLine($"{GetPrefix(depth, last)}{Path.GetFileName(folder)}");
                    Console.WriteLine($"{GetPrefix(depth + 1, last)}<cannot list files>");
                }
                if (files != null) {
                    foreach (var file in files) {
                        var prefix = GetPrefix(depth + 1, last);
                        if (showSize) {
                            FileInfo fi = null;
                            try {
                                fi = new FileInfo(file);
                                prefix += $"[{fi.Length,8}]  ";

                            } catch {
                                prefix += $"[   error]  ";
                            }
                        }
                        Console.WriteLine($"{prefix}{Path.GetFileName(file)}");
                        fileCount++;
                    }
                }

                string[] dirs = null;
                try {
                    dirs = Directory.GetDirectories(folder);
                } catch {
                    if (files == null || files.Length == 0) {
                        Console.WriteLine($"{GetPrefix(depth + 1, last)}<cannot list sub-directories>");
                    } else {
                        Console.WriteLine($"{GetPrefix(depth + 1)}<cannot list sub-directories>");
                    }
                }
                if (dirs != null) {
                    for (var i = 0; i < dirs.Length; i++) {
                        ShowFolder(dirs[i], depth + 1, i == dirs.Length - 1);
                        folderCount++;
                    }
                }

                string GetPrefix(int d, bool isLast = false)
                {
                    var p = string.Empty;

                    for (var i = 0; i < d; i++) {
                        if (i == d - 1) {
                            p += "+--";
                        } else if (isLast && i == d - 2) {
                            p += "   ";
                        } else {
                            p += "|  ";
                        }
                    }

                    return p;
                }
            }
        }

        protected void DirectoryListTest(string path)
        {
            Console.WriteLine($"Enumerating path '{path}'");

            var dirs = Directory.GetDirectories(path);
            Console.WriteLine($" Found {dirs.Length} Directories {((dirs.Length > 0) ? ":" : string.Empty)}");
            foreach (var d in dirs) {
                Console.WriteLine($"   {d}");
            }
            var files = Directory.GetFiles(path);
            Console.WriteLine($" Found {files.Length} Files {((files.Length > 0) ? ":" : string.Empty)}");
            foreach (var f in files) {
                Console.WriteLine($"   {f}");
            }
        }

        protected void DirectoryListTest2()
        {
            Console.WriteLine("Enumerating logical drives...");

            var drives = Directory.GetLogicalDrives();

            Console.WriteLine($" Found {drives.Length} logical drives");

            foreach (var d in drives) {
                Console.WriteLine($"  DRIVE '{d}'");

                ShowFolder(d, 3);

                void ShowFolder(string path, int indent)
                {
                    var name = Path.GetDirectoryName(path);
                    name = string.IsNullOrEmpty(name) ? "/" : name;
                    Console.WriteLine($"{new string(' ', indent)}+ {name}");

                    foreach (var fse in Directory.GetFileSystemEntries(path)) {
                        Console.WriteLine($"{new string(' ', indent + 3)}fse {fse}");
                    }

                    foreach (var dir in Directory.GetDirectories(d)) {
                        ShowFolder(dir, indent + 1);
                    }

                    foreach (var f in Directory.GetFiles(path)) {
                        Console.WriteLine($"{new string(' ', indent + 3)}f{f}");

                        var fi = new FileInfo(f);
                        if (fi.Exists) {
                            Console.WriteLine($"{new string(' ', indent + 4)} Exists as file");
                        }
                        var di = new DirectoryInfo(f);
                        if (fi.Exists) {
                            Console.WriteLine($"{new string(' ', indent + 4)} Exists as directory");
                        }
                    }
                }
            }

            Console.WriteLine("Opening file as a dir...");
            foreach (var f in Directory.GetFiles("/meadow0")) {
                Console.WriteLine($"f {f}");
            }
        }

    }
}
