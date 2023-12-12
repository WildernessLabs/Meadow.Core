using Meadow.Cloud;
using System;
using System.IO;
using System.Runtime.InteropServices;
using SRI = System.Runtime.InteropServices;

namespace Meadow;

public partial class MeadowCloudService : IMeadowCloudService
{
    private const string LinuxPrivateKeyFileName = "id_rsa";

    private DirectoryInfo GetLinuxPrivateKeyDirectory()
    {
        return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh"));
    }

    private string? GetLinuxPrivateKeyInPemFormat()
    {
        if (!SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new PlatformNotSupportedException();
        }

        var sshFolder = GetLinuxPrivateKeyDirectory();

        if (!sshFolder.Exists)
        {
            Resolver.Log.Error("SSH folder not found");
            return null;
        }
        else
        {
            var pkFile = Path.Combine(sshFolder.FullName, LinuxPrivateKeyFileName);
            if (!File.Exists(pkFile))
            {
                Resolver.Log.Error("Private key not found");
                return null;
            }

            var pkFileContent = File.ReadAllText(pkFile);
            if (!pkFileContent.Contains(RsaPrivateKeyHeader, StringComparison.OrdinalIgnoreCase))
            {
                // DEV NOTE:  this is not ideal.  On Linux, we *convert* the private key from OpenSSH to our desired format in place.
                //            we *overwrite* the existing key file this way (we'll make a backup first).  Calling ssh-keyget always yields the
                //            public key, even when called on the private key as input
                File.Copy(pkFile, $"{pkFile}.bak");

                ExecuteBashCommandLine($"ssh-keygen -p -m pem -N '' -f {pkFile}");

                pkFileContent = File.ReadAllText(pkFile);
            }

            return pkFileContent;
        }
    }
}