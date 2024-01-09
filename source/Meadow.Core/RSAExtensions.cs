using System;
using System.Linq;
using System.Security.Cryptography;

namespace Meadow;

/// <summary>
/// 
/// </summary>
/// <remarks>
/// Because Core is built against netstandard 2.1, we don't have access to ImportFromPem and have to implement it ourselves
/// </remarks>
internal static class RSAExtensions
{
    public enum RsaPublicKeyFormat
    {
        RsaPublicKey,
        RsaPrivateKey,
        SubjectPublicKeyInfo
    }

    private const string RsaPublickeyPemHeader = "-----BEGIN RSA PUBLIC KEY-----";
    private const string RsaPublickeyPemFooter = "-----END RSA PUBLIC KEY-----";
    private const string RsaPrivatekeyPemHeader = "-----BEGIN RSA PRIVATE KEY-----";
    private const string RsaPrivatekeyPemFooter = "-----END RSA PRIVATE KEY-----";
    private const string SubjectPublicKeyInfoPemHeader = "-----BEGIN PUBLIC KEY-----";
    private const string SubjectPublicKeyInfoPemFooter = "-----END PUBLIC KEY-----";

#if !(NET5_0 || NET5_0_OR_GREATER)
    //
    // Add missing method.
    //
    public static void ImportFromPem(
      this RSA key,
      string source)
      => ImportFromPem(key, source, out var _);

#endif

    public static void ImportFromPem(
      this RSA key,
      string source,
      out RsaPublicKeyFormat format)
    {
        source = source.Trim();

        //
        // Inspect header to determine format.
        //
        if (source.StartsWith(SubjectPublicKeyInfoPemHeader) &&
            source.EndsWith(SubjectPublicKeyInfoPemFooter))
        {
            format = RsaPublicKeyFormat.SubjectPublicKeyInfo;
        }
        else if (source.StartsWith(RsaPublickeyPemHeader) &&
                 source.EndsWith(RsaPublickeyPemFooter))
        {
            format = RsaPublicKeyFormat.RsaPublicKey;
        }
        else if (source.StartsWith(RsaPrivatekeyPemHeader) &&
                 source.EndsWith(RsaPrivatekeyPemFooter))
        {
            format = RsaPublicKeyFormat.RsaPrivateKey;
        }
        else
        {
            throw new FormatException("Missing key header/footer");
        }

        //
        // Decode body to get DER blob.
        //
        var der = Convert.FromBase64String(string.Concat(
          source
            .Split('\n')
            .Select(s => s.Trim())
            .Where(line => !line.StartsWith("-----"))));
        if (format == RsaPublicKeyFormat.RsaPublicKey)
        {
            key.ImportRSAPublicKey(der, out var _);
        }
        else if (format == RsaPublicKeyFormat.RsaPrivateKey)
        {
            key.ImportRSAPrivateKey(der, out var _);
        }
        else
        {
            key.ImportSubjectPublicKeyInfo(der, out var _);
        }
    }
}
