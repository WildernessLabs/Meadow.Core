using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents a value of Electric Current.
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Current :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Current` object.
        /// </summary>
        /// <param name="value">The Current value.</param>
        /// <param name="type">Amps by default.</param>
        public Current(double value, UnitType type = UnitType.Amps)
        {
            //always store reference value
            Unit = type;
            Value = CurrentConversions.Convert(value, type, UnitType.Amps);
        }

        public Current(Current Current)
        {
            Value = Current.Value;
            Unit = Current.Unit;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

        /// <summary>
        /// The type of units available to describe the Current.
        /// </summary>
        public enum UnitType
        {
            Amps,
            Milliamps,
            Microamps,
            Kiloamps,
            Megaamps,
            Gigaamps,
           }

        public double Amps => From(UnitType.Amps);
        public double Milliamps => From(UnitType.Milliamps);
        public double Microamps => From(UnitType.Microamps);
        public double Kiloamps => From(UnitType.Kiloamps);
        public double Megaamps => From(UnitType.Megaamps);
        public double Gigaamps => From(UnitType.Gigaamps);
 
        [Pure]
        public double From(UnitType convertTo)
        {
            return CurrentConversions.Convert(Value, Current.UnitType.Amps, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Current)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Current(ushort value) => new Current(value);
        //[Pure] public static implicit operator Current(short value) => new Current(value);
        //[Pure] public static implicit operator Current(uint value) => new Current(value);
        //[Pure] public static implicit operator Current(long value) => new Current(value);
        //[Pure] public static implicit operator Current(int value) => new Current(value);
        //[Pure] public static implicit operator Current(float value) => new Current(value);
        //[Pure] public static implicit operator Current(double value) => new Current(value);
        //[Pure] public static implicit operator Current(decimal value) => new Current((double)value);

        // Comparison
        [Pure] public bool Equals(Current other) => Value == other.Value;
        [Pure] public static bool operator ==(Current left, Current right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Current left, Current right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Current other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Current left, Current right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Current left, Current right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Current left, Current right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Current left, Current right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Current operator +(Current lvalue, Current rvalue) => new Current(lvalue.Value + rvalue.Value);
        [Pure] public static Current operator -(Current lvalue, Current rvalue) => new Current(lvalue.Value - rvalue.Value);
        [Pure] public static Current operator *(Current value, double operand) => new Current(value.Value * operand);
        [Pure] public static Current operator /(Current value, double operand) => new Current(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Current Abs() { return new Current(Math.Abs(this.Value)); }

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