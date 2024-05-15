using System;

namespace Meadow.Devices.Esp32.MessagePayloads;

/// <summary>
/// An ESP32-specific MeadowSystemErrorInfo
/// </summary>
public class Esp32SystemErrorInfo : MeadowSystemErrorInfo
{
    /// <summary>
    /// The error's StatusCode
    /// </summary>
    public StatusCodes StatusCode { get; }

    /// <summary>
    /// The error's Function
    /// </summary>
    public int Function { get; }

    internal Esp32SystemErrorInfo(int function, StatusCodes statusCode)
        : base(
            statusCode switch
            {
                StatusCodes.InvalidConfigurationFile => "Invalid configuration file",
                StatusCodes.InvalidWiFiConfigurationFile => "Invalid WiFi configuration file",
                StatusCodes.InvalidCellConfigurationFile => "Invalid Cell configuration file",
                _ => "ESP32 Error"
            },
            SystemErrorNumber.CoprocessorError)
    {
        Function = function;
        StatusCode = statusCode;
    }

    /// <inheritdoc/>
    public override string ToString()
    {
        var s = $"{this.GetType().Name} ({StatusCode}): {Message}";
        if (Exception != null)
        {
            s += $"{Environment.NewLine}Inner: {Exception.GetType().Name}: {Exception.Message}";
        }

        return s;
    }
}
