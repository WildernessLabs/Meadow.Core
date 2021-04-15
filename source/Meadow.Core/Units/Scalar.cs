using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Scalar
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class Scalar : IUnitType, IComparable, IFormattable, IConvertible, IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Scalar` object.
        /// </summary>
        /// <param name="value">The Scalar value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public Scalar(double value)
        {
            Value = value;
        }

        /// <summary>
        /// The Scalar expressed as a value.
        /// </summary>
        public double Value { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Scalar)obj);
        }

        [Pure] public bool Equals(Scalar other) => Value == other.Value;

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        [Pure] public static bool operator ==(Scalar left, Scalar right) => Equals(left, right);
        [Pure] public static bool operator !=(Scalar left, Scalar right) => !Equals(left, right);
        [Pure] public int CompareTo(Scalar other) => Equals(this, other) ? 0 : Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Scalar left, Scalar right) => Comparer<Scalar>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Scalar left, Scalar right) => Comparer<Scalar>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Scalar left, Scalar right) => Comparer<Scalar>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Scalar left, Scalar right) => Comparer<Scalar>.Default.Compare(left, right) >= 0;

        [Pure] public static implicit operator Scalar(int value) => new Scalar(value);

        [Pure]
        public static Scalar operator +(Scalar lvalue, Scalar rvalue)
        {
            var total = lvalue.Value + rvalue.Value;
            return new Scalar(total);
        }

        [Pure]
        public static Scalar operator -(Scalar lvalue, Scalar rvalue)
        {
            var total = lvalue.Value - rvalue.Value;
            return new Scalar(total);
        }

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
        // can't do this.
        //public int CompareTo(double? other) => Value.CompareTo(other);
    }
}