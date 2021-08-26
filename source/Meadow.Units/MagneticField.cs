using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents MagneticField
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct MagneticField :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `MagneticField` object.
        /// </summary>
        /// <param name="value">The MagneticField value.</param>
        /// <param name="type">kilometers meters per second by default.</param>
        public MagneticField(double value, UnitType type = UnitType.Tesla)
        {
            Value = MagneticFieldConversions.Convert(value, type, UnitType.Tesla);
        }

        public MagneticField(MagneticField magneticField)
        {
            Value = magneticField.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the MagneticField.
        /// </summary>
        public enum UnitType
        {
            MegaTesla,
			KiloTesla,
			Tesla,
			MilliTesla,
			MicroTesla,
			NanoTesla,
			PicoTesla,
			Gauss
        }

        public double MegaTesla => From(UnitType.MegaTesla);
        public double KiloTesla => From(UnitType.KiloTesla);
        public double Tesla => From(UnitType.Tesla);
        public double MilliTesla => From(UnitType.MilliTesla);
        public double MicroTesla => From(UnitType.MicroTesla);
        public double NanoTesla => From(UnitType.NanoTesla);
        public double PicoTesla => From(UnitType.PicoTesla);
        public double Gauss => From(UnitType.Gauss);

        [Pure]
        public double From(UnitType convertTo)
        {
            return MagneticFieldConversions.Convert(Value, UnitType.Tesla, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((MagneticField)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator MagneticField(ushort value) => new MagneticField(value);
        //[Pure] public static implicit operator MagneticField(short value) => new MagneticField(value);
        //[Pure] public static implicit operator MagneticField(uint value) => new MagneticField(value);
        //[Pure] public static implicit operator MagneticField(long value) => new MagneticField(value);
        //[Pure] public static implicit operator MagneticField(int value) => new MagneticField(value);
        //[Pure] public static implicit operator MagneticField(float value) => new MagneticField(value);
        //[Pure] public static implicit operator MagneticField(double value) => new MagneticField(value);
        //[Pure] public static implicit operator MagneticField(decimal value) => new MagneticField((double)value);

        // Comparison
        [Pure] public bool Equals(MagneticField other) => Value == other.Value;
        [Pure] public static bool operator ==(MagneticField left, MagneticField right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(MagneticField left, MagneticField right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(MagneticField other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(MagneticField left, MagneticField right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(MagneticField left, MagneticField right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(MagneticField left, MagneticField right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(MagneticField left, MagneticField right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static MagneticField operator +(MagneticField lvalue, MagneticField rvalue) => new MagneticField(lvalue.Value + rvalue.Value);
        [Pure] public static MagneticField operator -(MagneticField lvalue, MagneticField rvalue) => new MagneticField(lvalue.Value - rvalue.Value);
        [Pure] public static MagneticField operator *(MagneticField value, double operand) => new MagneticField(value.Value * operand);
        [Pure] public static MagneticField operator /(MagneticField value, double operand) => new MagneticField(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public MagneticField Abs() { return new MagneticField(Math.Abs(this.Value)); }

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