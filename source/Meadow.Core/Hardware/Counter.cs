﻿using System;
using System.Threading;

namespace Meadow.Hardware;

/// <summary>
/// Represents a class that counts edges on an interrupt-capable input port
/// </summary>
public class Counter : ICounter, IDisposable
{
    private readonly IDigitalInterruptPort input;
    private long count;
    private readonly bool portCreated = false;
    private bool isDisposed;

    /// <summary>
    /// Enables or disables the counter
    /// </summary>
    public bool Enabled { get; set; }

    /// <summary>
    /// Returns the current Counter value
    /// </summary>
    public ulong Count => (uint)count;

    /// <summary>
    /// Creates a Counter instance
    /// </summary>
    /// <param name="pin"></param>
    /// <param name="edge"></param>
    /// <exception cref="ArgumentException"></exception>
    public Counter(IPin pin, InterruptMode edge)
    {
        if (edge == InterruptMode.None)
        {
            throw new ArgumentException("You must have a count edge selected");
        }

        if (!pin.Supports<IDigitalChannelInfo>(c => c.InterruptCapable))
        {
            throw new ArgumentException($"Pin {pin.Name} is not interrupt capable");
        }


        this.input = pin.CreateDigitalInterruptPort(edge);
        portCreated = true;

        input.Changed += OnInputChanged;
    }

    /// <summary>
    /// Creates a Counter instance
    /// </summary>
    /// <param name="input">The IDigitalInterruptPort to count on</param>
    /// <exception cref="ArgumentException"></exception>
    public Counter(IDigitalInterruptPort input)
    {
        if (input.InterruptMode == InterruptMode.None)
        {
            throw new ArgumentException("input must have a set interrupt mode");
        }

        this.input = input;
        input.Changed += OnInputChanged;
    }

    private void OnInputChanged(object sender, DigitalPortResult e)
    {
        if (Enabled)
        {
            Interlocked.Add(ref count, 1);
        }
    }

    /// <summary>
    /// The transition type(s) to count
    /// </summary>
    public InterruptMode TransitionType
    {
        get => input.InterruptMode;
    }

    /// <summary>
    /// Resets the Count to zero
    /// </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref count, 0);
    }

    /// <summary>
    /// Releases disposable resources
    /// </summary>
    /// <param name="disposing"></param>
    protected virtual void Dispose(bool disposing)
    {
        if (!isDisposed)
        {
            if (disposing)
            {
                if (portCreated) input.Dispose();
            }

            isDisposed = true;
        }
    }

    /// <summary>
    /// Disposes the current instance
    /// </summary>
    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }
}