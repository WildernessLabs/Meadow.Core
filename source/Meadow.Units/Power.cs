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

        /// <summary>
        /// Creates a new `Power` object from an existing Power object
        /// </summary>
        /// <param name="power"></param>
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
            /// <summary>
            /// Gigawatts
            /// </summary>
            Gigawatts,
            /// <summary>
            /// Megawatts
            /// </summary>
            Megawatts,
            /// <summary>
            /// Kilowatts
            /// </summary>
            Kilowatts,
            /// <summary>
            /// Watts
            /// </summary>
            Watts,
            /// <summary>
            /// Milliwatts
            /// </summary>
            Milliwatts,
            /// <summary>
            /// Horsepower - metric
            /// </summary>
            HorsePowerMetric,
            /// <summary>
            /// Horsepower - IT
            /// </summary>
            HorsePowerIT,
            /// <summary>
            /// Calories per second
            /// </summary>
            CaloriesPerSecond,
            /// <summary>
            /// Calories per minute
            /// </summary>
            CaloriesPerMinute,
            /// <summary>
            /// Calories per hour
            /// </summary>
            CaloriesPerHour,
            /// <summary>
            /// BTUs per second
            /// </summary>
            BTUsPerSecond,
            /// <summary>
            /// BTUs per minute
            /// </summary>
            BTUsPerMinute,
            /// <summary>
            /// BTUs per hour
            /// </summary>
            BTUsPerHour,
            /// <summary>
            /// Footpounds per second
            /// </summary>
            FootPoundsPerSecond,
            /// <summary>
            /// Footpounds per minute
            /// </summary>
            FootPoundsPerMinute,
            /// <summary>
            /// Footpounds per hour
            /// </summary>
            FootPoundsPerHour,
            /// <summary>
            /// Tons refridgeration
            /// </summary>
            TonsRefridgeration
        }

        /// <summary>
        /// Power in gigawatts
        /// </summary>
        public double Gigawatts => From(UnitType.Gigawatts);
        /// <summary>
        /// Power in megawatts
        /// </summary>
        public double Megawatts => From(UnitType.Megawatts);
        /// <summary>
        /// Power in kilowatts
        /// </summary>
        public double Kilowatts => From(UnitType.Kilowatts);
        /// <summary>
        /// Power in watts
        /// </summary>
        public double Watts => From(UnitType.Watts);
        /// <summary>
        /// Power in milliwatts
        /// </summary>
        public double Milliwatts => From(UnitType.Milliwatts);
        /// <summary>
        /// Power in horsepower metric
        /// </summary>
        public double HorsePowerMetric => From(UnitType.HorsePowerMetric);
        /// <summary>
        /// Power in horsepower IT
        /// </summary>
        public double HorsePowerIT => From(UnitType.HorsePowerIT);
        /// <summary>
        /// Power in calories per second
        /// </summary>
        public double CaloriesPerSecond => From(UnitType.CaloriesPerSecond);
        /// <summary>
        /// Power in calories per minute
        /// </summary>
        public double CaloriesPerMinute => From(UnitType.CaloriesPerMinute);
        /// <summary>
        /// Power in calories per hour
        /// </summary>
        public double CaloriesPerHour => From(UnitType.CaloriesPerHour);
        /// <summary>
        /// Power in BTUs per second
        /// </summary>
        public double BTUsPerSecond => From(UnitType.BTUsPerSecond);
        /// <summary>
        /// Power in BTUs per minute
        /// </summary>
        public double BTUsPerMinute => From(UnitType.BTUsPerMinute);
        /// <summary>
        /// Power in BTUs per hour
        /// </summary>
        public double BTUsPerHour => From(UnitType.BTUsPerHour);
        /// <summary>
        /// Power in footpounds per second
        /// </summary>
        public double FootPoundsPerSecond => From(UnitType.FootPoundsPerSecond);
        /// <summary>
        /// Power in footpounds per minute
        /// </summary>
        public double FootPoundsPerMinute => From(UnitType.FootPoundsPerMinute);
        /// <summary>
        /// Power in footpounds per hour
        /// </summary>
        public double FootPoundsPerHour => From(UnitType.FootPoundsPerHour);
        /// <summary>
        /// Power in tons refridgeration
        /// </summary>
        public double TonsRefridgeration => From(UnitType.TonsRefridgeration);

        /// <summary>
        /// Get a double value for a specific unit
        /// </summary>
        /// <param name="convertTo">unit to covert to</param>
        /// <returns>the converted value</returns>
        [Pure] public double From(UnitType convertTo)
        {
            return PowerConversions.Convert(Value, UnitType.Watts, convertTo);
        }

        /// <summary>
        /// Compare to another Power object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Power)obj);
        }

        /// <summary>
        /// Get hash of object
        /// </summary>
        /// <returns>int32 hash value</returns>
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
        /// <summary>
        /// Compare to another Power object
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public bool Equals(Power other) => Value == other.Value;

        /// <summary>
        /// Equals operator to compare two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(Power left, Power right) => Equals(left.Value, right.Value);

        /// <summary>
        /// Not equals operator to compare two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(Power left, Power right) => !Equals(left.Value, right.Value);

        /// <summary>
        /// Compare to another Power object
        /// </summary>
        /// <param name="other"></param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(Power other) => Equals(Value, other.Value) ? 0 : Value.CompareTo(other.Value);

        /// <summary>
        /// Less than operator to compare two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;

        /// <summary>
        /// Greater than operator to compare two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;

        /// <summary>
        /// Less than or equal operator to compare two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(Power left, Power right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Power object with a value of left + right</returns>
        [Pure] public static Power operator +(Power left, Power right) => new (left.Value + right.Value);

        /// <summary>
        /// Subtraction operator to subtract two Power objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Power object with a value of left - right</returns>
        [Pure] public static Power operator -(Power left, Power right) => new (left.Value - right.Value);

        /// <summary>
        /// Multipication operator to multiply by a double
        /// </summary>
        /// <param name="value">object to multiply</param>
        /// <param name="operand">operand to multiply object</param>
        /// <returns>A new Power object with a value of value multiplied by the operand</returns>
        [Pure] public static Power operator *(Power value, double operand) => new (value.Value * operand);

        /// <summary>
        /// Division operator to divide by a double
        /// </summary>
        /// <param name="value">object to be divided</param>
        /// <param name="operand">operand to divide object</param>
        /// <returns>A new Power object with a value of value divided by the operand</returns>
        [Pure] public static Power operator /(Power value, double operand) => new (value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Power Abs() { return new Power(Math.Abs(this.Value)); }

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
        /// Compare to another Power object
        /// </summary>
        /// <param name="obj">The other Power cast to object</param>
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
        /// <param name="conversionType">conversion type</param>
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