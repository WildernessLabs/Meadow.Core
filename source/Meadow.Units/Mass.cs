using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents mass, or weight of an object
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Mass :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Mass` object.
        /// </summary>
        /// <param name="value">The mass value.</param>
        /// <param name="type">Grams by default.</param>
        public Mass(double value, UnitType type = UnitType.Grams)
        {
            Value = MassConversions.Convert(value, type, UnitType.Grams);
        }

        public Mass(Mass mass)
        {
            this.Value = mass.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the temperature.
        /// </summary>
        public enum UnitType
        {
            Grams,
            Kilograms,
            Ounces,
            Pounds,
            TonsMetric,
            TonsUSShort,
            TonsUKLong,
            Grains,
            Carats
        }

        public double Grams => From(UnitType.Grams);
        public double Kilograms => From(UnitType.Kilograms);
        public double Ounces => From(UnitType.Ounces);
        public double Pounds => From(UnitType.Pounds);
        public double TonsMetric => From(UnitType.TonsMetric);
        public double TonsUSShort => From(UnitType.TonsUSShort);
        public double TonsUKLong => From(UnitType.TonsUKLong);
        public double Grains => From(UnitType.Grains);
        public double Karats => From(UnitType.Carats);

        [Pure]
        public double From(UnitType convertTo)
        {
            return MassConversions.Convert(Value, UnitType.Grams, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Mass)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Mass(ushort value) => new Mass(value);
        //[Pure] public static implicit operator Mass(short value) => new Mass(value);
        //[Pure] public static implicit operator Mass(uint value) => new Mass(value);
        //[Pure] public static implicit operator Mass(long value) => new Mass(value);
        //[Pure] public static implicit operator Mass(int value) => new Mass(value);
        //[Pure] public static implicit operator Mass(float value) => new Mass(value);
        //[Pure] public static implicit operator Mass(double value) => new Mass(value);
        //[Pure] public static implicit operator Mass(decimal value) => new Mass((double)value);

        // Comparison
        [Pure] public bool Equals(Mass other) => Value == other.Value;
        [Pure] public static bool operator ==(Mass left, Mass right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Mass left, Mass right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Mass other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Mass left, Mass right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Mass left, Mass right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Mass left, Mass right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Mass left, Mass right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Mass operator +(Mass lvalue, Mass rvalue) => new Mass(lvalue.Value + rvalue.Value);
        [Pure] public static Mass operator -(Mass lvalue, Mass rvalue) => new Mass(lvalue.Value - rvalue.Value);
        [Pure] public static Mass operator *(Mass value, double operand) => new Mass(value.Value * operand);
        [Pure] public static Mass operator /(Mass value, double operand) => new Mass(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Mass Abs() { return new Mass(Math.Abs(this.Value)); }

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