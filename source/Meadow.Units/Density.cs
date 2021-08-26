using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Density
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Density :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Density` object.
        /// </summary>
        /// <param name="value">The Density value.</param>
        /// <param name="type">Kilograms per meters cubed by default.</param>
        public Density(double value, UnitType type = UnitType.KilogramsPerMetersCubed)
        {
            Value = DensityConversions.Convert(value, type, UnitType.KilogramsPerMetersCubed);
        }

        public Density(Density density)
        {
            this.Value = density.Value;
        }

        /// <summary>
        /// Internal canonical value.
        /// </summary>
        private readonly double Value;

        /// <summary>
        /// The type of units available to describe the Density.
        /// </summary>
        public enum UnitType
        {
            GramsPerCentimetersCubed,
            GramsPerMetersCubed,
            GramsPerLiter,
            KilogramsPerMetersCubed,
            OuncesPerInchesCubed,
            OuncesPerFeetCubed,
            PoundsPerInchesCubed,
            PoundsPerFeetCubed,
            Water
        }

        public double GramsPerCentimetersCubed => From(UnitType.GramsPerCentimetersCubed);
        public double GramsPerMetersCubed => From(UnitType.GramsPerMetersCubed);
        public double GramsPerLiter => From(UnitType.GramsPerLiter);
        public double KilogramsPerMetersCubed => From(UnitType.KilogramsPerMetersCubed);
        public double OuncesPerInchesCubed => From(UnitType.OuncesPerInchesCubed);
        public double OuncesPerFeetCubed => From(UnitType.OuncesPerFeetCubed);
        public double PoundsPerInchesCubed => From(UnitType.PoundsPerInchesCubed);
        public double PoundsPerFeetCubed => From(UnitType.PoundsPerFeetCubed);
        public double Water => From(UnitType.Water);

        [Pure]
        public double From(UnitType convertTo)
        {
            return DensityConversions.Convert(Value, UnitType.KilogramsPerMetersCubed, convertTo);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Density)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Density(ushort value) => new Density(value);
        //[Pure] public static implicit operator Density(short value) => new Density(value);
        //[Pure] public static implicit operator Density(uint value) => new Density(value);
        //[Pure] public static implicit operator Density(long value) => new Density(value);
        //[Pure] public static implicit operator Density(int value) => new Density(value);
        //[Pure] public static implicit operator Density(float value) => new Density(value);
        //[Pure] public static implicit operator Density(double value) => new Density(value);
        //[Pure] public static implicit operator Density(decimal value) => new Density((double)value);

        // Comparison
        [Pure] public bool Equals(Density other) => Value == other.Value;
        [Pure] public static bool operator ==(Density left, Density right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Density left, Density right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Density other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Density left, Density right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Density operator +(Density lvalue, Density rvalue) => new Density(lvalue.Value + rvalue.Value);
        [Pure] public static Density operator -(Density lvalue, Density rvalue) => new Density(lvalue.Value - rvalue.Value);
        [Pure] public static Density operator *(Density value, double operand) => new Density(value.Value * operand);
        [Pure] public static Density operator /(Density value, double operand) => new Density(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Density Abs() { return new Density(Math.Abs(this.Value)); }

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