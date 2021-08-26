using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents AngularAcceleration
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct AngularAcceleration :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `AngularAcceleration` object.
        /// </summary>
        /// <param name="value">The AngularAcceleration value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public AngularAcceleration(double value, UnitType type = UnitType.RevolutionsPerSecondSquared)
        {
            Value = AngularAccelerationConversions.Convert(value, type, UnitType.RevolutionsPerSecondSquared);
        }

        public AngularAcceleration(AngularAcceleration acceleration)
        {
            Value = acceleration.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the AngularAcceleration.
        /// </summary>
        public enum UnitType
        {
            RevolutionsPerSecondSquared,
            RevolutionsPerMinuteSquared,
            RadiansPerSecondSquared,
            RadiansPerMinuteSquared,
            DegreesPerSecondSquared,
            DegreesPerMinuteSquared
        }

        public double RevolutionsPerSecondSquared => From(UnitType.RevolutionsPerSecondSquared);
        public double RevolutionsPerMinuteSquared => From(UnitType.RevolutionsPerMinuteSquared);
        public double RadiansPerSecondSquared => From(UnitType.RadiansPerSecondSquared);
        public double RadiansPerMinuteSquared => From(UnitType.RadiansPerMinuteSquared);
        public double DegreesPerSecondSquared => From(UnitType.DegreesPerSecondSquared);
        public double DegreesPerMinuteSquared => From(UnitType.DegreesPerMinuteSquared);

        [Pure]
        public double From(UnitType convertTo)
        {
            return AngularAccelerationConversions.Convert(Value, UnitType.RevolutionsPerSecondSquared, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((AngularAcceleration)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator AngularAcceleration(ushort value) => new AngularAcceleration(value);
        //[Pure] public static implicit operator AngularAcceleration(short value) => new AngularAcceleration(value);
        //[Pure] public static implicit operator AngularAcceleration(uint value) => new AngularAcceleration(value);
        //[Pure] public static implicit operator AngularAcceleration(long value) => new AngularAcceleration(value);
        //[Pure] public static implicit operator AngularAcceleration(int value) => new AngularAcceleration(value);
        //[Pure] public static implicit operator AngularAcceleration(float value) => new AngularAcceleration(value);
        //[Pure] public static implicit operator AngularAcceleration(double value) => new AngularAcceleration(value);
        //[Pure] public static implicit operator AngularAcceleration(decimal value) => new AngularAcceleration((double)value);

        // Comparison
        [Pure] public bool Equals(AngularAcceleration other) => Value == other.Value;
        [Pure] public static bool operator ==(AngularAcceleration left, AngularAcceleration right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(AngularAcceleration left, AngularAcceleration right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(AngularAcceleration other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(AngularAcceleration left, AngularAcceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(AngularAcceleration left, AngularAcceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(AngularAcceleration left, AngularAcceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(AngularAcceleration left, AngularAcceleration right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static AngularAcceleration operator +(AngularAcceleration lvalue, AngularAcceleration rvalue) => new AngularAcceleration(lvalue.Value + rvalue.Value);
        [Pure] public static AngularAcceleration operator -(AngularAcceleration lvalue, AngularAcceleration rvalue) => new AngularAcceleration(lvalue.Value - rvalue.Value);
        [Pure] public static AngularAcceleration operator *(AngularAcceleration value, double operand) => new AngularAcceleration(value.Value * operand);
        [Pure] public static AngularAcceleration operator /(AngularAcceleration value, double operand) => new AngularAcceleration(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public AngularAcceleration Abs() { return new AngularAcceleration(Math.Abs(this.Value)); }

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