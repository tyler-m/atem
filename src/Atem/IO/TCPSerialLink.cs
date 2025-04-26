using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Atem.Core.Input;

namespace Atem.IO
{
    public class TCPSerialLink : IDisposable
    {
        private const int ClockDelay = 1024;
        private const byte FallbackByte = 0xFF;
        private const int DefaultPort = 8080;
        private const string DefaultHostname = "localhost";

        private readonly SerialManager _serialManager;
        private TcpListener _listener;
        private TcpClient _client;
        private NetworkStream _stream;
        private readonly byte[] _buffer = new byte[1];
        private bool _isAwaitingSlaveByte;
        private bool _isAwaitingMasterByte;
        private bool _hasReceivedMasterByte;
        private byte _receivedByte;
        private int _clocksSinceByteReceived;

        public bool IsConnected { get; private set; }
        public string Hostname { get; private set; } = DefaultHostname;
        public int Port { get; private set; } = DefaultPort;
        public bool IsHost { get; private set; }

        public TCPSerialLink(SerialManager serialManager)
        {
            _serialManager = serialManager;
            _serialManager.OnTransferRequest += TransferRequest;
            _serialManager.OnClock += Clock;
        }

        public async Task StartAsync(int port)
        {
            if (IsConnected) return;

            IsConnected = true;
            
            try
            {
                IsHost = true;
                _listener = new TcpListener(IPAddress.Any, port);
                _listener.Start();
                _client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
                _stream = _client.GetStream();
                _ = HandleIncomingData();
            }
            catch
            {
                HandleDisconnect();
            }          
        }

        public async Task ConnectAsync(string hostname, int port)
        {
            if (IsConnected) return;

            IsConnected = true;

            try
            {
                Hostname = hostname;
                _client = new TcpClient();
                await _client.ConnectAsync(Hostname, port).ConfigureAwait(false);
                _stream = _client.GetStream();
                _ = HandleIncomingData();
                IsHost = false;
            }
            catch
            {
                HandleDisconnect();
            }
        }

        public async Task HandleIncomingData()
        {
            try
            {
                while (true)
                {
                    if (await _stream.ReadAsync(_buffer, 0, 1, default).ConfigureAwait(false) == 0)
                    {
                        HandleDisconnect();
                        break;
                    }

                    if (_serialManager.Master)
                    {
                        if (_isAwaitingSlaveByte)
                        {
                            _serialManager.SB = _buffer[0];
                            _isAwaitingSlaveByte = false;
                            _serialManager.TransferEnabled = false;
                            _serialManager.RequestInterrupt();
                        }
                    }
                    else
                    {
                        _clocksSinceByteReceived = 0;
                        _hasReceivedMasterByte = true;
                        _receivedByte = _buffer[0];
                    }
                }
            }
            catch
            {
                HandleDisconnect();
            }
        }

        public void TransferRequest()
        {
            if (!IsConnected) return;

            if (_serialManager.Master)
            {
                _isAwaitingSlaveByte = _serialManager.TransferEnabled;

                if (_isAwaitingSlaveByte)
                {
                    Write(_serialManager.SB);
                }
            }
            else
            {
                _isAwaitingMasterByte = _serialManager.TransferEnabled;
            }
        }

        public void Clock()
        {
            if (!IsConnected) return;

            if (_hasReceivedMasterByte)
            {
                _clocksSinceByteReceived++;

                // allow for some clock-based delay so the game has
                // time to update _serialManager.SB before sending a
                // byte back
                if (_clocksSinceByteReceived >= ClockDelay)
                {
                    if (_isAwaitingMasterByte)
                    {
                        Write(_serialManager.SB);
                        _serialManager.SB = _receivedByte;
                        _isAwaitingMasterByte = false;
                        _serialManager.TransferEnabled = false;
                        _serialManager.RequestInterrupt();
                    }
                    else
                    {
                        Write(FallbackByte);
                    }

                    _hasReceivedMasterByte = false;
                    _clocksSinceByteReceived = 0;
                }
            }
        }

        private void Write(byte value)
        {
            if (!IsConnected) return;

            try
            {
                _stream.Write([value], 0, 1);
            }
            catch
            {
                HandleDisconnect();
            }
        }

        private void HandleDisconnect()
        {
            _stream?.Dispose();
            _stream = null;

            _client?.Dispose();
            _client = null;

            _listener?.Stop();
            _listener = null;

            IsConnected = false;
            _isAwaitingSlaveByte = false;
            _isAwaitingMasterByte = false;
            _hasReceivedMasterByte = false;
            _buffer[0] = 0;
            _receivedByte = 0;
            _clocksSinceByteReceived = 0;
            IsHost = false;
        }

        public void Dispose()
        {
            _stream?.Dispose();
            _client?.Dispose();
            _listener?.Stop();

            GC.SuppressFinalize(this);
        }
    }
}