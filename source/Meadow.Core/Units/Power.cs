using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Power
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Power : IUnitType, IComparable, IFormattable, IConvertible, IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Power` object.
        /// </summary>
        /// <param name="value">The Power value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Power(double value, UnitType type = UnitType.Watts)
        {
            //always store reference value
            Unit = type;
            _value = PowerConversions.Convert(value, type, UnitType.Watts);
        }

        /// <summary>
        /// The Power expressed as a value.
        /// </summary>
        public double Value
        {
            get => PowerConversions.Convert(_value, UnitType.Watts, Unit);
            set => _value = PowerConversions.Convert(value, Unit, UnitType.Watts);
        }

        private double _value;

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

        /// <summary>
        /// The type of units available to describe the Power.
        /// </summary>
        public enum UnitType
        {
            Gigawatts,
            Megawatts,
            Kilowatts,
            Watts,
            Milliwatts,
            HorsePowerMetric,
            HorsePowerIT,
            CaloriesPerSecond,
            CaloriesPerMinute,
            CaloriesPerHour,
            BTUsPerSecond,
            BTUsPerMinute,
            BTUsPerHour,
            FootPoundsPerSecond,
            FootPoundsPerMinute,
            FootPoundsPerHour,
            TonsRefridgeration
        }

        public double Gigawatts => From(UnitType.Gigawatts);
        public double Megawatts => From(UnitType.Megawatts);
        public double Kilowatts => From(UnitType.Kilowatts);
        public double Watts => From(UnitType.Watts);
        public double Milliwatts => From(UnitType.Milliwatts);
        public double HorsePowerMetric => From(UnitType.HorsePowerMetric);
        public double HorsePowerIT => From(UnitType.HorsePowerIT);
        public double CaloriesPerSecond => From(UnitType.CaloriesPerSecond);
        public double CaloriesPerMinute => From(UnitType.CaloriesPerMinute);
        public double CaloriesPerHour => From(UnitType.CaloriesPerHour);
        public double BTUsPerSecond => From(UnitType.BTUsPerSecond);
        public double BTUsPerMinute => From(UnitType.BTUsPerMinute);
        public double BTUsPerHour => From(UnitType.BTUsPerHour);
        public double FootPoundsPerSecond => From(UnitType.FootPoundsPerSecond);
        public double FootPoundsPerMinute => From(UnitType.FootPoundsPerMinute);
        public double FootPoundsPerHour => From(UnitType.FootPoundsPerHour);
        public double TonsRefridgeration => From(UnitType.TonsRefridgeration);


        [Pure]
        public double From(UnitType convertTo)
        {
            return PowerConversions.Convert(_value, UnitType.Watts, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Power)obj);
        }

        [Pure] public bool Equals(Power other) => _value == other._value;

        [Pure] public override int GetHashCode() => _value.GetHashCode();

        [Pure] public static bool operator ==(Power left, Power right) => Equals(left, right);
        [Pure] public static bool operator !=(Power left, Power right) => !Equals(left, right);
        [Pure] public int CompareTo(Power other) => Equals(this, other) ? 0 : _value.CompareTo(other._value);
        [Pure] public static bool operator <(Power left, Power right) => Comparer<Power>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Power left, Power right) => Comparer<Power>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Power left, Power right) => Comparer<Power>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Power left, Power right) => Comparer<Power>.Default.Compare(left, right) >= 0;

        [Pure] public static implicit operator Power(int value) => new Power(value);

        [Pure]
        public static Power operator +(Power lvalue, Power rvalue)
        {
            var total = lvalue.Watts + rvalue.Watts;
            return new Power(total, UnitType.Watts);
        }

        [Pure]
        public static Power operator -(Power lvalue, Power rvalue)
        {
            var total = lvalue.Watts - rvalue.Watts;
            return new Power(total, UnitType.Watts);
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