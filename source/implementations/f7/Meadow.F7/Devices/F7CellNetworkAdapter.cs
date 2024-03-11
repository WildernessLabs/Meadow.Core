using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
namespace Meadow.Devices;
using Meadow.Networking;
using Meadow.Peripherals.Sensors.Location.Gnss;

/// <summary>
/// This file holds the Cell specific methods, properties etc for the ICellNetwork interface.
/// </summary>
internal unsafe class F7CellNetworkAdapter : NetworkAdapterBase, ICellNetworkAdapter
{
    private readonly Esp32Coprocessor _esp32;
    private string? _imei;
    private string? _csq;
    private string? _at_cmds_output;

    /// <summary>
    /// Represents a signal strength value that indicates no signal or an extremely weak signal.
    /// </summary>
    public const int NoSignal = -9999;

    /// <summary>
    /// Extract values from an <b>input</b> string based on a regex <b>pattern</b>
    /// </summary>
    private static string ExtractValue(string? input, string pattern)
    {
        if (input == null)
        {
            return string.Empty;
        }

        Match match = Regex.Match(input, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return string.Empty;
    }

    /// <summary>
    /// Converts CSQ (Cellular Signal Quality) to dBm.
    /// CSQ is a value from 0 to 31, where 99 indicates no signal or unknown signal strength.
    /// </summary>
    /// <param name="csq">The CSQ value to convert.</param>
    /// <returns>The signal strength in dBm, or <b>NoSignal</b> if CSQ is 99 (no signal).</returns>
    private int ConvertCsqToDbm(int csq) {
        if (csq == 99) {
            return NoSignal; // No signal or unknown signal strength
        } else {
            return -113 + 2 * csq;
        }
    }

    /// <summary>
    /// Reset cell temporary data that depends on the connection.
    /// </summary>
    private void ResetCellTempData()
    {
        _at_cmds_output = "";
        _csq = null;
        // Since the IMEI doesn't depend on the connection, it'll not be reseted.
    }

    /// <summary>
    /// Get the cell module AT commands output <b>AtCmdsOutput</b> at the connection time, and then cache it.
    /// Avoid calling this method during operations such as scanning networks or fetching signal quality to prevent interference with ongoing processes.
    /// </summary>
    private void UpdateAtCmdsOutput()
    {
        var buffer = Marshal.AllocHGlobal(1024);

        try
        {
            var len = Core.Interop.Nuttx.meadow_get_cell_at_cmds_output(buffer);

            if (len > 0)
            {
                _at_cmds_output = Encoding.UTF8.GetString((byte*)buffer.ToPointer(), len);
            }
            else
            {
                _at_cmds_output = null;
                Resolver.Log.Error("Fail to get the AT commands output!");
            }
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    /// <inheritdoc/>
    public override string Name => "Cell PPP";

    /// <summary>
    /// Create a new instance of the F7CellNetworkAdapter class
    /// </summary>
    /// <param name="esp32">The ESP32 coprocessor</param>
    public F7CellNetworkAdapter(Esp32Coprocessor esp32)
        : base(System.Net.NetworkInformation.NetworkInterfaceType.Ppp)
    {
        _esp32 = esp32;
        _esp32.CellMessageReceived += (s, e) =>
        {
            InvokeEvent(e.fn, e.status, e.data);
        };
    }

    /// <summary>
    /// Use the event data to work out which event to invoke and create any event args that will be consumed.
    /// </summary>
    /// <param name="eventId">Event ID.</param>
    /// <param name="statusCode">Status of the event.</param>
    /// <param name="payload">Optional payload containing data specific to the result of the event.</param>
    protected void InvokeEvent(CellFunction eventId, StatusCodes statusCode, byte[] payload)
    {
        Resolver.Log.Trace($"Cell InvokeEvent {eventId} returned {statusCode}");

        switch (eventId)
        {
            case CellFunction.NetworkConnectedEvent:
                Resolver.Log.Trace("Cell connected event triggered!");

                // TODO: Get the IP, gateway and subnet from the OS land.
                var args = new CellNetworkConnectionEventArgs(IPAddress.Loopback, IPAddress.Any, IPAddress.None);

                UpdateAtCmdsOutput();

                this.Refresh();
                RaiseNetworkConnected(args);
                break;
            case CellFunction.NetworkDisconnectedEvent:
                Resolver.Log.Trace("Cell disconnected event triggered!");
                
                Resolver.Log.Trace($"Cell connection error: {GetCellConnectionError()}");

                ResetCellTempData();

                RaiseNetworkDisconnected(null);
                break;
            case CellFunction.NetworkAtCmdEvent:
                Resolver.Log.Trace("Cell at cmd event triggered!");

                UpdateAtCmdsOutput();
                break;
            case CellFunction.NetworkErrorEvent:
                Resolver.Log.Trace("Cell error event triggered!");

                Resolver.Log.Trace($"Cell connection error: {GetCellConnectionError()}");
                break;
            default:
                Resolver.Log.Trace("Event type not found");
                break;
        }
    }

    /// <summary>
    /// Returns <c>true</c> if the adapter is connected, otherwise <c>false</c>
    /// </summary>
    public override bool IsConnected
    {
        get
        {
            try
            {
                return Core.Interop.Nuttx.meadow_cell_is_connected();
            }
            catch (Exception e)
            {
                Resolver.Log.Error($"Failed to read meadow_cell_is_connected(): {e.Message}");
                return false;
            }
        }
    }

    /// <summary>
    /// Returns the cell module <b>IMEI</b> if the device has already been connected at least once, otherwise an empty string
    /// </summary>
    public string Imei
    {
        get
        {
            if (_imei == null)
            {
                string imeiPattern = @"\+GSN\r\r\n(\d+)";

                string imei = ExtractValue(_at_cmds_output, imeiPattern);
                if (imei != null)
                {
                    _imei = imei;
                }
                else
                {
                    Resolver.Log.Error("IMEI not found! Please ensure that you have established a cellular connection at least once!");
                }
            }

            return _imei ?? string.Empty;
        }
    }

    /// <summary>
    /// Returns the cell signal quality <b>CSQ</b> in dBm at the connection time, if the device is connected, otherwise returns <b>NoSignal</b>
    /// </summary>
    public int Csq
    {
        get
        {
            if (!IsConnected || _csq == null)
            {
                string csqPattern = @"\+CSQ:\s+(\d+),\d+";
                string csq = ExtractValue(_at_cmds_output, csqPattern);
                if (csq != null)
                {
                    _csq = csq;
                }
                else
                {
                    Resolver.Log.Error("CSQ not found! Please ensure that you have established a cellular connection first!");
                }
            }

            var csqValue = int.Parse(_csq ?? "99");
            return ConvertCsqToDbm(csqValue);
        }
    }

    /// <summary>
    /// Returns the cell module AT commands output <b>AtCmdsOutput</b> if the device tried to connect at least once, otherwise an empty string
    /// Avoid calling this method during operations such as scanning networks or fetching signal quality to prevent interference with ongoing processes.
    /// </summary>
    public string AtCmdsOutput
    {
        get
        {
            UpdateAtCmdsOutput();

            if (_at_cmds_output == null)
            {
                Resolver.Log.Error("AT commands output not found! Please ensure that you tried to established a cellular connection first!");
            }

            return _at_cmds_output ?? string.Empty;
        }
    }

    /// <summary>
    /// Returns the list of cell networks found, including its operator code, if the device is in scanning mode, otherwise, an empty array
    /// </summary>
    public CellNetwork[] OfflineNetworkScan()
    {
        Resolver.Log.Error("OfflineNetworkScan method has been deprecated! Please consult the cellular docs to learn how to use the network scanner");
        return Array.Empty<CellNetwork>();
    }

    /// <summary>
    /// Returns error according to an input value, otherwise <b>Undefined Cell error</b>
    /// </summary>
    private string ParseCellConnectionError(string input)
    {
        if (input != null)
        {
            string pattern = @"\+CME ERROR:\s*(.+)";
            Match match = Regex.Match(input, pattern);

            if (match.Success && match.Groups.Count >= 2)
            {
                return match.Groups[1].Value.Trim();
            }
        }
        return string.Empty;
    }


    /// <summary>
    /// Gets the error message corresponding to the current cell connection error code.
    /// If the error code is not recognized, returns "Undefined error".
    /// </summary>
    private string GetCellConnectionError()
    {
        int errno = Core.Interop.Nuttx.meadow_get_cell_error();

        var logInfoMessage = _at_cmds_output?.Length > 1 // To avoid logging potential residual data from UART
            ? $"You can also look at the AT commands logs for extra information: {_at_cmds_output}"
            : "";

        CellError cellError = (CellError)errno;
        var cellErrorMessage = ParseCellConnectionError(_at_cmds_output);

        return cellError switch
        {
            CellError.InvalidNetworkSettings => "Invalid cell settings. Please check your cell configuration file.",
            CellError.InvalidCellModule => "Invalid cell module. Please ensure you have configured the correct module name.",
            CellError.NetworkConnectionLost => !string.IsNullOrEmpty(cellErrorMessage)
                ? $"Cellular connection lost, please check your antenna and signal coverage, error: {cellErrorMessage}, consult the cell module datasheet to know more about that."
                : $"Cellular connection lost, please check your antenna and signal coverage.",
            CellError.NetworkTimeout => !string.IsNullOrEmpty(cellErrorMessage)
                ? $"Timeout occurred while attempting to connect, error: {cellErrorMessage}. Please check if the hardware setup, the cellular settings, and the reserved pins are properly configured."
                : $"Timeout occurred while attempting to connect. Please check if the hardware setup, the cellular settings, and the reserved pins are properly configured. {logInfoMessage}",
            _ => $"An undefined error occurred. Please consult the troubleshooting section of the cellular documentation for more information. {logInfoMessage}",
        };
    }
    
    /// <summary>
    /// Set the cell state
    /// </summary>
    /// <param name="CellState">State of the cell.</param>
    private static void CellSetState(CellNetworkState CellState)
    {
        int state = 0;
        switch (CellState)
        {
            case CellNetworkState.Resumed:
                state = 0;
                break;
            case CellNetworkState.Paused:
                state |= 1 << 0;
                break;
            case CellNetworkState.TrackingGPSLocation:
                state |= 1 << 1;
                break;
            case CellNetworkState.FetchingSignalQuality:
                state |= 1 << 2;
                break;
            case CellNetworkState.ScanningNetworks:
                state |= 1 << 3;
                break;
            default:
                state |= 1 << 0;
                break;
        }
        Core.Interop.Nuttx.meadow_cell_change_state(state);
    }

    /// <summary>
    /// Get current signal quality
    /// </summary>
    /// <param name="timeout">Timeout to check signal quality.</param>
    /// <returns>A number representing the Cell Signal Quality (CSQ) in dBm, or <b>NoSignal</b> if unavailable.</returns>
    public int GetSignalQuality(int timeout)
    {
        Resolver.Log.Trace("Fetching cellular signal quality... It might take a few minutes and temporary disconnect you from the cellular network.");

        string csqPattern = @"\+CSQ:\s+(\d+),\d+";
        _at_cmds_output = string.Empty;

        CellSetState(CellNetworkState.FetchingSignalQuality);

        while (timeout > 0)
        {
            // Ensure that the _at_cmds_output is not updated by another method while waiting for the signal quality value
            if (_at_cmds_output != string.Empty)
            {
                break;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            timeout--;
        }

        CellSetState(CellNetworkState.Resumed);

        if (_at_cmds_output == null)
        {
            Resolver.Log.Error("AT commands output not found!");
            return NoSignal;
        }

        var csqValueStr = ExtractValue(_at_cmds_output, csqPattern);
        int csqValue = String.IsNullOrEmpty(csqValueStr) ? 99 : int.Parse(csqValueStr);
        return ConvertCsqToDbm(csqValue);
    }

    /// <summary>
    /// Scans for available networks without entering "Scan Mode"
    /// </summary>
    /// <param name="timeout">The scan timeout duration in seconds.</param>
    /// <returns>An array of CellNetwork objects representing available networks.</returns>
    public CellNetwork[] ScanForAvailableNetworks(int timeout)
    {
        Resolver.Log.Trace("Scanning for available cellular networks... It might take a few minutes and temporary disconnect you from the cellular network.");

        CellSetState(CellNetworkState.ScanningNetworks);
        _at_cmds_output = string.Empty;

        while (timeout > 0)
        {
            // Ensure that the _at_cmds_output is not updated by another method while waiting for the cell network's output
            if (_at_cmds_output != string.Empty)
            {
                break;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            timeout--;
        }

        CellSetState(CellNetworkState.Resumed);

        if (_at_cmds_output == null)
        {
            throw new System.IO.IOException("No available networks");
        }

        return Core.Interop.Nuttx.ParseCellNetworkScannerOutput(_at_cmds_output).ToArray();
    }

    /// <summary>
    /// Execute GNSS-related AT commands and retrieve combined output, including NMEA sentences.
    /// </summary>
    /// <param name="resultTypes">An array of supported GNSS result types for data processing.</param>
    /// <param name="timeout">The timeout in milliseconds</param>
    /// <returns>A string containing combined output from GNSS-related AT commands, including NMEA sentences.</returns>
    public string FetchGnssAtCmdsOutput(IGnssResult[] resultTypes, int timeout)
    {
        Resolver.Log.Trace("Retrieving GPS location... It might take a few minutes and temporary disconnect you from the cellular network.");
        _at_cmds_output = string.Empty;

        // TODO: Take into consideration the GNSS result types
        CellSetState(CellNetworkState.TrackingGPSLocation);

        //ToDo: switch to TimeSpan
        while (timeout > 0)
        {
            // Ensure that the _at_cmds_output is not updated by another method while waiting for the GNSS fixes
            if (_at_cmds_output != string.Empty)
            {
                break;
            }

            Thread.Sleep(TimeSpan.FromMilliseconds(1000));
            timeout--;
        }

        var gnssAtCmdsOutput = _at_cmds_output;

        CellSetState(CellNetworkState.Resumed);

        if (gnssAtCmdsOutput == null)
        {
            Resolver.Log.Error("AT commands output not found!");
            return string.Empty;
        }
        return gnssAtCmdsOutput;
    }
}