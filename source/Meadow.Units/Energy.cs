using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Energy
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Energy :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Energy` object.
        /// </summary>
        /// <param name="value">The Energy value.</param>
        /// <param name="type">Joules by default.</param>
        public Energy(double value, UnitType type = UnitType.Joules)
        {
            Value = EnergyConversions.Convert(value, type, UnitType.Joules);
        }

        public Energy(Energy energy)
        {
            this.Value = energy.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the Energy.
        /// </summary>
        public enum UnitType
        {
            BTU,
            Calories,
            Joules,
            Kilocalories,
            Kilojoules,
            KilowattHours,
            Therms,
            WattHours,
            WattSeconds
        }

        public double BTU => From(UnitType.BTU);
        public double Calories => From(UnitType.Calories);
        public double Joules => From(UnitType.Joules);
        public double Kilocalories => From(UnitType.Kilocalories);
        public double Kilojoules => From(UnitType.Kilojoules);
        public double KilowattHours => From(UnitType.KilowattHours);
        public double Therms => From(UnitType.Therms);
        public double WattHours => From(UnitType.WattHours);
        public double WattSecond => From(UnitType.WattSeconds);


        [Pure]
        public double From(UnitType convertTo)
        {
            return EnergyConversions.Convert(Value, UnitType.Joules, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Energy)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Energy(ushort value) => new Energy(value);
        //[Pure] public static implicit operator Energy(short value) => new Energy(value);
        //[Pure] public static implicit operator Energy(uint value) => new Energy(value);
        //[Pure] public static implicit operator Energy(long value) => new Energy(value);
        //[Pure] public static implicit operator Energy(int value) => new Energy(value);
        //[Pure] public static implicit operator Energy(float value) => new Energy(value);
        //[Pure] public static implicit operator Energy(double value) => new Energy(value);
        //[Pure] public static implicit operator Energy(decimal value) => new Energy((double)value);

        // Comparison
        [Pure] public bool Equals(Energy other) => Value == other.Value;
        [Pure] public static bool operator ==(Energy left, Energy right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Energy left, Energy right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Energy other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Energy left, Energy right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Energy left, Energy right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Energy left, Energy right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Energy left, Energy right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Energy operator +(Energy lvalue, Energy rvalue) => new Energy(lvalue.Value + rvalue.Value);
        [Pure] public static Energy operator -(Energy lvalue, Energy rvalue) => new Energy(lvalue.Value - rvalue.Value);
        [Pure] public static Energy operator *(Energy value, double operand) => new Energy(value.Value * operand);
        [Pure] public static Energy operator /(Energy value, double operand) => new Energy(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Energy Abs() { return new Energy(Math.Abs(this.Value)); }

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