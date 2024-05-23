using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.IO;

namespace Meadow;

/// <summary>
/// Provides base implementation of common ReliabilityService functionality
/// </summary>
public abstract class ReliabilityServiceBase : IReliabilityService
{
    private readonly List<MeadowSystemErrorInfo> _systemErrorCache = new();
    private EventHandler<MeadowSystemErrorInfo>? _systemError;

    private readonly string[] _reportFiles;

    /// <inheritdoc/>
    public abstract void OnBootFromCrash();

    /// <inheritdoc/>    
    public bool LastBootWasFromCrash => IsCrashDataAvailable; // TODO: there are probably factors outside of these files that might make this true

    /// <inheritdoc/>
    public virtual ResetReason LastResetReason => ResetReason.Unknown;

    /// <inheritdoc/>
    public virtual int SystemResetCount => -1;

    /// <inheritdoc/>
    public virtual int SystemPowerCycleCount => -1;

    /// <inheritdoc/>
    public event EventHandler<MeadowSystemErrorInfo>? MeadowSystemError
    {
        add
        {
            _systemError += value;
            lock (_systemErrorCache)
            {
                if (_systemErrorCache.Count > 0)
                {
                    foreach (var e in _systemErrorCache)
                    {
                        _systemError?.Invoke(this, e);
                    }
                    _systemErrorCache.Clear();
                }
            }
        }
        remove => _systemError -= value;
    }

    /// <summary>
    ///  Creates a ReliabilityServiceBase
    /// </summary>
    public ReliabilityServiceBase()
    {
        _reportFiles = new string[]
        {
            MeadowOS.FileSystem.AppCrashFile,
            MeadowOS.FileSystem.RuntimeCrashFile
        };
    }

    /// <inheritdoc/>
    public virtual void OnMeadowSystemError(MeadowSystemErrorInfo errorInfo)
    {
        if (_systemError == null)
        {
            _systemErrorCache.Add(errorInfo);
        }
        else
        {
            _systemError?.Invoke(this, errorInfo);
        }
    }

    /// <inheritdoc/>
    public bool IsCrashDataAvailable
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

    /// <inheritdoc/>
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

    /// <inheritdoc/>
    public string[] GetCrashData()
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

        return crashData.ToArray();
    }

    /// <inheritdoc/>
    public virtual IEnumerable<PlatformOsMessage>? GetStartupMessages()
    {
        return Array.Empty<PlatformOsMessage>();
    }
}
