using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Illuminance
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Illuminance :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Illuminance` object.
        /// </summary>
        /// <param name="value">The Illuminance value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Illuminance(double value, UnitType type = UnitType.Lux)
        {
            Value = IlluminanceConversions.Convert(value, type, UnitType.Lux);
        }

        public Illuminance(Illuminance illuminance)
        {
            this.Value = illuminance.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the Illuminance.
        /// </summary>
        public enum UnitType
        {
            KiloLux,
            Lux,
            FootCandles,
        }

        public double KiloLux => From(UnitType.KiloLux);
        public double Lux => From(UnitType.Lux);
        public double FootCandles => From(UnitType.FootCandles);

        [Pure]
        public double From(UnitType convertTo)
        {
            return IlluminanceConversions.Convert(Value, UnitType.Lux, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Illuminance)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Illuminance(ushort value) => new Illuminance(value);
        //[Pure] public static implicit operator Illuminance(short value) => new Illuminance(value);
        //[Pure] public static implicit operator Illuminance(uint value) => new Illuminance(value);
        //[Pure] public static implicit operator Illuminance(long value) => new Illuminance(value);
        //[Pure] public static implicit operator Illuminance(int value) => new Illuminance(value);
        //[Pure] public static implicit operator Illuminance(float value) => new Illuminance(value);
        //[Pure] public static implicit operator Illuminance(double value) => new Illuminance(value);
        //[Pure] public static implicit operator Illuminance(decimal value) => new Illuminance((double)value);

        // Comparison
        [Pure] public bool Equals(Illuminance other) => Value == other.Value;
        [Pure] public static bool operator ==(Illuminance left, Illuminance right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Illuminance left, Illuminance right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Illuminance other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Illuminance left, Illuminance right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Illuminance left, Illuminance right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Illuminance left, Illuminance right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Illuminance left, Illuminance right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Illuminance operator +(Illuminance lvalue, Illuminance rvalue) => new Illuminance(lvalue.Value + rvalue.Value);
        [Pure] public static Illuminance operator -(Illuminance lvalue, Illuminance rvalue) => new Illuminance(lvalue.Value - rvalue.Value);
        [Pure] public static Illuminance operator *(Illuminance value, double operand) => new Illuminance(value.Value * operand);
        [Pure] public static Illuminance operator /(Illuminance value, double operand) => new Illuminance(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Illuminance Abs() { return new Illuminance(Math.Abs(this.Value)); }

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