using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Acceleration
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Acceleration :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Acceleration` object.
        /// </summary>
        /// <param name="value">The Acceleration value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Acceleration(double value, UnitType type = UnitType.MetersPerSecondSquared)
        {
            Value = AccelerationConversions.Convert(value, type, UnitType.MetersPerSecondSquared);
        }

        public Acceleration(Acceleration acceleration)
        {
            this.Value = acceleration.Value;
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
            MetersPerSecondSquared,
            CentimetersPerSecondSquared,
            Galileo,
            MilliGalileo,
            MilliGravity,
            Gravity,
            FeetPerSecondSquared,
            InchesPerSecondSquared,
        }

        public double MetersPerSecondSquared => From(UnitType.MetersPerSecondSquared);
        public double CentimetersPerSecondSquared => From(UnitType.CentimetersPerSecondSquared);
        public double Gravity => From(UnitType.Gravity);
        public double FeetPerSecondSquared => From(UnitType.FeetPerSecondSquared);
        public double InchesPerSecondSquared => From(UnitType.InchesPerSecondSquared);

        [Pure]
        public double From(UnitType convertTo)
        {
            return AccelerationConversions.Convert(Value, UnitType.MetersPerSecondSquared, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Acceleration)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Acceleration(ushort value) => new Acceleration(value);
        //[Pure] public static implicit operator Acceleration(short value) => new Acceleration(value);
        //[Pure] public static implicit operator Acceleration(uint value) => new Acceleration(value);
        //[Pure] public static implicit operator Acceleration(long value) => new Acceleration(value);
        //[Pure] public static implicit operator Acceleration(int value) => new Acceleration(value);
        //[Pure] public static implicit operator Acceleration(float value) => new Acceleration(value);
        //[Pure] public static implicit operator Acceleration(double value) => new Acceleration(value);
        //[Pure] public static implicit operator Acceleration(decimal value) => new Acceleration((double)value);

        // Comparison
        [Pure] public bool Equals(Acceleration other) => Value == other.Value;
        [Pure] public static bool operator ==(Acceleration left, Acceleration right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Acceleration left, Acceleration right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Acceleration other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Acceleration left, Acceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Acceleration left, Acceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Acceleration left, Acceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Acceleration left, Acceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Acceleration operator +(Acceleration lvalue, Acceleration rvalue) => new Acceleration(lvalue.Value + rvalue.Value);
        [Pure] public static Acceleration operator -(Acceleration lvalue, Acceleration rvalue) => new Acceleration(lvalue.Value - rvalue.Value);
        [Pure] public static Acceleration operator *(Acceleration value, double operand) => new Acceleration(value.Value * operand);
        [Pure] public static Acceleration operator /(Acceleration value, double operand) => new Acceleration(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Acceleration Abs() { return new Acceleration(Math.Abs(this.Value)); }

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