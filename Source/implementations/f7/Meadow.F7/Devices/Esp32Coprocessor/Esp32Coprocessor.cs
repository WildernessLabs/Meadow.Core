using Meadow.Core;
using Meadow.Devices.Esp32.MessagePayloads;
using Meadow.Gateways;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static Meadow.Core.Interop;
using static Meadow.Logging.Logger;

namespace Meadow.Devices;

/// <summary>
/// The Esp32Coprocessor class provide access to the features and functionality of the ESP32 coprocessor.
/// </summary>
/// <remarks>
/// This file contains the generic code (for example communication with the ESP32) used to support the higher
/// level functionality (such as WiFi).  Interface specific functionality is provided in separate files.
/// </remarks>
public partial class Esp32Coprocessor : ICoprocessor
{
    /// <summary>
    /// Maximum length of the SPI buffer that can be used for communication with the ESP32.
    /// </summary>
    public const uint MAXIMUM_SPI_BUFFER_LENGTH = 4000;

    private static readonly byte[] EmptyPayload = new byte[0];

    internal event EventHandler<(WiFiFunction fn, StatusCodes status, byte[] data)>? WiFiMessageReceived = default!;
    internal event EventHandler<(CellFunction fn, StatusCodes status, byte[] data)>? CellMessageReceived = default!;
    internal event EventHandler<(SystemFunction fn, StatusCodes status)>? SystemMessageReceived = default!;

    private EventHandler<(EthernetFunction fn, StatusCodes status, byte[] data)>? _ethernetMessageHandlers;
    private Queue<RawEventData> _queuedEthernetEvents = new();

    private record RawEventData
    {
        public Esp32Interfaces Interface { get; set; }
        public uint Function { get; set; }
        public uint StatusCode { get; set; }
        public byte[]? Payload { get; set; }
    }

    internal event EventHandler<(EthernetFunction fn, StatusCodes status, byte[] data)>? EthernetMessageReceived
    {
        add
        {
            _ethernetMessageHandlers += value;

            lock (_queuedEthernetEvents)
            {
                foreach (var evt in _queuedEthernetEvents)
                {
                    _ethernetMessageHandlers?.Invoke(this, new((EthernetFunction)evt.Function, (StatusCodes)evt.StatusCode, evt.Payload ?? EmptyPayload));
                }
                _queuedEthernetEvents.Clear();
            }
        }
        remove => _ethernetMessageHandlers -= value;
    }

    /// <summary>
    /// Possible debug levels.
    /// </summary>
    [Flags]
    private enum DebugOptions : uint { None = 0x00, Information = 0x01, Errors = 0x02, EventHandling = 0x04, Full = 0xffffffff }

    /// <summary>
    /// Current debug level for this class.
    /// </summary>
    /// <remarks>
    /// The flags set in this variable determine the type and amount of output generated when
    /// debugging this class.
    /// </remarks>
    private static DebugOptions _debugLevel = DebugOptions.None;

    /// <summary>
    /// Event handler service thread.
    /// </summary>
    private Thread? _eventHandlerThread = null;

    /// <summary>
    /// Current status of the coprocessor.
    /// </summary>
    public ICoprocessor.CoprocessorState Status { get; private set; }

    /// <summary>
    /// Reason for the last power cycle / reset of the coprocessor.
    /// </summary>
    public ICoprocessor.CoprocessorResetReason ResetReason
    {
        get => (ICoprocessor.CoprocessorResetReason)F7PlatformOS.GetByte(IPlatformOS.ConfigurationValues.ResetReason);
    }

    /// <summary>
    /// Default constructor of the Esp32Coprocessor class.
    /// </summary>
    internal Esp32Coprocessor()
    {
        Status = ICoprocessor.CoprocessorState.NotReady;

        if (_eventHandlerThread == null)
        {
            _eventHandlerThread = new Thread(EventHandlerServiceThread)
            {
                IsBackground = true
            };
            _eventHandlerThread.Start();
        }
    }

    /// <summary>
    /// Send a parameterless command (i.e a command where no payload is required) to the ESP32.
    /// </summary>
    /// <param name="where">Interface the command is destined for.</param>
    /// <param name="function">Command to be sent.</param>
    /// <param name="block">Is this a blocking command?</param>
    /// <param name="encodedResult">4000 byte array to hold any data returned by the command.</param>
    /// <returns>StatusCodes enum indicating if the command was successful or if an error occurred.</returns>
    internal StatusCodes SendCommand(byte where, UInt32 function, bool block, byte[]? encodedResult)
    {
        return SendCommand(where, function, block, null, encodedResult);
    }

    /// <summary>
    /// Send a bluetooth command and its payload to the ESP32.
    /// </summary>
    /// <param name="function">Command to be sent.</param>
    /// <param name="block">Is this a blocking command?</param>
    /// <param name="encodedRequest">Payload for the command to be executed by the ESP32.</param>
    /// <param name="encodedResult">4000 byte array to hold any data returned by the command.</param>
    /// <returns>StatusCodes enum indicating if the command was successful or if an error occurred.</returns>
    protected StatusCodes SendBluetoothCommand(BluetoothFunction function, bool block, byte[]? encodedRequest, byte[]? encodedResult)
    {
        return SendCommand((byte)Esp32Interfaces.BlueTooth, (uint)function, block, encodedRequest, encodedResult);
    }


    // TODO: Mark, shouldn't this be an async call?
    /// <summary>
    /// Send a command and its payload to the ESP32.
    /// </summary>
    /// <param name="destination">Interface the command is destined for.</param>
    /// <param name="function">Command to be sent.</param>
    /// <param name="block">Is this a blocking command?</param>
    /// <param name="encodedRequest">Payload for the command to be executed by the ESP32.</param>
    /// <param name="encodedResult">4000 byte array to hold any data returned by the command.</param>
    /// <returns>StatusCodes enum indicating if the command was successful or if an error occurred.</returns>
    internal StatusCodes SendCommand(byte destination, uint function, bool block, byte[]? encodedRequest, byte[]? encodedResult)
    {
        var payloadGcHandle = default(GCHandle);
        var resultGcHandle = default(GCHandle);
        StatusCodes result = StatusCodes.CompletedOk;
        try
        {
            var command = new Nuttx.UpdEsp32Command()
            {
                Interface = destination,
                Function = function,
                StatusCode = (UInt32)StatusCodes.CompletedOk,
                Block = (byte)(block ? 1 : 0),
                PayloadLength = 0,
                Payload = IntPtr.Zero,
                ResultLength = 0,
                Result = IntPtr.Zero
            };

            if (encodedRequest != null && encodedRequest.Length > 0)
            {
                payloadGcHandle = GCHandle.Alloc(encodedRequest, GCHandleType.Pinned);
                command.Payload = payloadGcHandle.AddrOfPinnedObject();
                command.PayloadLength = (uint)encodedRequest.Length;
            }
            if (encodedResult != null && encodedResult.Length > 0 && block)
            {
                resultGcHandle = GCHandle.Alloc(encodedResult, GCHandleType.Pinned);
                command.Result = resultGcHandle.AddrOfPinnedObject();
                command.ResultLength = (UInt32)encodedResult.Length;
            }
            else
            {
                command.ResultLength = 0;
            }

            int updResult = UPD.Ioctl(Nuttx.UpdIoctlFn.Esp32Command, ref command);
            if (updResult == 0)
            {
                result = (StatusCodes)command.StatusCode;
            }
            else
            {
                Resolver.Log.Warn($"ESP Ioctl returned non-success: {updResult}", MessageGroup.Esp);
                result = StatusCodes.Failure;
            }
        }
        catch (Exception ex)
        {
            Resolver.Log.Error($"Exception sending ESP32 command: {ex.Message}", MessageGroup.Esp);
        }
        finally
        {
            if (payloadGcHandle.IsAllocated)
            {
                payloadGcHandle.Free();
            }
            if (resultGcHandle.IsAllocated)
            {
                resultGcHandle.Free();
            }
        }
        return result;
    }

    /// <summary>
    /// Get complex event data from NuttX
    /// </summary>
    /// <param name="eventData">Basic event data already retrieved.</param>
    /// <param name="payload">Data buffer that can receive the event data.</param>
    /// <returns>StatusCodes enum indicating if the operation was successful or if an error occurred.</returns>
    private StatusCodes GetEventData(EventData eventData, out byte[]? payload)
    {
        Resolver.Log.Trace($"Getting event data for message ID 0x{eventData.MessageId:x08}", MessageGroup.Esp);
        var resultGcHandle = default(GCHandle);
        StatusCodes result = StatusCodes.CompletedOk;
        try
        {
            byte[] encodedResult = new byte[4000];
            Array.Clear(encodedResult, 0, encodedResult.Length);
            resultGcHandle = GCHandle.Alloc(encodedResult, GCHandleType.Pinned);

            var request = new Nuttx.UpdEsp32EventDataPayload()
            {
                MessageID = eventData.MessageId,
                Payload = resultGcHandle.AddrOfPinnedObject(),
                PayloadLength = (UInt32)encodedResult.Length
            };

            int updResult = UPD.Ioctl(Nuttx.UpdIoctlFn.UpdEsp32EventDataPayload, ref request);
            if (updResult == 0)
            {
                payload = new byte[request.PayloadLength];
                Array.Copy(encodedResult, payload, request.PayloadLength);
                result = StatusCodes.CompletedOk;
            }
            else
            {
                payload = null;
                result = StatusCodes.Failure;
            }
        }
        finally
        {
            if (resultGcHandle.IsAllocated)
            {
                resultGcHandle.Free();
            }
        }
        return result;
    }

    /// <summary>
    /// Interrupt service handler for the ESP32 coprocessor.
    /// </summary>`
    /// <param name="o"></param>
    private void EventHandlerServiceThread(object o)
    {
        Resolver.Log.Trace("Starting Esp32Coprocessor event handler thread.", MessageGroup.Esp);
        IntPtr queue = Interop.Nuttx.mq_open(new StringBuilder("/Esp32Events"), Nuttx.QueueOpenFlag.ReadOnly);
        byte[] rxBuffer = new byte[22];       // Maximum amount of data that can be read from a NuttX message queue.
        while (true)
        {
            int priority = 0;
            try
            {
                Resolver.Log.Trace("Waiting for event.", MessageGroup.Esp);
                int result;
                do
                {
                    result = Interop.Nuttx.mq_receive(queue, rxBuffer, rxBuffer.Length, ref priority);
                } while (result < 0 && UPD.GetLastError() == Nuttx.ErrorCode.InterruptedSystemCall);

                Resolver.Log.Trace("Event received.", MessageGroup.Esp);
                if (result >= 0)
                {
                    Resolver.Log.Trace("Processing event.", MessageGroup.Esp);
                    EventData eventData = Encoders.ExtractEventData(rxBuffer, 0);
                    byte[]? payload = null;
                    string functionName = string.Empty;
                    switch ((Esp32Interfaces)eventData.Interface)
                    {
                        case Esp32Interfaces.WiFi:
                            functionName = ((WiFiFunction)eventData.Function).ToString();
                            break;
                        case Esp32Interfaces.BlueTooth:
                            functionName = ((BluetoothFunction)eventData.Function).ToString();
                            break;
                        case Esp32Interfaces.Cell:
                            functionName = ((CellFunction)eventData.Function).ToString();
                            break;
                        case Esp32Interfaces.System:
                            functionName = ((SystemFunction)eventData.Function).ToString();
                            break;
                        case Esp32Interfaces.WiredEthernet:
                            functionName = ((EthernetFunction)eventData.Function).ToString();
                            break;
                        case Esp32Interfaces.Transport:
                            functionName = ((TransportFunction)eventData.Function).ToString();
                            break;
                        default:
                            functionName = "Unknown";
                            break;
                    }
                    if (eventData.MessageId == 0)
                    {
                        Resolver.Log.Trace($"Simple event, interface {((Esp32Interfaces)eventData.Interface).ToString()}, event code: {functionName}, status code {((StatusCodes)eventData.StatusCode).ToString()} (0x{eventData.StatusCode:x08})", MessageGroup.Esp);
                    }
                    else
                    {
                        Resolver.Log.Trace($"Complex event, interface {((Esp32Interfaces)eventData.Interface).ToString()}, event code: {functionName}, status code {((StatusCodes)eventData.StatusCode).ToString()} (0x{eventData.StatusCode:x08})", MessageGroup.Esp);
                        GetEventData(eventData, out payload);
                    }
                    Resolver.Log.Trace("Event data collected, raising event.", MessageGroup.Esp);
                    Task.Run(() =>
                    {
                        switch ((Esp32Interfaces)eventData.Interface)
                        {
                            case Esp32Interfaces.WiredEthernet:
                                lock (_queuedEthernetEvents)
                                {
                                    if (_ethernetMessageHandlers == null)
                                    {
                                        _queuedEthernetEvents.Enqueue(new RawEventData
                                        {
                                            Interface = Esp32Interfaces.WiredEthernet,
                                            Function = eventData.Function,
                                            StatusCode = eventData.StatusCode,
                                            Payload = payload
                                        });
                                    }
                                    else
                                    {
                                        _ethernetMessageHandlers.Invoke(this, ((EthernetFunction)eventData.Function, (StatusCodes)eventData.StatusCode, payload ?? EmptyPayload));
                                    }
                                }
                                break;
                            case Esp32Interfaces.WiFi:
                                WiFiMessageReceived?.Invoke(this, ((WiFiFunction)eventData.Function, (StatusCodes)eventData.StatusCode, payload ?? EmptyPayload));
                                break;
                            case Esp32Interfaces.BlueTooth:
                                InvokeEvent((BluetoothFunction)eventData.Function, (StatusCodes)eventData.StatusCode, payload ?? EmptyPayload);
                                break;
                            case Esp32Interfaces.Cell:
                                CellMessageReceived?.Invoke(this, ((CellFunction)eventData.Function, (StatusCodes)eventData.StatusCode, payload ?? EmptyPayload));
                                break;
                            case Esp32Interfaces.System:
                                SystemMessageReceived?.Invoke(this, ((SystemFunction)eventData.Function, (StatusCodes)eventData.StatusCode));
                                break;
                            default:
                                Resolver.Log.Warn($"Received an ESP32 event for interface {eventData.Interface.ToString()}.  Ignored");
                                break;
                        }
                    }).RethrowUnhandledExceptions();
                }
                else
                {
                    throw new Exception($"ESP32 Coprocessor event handler error code {result}");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }

    /// <summary>
    /// Reset the ESP32.
    /// </summary>
    public void Reset()
    {
        SendCommand((byte)Esp32Interfaces.Transport, (UInt32)TransportFunction.ResetEsp32, false, EmptyPayload);
    }

    /// <summary>
    /// Battery charge level in volts.
    /// </summary>
    public double GetBatteryLevel()
    {
        byte[] result = new byte[MAXIMUM_SPI_BUFFER_LENGTH];
        double voltage = 0;
        if (SendCommand((byte)Esp32Interfaces.System, (UInt32)SystemFunction.GetBatteryChargeLevel, true, result) == StatusCodes.CompletedOk)
        {
            GetBatteryChargeLevelResponse response = Encoders.ExtractGetBatteryChargeLevelResponse(result, 0);
            voltage = response.Level / 1000f;
        }
        return voltage;
    }
}