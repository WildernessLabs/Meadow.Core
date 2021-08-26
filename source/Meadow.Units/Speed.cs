using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Speed
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Speed :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Speed` object.
        /// </summary>
        /// <param name="value">The Speed value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Speed(double value, UnitType type = UnitType.KilometersPerSecond)
        {
            Value = SpeedConversions.Convert(value, type, UnitType.KilometersPerSecond);
        }

        public Speed(Speed speed)
        {
            this.Value = speed.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the speed.
        /// </summary>
        public enum UnitType
        {
            FeetPerMinute,
            FeetPerSecond,
            KilometersPerHour,
            KilometersPerMinute,
            KilometersPerSecond,
            Knots,
            MetersPerMinute,
            MetersPerSecond,
            MilesPerHour,
            MilesPerMinute,
            MilesPerSecond,
            SpeedOfLight,
            Mach,
        }

        public double FeetPerSecond => From(UnitType.FeetPerSecond);
        public double FeetPerMinute => From(UnitType.FeetPerMinute);
        public double KilometersPerHour => From(UnitType.KilometersPerHour);
        public double KilometersPerMinute => From(UnitType.KilometersPerMinute);
        public double KilometersPerSecond => From(UnitType.KilometersPerSecond);
        public double Knots => From(UnitType.Knots);
        public double MetersPerMinute => From(UnitType.MetersPerMinute);
        public double MetersPerSecond => From(UnitType.MetersPerSecond);
        public double MilesPerHour => From(UnitType.MilesPerHour);
        public double MilesPerMinute => From(UnitType.MilesPerMinute);
        public double MilesPerSecond => From(UnitType.MilesPerSecond);
        public double SpeedOfLight => From(UnitType.SpeedOfLight);
        public double Mach => From(UnitType.Mach);

        [Pure]
        public double From(UnitType convertTo)
        {
            return SpeedConversions.Convert(Value, UnitType.KilometersPerSecond, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Speed)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Speed(ushort value) => new Speed(value);
        //[Pure] public static implicit operator Speed(short value) => new Speed(value);
        //[Pure] public static implicit operator Speed(uint value) => new Speed(value);
        //[Pure] public static implicit operator Speed(long value) => new Speed(value);
        //[Pure] public static implicit operator Speed(int value) => new Speed(value);
        //[Pure] public static implicit operator Speed(float value) => new Speed(value);
        //[Pure] public static implicit operator Speed(double value) => new Speed(value);
        //[Pure] public static implicit operator Speed(decimal value) => new Speed((double)value);

        // Comparison
        [Pure] public bool Equals(Speed other) => Value == other.Value;
        [Pure] public static bool operator ==(Speed left, Speed right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Speed left, Speed right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Speed other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Speed operator +(Speed lvalue, Speed rvalue) => new Speed(lvalue.Value + rvalue.Value);
        [Pure] public static Speed operator -(Speed lvalue, Speed rvalue) => new Speed(lvalue.Value - rvalue.Value);
        [Pure] public static Speed operator *(Speed value, double operand) => new Speed(value.Value * operand);
        [Pure] public static Speed operator /(Speed value, double operand) => new Speed(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Speed Abs() { return new Speed(Math.Abs(this.Value)); }

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