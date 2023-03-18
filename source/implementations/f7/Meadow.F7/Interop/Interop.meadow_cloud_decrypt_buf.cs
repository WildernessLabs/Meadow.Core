using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            private static extern int meadow_cloud_decrypt_buf(IntPtr encrypted_buf, int encrypted_len, IntPtr decrypted_buf);

            const int max_buf_size = 1024;

            public static byte[] MeadowCloudDecrypt(byte[] buf)
            {
                IntPtr encrypted_buf = Marshal.AllocHGlobal(max_buf_size);
                IntPtr decrypted_buf = Marshal.AllocHGlobal(max_buf_size);

                Marshal.Copy (buf, 0, encrypted_buf, buf.Length);
                int result_len = meadow_cloud_decrypt_buf(encrypted_buf, buf.Length, decrypted_buf);

                byte[] result = new byte[result_len];
                Marshal.Copy (decrypted_buf, result, 0, result_len);
                Marshal.FreeHGlobal(encrypted_buf);
                Marshal.FreeHGlobal(decrypted_buf);
                return result;
            }
        }
    }
}
