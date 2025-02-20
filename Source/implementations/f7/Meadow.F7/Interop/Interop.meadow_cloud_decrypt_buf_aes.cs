using System;
using System.Runtime.InteropServices;

namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        private static extern int meadow_cloud_decrypt_buf_aes(IntPtr encrypted_buf, int encrypted_len, IntPtr key, IntPtr iv, IntPtr decrypted_buf);

        const int aes_key_size = 32;

        const int aes_iv_size = 16;

        public static byte[] MeadowCloudDecryptAES(byte[] encrypted_data, byte[] key, byte[] iv)
        {
            IntPtr encryptedBufferPtr = IntPtr.Zero;
            IntPtr decryptedBufferPtr = IntPtr.Zero;
            IntPtr keyBufferPtr = IntPtr.Zero;
            IntPtr ivBufferPtr = IntPtr.Zero;

            if (encrypted_data.Length % 16 != 0)
                throw new ArgumentException(nameof(encrypted_data));

            if (key.Length != aes_key_size)
                throw new ArgumentException(nameof(key));

            if (iv.Length != aes_iv_size)
                throw new ArgumentException(nameof(iv));

            try
            {
                encryptedBufferPtr = Marshal.AllocHGlobal(encrypted_data.Length);
                decryptedBufferPtr = Marshal.AllocHGlobal(encrypted_data.Length);
                ivBufferPtr = Marshal.AllocHGlobal(aes_iv_size);
                keyBufferPtr = Marshal.AllocHGlobal(aes_key_size);

                Marshal.Copy(encrypted_data, 0, encryptedBufferPtr, encrypted_data.Length);
                Marshal.Copy(iv, 0, ivBufferPtr, iv.Length);
                Marshal.Copy(key, 0, keyBufferPtr, key.Length);

                var decryptLength = meadow_cloud_decrypt_buf_aes(encryptedBufferPtr, encrypted_data.Length, keyBufferPtr, ivBufferPtr, decryptedBufferPtr);

                byte[] result = new byte[decryptLength];
                Marshal.Copy(decryptedBufferPtr, result, 0, result.Length);
                return result;
            }
            finally
            {
                Marshal.FreeHGlobal(encryptedBufferPtr);
                Marshal.FreeHGlobal(decryptedBufferPtr);
                Marshal.FreeHGlobal(keyBufferPtr);
                Marshal.FreeHGlobal(ivBufferPtr);
            }
        }
    }
}
