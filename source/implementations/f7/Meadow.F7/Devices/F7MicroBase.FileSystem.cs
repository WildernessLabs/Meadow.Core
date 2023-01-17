using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Meadow.Devices
{
    public partial class F7MicroBase
    {
        internal static class FileSystem
        {
            public static string[] EnumDirectory(string root)
            {
                var cmd = new Core.Interop.Nuttx.UpdEnumDirCmd();

                // Dev Note: marshalling for mono/nuttx is....interesting.  
                // Couldn't get a simple string /stringbuilder to marshal, so here we are
                var buffer1 = Encoding.ASCII.GetBytes(root);
                var buffer2 = new byte[2048];

                var gch = GCHandle.Alloc(buffer1, GCHandleType.Pinned);
                var gch2 = GCHandle.Alloc(buffer2, GCHandleType.Pinned);

                try
                {

                    cmd.RootFolder = gch.AddrOfPinnedObject();
                    cmd.Buffer = gch2.AddrOfPinnedObject();
                    cmd.BufferLength = buffer2.Length;

                    var result = UPD.Ioctl(Core.Interop.Nuttx.UpdIoctlFn.DirEnum, ref cmd);
                    if (result != 0)
                    {
                        var error = UPD.GetLastError();
                        throw new NativeException(error.ToString());
                    }

                    var all = Encoding.ASCII.GetString(buffer2).TrimEnd('\0');

                    var allDevices = all.Split('\n');

                    return allDevices;
                }
                finally
                {
                    if (gch.IsAllocated) gch.Free();
                    if (gch2.IsAllocated) gch2.Free();
                }
            }
        }
    }
}
