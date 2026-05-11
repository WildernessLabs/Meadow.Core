using Meadow.Devices;
using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Meadow.Hardware;

/// <summary>
/// F7-specific digital output port that writes the STM32 GPIO BSRR register directly
/// from user space, and reads the ODR register for state queries. The register
/// addresses and pin bit are resolved once at construction and cached as fields, so
/// the State setter avoids the per-call IPin → (port, pin, address) dictionary lookup
/// and lock that the generic <see cref="DigitalOutputPort"/> incurs.
///
/// Because <see cref="State"/> reads ODR directly, it always reflects the actual pin
/// output, even after writes performed through <see cref="GetRawWriteHandle"/>. No
/// cached state field is maintained — there is nothing for the caller to keep in sync.
/// </summary>
public sealed class F7DigitalOutputPort : DigitalOutputPortBase
{
    private readonly F7GPIOManager _ioController;
    private readonly unsafe uint* _bsrr;
    private readonly unsafe uint* _odr;
    private readonly uint _setMask;     // logical "set high" — accounts for InverseLogic
    private readonly uint _clearMask;   // logical "set low" — accounts for InverseLogic
    private readonly uint _pinBit;      // raw bit position, e.g. 1u << 5 for pin 5
    private bool _disposed;

    private unsafe F7DigitalOutputPort(
        IPin pin,
        F7GPIOManager ioController,
        IDigitalChannelInfo channel,
        bool initialState,
        OutputType initialOutputType,
        uint bsrrAddress,
        uint odrAddress,
        uint pinBit)
        : base(pin, channel, initialState, initialOutputType)
    {
        _ioController = ioController;
        _bsrr = (uint*)bsrrAddress;
        _odr = (uint*)odrAddress;
        _pinBit = pinBit;

        // Pre-compute logical set/clear masks. BSRR low half sets, high half clears.
        // If InverseLogic, swap so caller "set true" still drives logical-high semantics.
        var rawSet = pinBit;
        var rawClear = pinBit << 16;
        if (InverseLogic)
        {
            _setMask = rawClear;
            _clearMask = rawSet;
        }
        else
        {
            _setMask = rawSet;
            _clearMask = rawClear;
        }

        var (reserved, message) = _ioController.DeviceChannelManager.ReservePin(pin, ChannelConfigurationType.DigitalOutput);
        if (!reserved)
        {
            throw new PortInUseException($"{nameof(F7DigitalOutputPort)}: {message}");
        }

        _ioController.ConfigureOutput(pin, initialState, initialOutputType);
    }

    /// <inheritdoc/>
    public override unsafe bool State
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            // Read actual pin output state from ODR, apply InverseLogic for logical value.
            var electricallyHigh = (*_odr & _pinBit) != 0;
            return InverseLogic ? !electricallyHigh : electricallyHigh;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            *_bsrr = value ? _setMask : _clearMask;
        }
    }

    /// <summary>
    /// Toggles the port's current state in a single BSRR write. Reads the actual
    /// electrical state from ODR, then writes the opposite to BSRR. Always correct
    /// regardless of how the port was last written (including through the raw handle).
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe void Toggle()
    {
        if ((*_odr & _pinBit) != 0)
            *_bsrr = _pinBit << 16;  // electrical: clear
        else
            *_bsrr = _pinBit;        // electrical: set
    }

    /// <summary>
    /// Hands the caller a direct pointer to the GPIO BSRR register plus the bit masks
    /// for set and clear, for hot-loop bit-banging that needs to bypass even the
    /// <see cref="State"/> setter dispatch. Typical use: WS2812 / 1-Wire / custom
    /// serial protocols where managed call overhead is the bottleneck.
    ///
    /// Caveats:
    ///   - Caller must execute the writes inside an <c>unsafe</c> block.
    ///   - Masks are in electrical (post-InverseLogic) terms: <paramref name="setMask"/>
    ///     drives the pin physically high regardless of <c>InverseLogic</c>.
    ///   - No protection against concurrent access. STM32 BSRR writes are atomic at the
    ///     hardware level (no torn writes); multi-thread coordination is the caller's
    ///     responsibility.
    ///   - <see cref="State"/> reads remain accurate after raw writes because the
    ///     getter reads the ODR register directly — no cached state to re-sync.
    /// </summary>
    public unsafe void GetRawWriteHandle(out uint* bsrr, out uint setMask, out uint clearMask)
    {
        bsrr = _bsrr;
        // Hand out electrical masks (un-inverted), independent of how the port was
        // constructed. This is what direct register users want: a clear "set the pin
        // high" vs "set the pin low" mask pair.
        setMask = _pinBit;
        clearMask = _pinBit << 16;
    }

    /// <summary>
    /// Creates an F7-specific digital output port that writes the GPIO BSRR register
    /// directly. Falls back to <see cref="DigitalOutputPort.From"/> semantics for pin
    /// channel resolution.
    /// </summary>
    public static F7DigitalOutputPort From(
        IPin pin,
        F7GPIOManager ioController,
        bool initialState = false,
        OutputType initialOutputType = OutputType.PushPull)
    {
        var channel = pin.SupportedChannels.OfType<IDigitalChannelInfo>().FirstOrDefault()
            ?? throw new Exception("Unable to create an output port on the pin, because it doesn't have a digital channel");

        var handle = ioController.GetDiscreteWriteHandle(pin);
        return new F7DigitalOutputPort(pin, ioController, channel, initialState, initialOutputType,
            handle.bsrrAddress, handle.odrAddress, handle.pinBit);
    }

    /// <inheritdoc/>
    protected override void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            _ioController.DeviceChannelManager.ReleasePin(Pin);
        }
        _disposed = true;
    }

    /// <summary>
    /// Finalizer.
    /// </summary>
    ~F7DigitalOutputPort()
    {
        Dispose(false);
    }
}
