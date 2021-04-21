using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Angle
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class Angle : IUnitType, IComparable, IFormattable, IConvertible, IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Angle` object.
        /// </summary>
        /// <param name="value">The Angle value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Angle(double value, UnitType type = UnitType.Degrees)
        {
            //always store reference value
            Unit = type;
            _value = AngleConversions.Convert(value, type, UnitType.Degrees);
        }

        /// <summary>
        /// The Angle expressed as a value.
        /// </summary>
        public double Value
        {
            get => AngleConversions.Convert(_value, UnitType.Degrees, Unit);
            set => _value = AngleConversions.Convert(value, Unit, UnitType.Degrees);
        }

        private double _value;

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

        /// <summary>
        /// The type of units available to describe the Angle.
        /// </summary>
        public enum UnitType
        {
            Revolutions,
            Degrees,
            Radians,
            Gradians,
            Minutes,
            Seconds
        }

        public double Revolutions => From(UnitType.Revolutions);
        public double Degrees => From(UnitType.Degrees);
        public double Radians => From(UnitType.Radians);
        public double Gradians => From(UnitType.Gradians);
        public double Minutes => From(UnitType.Minutes);
        public double Seconds => From(UnitType.Seconds);

        [Pure]
        public double From(UnitType convertTo)
        {
            return AngleConversions.Convert(_value, UnitType.Degrees, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Angle)obj);
        }

        [Pure] public bool Equals(Angle other) => _value == other._value;

        [Pure] public override int GetHashCode() => _value.GetHashCode();

        [Pure] public static bool operator ==(Angle left, Angle right) => Equals(left, right);
        [Pure] public static bool operator !=(Angle left, Angle right) => !Equals(left, right);
        [Pure] public int CompareTo(Angle other) => Equals(this, other) ? 0 : _value.CompareTo(other._value);
        [Pure] public static bool operator <(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Degrees, right.Degrees) < 0;
        [Pure] public static bool operator >(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Degrees, right.Degrees) > 0;
        [Pure] public static bool operator <=(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Degrees, right.Degrees) <= 0;
        [Pure] public static bool operator >=(Angle left, Angle right) => Comparer<double>.Default.Compare(left.Degrees, right.Degrees) >= 0;

        [Pure] public static implicit operator Angle(int value) => new Angle(value);

        [Pure]
        public static Angle operator +(Angle lvalue, Angle rvalue)
        {
            var total = lvalue.Degrees + rvalue.Degrees;
            return new Angle(total, UnitType.Degrees);
        }

        [Pure]
        public static Angle operator -(Angle lvalue, Angle rvalue)
        {
            var total = lvalue.Degrees - rvalue.Degrees;
            return new Angle(total, UnitType.Degrees);
        }

        [Pure]
        public static Angle operator *(Angle lvalue, Angle rvalue)
        {
            var total = lvalue.Degrees * rvalue.Degrees;
            return new Angle(total, UnitType.Degrees);
        }

        [Pure]
        public static Angle operator /(Angle lvalue, Angle rvalue)
        {
            var total = lvalue.Degrees / rvalue.Degrees;
            return new Angle(total, UnitType.Degrees);
        }

        [Pure] public override string ToString() => _value.ToString();
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        // IComparable
        [Pure] public int CompareTo(object obj) => _value.CompareTo(obj);

        [Pure] public TypeCode GetTypeCode() => _value.GetTypeCode();
        [Pure] public bool ToBoolean(IFormatProvider provider) => ((IConvertible)_value).ToBoolean(provider);
        [Pure] public byte ToByte(IFormatProvider provider) => ((IConvertible)_value).ToByte(provider);
        [Pure] public char ToChar(IFormatProvider provider) => ((IConvertible)_value).ToChar(provider);
        [Pure] public DateTime ToDateTime(IFormatProvider provider) => ((IConvertible)_value).ToDateTime(provider);
        [Pure] public decimal ToDecimal(IFormatProvider provider) => ((IConvertible)_value).ToDecimal(provider);
        [Pure] public double ToDouble(IFormatProvider provider) => _value;
        [Pure] public short ToInt16(IFormatProvider provider) => ((IConvertible)_value).ToInt16(provider);
        [Pure] public int ToInt32(IFormatProvider provider) => ((IConvertible)_value).ToInt32(provider);
        [Pure] public long ToInt64(IFormatProvider provider) => ((IConvertible)_value).ToInt64(provider);
        [Pure] public sbyte ToSByte(IFormatProvider provider) => ((IConvertible)_value).ToSByte(provider);
        [Pure] public float ToSingle(IFormatProvider provider) => ((IConvertible)_value).ToSingle(provider);
        [Pure] public string ToString(IFormatProvider provider) => _value.ToString(provider);
        [Pure] public object ToType(Type conversionType, IFormatProvider provider) => ((IConvertible)_value).ToType(conversionType, provider);
        [Pure] public ushort ToUInt16(IFormatProvider provider) => ((IConvertible)_value).ToUInt16(provider);
        [Pure] public uint ToUInt32(IFormatProvider provider) => ((IConvertible)_value).ToUInt32(provider);
        [Pure] public ulong ToUInt64(IFormatProvider provider) => ((IConvertible)_value).ToUInt64(provider);

        [Pure]
        public int CompareTo(double? other)
        {
            return (other is null) ? -1 : (_value).CompareTo(other.Value);
        }

        [Pure] public bool Equals(double? other) => _value.Equals(other);
        [Pure] public bool Equals(double other) => _value.Equals(other);
        [Pure] public int CompareTo(double other) => _value.CompareTo(other);
        // can't do this.
        //public int CompareTo(double? other) => Value.CompareTo(other);
    }
}