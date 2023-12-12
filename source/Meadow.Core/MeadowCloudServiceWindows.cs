using Meadow.Cloud;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using SRI = System.Runtime.InteropServices;

namespace Meadow;

public partial class MeadowCloudService : IMeadowCloudService
{
    private const string WindowsPrivateKeyFileName = "id_rsa";

    private DirectoryInfo GetWindowsPrivateKeyDirectory()
    {
        return new DirectoryInfo(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".ssh"));
    }

    private string? GetWindowsPrivateKeyInPemFormat()
    {
        if (!SRI.RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            throw new PlatformNotSupportedException();
        }

        var sshFolder = GetWindowsPrivateKeyDirectory();

        if (!sshFolder.Exists)
        {
            Resolver.Log.Error("SSH folder not found");
            return null;
        }

        var pkFile = Path.Combine(sshFolder.FullName, WindowsPrivateKeyFileName);
        if (!File.Exists(pkFile))
        {
            Resolver.Log.Error("Private key not found");
            return null;
        }

        var pkFileContent = File.ReadAllText(pkFile);
        if (!pkFileContent.Contains(RsaPrivateKeyHeader, StringComparison.OrdinalIgnoreCase))
        {
            pkFileContent = ExecuteWindowsCommandLine("ssh-keygen", $"-e -m pem -f {pkFile}");
        }

        return pkFileContent;
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