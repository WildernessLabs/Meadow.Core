using Meadow.Units;
using System;
using System.Linq;

namespace Meadow.Hardware;

/// <summary>
/// Implements frequency and duty cycle analysis of digital signals using software-based timing measurements.
/// </summary>
public class SoftDigitalSignalAnalyzer : IDigitalSignalAnalyzer, IDisposable
{
    private IDigitalInterruptPort _inputPort;
    private bool _portCreated = false;
    private bool _captureDutyCycle = false;
    private readonly CircularBuffer<int> _periodQueue = new(10);
    private int _mostRecentPeriod;
    private int _rising;
    private int _falling;
    private double _duty;

    /// <summary>
    /// Returns true if the analyzer has been disposed
    /// </summary>
    public bool IsDisposed { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoftDigitalSignalAnalyzer"/> class to monitor a single digital input pin.
    /// </summary>
    /// <param name="pin">The digital input pin to monitor.</param>
    /// <param name="resistorMode">The digital input pin's resistor mode.</param>
    /// <param name="captureDutyCycle">Whether or not to capture duty cycle. Not capturing it is more efficient and allows faster frequency capture</param>
    public SoftDigitalSignalAnalyzer(IPin pin, ResistorMode resistorMode = ResistorMode.InternalPullDown, bool captureDutyCycle = true)
    {
        var edge = captureDutyCycle ? InterruptMode.EdgeBoth : InterruptMode.EdgeRising;

        _inputPort = pin.CreateDigitalInterruptPort(InterruptMode.EdgeBoth, resistorMode);
        _portCreated = true;
        _captureDutyCycle = captureDutyCycle;
        Initialize();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="SoftDigitalSignalAnalyzer"/> class to monitor a digital input port.
    /// </summary>
    /// <param name="port">The digital interrupt port to monitor.</param>
    /// <param name="captureDutyCycle">Whether or not to capture duty cycle. Not capturing it is more efficient and allows faster frequency capture</param>
    public SoftDigitalSignalAnalyzer(IDigitalInterruptPort port, bool captureDutyCycle = true)
    {
        _inputPort = port;
        _captureDutyCycle = captureDutyCycle;
        Initialize();
    }

    private void Initialize()
    {
        _inputPort.Changed += OnInterrupt;
    }

    private void OnInterrupt(object sender, DigitalPortResult e)
    {
        switch (e.New.State)
        {
            case false:
                if (_captureDutyCycle)
                {
                    _falling = Environment.TickCount;
                }
                break;
            case true:
                var now = Environment.TickCount;
                _mostRecentPeriod = Math.Abs(now - _rising);
                _periodQueue.Append(_mostRecentPeriod);
                if (_captureDutyCycle)
                {
                    _duty = 1.0 - ((now - _falling) / (double)_mostRecentPeriod);
                }
                _rising = now;


                break;
        }
    }

    /// <inheritdoc/>
    public double GetDutyCycle()
    {
        return _duty;
    }

    /// <inheritdoc/>
    public Frequency GetMeanFrequency()
    {
        if (_periodQueue.Count == 0) { return Frequency.Zero; }

        return new Frequency(1000d / _periodQueue.Average(), Frequency.UnitType.Hertz);
    }

    /// <inheritdoc/>
    public Frequency GetFrequency()
    {
        if (_mostRecentPeriod == 0) return Frequency.Zero;

        return new Frequency(1000d / _mostRecentPeriod);
    }

    /// <inheritdoc/>
    protected virtual void Dispose(bool disposing)
    {
        if (!IsDisposed)
        {
            if (disposing)
            {
                _inputPort.Changed -= OnInterrupt;

                if (_portCreated)
                {
                    _inputPort.Dispose();
                }
            }

            IsDisposed = true;
        }
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}
