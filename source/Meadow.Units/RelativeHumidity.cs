using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents _relative humidity_; expressed as a percentage, indicates a
    /// present state of absolute humidity relative to a maximum humidity given
    /// the same temperature.
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct RelativeHumidity :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `RelativeHumidity` object.
        /// </summary>
        /// <param name="value">The relative humidity value.</param>
        /// <param name="type">Percent, by default.</param>
        public RelativeHumidity(double value, UnitType type = UnitType.Percent)
        {
            Percent = value;
        }

        public RelativeHumidity(RelativeHumidity relativeHumidity)
        {
            this.Percent = relativeHumidity.Percent;
        }

        /// <summary>
        /// The relative expressed as a value percent.
        /// </summary>
        public double Percent { get; private set; }
        /// <summary>
        /// The type of units available to describe the temperature.
        /// </summary>
        public enum UnitType
        {
            Percent
        }

        //=============================
        // Boilerplate interface stuff.

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((RelativeHumidity)obj);
        }

        [Pure] public override int GetHashCode() => Percent.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator RelativeHumidity(ushort value) => new RelativeHumidity(value);
        //[Pure] public static implicit operator RelativeHumidity(short value) => new RelativeHumidity(value);
        //[Pure] public static implicit operator RelativeHumidity(uint value) => new RelativeHumidity(value);
        //[Pure] public static implicit operator RelativeHumidity(long value) => new RelativeHumidity(value);
        //[Pure] public static implicit operator RelativeHumidity(int value) => new RelativeHumidity(value);
        //[Pure] public static implicit operator RelativeHumidity(float value) => new RelativeHumidity(value);
        //[Pure] public static implicit operator RelativeHumidity(double value) => new RelativeHumidity(value);
        //[Pure] public static implicit operator RelativeHumidity(decimal value) => new RelativeHumidity((double)value);

        // Comparison
        [Pure] public bool Equals(RelativeHumidity other) => Percent == other.Percent;
        [Pure] public static bool operator ==(RelativeHumidity left, RelativeHumidity right) => Equals(left.Percent, right.Percent);
        [Pure] public static bool operator !=(RelativeHumidity left, RelativeHumidity right) => !Equals(left.Percent, right.Percent);
        [Pure] public int CompareTo(RelativeHumidity other) => Equals(this.Percent, other.Percent) ? 0 : this.Percent.CompareTo(other.Percent);
        [Pure] public static bool operator <(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) < 0;
        [Pure] public static bool operator >(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) > 0;
        [Pure] public static bool operator <=(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) <= 0;
        [Pure] public static bool operator >=(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) >= 0;

        // Math
        [Pure] public static RelativeHumidity operator +(RelativeHumidity lvalue, RelativeHumidity rvalue) => new RelativeHumidity(lvalue.Percent + rvalue.Percent);
        [Pure] public static RelativeHumidity operator -(RelativeHumidity lvalue, RelativeHumidity rvalue) => new RelativeHumidity(lvalue.Percent - rvalue.Percent);
        [Pure] public static RelativeHumidity operator *(RelativeHumidity value, double operand) => new RelativeHumidity(value.Percent * operand);
        [Pure] public static RelativeHumidity operator /(RelativeHumidity value, double operand) => new RelativeHumidity(value.Percent / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public RelativeHumidity Abs() { return new RelativeHumidity(Math.Abs(this.Percent)); }

        // ToString()
        [Pure] public override string ToString() => Percent.ToString();
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => Percent.ToString(format, formatProvider);

        // IComparable
        [Pure] public int CompareTo(object obj) => Percent.CompareTo(obj);
        [Pure] public TypeCode GetTypeCode() => Percent.GetTypeCode();
        [Pure] public bool ToBoolean(IFormatProvider provider) => ((IConvertible)Percent).ToBoolean(provider);
        [Pure] public byte ToByte(IFormatProvider provider) => ((IConvertible)Percent).ToByte(provider);
        [Pure] public char ToChar(IFormatProvider provider) => ((IConvertible)Percent).ToChar(provider);
        [Pure] public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)Percent).ToDateTime(provider);
        [Pure] public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)Percent).ToDecimal(provider);
        [Pure] public double ToDouble(IFormatProvider provider) => Percent;
        [Pure] public short ToInt16(IFormatProvider provider) => ((IConvertible)Percent).ToInt16(provider);
        [Pure] public int ToInt32(IFormatProvider provider) => ((IConvertible)Percent).ToInt32(provider);
        [Pure] public long ToInt64(IFormatProvider provider) => ((IConvertible)Percent).ToInt64(provider);
        [Pure] public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)Percent).ToSByte(provider);
        [Pure] public float ToSingle(IFormatProvider provider) => ((IConvertible)Percent).ToSingle(provider);
        [Pure] public string ToString(IFormatProvider provider) => Percent.ToString(provider);
        [Pure] public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Percent).ToType(conversionType, provider);
        [Pure] public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)Percent).ToUInt16(provider);
        [Pure] public uint ToUInt32(IFormatProvider provider) => ((IConvertible)Percent).ToUInt32(provider);
        [Pure] public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)Percent).ToUInt64(provider);

        [Pure]
        public int CompareTo(double? other)
        {
            return (other is null) ? -1 : (Percent).CompareTo(other);
        }

        [Pure] public bool Equals(double? other) => Percent.Equals(other);
        [Pure] public bool Equals(double other) => Percent.Equals(other);
        [Pure] public int CompareTo(double other) => Percent.CompareTo(other);
    }
}
