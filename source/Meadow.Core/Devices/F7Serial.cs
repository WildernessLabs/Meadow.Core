﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow.Devices
{
    internal class F7Serial
    {
        private const string DriverFolder = "/dev";
        private const string SerialPortDriverPrefix = "tty";

        public static string[] GetAvailablePorts()
        {
            var allDevices = F7Micro.FileSystem.EnumDirectory(DriverFolder);
            var list = new List<string>();
            foreach (var s in allDevices)
            {
                if (s.StartsWith(SerialPortDriverPrefix))
                {
                    list.Add(s);
                }
            }

            return list.ToArray();
        }
    }
}
