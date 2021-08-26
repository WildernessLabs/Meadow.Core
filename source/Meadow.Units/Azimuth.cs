using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    // TODO: add DegreesMinutes type. 

    // Notes:
    //  1. there are additional precision compass cardinal points: http://tamivox.org/dave/compass/index.html
    
    /// <summary>
    /// Represents a cardinal direction; 
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Azimuth :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Azimuth` object.
        /// </summary>
        /// <param name="value">The cardinal direction value.</param>
        /// <param name="type">_Compass Decimal Degrees_ (`°`), by default.</param>
        public Azimuth(double value)
        {
            Value = value;
        }

        public Azimuth(Azimuth16PointCardinalNames cardinalPoint)
        {
            Value = AzimuthConversions.Compass16CardinalsToDegrees(cardinalPoint);
        }

        public Azimuth(Azimuth azimuth)
        {
            this.Value = azimuth.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the azimuth.
        /// </summary>
        public enum UnitType
        {
            DecimalDegrees,
            Compass16CardinalPointNames
        }

        //========================
        // TO property conversions

        /// <summary>
        /// Gets the cardinal direction value expressed as a unit _Decimal Degrees_ (`°`)
        /// </summary>
        public double DecimalDegrees {
            get {
                return Value;
            }
        }

        /// <summary>
        /// Gets the cardinal direction value expressed as a unit a 16 division cardinal point
        /// name.
        /// </summary>
        public Azimuth16PointCardinalNames Compass16PointCardinalName {
            get {
                return AzimuthConversions.DegressToCompass16PointCardinalName(Value);
            }
        }

        //=============================
        // FROM convenience conversions

        /// <summary>
        /// Creates a new `Azimuth` object from a unit value in _Decimal Degrees_ (`°`).
        /// </summary>
        /// <param name="value">The cardinal direction value.</param>
        /// <returns>A new cardinal direction object.</returns>
        [Pure] public static Azimuth FromDecimalDegrees(double value) => new Azimuth(value);

        /// <summary>
        /// Creates a new `Azimuth` object from a unit value in _Decimal Degrees_ (`°`).
        /// </summary>
        /// <param name="value">The cardinal direction value.</param>
        /// <returns>A new cardinal direction object.</returns>
        [Pure] public static Azimuth FromCompass16PointCardinalName(Azimuth16PointCardinalNames name) => new Azimuth(name);

        //=============================
        // Boilerplate interface stuff.

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Azimuth)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Azimuth(ushort value) => new Azimuth(value);
        //[Pure] public static implicit operator Azimuth(short value) => new Azimuth(value);
        //[Pure] public static implicit operator Azimuth(uint value) => new Azimuth(value);
        //[Pure] public static implicit operator Azimuth(long value) => new Azimuth(value);
        //[Pure] public static implicit operator Azimuth(int value) => new Azimuth(value);
        //[Pure] public static implicit operator Azimuth(float value) => new Azimuth(value);
        //[Pure] public static implicit operator Azimuth(double value) => new Azimuth(value);
        //[Pure] public static implicit operator Azimuth(decimal value) => new Azimuth((double)value);

        // Comparison
        [Pure] public bool Equals(Azimuth other) => Value == other.Value;
        [Pure] public static bool operator ==(Azimuth left, Azimuth right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Azimuth left, Azimuth right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Azimuth other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Azimuth left, Azimuth right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Azimuth left, Azimuth right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Azimuth left, Azimuth right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Azimuth left, Azimuth right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Azimuth operator +(Azimuth lvalue, Azimuth rvalue) => new Azimuth(StandardizeAngle(lvalue.Value + rvalue.Value));
        [Pure] public static Azimuth operator -(Azimuth lvalue, Azimuth rvalue) => new Azimuth(StandardizeAngle(lvalue.Value - rvalue.Value));
        [Pure] public static Azimuth operator *(Azimuth value, double operand) => new Azimuth(StandardizeAngle(value.Value * operand));
        [Pure] public static Azimuth operator /(Azimuth value, double operand) => new Azimuth(StandardizeAngle(value.Value / operand));

        private static double StandardizeAngle(double value)
        {
            value = value % 360d;
            if (value < 0) return value + 360d;
            return value;
        }

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Azimuth Abs() { return new Azimuth(Math.Abs(this.Value)); }

        // ToString()
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
            return (other is null) ? -1 : (Value).CompareTo(other.Value);
        }

        [Pure] public bool Equals(double? other) => Value.Equals(other);
        [Pure] public bool Equals(double other) => Value.Equals(other);
        [Pure] public int CompareTo(double other) => Value.CompareTo(other);
    }
}
