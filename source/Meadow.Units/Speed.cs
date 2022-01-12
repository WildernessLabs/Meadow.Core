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

        /// <summary>
        /// Creates a new `Speed` object from an existing Speed object
        /// </summary>
        /// <param name="speed"></param>
        public Speed(Speed speed)
        {
            Value = speed.Value;
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
            /// <summary>
            /// Feet per minute
            /// </summary>
            FeetPerMinute,
            /// <summary>
            /// Feet per second
            /// </summary>
            FeetPerSecond,
            /// <summary>
            /// Kilometers per hour
            /// </summary>
            KilometersPerHour,
            /// <summary>
            /// Kilometers per minute
            /// </summary>
            KilometersPerMinute,
            /// <summary>
            /// Kilometers per second
            /// </summary>
            KilometersPerSecond,
            /// <summary>
            /// Knots
            /// </summary>
            Knots,
            /// <summary>
            /// Meters per minute
            /// </summary>
            MetersPerMinute,
            /// <summary>
            /// Meters per second
            /// </summary>
            MetersPerSecond,
            /// <summary>
            /// Miles per hour
            /// </summary>
            MilesPerHour,
            /// <summary>
            /// Miles per minute
            /// </summary>
            MilesPerMinute,
            /// <summary>
            /// Miles per second
            /// </summary>
            MilesPerSecond,
            /// <summary>
            /// Speed of light
            /// </summary>
            SpeedOfLight,
            /// <summary>
            /// Mach 
            /// </summary>
            Mach,
        }

        /// <summary>
        /// Get speed in feet per second
        /// </summary>
        public double FeetPerSecond => From(UnitType.FeetPerSecond);
        /// <summary>
        /// Get speed in feet per minute
        /// </summary>
        public double FeetPerMinute => From(UnitType.FeetPerMinute);
        /// <summary>
        /// Get speed in kilometers per hour
        /// </summary>
        public double KilometersPerHour => From(UnitType.KilometersPerHour);
        /// <summary>
        /// Get speed in kilometers per minute
        /// </summary>
        public double KilometersPerMinute => From(UnitType.KilometersPerMinute);
        /// <summary>
        /// Get speed in kilometers per second
        /// </summary>
        public double KilometersPerSecond => From(UnitType.KilometersPerSecond);
        /// <summary>
        /// Get speed in knots
        /// </summary>
        public double Knots => From(UnitType.Knots);
        /// <summary>
        /// Get speed in meters per minute
        /// </summary>
        public double MetersPerMinute => From(UnitType.MetersPerMinute);
        /// <summary>
        /// Get speed in meters per second
        /// </summary>
        public double MetersPerSecond => From(UnitType.MetersPerSecond);
        /// <summary>
        /// Get speed in miles per hour
        /// </summary>
        public double MilesPerHour => From(UnitType.MilesPerHour);
        /// <summary>
        /// Get speed in miles per minute 
        /// </summary>
        public double MilesPerMinute => From(UnitType.MilesPerMinute);
        /// <summary>
        /// Get speed in miles per second
        /// </summary>
        public double MilesPerSecond => From(UnitType.MilesPerSecond);
        /// <summary>
        /// Get speed as a multiple of the speed of light - 299792458m/s
        /// </summary>
        public double SpeedOfLight => From(UnitType.SpeedOfLight);
        /// <summary>
        /// Get speed as a multiple of mach
        /// </summary>
        public double Mach => From(UnitType.Mach);

        /// <summary>
        /// Get a double value for a specific unit
        /// </summary>
        /// <param name="convertTo">unit to covert to</param>
        /// <returns>the converted value</returns>
        [Pure] public double From(UnitType convertTo)
        {
            return SpeedConversions.Convert(Value, UnitType.KilometersPerSecond, convertTo);
        }

        /// <summary>
        /// Compare to another Speed object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Speed)obj);
        }

        /// <summary>
        /// Get hash of object
        /// </summary>
        /// <returns>int32 hash value</returns>
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
        /// <summary>
        /// Compare to another Speed object
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public bool Equals(Speed other) => Value == other.Value;

        /// <summary>
        /// Equals operator to compare two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(Speed left, Speed right) => Equals(left.Value, right.Value);

        /// <summary>
        /// Not equals operator to compare two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(Speed left, Speed right) => !Equals(left.Value, right.Value);

        /// <summary>
        /// Compare to another Speed object
        /// </summary>
        /// <param name="other"></param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(Speed other) => Equals(Value, other.Value) ? 0 : Value.CompareTo(other.Value);

        /// <summary>
        /// Less than operator to compare two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;

        /// <summary>
        /// Greater than operator to compare two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;

        /// <summary>
        /// Less than or equal operator to compare two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(Speed left, Speed right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Speed object with a value of left + right</returns>
        [Pure] public static Speed operator +(Speed left, Speed right) => new (left.Value + right.Value);

        /// <summary>
        /// Subtraction operator to subtract two Speed objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Speed object with a value of left - right</returns>
        [Pure] public static Speed operator -(Speed left, Speed right) => new (left.Value - right.Value);

        /// <summary>
        /// Multipication operator to multiply by a double
        /// </summary>
        /// <param name="value">object to multiply</param>
        /// <param name="operand">operand to multiply object</param>
        /// <returns>A new Speed object with a value of value multiplied by the operand</returns>
        [Pure] public static Speed operator *(Speed value, double operand) => new (value.Value * operand);

        /// <summary>
        /// Division operator to divide by a double
        /// </summary>
        /// <param name="value">object to be divided</param>
        /// <param name="operand">operand to divide object</param>
        /// <returns>A new Speed object with a value of value divided by the operand</returns>
        [Pure] public static Speed operator /(Speed value, double operand) => new (value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Speed Abs() { return new Speed(Math.Abs(this.Value)); }

        /// <summary>
        /// Get a string represention of the object
        /// </summary>
        /// <returns>A string representing the object</returns>
        [Pure] public override string ToString() => Value.ToString();

        /// <summary>
        /// Get a string represention of the object
        /// </summary>
        /// <param name="format">format</param>
        /// <param name="formatProvider">format provider</param>
        /// <returns>A string representing the object</returns>
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => Value.ToString(format, formatProvider);

        // IComparable
        /// <summary>
        /// Compare to another Speed object
        /// </summary>
        /// <param name="obj">The other Speed cast to object</param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(object obj) => Value.CompareTo(obj);

        /// <summary>
        /// Get type code of object
        /// </summary>
        /// <returns>The TypeCode</returns>
        [Pure] public TypeCode GetTypeCode() => Value.GetTypeCode();

        /// <summary>
        /// Covert to boolean
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>bool representation of the object</returns>
        [Pure] public bool ToBoolean(IFormatProvider provider) => ((IConvertible)Value).ToBoolean(provider);

        /// <summary>
        /// Covert to byte
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>byte representation of the object</returns>
        [Pure] public byte ToByte(IFormatProvider provider) => ((IConvertible)Value).ToByte(provider);

        /// <summary>
        /// Covert to char
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>char representation of the object</returns>
        [Pure] public char ToChar(IFormatProvider provider) => ((IConvertible)Value).ToChar(provider);

        /// <summary>
        /// Covert to DateTime
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>DateTime representation of the object</returns>
        [Pure] public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)Value).ToDateTime(provider);

        /// <summary>
        /// Covert to Decimal
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>Decimal representation of the object</returns>
        [Pure] public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)Value).ToDecimal(provider);

        /// <summary>
        /// Covert to double
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>double representation of the object</returns>
        [Pure] public double ToDouble(IFormatProvider provider) => Value;

        /// <summary>
        /// Covert to in16
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>int16 representation of the object</returns>
        [Pure] public short ToInt16(IFormatProvider provider) => ((IConvertible)Value).ToInt16(provider);

        /// <summary>
        /// Covert to int32
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>int32 representation of the object</returns>
        [Pure] public int ToInt32(IFormatProvider provider) => ((IConvertible)Value).ToInt32(provider);

        /// <summary>
        /// Covert to int64
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>int64 representation of the object</returns>
        [Pure] public long ToInt64(IFormatProvider provider) => ((IConvertible)Value).ToInt64(provider);

        /// <summary>
        /// Covert to sbyte
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>sbyte representation of the object</returns>
        [Pure] public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)Value).ToSByte(provider);

        /// <summary>
        /// Covert to float
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>float representation of the object</returns>
        [Pure] public float ToSingle(IFormatProvider provider) => ((IConvertible)Value).ToSingle(provider);

        /// <summary>
        /// Covert to string
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>string representation of the object</returns>
        [Pure] public string ToString(IFormatProvider provider) => Value.ToString(provider);

        /// <summary>
        /// Covert to type
        /// </summary>
        /// <param name="conversionType">conversion unit type</param>
        /// <param name="provider">format provider</param>
        /// <returns>type representation of the object</returns>
        [Pure] public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)Value).ToType(conversionType, provider);

        /// <summary>
        /// Covert to uint16
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>uint16 representation of the object</returns>
        [Pure] public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)Value).ToUInt16(provider);

        /// <summary>
        /// Covert to uint32
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>uint32 representation of the object</returns>
        [Pure] public uint ToUInt32(IFormatProvider provider) => ((IConvertible)Value).ToUInt32(provider);

        /// <summary>
        /// Covert to uint64
        /// </summary>
        /// <param name="provider">format provider</param>
        /// <returns>uint64 representation of the object</returns>
        [Pure] public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)Value).ToUInt64(provider);

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(double? other)
        {
            return (other is null) ? -1 : (Value).CompareTo(other.Value);
        }

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public bool Equals(double? other) => Value.Equals(other);

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public bool Equals(double other) => Value.Equals(other);

        /// <summary>
        /// Compare the default value to a double 
        /// </summary>
        /// <param name="other">value to compare</param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(double other) => Value.CompareTo(other);
    }
}