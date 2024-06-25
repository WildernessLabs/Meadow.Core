using Meadow.Hardware;
using System;
using System.IO.Ports;
using System.Linq;

namespace Meadow;

/// <summary>
/// An implementation of ISerialPort that wraps a System.IO.SerialPort
/// </summary>
public class MacSerialPort : ISerialPort, IDisposable
{
    /// <inheritdoc/>
    public event Hardware.SerialDataReceivedEventHandler DataReceived = default!;
    /// <inheritdoc/>
    public event EventHandler BufferOverrun = default!;

    private SerialPort _port;

    /// <inheritdoc/>
    public int BytesToRead => _port.BytesToRead;
    /// <inheritdoc/>
    public bool IsOpen => _port.IsOpen;
    /// <inheritdoc/>
    public string PortName => _port.PortName;
    /// <inheritdoc/>
    public int ReceiveBufferSize => _port.ReadBufferSize;

    /// <summary>
    /// Creates a WindowsSerialPort
    /// </summary>
    /// <param name="portName">The system name of the serial port</param>
    /// <param name="baudRate">The port communication baud rate</param>
    /// <param name="dataBits">The port communication data bits</param>
    /// <param name="parity">The port communication parity</param>
    /// <param name="stopBits">The port communication stopBits</param>
    /// <param name="readBufferSize">The size of the port readBuffer </param>
    public MacSerialPort(
        SerialPortName portName,
        int baudRate,
        int dataBits = 8,
        Hardware.Parity parity = Hardware.Parity.None,
        Hardware.StopBits stopBits = Hardware.StopBits.One,
        int readBufferSize = 4096)
    {
        // there's a bug in SerialPort.GetPortNames
        // https://github.com/dotnet/runtime/issues/93240
        var name = SerialPort
            .GetPortNames()
            .Select(n =>
            {
                var end = n.IndexOf('\0');
                return n.Substring(0, end < 0 ? n.Length : end);
            })
            .FirstOrDefault(n => string.Compare(n, portName.SystemName, true) == 0 || string.Compare(n, portName.FriendlyName, true) == 0
            );

        if (name == null)
        {
            throw new ArgumentException($"Cannot find a serial port with a name matching '{portName.SystemName}' or '{portName.FriendlyName}'");
        }

        _port = new SerialPort(name, baudRate, (System.IO.Ports.Parity)parity, dataBits, (System.IO.Ports.StopBits)stopBits);
        _port.ReadBufferSize = readBufferSize;
    }

    /// <summary>
    /// Creates a WindowsSerialPort
    /// </summary>
    /// <param name="portName">The system name of the serial port</param>
    /// <param name="baudRate">The port communication baud rate</param>
    /// <param name="dataBits">The port communication data bits</param>
    /// <param name="parity">The port communication parity</param>
    /// <param name="stopBits">The port communication stopBits</param>
    /// <param name="readBufferSize">The size of the port readBuffer </param>
    public MacSerialPort(
        string portName,
        int baudRate,
        int dataBits = 8,
        Hardware.Parity parity = Hardware.Parity.None,
        Hardware.StopBits stopBits = Hardware.StopBits.One,
        int readBufferSize = 4096)
        : this(new SerialPortName(portName, portName, null), baudRate, dataBits, parity, stopBits, readBufferSize)
    {
    }

    /// <inheritdoc/>
    public int BaudRate
    {
        get => _port.BaudRate;
        set => _port.BaudRate = value;
    }

    /// <inheritdoc/>
    public int DataBits
    {
        get => _port.DataBits;
        set => _port.DataBits = value;
    }

    /// <inheritdoc/>
    public Hardware.Parity Parity
    {
        get => (Hardware.Parity)_port.Parity;
        set => _port.Parity = (System.IO.Ports.Parity)value;
    }

    /// <inheritdoc/>
    public Hardware.StopBits StopBits
    {
        get => (Hardware.StopBits)_port.StopBits;
        set => _port.StopBits = (System.IO.Ports.StopBits)value;
    }

    /// <inheritdoc/>
    public TimeSpan ReadTimeout
    {
        get => TimeSpan.FromMilliseconds(_port.ReadTimeout);
        set => _port.ReadTimeout = (int)value.TotalMilliseconds;
    }

    /// <inheritdoc/>
    public TimeSpan WriteTimeout
    {
        get => TimeSpan.FromMilliseconds(_port.WriteTimeout);
        set => _port.WriteTimeout = (int)value.TotalMilliseconds;
    }

    /// <inheritdoc/>
    public void ClearReceiveBuffer()
    {
        _port.DiscardOutBuffer();
    }

    /// <inheritdoc/>
    public void Close()
    {
        _port.Close();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        _port.Dispose();
    }

    /// <inheritdoc/>
    public void Open()
    {
        _port.Open();
    }

    /// <inheritdoc/>
    public int Peek()
    {
        throw new NotSupportedException();
    }

    /// <inheritdoc/>
    public int Read(byte[] buffer, int offset, int count)
    {
        return _port.Read(buffer, offset, count);
    }

    /// <inheritdoc/>
    public byte[] ReadAll()
    {
        var buffer = new byte[_port.BytesToRead];
        _port.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    /// <inheritdoc/>
    public int ReadByte()
    {
        return _port.ReadByte();
    }

    /// <inheritdoc/>
    public int Write(byte[] buffer)
    {
        _port.Write(buffer, 0, buffer.Length);
        return buffer.Length;
    }

    /// <inheritdoc/>
    public int Write(byte[] buffer, int offset, int count)
    {
        _port.Write(buffer, offset, count);
        return count;
    }
}
