using Meadow.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Meadow.Simulation
{
    internal class WebSocketServer
    {
        private Logger _logger;
        private TcpListener _listener;
        private IAsyncResult _client;

        public WebSocketServer(Logger logger, int port = 8081)
        {
            _logger = logger;

            _listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            _listener.Start();
            var result = _listener.BeginAcceptTcpClient(AcceptProc, _listener);
        }

        private List<TcpClient> _clients = new List<TcpClient>();

        private void AcceptProc(IAsyncResult result)
        {
            _logger.Info("Client Connected");

            if (result.AsyncState is TcpListener { } listener)
            {
                var client = listener.EndAcceptTcpClient(result);
                if(NegotiateClientConnection(client))
                {
                    lock (_clients)
                    {
                        _clients.Add(client);
                    }
                }

                // allow multiple clients
                listener.BeginAcceptTcpClient(AcceptProc, listener);
            }
        }

        public void SendMessage(string message)
        {
            var data = Encoding.UTF8.GetBytes(message);
            var frameData = new List<byte>();
            frameData.Add(0x81); // utf text
            if(data.Length < 126)
            {
                frameData.Add((byte)data.Length);
            }
            else if(data.Length < ushort.MaxValue)
            {
                frameData.Add((byte)data.Length);
                frameData.AddRange(BitConverter.GetBytes((ushort)data.Length));
            }
            else
            {
                throw new Exception("length > 64k not supported");
            }
            frameData.AddRange(data);

            lock (_clients)
            {
                for (var i = _clients.Count - 1; i >= 0; i--)
                {
                    if (!_clients[i].Connected)
                    {
                        _clients.RemoveAt(i);
                    }
                    else
                    {
                        var s = _clients[i].GetStream();
                        s.Write(frameData.ToArray(), 0, frameData.Count);
                    }
                }
            }
        }

        private bool NegotiateClientConnection(TcpClient client)
        {
            var stream = client.GetStream();

            var buffer = new byte[4096];

            if (client.Connected)
            {
                if (stream.DataAvailable)
                {
                                        var read = stream.Read(buffer, 0, buffer.Length);
                                        _logger.Info($"Received {read} bytes");

                    var request = Encoding.UTF8.GetString(buffer, 0, read);

                    if (Regex.IsMatch(request, "^GET", RegexOptions.IgnoreCase))
                    {
                        var wskey = $"{Regex.Match(request, "Sec-WebSocket-Key: (.*)").Groups[1].Value.Trim()}258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
                        var sha = System.Security.Cryptography.SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(wskey));

                        var response = Encoding.UTF8.GetBytes(
                            "HTTP/1.1 101 Switching Protocols\r\n" +
                            "Connection: Upgrade\r\n" +
                            "Upgrade: websocket\r\n" +
                            "Sec-WebSocket-Accept: " + Convert.ToBase64String(sha) + "\r\n\r\n");

                        stream.Write(response, 0, response.Length);

                        return true;
                    }
                }
            }

            return false;
        }
    }
}
