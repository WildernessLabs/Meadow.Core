using System;
using System.Runtime.InteropServices;

namespace Meadow.Core;

internal static partial class Interop
{
    public static partial class Nuttx
    {
        [DllImport(LIBRARY_NAME, SetLastError = true)]
        private static extern int meadow_cloud_decrypt_buf(IntPtr encrypted_buf, int encrypted_len, IntPtr decrypted_buf);

        const int max_buf_size = 1024;

        public static byte[] MeadowCloudDecryptRsa(byte[] buf)
        {
            IntPtr encryptedBufferPtr = IntPtr.Zero;
            IntPtr decryptedBufferPtr = IntPtr.Zero;

            try
            {
                encryptedBufferPtr = Marshal.AllocHGlobal(max_buf_size);
                decryptedBufferPtr = Marshal.AllocHGlobal(max_buf_size);

                Marshal.Copy(buf, 0, encryptedBufferPtr, buf.Length);
                int result_len = meadow_cloud_decrypt_buf(encryptedBufferPtr, buf.Length, decryptedBufferPtr);

                byte[] result = new byte[result_len];
                Marshal.Copy(decryptedBufferPtr, result, 0, result_len);
                return result;
            }
            finally
            {
                if (encryptedBufferPtr != IntPtr.Zero) Marshal.FreeHGlobal(encryptedBufferPtr);
                if (decryptedBufferPtr != IntPtr.Zero) Marshal.FreeHGlobal(decryptedBufferPtr);
            }
        }
    }
}
