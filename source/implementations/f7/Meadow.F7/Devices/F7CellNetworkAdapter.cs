using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Hardware;
using System;
using System.Net;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections.Generic;
namespace Meadow.Devices;
using Meadow.Networking;

/// <summary>
/// This file holds the Cell specific methods, properties etc for the ICellNetwork interface.
/// </summary>
unsafe internal class F7CellNetworkAdapter : NetworkAdapterBase, ICellNetworkAdapter
{
    private Esp32Coprocessor _esp32;
    private string _imei;
    private string _csq;
    private string _pppdOutput;
    private List<CellNetwork> cellScan;

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
        _pppdOutput = null;
        _csq = null;
        // Since the IMEI doesn't depend on the connection, it'll not be reseted.
    }

    /// <summary>
    /// Get the cell module AT commands output <b>PppdOutput</b> at the connection time, and then cache it.
    /// </summary>
    private void UpdatePppdOutput ()
    {
        IntPtr buffer = IntPtr.Zero;
        buffer = Marshal.AllocHGlobal(1024);
        
        try
        {
            var len = Core.Interop.Nuttx.meadow_get_cell_pppd_output(buffer);

            if (len > 0)
            {
                _pppdOutput = Encoding.UTF8.GetString((byte*)buffer.ToPointer(), len);
            }
            else
            {
                Resolver.Log.Error("Fail to get the PPPD output!");
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

                // Sample IPAddress values for the object creation
                var ipAddress = IPAddress.Parse("192.168.1.100");
                var subnet = IPAddress.Parse("255.255.255.0");
                var gateway = IPAddress.Parse("192.168.1.1");

                var args = new CellNetworkConnectionEventArgs(ipAddress, subnet, gateway);

                UpdatePppdOutput();

                RaiseNetworkConnected(args);
                break;
            case CellFunction.NetworkDisconnectedEvent:
                Resolver.Log.Trace("Cell disconnected event triggered!");

                ResetCellTempData();

                RaiseNetworkDisconnected();
                break;
            default:
                Console.WriteLine("Event type not found");
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
                
                string imei = ExtractValue(_pppdOutput, imeiPattern);
                if (imei != null)
                    _imei = imei;
                else
                    Resolver.Log.Error("IMEI not found! Please ensure that you have established a cellular connection first!");
            }

            return _imei;
        }
    }

    /// <summary>
    /// Returns the cell signal quality <b>CSQ</b> at the connection time, if the device is connected, otherwise <b>null</b>
    /// </summary>
    public string Csq 
    {
        get 
        { 
            if (_csq == null)
            {
                string csqPattern = @"\+CSQ:\s+(\d+),\d+";
                string csq = ExtractValue(_pppdOutput, csqPattern);
                if (csq != null)
                    _csq = csq;
                else
                    Resolver.Log.Error("CSQ not found! Please ensure that you have established a cellular connection first!");
            }

            return _csq;
        }
    }

    /// <summary>
    /// Returns the cell module AT commands output <b>PppdOutput</b> if the device has already been connected at least once, otherwise <b>null</b>
    /// </summary>
    public string PppdOutput
    {
        get
        {
            if (_pppdOutput == null)
            {
                Resolver.Log.Error("PPPD output not found! Please ensure that you have established a cellular connection first!");
            } 
            
            return _pppdOutput;
        }
    }
    
    /// <inheritdoc/>
    public List <CellNetwork> Scan()
    {
        cellScan = Core.Interop.Nuttx.MeadowCellScannerNetwork();
        
        if (cellScan == null)
        {
            Resolver.Log.Error("Operator not found");
        }
        return cellScan;
    }

}
