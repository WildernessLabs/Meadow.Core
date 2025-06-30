using Meadow.Gateways;
using Meadow.Gateways.Bluetooth;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow;

public class SimulatedBluetoothAdapter : IBluetoothAdapter
{
    /// <inheritdoc/>
    public event EventHandler? ServerStarting;

    /// <inheritdoc/>
    public event EventHandler? ServerStarted;

    /// <inheritdoc/>
    public event EventHandler? ServerStopping;

    /// <inheritdoc/>
    public event EventHandler? ServerStopped;

    /// <inheritdoc/>
    public event EventHandler? ClientConnected;

    /// <inheritdoc/>
    public event EventHandler? ClientDisconnected;

    private SimulatedBluetoothServer? _server;

    public bool StartBluetoothServer(IDefinition configuration)
    {
        if (_server != null)
        {
            throw new Exception("server already running");
        }

        _server = new SimulatedBluetoothServer(configuration);
        return true;
    }

    public bool StopBluetoothServer()
    {
        _server?.Stop();
        _server?.Dispose();
        _server = null;
        return true;
    }
}

internal class SimulatedBluetoothServer
{
    private readonly IDefinition _configuration;
    private readonly ConcurrentDictionary<string, WebSocket> _connectedClients = new();
    private readonly HttpListener _listener;
    private readonly CancellationTokenSource _cancellationSource = new();
    private readonly Task _listenerTask;

    public SimulatedBluetoothServer(IDefinition configuration, int port = 8080)
    {
        _configuration = configuration;
        _listener = new HttpListener();
        _listener.Prefixes.Add($"http://0.0.0.0:{port}/");

        foreach (var service in configuration.Services)
        {
            foreach (var characteristic in service.Characteristics)
            {
                characteristic.ValueSet += OnDeviceSideValueSet;
            }
        }

        _listener.Start();
        _listenerTask = HandleIncomingConnections();
    }

    private void OnDeviceSideValueSet(ICharacteristic c, object data)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        _cancellationSource.Cancel();
        _listener.Stop();
        _listenerTask?.Wait();
    }

    public void Dispose()
    {
        Stop();
        _cancellationSource.Dispose();
        _listener.Close();
    }

    private async Task HandleIncomingConnections()
    {
        try
        {
            while (!_cancellationSource.Token.IsCancellationRequested)
            {
                var context = await _listener.GetContextAsync();
                if (context.Request.IsWebSocketRequest)
                {
                    var webSocketContext = await context.AcceptWebSocketAsync(null);
                    var clientId = Guid.NewGuid().ToString();
                    _ = HandleWebSocketConnection(webSocketContext.WebSocket, clientId);
                }
                else
                {
                    context.Response.StatusCode = 400;
                    context.Response.Close();
                }
            }
        }
        catch (HttpListenerException) when (_cancellationSource.Token.IsCancellationRequested)
        {
            // Normal shutdown
        }
    }

    private class Message
    {
        public string Type { get; set; }
        public string ServiceUuid { get; set; }
        public string CharacteristicUuid { get; set; }
        public string Value { get; set; }
    }

    private async Task HandleWebSocketConnection(WebSocket webSocket, string clientId)
    {
        _connectedClients[clientId] = webSocket;

        try
        {
            var buffer = new byte[1024 * 4];
            while (webSocket.State == WebSocketState.Open && !_cancellationSource.Token.IsCancellationRequested)
            {
                var result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    _cancellationSource.Token);

                if (result.MessageType == WebSocketMessageType.Close)
                    break;

                var message = JsonSerializer.Deserialize<Message>(
                    Encoding.UTF8.GetString(buffer, 0, result.Count));

                await HandleMessage(webSocket, message);
            }
        }
        catch (OperationCanceledException) when (_cancellationSource.Token.IsCancellationRequested)
        {
            // Normal shutdown
        }
        catch (Exception ex)
        {
            // Log error if needed
            Console.WriteLine($"Error handling WebSocket connection: {ex}");
        }
        finally
        {
            _connectedClients.TryRemove(clientId, out _);
            if (webSocket.State == WebSocketState.Open)
            {
                try
                {
                    await webSocket.CloseAsync(
                        WebSocketCloseStatus.NormalClosure,
                        "",
                        CancellationToken.None);
                }
                catch
                {
                    // Best effort close
                }
            }
        }
    }

    private readonly Dictionary<string, string> _valueCache = new();

    private async Task HandleMessage(WebSocket webSocket, Message message)
    {
        switch (message.Type.ToLower())
        {
            case "discovery":
                var serviceInfo = _configuration.Services.Select(s => new
                {
                    s.Uuid,
                    s.Name,
                    Characteristics = s.Characteristics.Select(c => new
                    {
                        c.Uuid,
                        c.Name,
                        //c.CanRead,
                        //c.CanWrite,
                        //c.CanNotify
                    })
                });
                await SendResponse(webSocket, new { Type = "discovery", Services = serviceInfo });
                break;

            case "read":
                var sid = ushort.Parse(message.ServiceUuid);
                var service = _configuration.Services.FirstOrDefault(s => s.Uuid == sid);
                if (service != null)
                {
                    var c = service.Characteristics.FirstOrDefault(c => string.Compare(c.Uuid, message.CharacteristicUuid, true) == 0);

                    if (c != null && c.CanBeRead())
                    {
                        await SendResponse(webSocket, new
                        {
                            Type = "read",
                            ServiceUuid = message.ServiceUuid,
                            CharacteristicUuid = message.CharacteristicUuid,
                            Value = _valueCache.GetValueOrDefault(c.Uuid, string.Empty)
                        });
                    }
                }

                break;

            case "write":
                //    var sid = ushort.Parse(message.ServiceUuid);
                //    var service = _configuration.Services.FirstOrDefault(s => s.Uuid == sid);
                //    if (service != null)
                //    {
                //        var c = service.Characteristics.FirstOrDefault(c => string.Compare(c.Uuid, message.CharacteristicUuid, true) == 0);

                //        if (c != null && c.CanBeWritten())
                //        {
                //        }
                //    }

                //    if (_configuration.Services.TryGetValue(message.ServiceUuid, out service) &&
                //service.Characteristics.TryGetValue(message.CharacteristicUuid, out characteristic) &&
                //characteristic.CanWrite)
                //    {
                //        characteristic.Value = Convert.FromBase64String(message.Value);
                //        if (characteristic.CanNotify)
                //        {
                //            await NotifyValueChange(message.ServiceUuid, message.CharacteristicUuid, characteristic.Value);
                //        }
                //    }
                break;
        }
    }

    private async Task SendResponse(WebSocket socket, object data)
    {
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);
        await socket.SendAsync(
            new ArraySegment<byte>(bytes),
            WebSocketMessageType.Text,
            true,
            _cancellationSource.Token);
    }

    private async Task NotifyValueChange(string serviceUuid, string characteristicUuid, byte[] value)
    {
        var notification = new
        {
            Type = "notification",
            ServiceUuid = serviceUuid,
            CharacteristicUuid = characteristicUuid,
            Value = Convert.ToBase64String(value)
        };

        foreach (var client in _connectedClients.Values)
        {
            if (client.State == WebSocketState.Open)
            {
                try
                {
                    await SendResponse(client, notification);
                }
                catch
                {
                    // Best effort notification
                }
            }
        }
    }
}

