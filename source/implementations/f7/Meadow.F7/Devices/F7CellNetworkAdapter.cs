using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
namespace Meadow.Devices;
using Meadow.Networking;

/// <summary>
/// This file holds the Cell specific methods, properties etc for the ICellNetwork interface.
/// </summary>
internal unsafe class F7CellNetworkAdapter : NetworkAdapterBase, ICellNetworkAdapter
{
    private readonly Esp32Coprocessor _esp32;
    private string _imei;
    private string _csq;
    private string _at_cmds_output;
    private string _error_str;

    private static string ExtractValue(string input, string pattern)
    {
        Match match = Regex.Match(input, pattern);
        if (match.Success)
        {
            return match.Groups[1].Value;
        }

        return null;
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

                RaiseNetworkConnected(args);
                break;
            case CellFunction.NetworkDisconnectedEvent:
                Resolver.Log.Trace("Cell disconnected event triggered!");

                ResetCellTempData();

                RaiseNetworkDisconnected();
                break;
            case CellFunction.NetworkErrorEvent:
                _error_str = GetError();

                Resolver.Log.Error($"{_error_str}");
                break;
            default:
                Resolver.Log.Trace("Event type not found");
                break;
        }
    }

    /// <summary>
    /// Returns <b>true</b> if the adapter is connected, otherwise <b>false</b>
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
    /// Returns the cell module <b>IMEI</b> if the device has already been connected at least once, otherwise <b>null</b>
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
    /// Returns the cell signal quality <b>CSQ</b> at the connection time, if the device is connected, otherwise <b>null</b>
    /// </summary>
    public string Csq
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

            return _csq ?? string.Empty;
        }
    }

    /// <summary>
    /// Returns the cell module AT commands output <b>AtCmdsOutput</b> if the device tried to connect at least once, otherwise <b>null</b>
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
    /// Returns the list of cell networks found, including its operator code, if the device is in scanning mode, otherwise <b>null</b>
    /// </summary>
    public CellNetwork[] Scan()
    {
        return Core.Interop.Nuttx.MeadowCellNetworkScanner();
    }
    
    /// <summary>
    /// Returns error according to an input value, otherwise <b>Undefined Cell error</b>
    /// </summary>
    private string ParseError(string input)
    {
        if (input !=null)
        {
            string pattern = @"\+CME ERROR: (.+)$";
            Match match = Regex.Match(input, pattern);

            if (match.Success && match.Groups.Count >= 2)
            {
                return match.Groups[1].Value;
            }
        }
        return "Undefined Cell error";
    }

    /// <summary>
    /// Returns the error string, otherwise <b>Undefined error</b>
    /// </summary>
    private string GetError()
    {
        int errno = Core.Interop.Nuttx.meadow_get_cell_error();

        CellError cellError = (CellError)errno;

        switch(cellError)
        {
            case CellError.InvalidNetworkSettings:
                return "Invalid cell settings. Please check your cell configuration file.";
            case CellError.InvalidCellModule:
                return "Invalid cell module. Please ensure you have configured the right module name.";
            case CellError.NetworkConnectionLost:
                return ParseError(_at_cmds_output);
            case CellError.NetworkTimeout:
                return "Timeout. Please check your pinout and wire connections to the module.";
            default:
                return "Undefined error";
        }
    }
}
