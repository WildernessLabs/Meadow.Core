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
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Speed : IUnitType, IComparable, IFormattable, IConvertible, IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Speed` object.
        /// </summary>
        /// <param name="value">The Speed value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Speed(double value, UnitType type = UnitType.KilometersPerSecond)
        {
            //always store reference value
            Unit = type;
            _value = SpeedConversions.Convert(value, type, UnitType.KilometersPerSecond);
        }

        /// <summary>
        /// The speed expressed as a value.
        /// </summary>
        public double Value
        {
            get => SpeedConversions.Convert(_value, UnitType.KilometersPerSecond, Unit);
            set => _value = SpeedConversions.Convert(value, Unit, UnitType.KilometersPerSecond);
        }

        private double _value;

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

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
            return SpeedConversions.Convert(_value, UnitType.KilometersPerSecond, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Speed)obj);
        }

        [Pure] public bool Equals(Speed other) => _value == other._value;

        [Pure] public override int GetHashCode() => _value.GetHashCode();

        [Pure] public static bool operator ==(Speed left, Speed right) => Equals(left, right);
        [Pure] public static bool operator !=(Speed left, Speed right) => !Equals(left, right);
        [Pure] public int CompareTo(Speed other) => Equals(this, other) ? 0 : _value.CompareTo(other._value);
        [Pure] public static bool operator <(Speed left, Speed right) => Comparer<Speed>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Speed left, Speed right) => Comparer<Speed>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Speed left, Speed right) => Comparer<Speed>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Speed left, Speed right) => Comparer<Speed>.Default.Compare(left, right) >= 0;

        [Pure] public static implicit operator Speed(int value) => new Speed(value);

        [Pure]
        public static Speed operator +(Speed lvalue, Speed rvalue)
        {
            var total = lvalue.MetersPerSecond + rvalue.MetersPerSecond;
            return new Speed(total, UnitType.MetersPerSecond);
        }

        [Pure]
        public static Speed operator -(Speed lvalue, Speed rvalue)
        {
            var total = lvalue.MetersPerSecond - rvalue.MetersPerSecond;
            return new Speed(total, UnitType.MetersPerSecond);
        }

        [Pure] public override string ToString() => _value.ToString();
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        // IComparable
        [Pure] public int CompareTo(object obj) => _value.CompareTo(obj);

        [Pure] public TypeCode GetTypeCode() => _value.GetTypeCode();
        [Pure] public bool ToBoolean(IFormatProvider provider) => ((IConvertible)_value).ToBoolean(provider);
        [Pure] public byte ToByte(IFormatProvider provider) => ((IConvertible)_value).ToByte(provider);
        [Pure] public char ToChar(IFormatProvider provider) => ((IConvertible)_value).ToChar(provider);
        [Pure] public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)_value).ToDateTime(provider);
        [Pure] public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)_value).ToDecimal(provider);
        [Pure] public double ToDouble(IFormatProvider provider) => _value;
        [Pure] public short ToInt16(IFormatProvider provider) => ((IConvertible)_value).ToInt16(provider);
        [Pure] public int ToInt32(IFormatProvider provider) => ((IConvertible)_value).ToInt32(provider);
        [Pure] public long ToInt64(IFormatProvider provider) => ((IConvertible)_value).ToInt64(provider);
        [Pure] public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)_value).ToSByte(provider);
        [Pure] public float ToSingle(IFormatProvider provider) => ((IConvertible)_value).ToSingle(provider);
        [Pure] public string ToString(IFormatProvider provider) => _value.ToString(provider);
        [Pure] public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)_value).ToType(conversionType, provider);
        [Pure] public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)_value).ToUInt16(provider);
        [Pure] public uint ToUInt32(IFormatProvider provider) => ((IConvertible)_value).ToUInt32(provider);
        [Pure] public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)_value).ToUInt64(provider);

        [Pure]
        public int CompareTo(double? other)
        {
            return (other is null) ? -1 : (_value).CompareTo(other.Value);
        }

        [Pure] public bool Equals(double? other) => _value.Equals(other);
        [Pure] public bool Equals(double other) => _value.Equals(other);
        [Pure] public int CompareTo(double other) => _value.CompareTo(other);
        // can't do this.
        //public int CompareTo(double? other) => Value.CompareTo(other);
    }
}