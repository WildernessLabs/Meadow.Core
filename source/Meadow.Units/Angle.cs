using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Angle
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Angle :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Angle` object.
        /// </summary>
        /// <param name="value">The Angle value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Angle(double value, UnitType type = UnitType.Degrees)
        {
            Value = AngleConversions.Convert(value, type, UnitType.Degrees);
        }

        public Angle(Angle angle)
        {
            this.Value = angle.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the Angle.
        /// </summary>
        public enum UnitType
        {
            Revolutions,
            Degrees,
            Radians,
            Gradians,
            Minutes,
            Seconds
        }

        public double Revolutions => From(UnitType.Revolutions);
        public double Degrees => From(UnitType.Degrees);
        public double Radians => From(UnitType.Radians);
        public double Gradians => From(UnitType.Gradians);
        public double Minutes => From(UnitType.Minutes);
        public double Seconds => From(UnitType.Seconds);

        [Pure]
        public double From(UnitType convertTo)
        {
            return AngleConversions.Convert(Value, UnitType.Degrees, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Angle)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Angle(ushort value) => new Angle(value);
        //[Pure] public static implicit operator Angle(short value) => new Angle(value);
        //[Pure] public static implicit operator Angle(uint value) => new Angle(value);
        //[Pure] public static implicit operator Angle(long value) => new Angle(value);
        //[Pure] public static implicit operator Angle(int value) => new Angle(value);
        //[Pure] public static implicit operator Angle(float value) => new Angle(value);
        //[Pure] public static implicit operator Angle(double value) => new Angle(value);
        //[Pure] public static implicit operator Angle(decimal value) => new Angle((double)value);

        // Comparison
        [Pure] public bool Equals(Angle other) => Value == other.Value;
        [Pure] public static bool operator ==(Angle left, Angle right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Angle left, Angle right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Angle other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Angle operator +(Angle lvalue, Angle rvalue) => new Angle(StandardizeDegrees(lvalue.Value + rvalue.Value), UnitType.Degrees);
        [Pure] public static Angle operator -(Angle lvalue, Angle rvalue) => new Angle(StandardizeDegrees(lvalue.Value - rvalue.Value), UnitType.Degrees);
        [Pure] public static Angle operator *(Angle value, double operand) => new Angle(StandardizeDegrees(value.Value * operand), UnitType.Degrees);
        [Pure] public static Angle operator /(Angle value, double operand) => new Angle(StandardizeDegrees(value.Value / operand), UnitType.Degrees);

        private static double StandardizeDegrees(double value)
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
        [Pure] public Angle Abs() { return new Angle(Math.Abs(this.Value), UnitType.Degrees); }

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