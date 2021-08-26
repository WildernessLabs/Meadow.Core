using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Torque
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Torque :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Torque` object.
        /// </summary>
        /// <param name="value">The Torque value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Torque(double value, UnitType type = UnitType.NewtonMeter)
        {
            Value = TorqueConversions.Convert(value, type, UnitType.NewtonMeter);
        }

        public Torque(Torque torque)
        {
            this.Value = torque.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the Torque.
        /// </summary>
        public enum UnitType
        {
            NewtonMeter,
            FootPound,
            KilogramMeter,
            KilogramCentimeter,
            GramCentimeter,
            InchPound,
            InchOunce,
            DyneCentimeter
        }

        public double NewtonMeter => From(UnitType.NewtonMeter);
        public double FootPound => From(UnitType.FootPound);
        public double KilogramMeter => From(UnitType.KilogramMeter);
        public double KilogramCentimeter => From(UnitType.KilogramCentimeter);
        public double GramCentimeter => From(UnitType.GramCentimeter);
        public double InchPound => From(UnitType.InchPound);
        public double InchOunce => From(UnitType.InchOunce);
        public double DyneCentimeter => From(UnitType.DyneCentimeter);

        [Pure]
        public double From(UnitType convertTo)
        {
            return TorqueConversions.Convert(Value, UnitType.NewtonMeter, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Torque)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Torque(ushort value) => new Torque(value);
        //[Pure] public static implicit operator Torque(short value) => new Torque(value);
        //[Pure] public static implicit operator Torque(uint value) => new Torque(value);
        //[Pure] public static implicit operator Torque(long value) => new Torque(value);
        //[Pure] public static implicit operator Torque(int value) => new Torque(value);
        //[Pure] public static implicit operator Torque(float value) => new Torque(value);
        //[Pure] public static implicit operator Torque(double value) => new Torque(value);
        //[Pure] public static implicit operator Torque(decimal value) => new Torque((double)value);

        // Comparison
        [Pure] public bool Equals(Torque other) => Value == other.Value;
        [Pure] public static bool operator ==(Torque left, Torque right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Torque left, Torque right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Torque other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Torque left, Torque right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Torque left, Torque right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Torque left, Torque right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Torque left, Torque right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Torque operator +(Torque lvalue, Torque rvalue) => new Torque(lvalue.Value + rvalue.Value);
        [Pure] public static Torque operator -(Torque lvalue, Torque rvalue) => new Torque(lvalue.Value - rvalue.Value);
        [Pure] public static Torque operator *(Torque value, double operand) => new Torque(value.Value * operand);
        [Pure] public static Torque operator /(Torque value, double operand) => new Torque(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Torque Abs() { return new Torque(Math.Abs(this.Value)); }

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