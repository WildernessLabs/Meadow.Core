using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents a value of Electric Potential, or _Voltage_.
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Voltage :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Voltage` object.
        /// </summary>
        /// <param name="value">The Voltage value.</param>
        /// <param name="type">Volts by default.</param>
        public Voltage(double value, UnitType type = UnitType.Volts)
        {
            Value = VoltageConversions.Convert(value, type, UnitType.Volts);
        }

        public Voltage(Voltage voltage)
        {
            this.Value = voltage.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the voltage.
        /// </summary>
        public enum UnitType
        {
            Volts,
            Millivolts,
            Microvolts,
            Kilovolts,
            Megavolts,
            Gigavolts,
            Statvolts //hahhhahahah
        }

        public double Volts => From(UnitType.Volts);
        public double Millivolts => From(UnitType.Millivolts);
        public double Microvolts => From(UnitType.Microvolts);
        public double Kilovolts => From(UnitType.Kilovolts);
        public double Megavolts => From(UnitType.Megavolts);
        public double Gigavolts => From(UnitType.Gigavolts);
        public double Statvolts => From(UnitType.Statvolts);

        [Pure]
        public double From(UnitType convertTo)
        {
            return VoltageConversions.Convert(Value, Voltage.UnitType.Volts, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Voltage)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Voltage(ushort value) => new Voltage(value);
        //[Pure] public static implicit operator Voltage(short value) => new Voltage(value);
        //[Pure] public static implicit operator Voltage(uint value) => new Voltage(value);
        //[Pure] public static implicit operator Voltage(long value) => new Voltage(value);
        //[Pure] public static implicit operator Voltage(int value) => new Voltage(value);
        //[Pure] public static implicit operator Voltage(float value) => new Voltage(value);
        //[Pure] public static implicit operator Voltage(double value) => new Voltage(value);
        //[Pure] public static implicit operator Voltage(decimal value) => new Voltage((double)value);

        // Comparison
        [Pure] public bool Equals(Voltage other) => Value == other.Value;
        [Pure] public static bool operator ==(Voltage left, Voltage right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Voltage left, Voltage right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Voltage other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Voltage operator +(Voltage lvalue, Voltage rvalue) => new Voltage(lvalue.Value + rvalue.Value);
        [Pure] public static Voltage operator -(Voltage lvalue, Voltage rvalue) => new Voltage(lvalue.Value - rvalue.Value);
        [Pure] public static Voltage operator *(Voltage value, double operand) => new Voltage(value.Value * operand);
        [Pure] public static Voltage operator /(Voltage value, double operand) => new Voltage(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Voltage Abs() { return new Voltage(Math.Abs(this.Value)); }

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