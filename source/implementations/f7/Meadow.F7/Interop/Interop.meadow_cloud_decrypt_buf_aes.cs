using System;
using System.Runtime.InteropServices;

namespace Meadow.Core
{
    internal static partial class Interop
    {
        public static partial class Nuttx
        {
            [DllImport(LIBRARY_NAME, SetLastError = true)]
            private static extern int meadow_cloud_decrypt_buf_aes(IntPtr encrypted_buf, int encrypted_len, IntPtr key, IntPtr iv, IntPtr decrypted_buf);
            const int aes_block_size = 16;
            const int aes_key_size = 16;

            public static byte[] MeadowCloudDecryptAES(byte[] encrypted_data, byte[] key, byte[] iv)
            {
                IntPtr encrypted_buf = Marshal.AllocHGlobal(encrypted_data.Length);
                IntPtr decrypted_buf = Marshal.AllocHGlobal(encrypted_data.Length);
                IntPtr iv_buf = Marshal.AllocHGlobal(aes_block_size);
                IntPtr key_buf = Marshal.AllocHGlobal(aes_key_size);

                Marshal.Copy (encrypted_data, 0, encrypted_buf, encrypted_data.Length);
                Marshal.Copy (iv, 0, iv_buf, iv.Length);
                Marshal.Copy (key, 0, key_buf, key.Length);

                int decrypted_len = meadow_cloud_decrypt_buf_aes(encrypted_buf, encrypted_data.Length, key_buf, iv_buf, decrypted_buf);

                //TODO: Assert decrypted_len = encrypted_len

                byte[] result = new byte[decrypted_len];
                Marshal.Copy (decrypted_buf, result, 0, decrypted_len);

                Marshal.FreeHGlobal(encrypted_buf);
                Marshal.FreeHGlobal(decrypted_buf);
                Marshal.FreeHGlobal(iv_buf);
                Marshal.FreeHGlobal(key_buf);
                return result;
            }
        }
    }
}
