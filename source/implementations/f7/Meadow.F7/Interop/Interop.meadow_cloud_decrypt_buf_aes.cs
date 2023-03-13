using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            private static extern int meadow_cloud_decrypt_buf_aes(IntPtr encrypted_buf, int encrypted_len, IntPtr key, int key_len, IntPtr decrypted_buf);

            const int aes_block_size = 16;

            public static byte[] MeadowCloudDecryptAES(byte[] buf, byte[] key)
            {
                IntPtr encrypted_buf = Marshal.AllocHGlobal(aes_block_size);
                IntPtr decrypted_buf = Marshal.AllocHGlobal(aes_block_size);
                IntPtr key_buf = Marshal.AllocHGlobal(key.Length);

                Marshal.Copy (buf, 0, encrypted_buf, aes_block_size);
                meadow_cloud_decrypt_buf_aes(encrypted_buf, buf.Length, key_buf, key.Length, decrypted_buf);

                byte[] result = new byte[aes_block_size];
                Marshal.Copy (decrypted_buf, result, 0, aes_block_size);
                Marshal.FreeHGlobal(encrypted_buf);
                Marshal.FreeHGlobal(decrypted_buf);
                Marshal.FreeHGlobal(key_buf);
                return result;
            }
        }
    }
}
