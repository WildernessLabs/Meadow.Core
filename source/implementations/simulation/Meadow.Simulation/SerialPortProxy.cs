using Meadow.Hardware;
using System;
using System.IO.Ports;

namespace Meadow.Simulation
{
    public class SerialPortProxy : ISerialPort
    {
        public event Hardware.SerialDataReceivedEventHandler DataReceived;
        public event EventHandler BufferOverrun;

        private SerialPort _port;

        public SerialPortProxy(
            SerialPortName portName,
            int baudRate,
            int dataBits = 8,
            Hardware.Parity parity = Hardware.Parity.None,
            Hardware.StopBits stopBits = Hardware.StopBits.One,
            int readBufferSize = 4096)
        {
            _port = new SerialPort(portName.SystemName, baudRate, (System.IO.Ports.Parity)parity, dataBits, (System.IO.Ports.StopBits)stopBits);
        }

        public int BytesToRead => _port.BytesToRead;
        public bool IsOpen => _port.IsOpen;
        public string PortName => _port.PortName;
        public int ReceiveBufferSize => _port.ReadBufferSize;

        public int BaudRate
        {
            get => _port.BaudRate;
            set => _port.BaudRate = value;
        }

        public int DataBits
        {
            get => _port.DataBits;
            set => _port.DataBits = value;
        }

        public Hardware.Parity Parity
        {
            get => (Hardware.Parity)_port.Parity;
            set => _port.Parity = (System.IO.Ports.Parity)value;
        }

        public Hardware.StopBits StopBits
        {
            get => (Hardware.StopBits)_port.StopBits;
            set => _port.StopBits = (System.IO.Ports.StopBits)value;
        }

        public TimeSpan ReadTimeout
        {
            get => TimeSpan.FromMilliseconds(_port.ReadTimeout);
            set => _port.ReadTimeout = (int)value.TotalMilliseconds;
        }

        public TimeSpan WriteTimeout
        {
            get => TimeSpan.FromMilliseconds(_port.WriteTimeout);
            set => _port.WriteTimeout = (int)value.TotalMilliseconds;
        }

        public void ClearReceiveBuffer()
        {
            _port.DiscardOutBuffer();
        }

        public void Close()
        {
            _port.Close();
        }

        public void Dispose()
        {
            _port.Dispose();
        }

        public void Open()
        {
            _port.Open();
        }

        public int Peek()
        {
            throw new NotSupportedException();
        }

        public int Read(byte[] buffer, int offset, int count)
        {
            return _port.Read(buffer, offset, count);
        }

        public int ReadAll(byte[] buffer)
        {
            return _port.Read(buffer, 0, _port.BytesToRead);
        }

        public int ReadByte()
        {
            return _port.ReadByte();
        }

        public int Write(byte[] buffer)
        {
            _port.Write(buffer, 0, buffer.Length);
            return buffer.Length;
        }

        public int Write(byte[] buffer, int offset, int count)
        {
            _port.Write(buffer, offset, count);
            return count;
        }
    }
}
