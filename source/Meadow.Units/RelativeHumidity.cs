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
        /// <param name="type">Relative humidity unit.</param>
        public RelativeHumidity(double value, UnitType type = UnitType.Percent)
        {
            Percent = value;
        }

        /// <summary>
        /// Creates a new `RelativeHumidity` object from an existing RelativeHumidity object
        /// </summary>
        /// <param name="relativeHumidity"></param>
        public RelativeHumidity(RelativeHumidity relativeHumidity)
        {
            Percent = relativeHumidity.Percent;
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
            /// <summary>
            /// Relative humidity as a percentage
            /// </summary>
            Percent
        }

        /// <summary>
        /// Compare to another RelativeHumidity object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((RelativeHumidity)obj);
        }

        /// <summary>
        /// Get hash of object
        /// </summary>
        /// <returns>int32 hash value</returns>
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
        /// <summary>
        /// Compare to another RelativeHumidity object
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public bool Equals(RelativeHumidity other) => Percent == other.Percent;

        /// <summary>
        /// Equals operator to compare two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(RelativeHumidity left, RelativeHumidity right) => Equals(left.Percent, right.Percent);

        /// <summary>
        /// Not equals operator to compare two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(RelativeHumidity left, RelativeHumidity right) => !Equals(left.Percent, right.Percent);

        /// <summary>
        /// Compare to another RelativeHumidity object
        /// </summary>
        /// <param name="other"></param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(RelativeHumidity other) => Equals(Percent, other.Percent) ? 0 : Percent.CompareTo(other.Percent);

        /// <summary>
        /// Less than operator to compare two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) < 0;

        /// <summary>
        /// Greater than operator to compare two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) > 0;

        /// <summary>
        /// Less than or equal operator to compare two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(RelativeHumidity left, RelativeHumidity right) => Comparer<double>.Default.Compare(left.Percent, right.Percent) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new RelativeHumidity object with a value of left + right</returns>
        [Pure] public static RelativeHumidity operator +(RelativeHumidity left, RelativeHumidity right) => new (left.Percent + right.Percent);

        /// <summary>
        /// Subtraction operator to subtract two RelativeHumidity objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new RelativeHumidity object with a value of left - right</returns>
        [Pure] public static RelativeHumidity operator -(RelativeHumidity left, RelativeHumidity right) => new (left.Percent - right.Percent);

        /// <summary>
        /// Multipication operator to multiply by a double
        /// </summary>
        /// <param name="value">object to multiply</param>
        /// <param name="operand">operand to multiply object</param>
        /// <returns>A new RelativeHumidity object with a value of value multiplied by the operand</returns>
        [Pure] public static RelativeHumidity operator *(RelativeHumidity value, double operand) => new (value.Percent * operand);

        /// <summary>
        /// Division operator to divide by a double
        /// </summary>
        /// <param name="value">object to be divided</param>
        /// <param name="operand">operand to divide object</param>
        /// <returns>A new RelativeHumidity object with a value of value divided by the operand</returns>
        [Pure] public static RelativeHumidity operator /(RelativeHumidity value, double operand) => new (value.Percent / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public RelativeHumidity Abs() { return new RelativeHumidity(Math.Abs(this.Percent)); }

        /// <summary>
        /// Get a string represention of the object
        /// </summary>
        /// <returns>A string representing the object</returns>
        [Pure] public override string ToString() => Percent.ToString();

        /// <summary>
        /// Get a string represention of the object
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="formatProvider">format provider</param>
        /// <returns>A string representing the object</returns>
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => Percent.ToString(format, formatProvider);

        // IComparable
        /// <summary>
        /// Compare to another RelativeHumidity object
        /// </summary>
        /// <param name="obj">The other RelativeHumidity cast to object</param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(object obj) => Percent.CompareTo(obj);

        /// <summary>
        /// Get type code of object
        /// </summary>
        /// <returns>The TypeCode</returns>
        [Pure] public TypeCode GetTypeCode() => Percent.GetTypeCode();

        /// <summary>
        /// Covert to boolean
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>bool representation of the object</returns>
        [Pure] public bool ToBoolean(IFormatProvider provider) => ((IConvertible)Percent).ToBoolean(provider);

        /// <summary>
        /// Covert to byte
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>byte representation of the object</returns>
        [Pure] public byte ToByte(IFormatProvider provider) => ((IConvertible)Percent).ToByte(provider);

        /// <summary>
        /// Covert to char
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>char representation of the object</returns>
        [Pure] public char ToChar(IFormatProvider provider) => ((IConvertible)Percent).ToChar(provider);

        /// <summary>
        /// Covert to DateTime
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>DateTime representation of the object</returns>
        [Pure] public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)Percent).ToDateTime(provider);

        /// <summary>
        /// Covert to Decimal
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>Decimal representation of the object</returns>
        [Pure] public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)Percent).ToDecimal(provider);

        /// <summary>
        /// Covert to double
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>double representation of the object</returns>
        [Pure] public double ToDouble(IFormatProvider provider) => Percent;

        /// <summary>
        /// Covert to in16
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>int16 representation of the object</returns>
        [Pure] public short ToInt16(IFormatProvider provider) => ((IConvertible)Percent).ToInt16(provider);

        /// <summary>
        /// Covert to int32
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>int32 representation of the object</returns>
        [Pure] public int ToInt32(IFormatProvider provider) => ((IConvertible)Percent).ToInt32(provider);

        /// <summary>
        /// Covert to int64
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>int64 representation of the object</returns>
        [Pure] public long ToInt64(IFormatProvider provider) => ((IConvertible)Percent).ToInt64(provider);

        /// <summary>
        /// Covert to sbyte
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>sbyte representation of the object</returns>
        [Pure] public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)Percent).ToSByte(provider);

        /// <summary>
        /// Covert to float
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>float representation of the object</returns>
        [Pure] public float ToSingle(IFormatProvider provider) => ((IConvertible)Percent).ToSingle(provider);

        /// <summary>
        /// Covert to string
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>string representation of the object</returns>
        [Pure] public string ToString(IFormatProvider provider) => Percent.ToString(provider);

        /// <summary>
        /// Covert to type
        /// </summary>
        /// <param name="conversionType">unit to convert</param>
        /// <param name="provider">format provider</param>
        /// <returns>type representation of the object</returns>
        [Pure] public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Percent).ToType(conversionType, provider);

        /// <summary>
        /// Covert to uint16
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>uint16 representation of the object</returns>
        [Pure] public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)Percent).ToUInt16(provider);

        /// <summary>
        /// Covert to uint32
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>uint32 representation of the object</returns>
        [Pure] public uint ToUInt32(IFormatProvider provider) => ((IConvertible)Percent).ToUInt32(provider);

        /// <summary>
        /// Covert to uint64
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>uint64 representation of the object</returns>
        [Pure] public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)Percent).ToUInt64(provider);

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(double? other)
        {
            return (other is null) ? -1 : (Percent).CompareTo(other.Value);
        }

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public bool Equals(double? other) => Percent.Equals(other);

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public bool Equals(double other) => Percent.Equals(other);

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(double other) => Percent.CompareTo(other);
    }
}