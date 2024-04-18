using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow;

/// <summary>
/// Responsible for tracking and maintianing application and device crash report information
/// </summary>
public sealed class CrashReporter
{
    private readonly string[] _reportFiles;

    internal CrashReporter()
    {
        _reportFiles = new string[]
            {
                Path.Combine(MeadowOS.FileSystem.DataDirectory, "meadow.error"),
                Path.Combine(MeadowOS.FileSystem.UserFileSystemRoot, "mono_error.txt")
            };
    }

    /// <summary>
    /// Returns <b>true</b> if any crash report data is available, otherwise <b>false</b>
    /// </summary>
    public bool CrashDataAvailable
    {
        get
        {
            foreach (var report in _reportFiles)
            {
                if (File.Exists(report)) return true;
            }

            return false;
        }
    }

    /// <summary>
    /// Erases all existing crash report data
    /// </summary>
    public void ClearCrashData()
    {
        foreach (var report in _reportFiles)
        {
            try
            {
                var fi = new FileInfo(report);
                if (fi.Exists)
                {
                    fi.Delete();
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Warn($"Unable to delete crash data from {report}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Gets all existing crash report data
    /// </summary>
    /// <returns>A list (typically with a length of 1) of crash reports</returns>
    public IEnumerable<string> GetCrashData()
    {
        var crashData = new List<string>();

        foreach (var report in _reportFiles)
        {
            try
            {
                var fi = new FileInfo(report);
                if (fi.Exists)
                {
                    crashData.Add(File.ReadAllText(fi.FullName));
                }
            }
            catch (Exception ex)
            {
                Resolver.Log.Warn($"Unable to read crash data from {report}: {ex.Message}");
            }
        }

        return crashData;
    }
}
