using Meadow.Logging;
using System;
using System.Net;
using System.Net.Sockets;

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

        private void AcceptProc(IAsyncResult result)
        {
            _logger.Info("Client Connected");

            if (result.AsyncState is TcpListener { } listener)
            {
                var client = listener.EndAcceptTcpClient(result);
                NegotiateClientConnection(client);

                // allow multiple clients
                listener.BeginAcceptTcpClient(AcceptProc, listener);
            }
        }

        private void NegotiateClientConnection(TcpClient client)
        {
            var stream = client.GetStream();

            var buffer = new byte[4096];

            while (client.Connected)
            {
                if (stream.DataAvailable)
                {
                    var read = stream.Read(buffer, 0, buffer.Length);
                    _logger.Info($"Received {read} bytes");

                    // TODO: pull the HTTP GET and negotiate
                }
            }
        }
    }
}
