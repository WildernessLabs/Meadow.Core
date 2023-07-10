using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Core.Unit.Tests")]

namespace Meadow;

internal class AppSettingsParser
{
    public MeadowAppSettings Parse(string settingsFile)
    {
        var settings = new MeadowAppSettings();

        var index = 0;
        var spacing = 2;
        var parent = string.Empty;
        var parents = new List<string>();
        var lastLevel = 0;

        int endLine;
        string lastKey;
        do
        {
            var level = 0;

            endLine = settingsFile.IndexOf('\n', index);

            while (settingsFile[index] == ' ')
            {
                index += spacing;
                level++;
            }
            var line = settingsFile.Substring(index, endLine - index - 1).TrimEnd();
            if (line != string.Empty)
            {

                if (level > lastLevel)
                {
                    //parent = parent.Length == 0 ? lastKey : $"{parent}.{lastKey}";
                    parent = string.Join('.', parents);
                }
                else if (level < lastLevel)
                {
                    parents.RemoveRange(level, parents.Count - level);
                    parent = string.Join('.', parents.Take(level));
                }
                lastLevel = level;

                var kvp = line
                    .Split(':', System.StringSplitOptions.RemoveEmptyEntries);

                switch (kvp.Length)
                {
                    case 0:
                        break;
                    case 1:
                        lastKey = kvp[0].Trim();
                        parents.Add(lastKey);
                        break;
                    default:
                        var name = $"{parent}.{kvp[0]}";
                        var value = kvp[1].Trim();
                        ApplySetting(settings, name, value);
                        break;
                }
            }

            index = endLine + 1;
        } while (endLine > 0 && index < settingsFile.Length);

        return settings;
    }

    private void ApplySetting(MeadowAppSettings settings, string settingName, string settingValue)
    {
        switch (settingName)
        {
            case "Logging.LogLevel.Default":
                if (Enum.TryParse<LogLevel>(settingValue, true, out LogLevel level))
                {
                    settings.LoggingSettings.LogLevel.Default = level;
                }
                break;
            case "Logging.ShowTicks":
                if (bool.TryParse(settingValue, out bool st))
                {
                    settings.LoggingSettings.ShowTicks = st;
                }
                break;

            case "Lifecycle.ResetOnAppFailure":
                if (bool.TryParse(settingValue, out bool r))
                {
                    settings.LifecycleSettings.RestartOnAppFailure = r;
                }
                break;
            case "Lifecycle.AppFailureRestartDelaySeconds":
                if (int.TryParse(settingValue, out int rd))
                {
                    settings.LifecycleSettings.AppFailureRestartDelaySeconds = rd;
                }
                break;

            case "Update.Enabled":
                if (bool.TryParse(settingValue, out bool ue))
                {
                    settings.UpdateSettings.Enabled = ue;
                }
                break;
            case "Update.UpdateServer":
                settings.UpdateSettings.UpdateServer = settingValue;
                break;
            case "Update.UpdatePort":
                if (int.TryParse(settingValue, out int up))
                {
                    settings.UpdateSettings.UpdatePort = up;
                }
                break;
            case "Update.AuthServer":
                settings.UpdateSettings.AuthServer = settingValue;
                break;
            case "Update.AuthPort":
                if (int.TryParse(settingValue, out int cp))
                {
                    settings.UpdateSettings.AuthPort = cp;
                }
                break;
        }
    }
}
