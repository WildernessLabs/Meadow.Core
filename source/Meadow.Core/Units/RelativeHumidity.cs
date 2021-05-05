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
    [ImmutableObject(false)]
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public struct RelativeHumidity : IUnitType, IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `RelativeHumidity` object.
        /// </summary>
        /// <param name="value">The relative humidity value.</param>
        /// <param name="type">Percent, by default.</param>
        public RelativeHumidity(double value, UnitType type = UnitType.Percent)
        {
            Value = value; Unit = type;
        }

        /// <summary>
        /// The relative expressed as a value percent.
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

        [Pure] public bool Equals(RelativeHumidity other) => Value == other.Value;

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        [Pure] public static bool operator ==(RelativeHumidity left, RelativeHumidity right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(RelativeHumidity left, RelativeHumidity right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(RelativeHumidity other) => Equals(this, other) ? 0 : Value.CompareTo(other.Value);
        [Pure] public static bool operator <(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        [Pure] public static implicit operator RelativeHumidity(int value) => new RelativeHumidity(value);

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
