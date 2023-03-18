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
                IntPtr encryptedBufferPtr = IntPtr.Zero;
                IntPtr decryptedBufferPtr = IntPtr.Zero;
                IntPtr keyBufferPtr = IntPtr.Zero;

                try
                {
                    encryptedBufferPtr = Marshal.AllocHGlobal(aes_block_size);
                    decryptedBufferPtr = Marshal.AllocHGlobal(aes_block_size);
                    keyBufferPtr = Marshal.AllocHGlobal(key.Length);

                    Marshal.Copy(buf, 0, encryptedBufferPtr, aes_block_size);
                    meadow_cloud_decrypt_buf_aes(encryptedBufferPtr, buf.Length, keyBufferPtr, key.Length, decryptedBufferPtr);

                    // TODO: there's a return value, can we detect success/fail here?

                    byte[] result = new byte[aes_block_size];
                    Marshal.Copy(decryptedBufferPtr, result, 0, aes_block_size);
                    return result;
                }
                finally
                {
                    Marshal.FreeHGlobal(encryptedBufferPtr);
                    Marshal.FreeHGlobal(decryptedBufferPtr);
                    Marshal.FreeHGlobal(keyBufferPtr);
                }
            }
        }
    }
}
