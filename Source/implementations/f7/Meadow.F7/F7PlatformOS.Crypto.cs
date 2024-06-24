using Meadow.Core;

namespace Meadow;

public partial class F7PlatformOS
{
    /// <inheritdoc/>
    public byte[] RsaDecrypt(byte[] encryptedValue, string publicKeyPem)
    {
        // the public key is hard-coded into the F7 OS, so we ignore the incoming PEM
        return Interop.Nuttx.MeadowCloudDecryptRsa(encryptedValue);
    }

    /// <summary>
    /// Performs AES decryption of a value using the Meadow device certificate.
    /// </summary>
    /// <remarks>
    /// This method is used by the Update Service
    /// </remarks>
    /// <param name="encryptedValue">The value to decrypt</param>
    /// <param name="iv">The initialization vector to use for decryption</param>
    /// <param name="key">The key used for encrypting the buffer</param>
    /// <returns>The decrypted value</returns>
    public byte[] AesDecrypt(byte[] encryptedValue, byte[] key, byte[] iv)
    {
        return Interop.Nuttx.MeadowCloudDecryptAES(encryptedValue, key, iv);
    }

    /// <inheritdoc/>
    public string? GetPublicKeyInPemFormat()
    {
        return "Meadow F7 PEM Not Supported";
    }
}
