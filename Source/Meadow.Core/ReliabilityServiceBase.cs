using Meadow.Hardware;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace Meadow;

/// <summary>
/// Provides base implementation of common ReliabilityService functionality
/// </summary>
public abstract class ReliabilityServiceBase : IReliabilityService
{
    private readonly List<(MeadowSystemErrorInfo error, bool recommendReset)> _systemErrorCache = new();
    private MeadowSystemErrorHandler? _systemError;
    private bool _startupEventSubscribeTimeout = false;
    private Stopwatch _uptimeStopwatch = new Stopwatch();

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
    public virtual TimeSpan UpTime => _uptimeStopwatch.Elapsed;

    /// <summary>
    /// Returns <b>true</b> if any listener has attached to the MeadowSystemError event
    /// </summary>
    protected bool ErrorListenerIsAttached => _systemError != null;

    /// <inheritdoc/>
    public event MeadowSystemErrorHandler MeadowSystemError
    {
        add
        {
            _systemError += value;
            lock (_systemErrorCache)
            {
                if (_systemErrorCache.Count > 0)
                {
                    bool forceReset = false, r = false;

                    foreach (var e in _systemErrorCache)
                    {
                        LogSystemError(e.error, e.recommendReset);

                        _systemError?.Invoke(e.error, e.recommendReset, out r);
                        forceReset |= r;
                    }

                    if (forceReset)
                    {
                        ResetDueToSystemError();
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
        _uptimeStopwatch.Start();

        EnsureCrashReportDirectoryExists();

        _reportFiles = new string[]
        {
            MeadowOS.FileSystem.AppCrashFile,
            MeadowOS.FileSystem.RuntimeCrashFile
        };

        Task.Run(async () =>
        {
            await Task.Delay(30000);
            _startupEventSubscribeTimeout = true;
        });

    }

    /// <summary>
    /// Processes a Meadow system error to determine if a device reset is recommended
    /// </summary>
    /// <param name="errorInfo">The error that has occurred</param>
    /// <param name="recommendReset">Return <b>true</b> to recommend device reset</param>
    protected virtual void ProcessSystemError(MeadowSystemErrorInfo errorInfo, out bool recommendReset)
    {
        recommendReset = false;
    }

    private void EnsureCrashReportDirectoryExists()
    {
        if (!Directory.Exists(MeadowOS.FileSystem.CrashReportDirectory))
        {
            try
            {
                Directory.CreateDirectory(MeadowOS.FileSystem.CrashReportDirectory);
            }
            catch (Exception ex)
            {
                Resolver.Log.Error($"Unable to create crash report directory: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Writes a system error to the App Crash file
    /// </summary>
    /// <param name="error">The error info to write</param>
    /// <param name="recommendedReset">Whether or not reset was recommended</param>
    protected void LogSystemError(MeadowSystemErrorInfo error, bool recommendedReset)
    {
        try
        {
            EnsureCrashReportDirectoryExists();

            var fi = new FileInfo(MeadowOS.FileSystem.AppCrashFile);
            if (!fi.Exists)
            {
                using var writer = fi.CreateText();
                writer.WriteLine($"{DateTime.UtcNow:s}");
                writer.WriteLine(error.ToString());
                writer.WriteLine($"Recommend reset: {recommendedReset}");
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Unable to record system error: {ex.Message}");
        }
    }

    private void ResetDueToSystemError()
    {
        Resolver.Log.Error($"Resetting due to a system error.");
        try
        {
            EnsureCrashReportDirectoryExists();

            var fi = new FileInfo(MeadowOS.FileSystem.AppCrashFile);
            if (!fi.Exists)
            {
                using var writer = fi.CreateText();
                writer.WriteLine($"{DateTime.UtcNow:s}");
                writer.WriteLine("ReliabilityService reset the device");
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Unable to record a reset: {ex.Message}");
        }
        Resolver.Device.PlatformOS.Reset();
    }

    /// <inheritdoc/>
    public void OnMeadowSystemError(MeadowSystemErrorInfo errorInfo)
    {
        ProcessSystemError(errorInfo, out bool recommendReset);

        if (_systemError == null && !_startupEventSubscribeTimeout)
        {
            // if we've only been up a short period of time, we need to cache potential errors that happened before app startup
            // however, if the app comes up and no handler is attached, we want the automated behavior to take over
            _systemErrorCache.Add((errorInfo, recommendReset));
        }
        else
        {
            if (!ErrorListenerIsAttached && recommendReset)
            {
                Resolver.Log.Error($"This is a fatal error. Resetting the device is recommended.");
            }

            bool forceReset;
            if (_systemError == null)
            {
                forceReset = recommendReset;
            }
            else
            {
                LogSystemError(errorInfo, recommendReset);
                _systemError.Invoke(errorInfo, recommendReset, out forceReset);
            }

            if (recommendReset != forceReset)
            {
                Resolver.Log.Info($"The application has overridden auto-reset behavior and should manually call reset.");
            }

            if (forceReset)
            {
                ResetDueToSystemError();
            }
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
