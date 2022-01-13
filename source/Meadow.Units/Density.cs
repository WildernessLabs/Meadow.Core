using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Density
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Density :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Density` object.
        /// </summary>
        /// <param name="value">The Density value.</param>
        /// <param name="type">Kilograms per meters cubed by default.</param>
        public Density(double value, UnitType type = UnitType.KilogramsPerMetersCubed)
        {
            Value = DensityConversions.Convert(value, type, UnitType.KilogramsPerMetersCubed);
        }

        /// <summary>
        /// Creates a new `Density` object from an existing Density object
        /// </summary>
        /// <param name="density"></param>
		public Density(Density density)
        {
            Value = density.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the Density.
        /// </summary>
        public enum UnitType
        {
            /// <summary>
            /// Grams per centimeters cubed
            /// </summary>
            GramsPerCentimetersCubed,
            /// <summary>
            /// Grams per meters cubed
            /// </summary>
            GramsPerMetersCubed,
            /// <summary>
            /// Grams per liter
            /// </summary>
            GramsPerLiter,
            /// <summary>
            /// Kilograms per meters cubed
            /// </summary>
            KilogramsPerMetersCubed,
            /// <summary>
            /// Ounces per inches cubed
            /// </summary>
            OuncesPerInchesCubed,
            /// <summary>
            /// Ounces per feet cubed
            /// </summary>
            OuncesPerFeetCubed,
            /// <summary>
            /// Pounds per inches cubed
            /// </summary>
            PoundsPerInchesCubed,
            /// <summary>
            /// Pounds per feet cubed
            /// </summary>
            PoundsPerFeetCubed,
            /// <summary>
            /// Density of water
            /// </summary>
            Water
        }

        /// <summary>
        /// Get the density in grams per centimeters cubed
        /// </summary>
        public double GramsPerCentimetersCubed => From(UnitType.GramsPerCentimetersCubed);
        /// <summary>
        /// Get the density in grams per meters cubed
        /// </summary>
        public double GramsPerMetersCubed => From(UnitType.GramsPerMetersCubed);
        /// <summary>
        /// Get the density in grams per liter cubed
        /// </summary>
        public double GramsPerLiter => From(UnitType.GramsPerLiter);
        /// <summary>
        /// Get the density in kilograms per meters cubed
        /// </summary>
        public double KilogramsPerMetersCubed => From(UnitType.KilogramsPerMetersCubed);
        /// <summary>
        /// Get the density in ounces per inches cubed
        /// </summary>
        public double OuncesPerInchesCubed => From(UnitType.OuncesPerInchesCubed);
        /// <summary>
        /// Get the density in ounces per feet cubed
        /// </summary>
        public double OuncesPerFeetCubed => From(UnitType.OuncesPerFeetCubed);
        /// <summary>
        /// Get the density in pounds per inches cubed
        /// </summary>
        public double PoundsPerInchesCubed => From(UnitType.PoundsPerInchesCubed);
        /// <summary>
        /// Get the density in pounds per feet cubed
        /// </summary>
        public double PoundsPerFeetCubed => From(UnitType.PoundsPerFeetCubed);
        /// <summary>
        /// Get the density relative to water
        /// </summary>
        public double Water => From(UnitType.Water);
		
        /// <summary>
        /// Get a double value for a specific unit
        /// </summary>
        /// <param name="convertTo">unit to covert to</param>
        /// <returns>the converted value</returns>
        [Pure] public double From(UnitType convertTo)
        {
            return DensityConversions.Convert(Value, UnitType.KilogramsPerMetersCubed, convertTo);
        }

        /// <summary>
        /// Compare to another Density object
        /// </summary>
        /// <param name="obj">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public override bool Equals(object obj)
        {
            if (obj is null) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Density)obj);
        }

        /// <summary>
        /// Get hash of object
        /// </summary>
        /// <returns>int32 hash value</returns>
        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Density(ushort value) => new Density(value);
        //[Pure] public static implicit operator Density(short value) => new Density(value);
        //[Pure] public static implicit operator Density(uint value) => new Density(value);
        //[Pure] public static implicit operator Density(long value) => new Density(value);
        //[Pure] public static implicit operator Density(int value) => new Density(value);
        //[Pure] public static implicit operator Density(float value) => new Density(value);
        //[Pure] public static implicit operator Density(double value) => new Density(value);
        //[Pure] public static implicit operator Density(decimal value) => new Density((double)value);

        // Comparison
        /// <summary>
        /// Compare to another Density object
        /// </summary>
        /// <param name="other">The object to compare</param>
        /// <returns>true if equal</returns>
        [Pure] public bool Equals(Density other) => Value == other.Value;

        /// <summary>
        /// Equals operator to compare two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if equal</returns>
        [Pure] public static bool operator ==(Density left, Density right) => Equals(left.Value, right.Value);

        /// <summary>
        /// Not equals operator to compare two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if not equal</returns>
        [Pure] public static bool operator !=(Density left, Density right) => !Equals(left.Value, right.Value);

        /// <summary>
        /// Compare to another Density object
        /// </summary>
        /// <param name="other"></param>
        /// <returns>0 if equal</returns>
        [Pure] public int CompareTo(Density other) => Equals(Value, other.Value) ? 0 : Value.CompareTo(other.Value);

        /// <summary>
        /// Less than operator to compare two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than right</returns>
        [Pure] public static bool operator <(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;

        /// <summary>
        /// Greater than operator to compare two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than right</returns>
        [Pure] public static bool operator >(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;

        /// <summary>
        /// Less than or equal operator to compare two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is less than or equal to right</returns>
        [Pure] public static bool operator <=(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;

        /// <summary>
        /// Greater than or equal operator to compare two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>true if left is greater than or equal to right</returns>
        [Pure] public static bool operator >=(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        /// <summary>
        /// Addition operator to add two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Density object with a value of left + right</returns>
        [Pure] public static Density operator +(Density left, Density right) => new (left.Value + right.Value);

        /// <summary>
        /// Subtraction operator to subtract two Density objects
        /// </summary>
        /// <param name="left">left value</param>
        /// <param name="right">right value</param>
        /// <returns>A new Density object with a value of left - right</returns>
        [Pure] public static Density operator -(Density left, Density right) => new (left.Value - right.Value);

        /// <summary>
        /// Multipication operator to multiply by a double
        /// </summary>
        /// <param name="value">object to multiply</param>
        /// <param name="operand">operand to multiply object</param>
        /// <returns>A new Density object with a value of value multiplied by the operand</returns>
        [Pure] public static Density operator *(Density value, double operand) => new (value.Value * operand);

        /// <summary>
        /// Division operator to divide by a double
        /// </summary>
        /// <param name="value">object to be divided</param>
        /// <param name="operand">operand to divide object</param>
        /// <returns>A new Density object with a value of value divided by the operand</returns>
        [Pure] public static Density operator /(Density value, double operand) => new (value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Density Abs() { return new Density(Math.Abs(this.Value)); }

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
        /// Compare to another Density object
        /// </summary>
        /// <param name="obj">The other Density cast to object</param>
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
        /// <param name="conversionType">type to convert to</param>
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