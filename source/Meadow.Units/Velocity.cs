using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents velocity, or the speed of movement of something.
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public struct Velocity : IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Velocity` object.
        /// </summary>
        /// <param name="value">The velocity value.</param>
        /// <param name="type">_Kilomerters per Hour_ (`Kph`), by default.</param>
        public Velocity(double value, UnitType type = UnitType.Kmh)
        {
            Value = value; Unit = type;
        }

        /// <summary>
        /// The temperature expressed as a value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

        /// <summary>
        /// The type of units available to describe the temperature.
        /// </summary>
        public enum UnitType
        {
            Kmh,
            Mph,
            Knots,
            Mps,
            Fps
        }

        //========================
        // TO property conversions

        /// <summary>
        /// Gets the velocity value expressed as a unit _Kilometers per Hour_ (`Kmh`)
        /// </summary>
        public double Kmh {
            get {
                switch (Unit) {
                    case UnitType.Kmh:
                        return Value;
                    case UnitType.Mph:
                        return VelocityConversions.MphToKmh(Value);
                    case UnitType.Knots:
                        return VelocityConversions.KnotsToKmh(Value);
                    case UnitType.Mps:
                        return VelocityConversions.MpsToKmh(Value);
                    case UnitType.Fps:
                        return VelocityConversions.FpsToKmh(Value);
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        /// <summary>
        /// Gets the velocity value expressed as a unit _Miles per Hour_ (`Mph`)
        /// </summary>
        public double Mph {
            get {
                switch (Unit) {
                    case UnitType.Kmh:
                        return VelocityConversions.KmhToMph(Value);
                    case UnitType.Mph:
                        return Value;
                    case UnitType.Knots:
                        return VelocityConversions.KmhToMph(VelocityConversions.KnotsToKmh(Value));
                    case UnitType.Mps:
                        return VelocityConversions.KmhToMph(VelocityConversions.MpsToKmh(Value));
                    case UnitType.Fps:
                        return VelocityConversions.KmhToMph(VelocityConversions.FpsToKmh(Value));
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        /// <summary>
        /// Gets the velocity value expressed as a unit _Nautical Miles per Hour_ (`Knots`)
        /// </summary>
        public double Knots {
            get {
                switch (Unit) {
                    case UnitType.Kmh:
                        return VelocityConversions.KmhToKnots(Value);
                    case UnitType.Mph:
                        return VelocityConversions.KmhToKnots(VelocityConversions.MphToKmh(Value));
                    case UnitType.Knots:
                        return Value;
                    case UnitType.Mps:
                        return VelocityConversions.KmhToKnots(VelocityConversions.MpsToKmh(Value));
                    case UnitType.Fps:
                        return VelocityConversions.KmhToKnots(VelocityConversions.FpsToKmh(Value));
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        /// <summary>
        /// Gets the velocity value expressed as a unit _Meters per Second_ (`Mps`)
        /// </summary>
        public double Mps {
            get {
                switch (Unit) {
                    case UnitType.Kmh:
                        return VelocityConversions.KmhToMps(Value);
                    case UnitType.Mph:
                        return VelocityConversions.KmhToMps(VelocityConversions.MphToKmh(Value)); ;
                    case UnitType.Knots:
                        return VelocityConversions.KmhToMps(VelocityConversions.KnotsToKmh(Value)); ;
                    case UnitType.Mps:
                        return VelocityConversions.KmhToMps(VelocityConversions.MpsToKmh(Value));
                    case UnitType.Fps:
                        return VelocityConversions.KmhToMps(VelocityConversions.FpsToKmh(Value));
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        /// <summary>
        /// Gets the velocity value expressed as a unit _Feet per Second_ (`Fps`)
        /// </summary>
        public double Fps {
            get {
                switch (Unit) {
                    case UnitType.Kmh:
                        return VelocityConversions.KmhToFps(Value);
                    case UnitType.Mph:
                        return VelocityConversions.KmhToFps(VelocityConversions.MphToKmh(Value)); ;
                    case UnitType.Knots:
                        return VelocityConversions.KmhToFps(VelocityConversions.KnotsToKmh(Value)); ;
                    case UnitType.Mps:
                        return VelocityConversions.KmhToFps(VelocityConversions.MpsToKmh(Value));
                    case UnitType.Fps:
                        return VelocityConversions.KmhToFps(VelocityConversions.FpsToKmh(Value));
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        //=============================
        // FROM convenience conversions

        /// <summary>
        /// Creates a new `Velocity` object from a unit value in _Kilometers per Hour_ (`Kmh`).
        /// </summary>
        /// <param name="value">The velocity value.</param>
        /// <returns>A new velocity object.</returns>
        [Pure] public static Velocity FromKmh(double value) => new Velocity(value, UnitType.Kmh);

        /// <summary>
        /// Creates a new `Velocity` object from a unit value in _Miles per Hour_ (`Mph`).
        /// </summary>
        /// <param name="value">The velocity value.</param>
        /// <returns>A new velocity object.</returns>
        [Pure] public static Velocity FromMph(double value) => new Velocity(value, UnitType.Mph);

        /// <summary>
        /// Creates a new `Velocity` object from a unit value in _Nautical Miles per Hour_ (`Knots`).
        /// </summary>
        /// <param name="value">The velocity value.</param>
        /// <returns>A new velocity object.</returns>
        [Pure] public static Velocity FromKnots(double value) => new Velocity(value, UnitType.Knots);

        /// <summary>
        /// Creates a new `Velocity` object from a unit value in _Meters per Second_ (`Mps`).
        /// </summary>
        /// <param name="value">The velocity value.</param>
        /// <returns>A new velocity object.</returns>
        [Pure] public static Velocity FromMps(double value) => new Velocity(value, UnitType.Mps);

        /// <summary>
        /// Creates a new `Velocity` object from a unit value in _Feet per Second_ (`Fps`).
        /// </summary>
        /// <param name="value">The velocity value.</param>
        /// <returns>A new velocity object.</returns>
        [Pure] public static Velocity FromFps(double value) => new Velocity(value, UnitType.Fps);

        //=============================
        // Boilerplate interface stuff.

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Velocity)obj);
        }

        [Pure] public bool Equals(Velocity other) => Value == other.Value;

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        [Pure] public static bool operator ==(Velocity left, Velocity right) => Equals(left, right);
        [Pure] public static bool operator !=(Velocity left, Velocity right) => !Equals(left, right);
        [Pure] public int CompareTo(Velocity other) => Equals(this, other) ? 0 : Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Velocity left, Velocity right) => Comparer<Velocity>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Velocity left, Velocity right) => Comparer<Velocity>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Velocity left, Velocity right) => Comparer<Velocity>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Velocity left, Velocity right) => Comparer<Velocity>.Default.Compare(left, right) >= 0;

        [Pure] public static implicit operator Velocity(int value) => new Velocity(value);

        [Pure] public override string ToString() => Value.ToString();
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

        // IComparable
        [Pure] public int CompareTo(object obj) => Value.CompareTo(obj);


        [Pure] public TypeCode GetTypeCode() => Value.GetTypeCode();
        [Pure] public bool ToBoolean(IFormatProvider provider) => ((IConvertible)Value).ToBoolean(provider);
        [Pure] public byte ToByte(IFormatProvider provider) => ((IConvertible)Value).ToByte(provider);
        [Pure] public char ToChar(IFormatProvider provider) => ((IConvertible)Value).ToChar(provider);
        [Pure] public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)Value).ToDateTime(provider);
        [Pure] public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)Value).ToDecimal(provider);
        [Pure] public double ToDouble(IFormatProvider provider) => Value;
        [Pure] public short ToInt16(IFormatProvider provider) => ((IConvertible)Value).ToInt16(provider);
        [Pure] public int ToInt32(IFormatProvider provider) => ((IConvertible)Value).ToInt32(provider);
        [Pure] public long ToInt64(IFormatProvider provider) => ((IConvertible)Value).ToInt64(provider);
        [Pure] public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)Value).ToSByte(provider);
        [Pure] public float ToSingle(IFormatProvider provider) => ((IConvertible)Value).ToSingle(provider);
        [Pure] public string ToString(IFormatProvider provider) => Value.ToString(provider);
        [Pure] public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Value).ToType(conversionType, provider);
        [Pure] public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)Value).ToUInt16(provider);
        [Pure] public uint ToUInt32(IFormatProvider provider) => ((IConvertible)Value).ToUInt32(provider);
        [Pure] public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)Value).ToUInt64(provider);

        [Pure]
        public int CompareTo(double? other)
        {
            return (other is null) ? -1 : ((IComparable<double>)Value).CompareTo(other.Value);
        }

        [Pure] public bool Equals(double? other) => Value.Equals(other);
        [Pure] public bool Equals(double other) => Value.Equals(other);
        [Pure] public int CompareTo(double other) => Value.CompareTo(other);
        // can't do this.
        //public int CompareTo(double? other) => Value.CompareTo(other);
    }
}
