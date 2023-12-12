using System;
using System.Linq;
using System.Security.Cryptography;

namespace Meadow;

/// <summary>
/// Extension methods for the RSA obect
/// </summary>
/// <remarks>
/// Because Core is built against netstandard 2.1, we don't have access to ImportFromPem and have to implement it ourselves
/// </remarks>
public static class RSAExtensions
{
    /// <summary>
    /// The format of an encryption key
    /// </summary>
    public enum KeyFormat
    {
        /// <summary>
        /// An RSA public key
        /// </summary>
        RsaPublicKey,
        /// <summary>
        /// An RSA Private Key
        /// </summary>
        RsaPrivateKey,
        /// <summary>
        /// A Subject Public Key
        /// </summary>
        SubjectPublicKeyInfo
    }

    private const string RsaPublickeyPemHeader = "-----BEGIN RSA PUBLIC KEY-----";
    private const string RsaPublickeyPemFooter = "-----END RSA PUBLIC KEY-----";
    private const string RsaPrivatekeyPemHeader = "-----BEGIN RSA PRIVATE KEY-----";
    private const string RsaPrivatekeyPemFooter = "-----END RSA PRIVATE KEY-----";
    private const string SubjectPublicKeyInfoPemHeader = "-----BEGIN PUBLIC KEY-----";
    private const string SubjectPublicKeyInfoPemFooter = "-----END PUBLIC KEY-----";

    /// <summary>
    /// Imports RSA key parameters from a PEM file
    /// </summary>
    /// <param name="key">The key on which to apply the parameters</param>
    /// <param name="source">the PEM file contents</param>
    public static void ImportFromPem(
      this RSA key,
      string source)
      => ImportFromPem(key, source, out var _);

    /// <summary>
    /// Imports RSA key parameters from a PEM file
    /// </summary>
    /// <param name="key">The key on which to apply the parameters</param>
    /// <param name="source">the PEM file contents</param>
    /// <param name="format">The detected format of the PEM data</param>
    public static void ImportFromPem(
      this RSA key,
      string source,
      out KeyFormat format)
    {
        source = source.Trim();

        //
        // Inspect header to determine format.
        //
        if (source.StartsWith(SubjectPublicKeyInfoPemHeader) &&
            source.EndsWith(SubjectPublicKeyInfoPemFooter))
        {
            format = KeyFormat.SubjectPublicKeyInfo;
        }
        else if (source.StartsWith(RsaPublickeyPemHeader) &&
                 source.EndsWith(RsaPublickeyPemFooter))
        {
            format = KeyFormat.RsaPublicKey;
        }
        else if (source.StartsWith(RsaPrivatekeyPemHeader) &&
                 source.EndsWith(RsaPrivatekeyPemFooter))
        {
            format = KeyFormat.RsaPrivateKey;
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
        if (format == KeyFormat.RsaPublicKey)
        {
            key.ImportRSAPublicKey(der, out var _);
        }
        else if (format == KeyFormat.RsaPrivateKey)
        {
            key.ImportRSAPrivateKey(der, out var _);
        }
        else
        {
            key.ImportSubjectPublicKeyInfo(der, out var _);
        }
    }
}
