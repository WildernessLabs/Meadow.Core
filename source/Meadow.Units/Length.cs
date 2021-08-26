using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Length
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Length :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Length` object.
        /// </summary>
        /// <param name="value">The Length value.</param>
        /// <param name="type">Meters by default.</param>
        public Length(double value, UnitType type = UnitType.Meters)
        {
            Value = LengthConversions.Convert(value, type, UnitType.Meters);
        }

        public Length(Length length)
        {
            this.Value = length.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the Length.
        /// </summary>
        public enum UnitType
        {
            Kilometers,
            Meters,
            Centimeters,
            Decimeters,
            Millimeters,
            Microns,
            Nanometers,
            Miles,
            NauticalMiles,
            Yards,
            Feet,
            Inches,
        }

        //==== Typed "To" methods
        public double Kilometers => From(UnitType.Kilometers);
        public double Meters => From(UnitType.Meters);
        public double Centimeters => From(UnitType.Centimeters);
        public double Decimeters => From(UnitType.Decimeters);
        public double Millimeters => From(UnitType.Millimeters);
        public double Microns => From(UnitType.Microns);
        public double Nanometer => From(UnitType.Nanometers);
        public double Miles => From(UnitType.Miles);
        public double NauticalMiles => From(UnitType.NauticalMiles);
        public double Yards => From(UnitType.Yards);
        public double Feet => From(UnitType.Feet);
        public double Inches => From(UnitType.Inches);

        [Pure]
        public double From(UnitType convertTo)
        {
            return LengthConversions.Convert(Value, UnitType.Meters, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Length)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Length(ushort value) => new Length(value);
        //[Pure] public static implicit operator Length(short value) => new Length(value);
        //[Pure] public static implicit operator Length(uint value) => new Length(value);
        //[Pure] public static implicit operator Length(long value) => new Length(value);
        //[Pure] public static implicit operator Length(int value) => new Length(value);
        //[Pure] public static implicit operator Length(float value) => new Length(value);
        //[Pure] public static implicit operator Length(double value) => new Length(value);
        //[Pure] public static implicit operator Length(decimal value) => new Length((double)value);

        // Comparison
        [Pure] public bool Equals(Length other) => Value == other.Value;
        [Pure] public static bool operator ==(Length left, Length right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Length left, Length right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Length other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Length left, Length right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Length left, Length right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Length left, Length right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Length left, Length right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Length operator +(Length lvalue, Length rvalue) => new Length(lvalue.Value + rvalue.Value);
        [Pure] public static Length operator -(Length lvalue, Length rvalue) => new Length(lvalue.Value - rvalue.Value);
        [Pure] public static Length operator *(Length value, double operand) => new Length(value.Value * operand);
        [Pure] public static Length operator /(Length value, double operand) => new Length(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Length Abs() { return new Length(Math.Abs(this.Value)); }

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