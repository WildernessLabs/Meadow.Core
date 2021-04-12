using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Volume
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class Volume : IUnitType, IComparable, IFormattable, IConvertible, IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Volume` object.
        /// </summary>
        /// <param name="value">The Volume value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Volume(double value, UnitType type = UnitType.Liters)
        {
            //always store reference value
            Unit = type;
            _value = VolumeConversions.Convert(value, type, UnitType.Liters);
        }

        /// <summary>
        /// The Volume expressed as a value.
        /// </summary>
        public double Value
        {
            get => VolumeConversions.Convert(_value, UnitType.Liters, Unit);
            set => _value = VolumeConversions.Convert(value, Unit, UnitType.Liters);
        }

        private double _value;

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

        /// <summary>
        /// The type of units available to describe the Volume.
        /// </summary>
        public enum UnitType
        {
            Gallons,
            Ounces,
            CubicFeet,
            CubicInches,
            Liters,
            Centiliters,
            Milliliters,
            CubicMeters,
        }

        public double Gallons => From(UnitType.Gallons);
        public double Ounces => From(UnitType.Ounces);
        public double CubicFeet => From(UnitType.CubicFeet);
        public double CubicInches => From(UnitType.CubicInches);
        public double Liters => From(UnitType.Liters);
        public double Centiliters => From(UnitType.Centiliters);
        public double Milliliters => From(UnitType.Milliliters);
        public double CubicMeters => From(UnitType.CubicMeters);

        [Pure]
        public double From(UnitType convertTo)
        {
            return VolumeConversions.Convert(_value, UnitType.Liters, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Volume)obj);
        }

        [Pure] public bool Equals(Volume other) => _value == other._value;

        [Pure] public override int GetHashCode() => _value.GetHashCode();

        [Pure] public static bool operator ==(Volume left, Volume right) => Equals(left, right);
        [Pure] public static bool operator !=(Volume left, Volume right) => !Equals(left, right);
        [Pure] public int CompareTo(Volume other) => Equals(this, other) ? 0 : _value.CompareTo(other._value);
        [Pure] public static bool operator <(Volume left, Volume right) => Comparer<Volume>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Volume left, Volume right) => Comparer<Volume>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Volume left, Volume right) => Comparer<Volume>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Volume left, Volume right) => Comparer<Volume>.Default.Compare(left, right) >= 0;

        [Pure] public static implicit operator Volume(int value) => new Volume(value);

        [Pure]
        public static Volume operator +(Volume lvalue, Volume rvalue)
        {
            var total = lvalue.Liters + rvalue.Liters;
            return new Volume(total, UnitType.Liters);
        }

        [Pure]
        public static Volume operator -(Volume lvalue, Volume rvalue)
        {
            var total = lvalue.Liters - rvalue.Liters;
            return new Volume(total, UnitType.Liters);
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