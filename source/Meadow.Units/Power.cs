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
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Power :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Power` object.
        /// </summary>
        /// <param name="value">The Power value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Power(double value, UnitType type = UnitType.Watts)
        {
            Value = PowerConversions.Convert(value, type, UnitType.Watts);
        }

        public Power(Power power)
        {
            this.Value = power.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

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
            return PowerConversions.Convert(Value, UnitType.Watts, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Power)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Power(ushort value) => new Power(value);
        //[Pure] public static implicit operator Power(short value) => new Power(value);
        //[Pure] public static implicit operator Power(uint value) => new Power(value);
        //[Pure] public static implicit operator Power(long value) => new Power(value);
        //[Pure] public static implicit operator Power(int value) => new Power(value);
        //[Pure] public static implicit operator Power(float value) => new Power(value);
        //[Pure] public static implicit operator Power(double value) => new Power(value);
        //[Pure] public static implicit operator Power(decimal value) => new Power((double)value);

        // Comparison
        [Pure] public bool Equals(Power other) => Value == other.Value;
        [Pure] public static bool operator ==(Power left, Power right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Power left, Power right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Power other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Power operator +(Power lvalue, Power rvalue) => new Power(lvalue.Value + rvalue.Value);
        [Pure] public static Power operator -(Power lvalue, Power rvalue) => new Power(lvalue.Value - rvalue.Value);
        [Pure] public static Power operator *(Power value, double operand) => new Power(value.Value * operand);
        [Pure] public static Power operator /(Power value, double operand) => new Power(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Power Abs() { return new Power(Math.Abs(this.Value)); }

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