using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SRI = System.Runtime.InteropServices;

namespace Meadow;

/// <summary>
/// An implementation of the ICryptographyService interface
/// </summary>
public class CryptographyService : ICryptographyService
{
    /// <inheritdoc/>
    public string? GetPublicKeyInPemFormat()
    {
        // TODO: add capability to load this from a file, etc. depending on environment or config

        if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var sshFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh"));

            if (!sshFolder.Exists)
            {
                throw new Exception("SSH folder not found");
            }
            else
            {
                var pkFile = Path.Combine(sshFolder.FullName, "id_rsa.pub");
                if (!File.Exists(pkFile))
                {
                    throw new Exception("Public key not found");
                }

                var pkFileContent = File.ReadAllText(pkFile);
                if (!pkFileContent.Contains("BEGIN RSA PUBLIC KEY", StringComparison.OrdinalIgnoreCase))
                {
                    // need to convert
                    pkFileContent = ExecuteWindowsCommandLine("ssh-keygen", $"-e -m pem -f {pkFile}");
                }

                return pkFileContent;
            }
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            // this will generate a PEM output *assuming* the key has already been created
            var keygenOutput = ExecuteBashCommandLine("ssh-keygen -f id_rsa.pub -e -m pem");
            if (!keygenOutput.Contains("BEGIN RSA PUBLIC KEY", StringComparison.OrdinalIgnoreCase))
            {
                // probably no key generated
                throw new Exception("Unable to retrieve a public key.  Please run 'ssh-keygen -t rsa'");
            }

            return keygenOutput;
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // ssh-agent sh -c 'ssh-add; ssh-add -L'
            var pubkey = this.ExecuteBashCommandLine("ssh-agent sh -c 'ssh-add; ssh-add -L'");
            if (!pubkey.Contains("BEGIN RSA PUBLIC KEY", StringComparison.OrdinalIgnoreCase))
            {
                var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh", "id_rsa.pub");
                pubkey = ExecuteBashCommandLine($"ssh-keygen -f {path} -e -m pem");
            }
            return pubkey;
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    /// <inheritdoc/>
    public string? GetPrivateKeyInPemFormat()
    {
        if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            || SRI.RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            var sshFolder = new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh"));

            if (!sshFolder.Exists)
            {
                throw new Exception("SSH folder not found");
            }
            else
            {
                var pkFile = Path.Combine(sshFolder.FullName, "id_rsa");
                if (!File.Exists(pkFile))
                {
                    throw new Exception("Private key not found");
                }

                return File.ReadAllText(pkFile);
            }
        }
        else if (SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            throw new PlatformNotSupportedException();
        }
        else
        {
            throw new PlatformNotSupportedException();
        }
    }

    private string ExecuteBashCommandLine(string command)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = "/bin/bash",
            Arguments = $"-c \"{command}\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }

    private string ExecuteWindowsCommandLine(string command, string args)
    {
        var psi = new ProcessStartInfo()
        {
            FileName = command,
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);

        process?.WaitForExit();

        return process?.StandardOutput.ReadToEnd() ?? string.Empty;
    }
}
