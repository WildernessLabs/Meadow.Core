using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents temperature; the physical quantity that expresses hot and cold.
    /// It is the manifestation of thermal energy, present in all matter, which
    /// is the source of the occurrence of heat, a flow of energy, when a body
    /// is in contact with another that is colder or hotter.
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Temperature :
        IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Temperature` object.
        /// </summary>
        /// <param name="value">The temperature value.</param>
        /// <param name="type">_Celsius_ (`C°`), by default.</param>
        public Temperature(double value, UnitType type = UnitType.Celsius)
        {
            Value = 0;
            switch (type) {
                case UnitType.Celsius:
                    Value = value;
                    break;
                case UnitType.Fahrenheit:
                    Value = TempConversions.FToC(value);
                    break;
                case UnitType.Kelvin:
                    Value = TempConversions.KToC(value);
                    break;
            }
        }

        public Temperature(Temperature temperature)
        {
            this.Value = temperature.Value;
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
            Celsius,
            Fahrenheit,
            Kelvin,
        }

        //========================
        // TO property conversions

        /// <summary>
        /// Gets the temperature value expressed as a unit _Celsius/Centrigrade_ (`C°`).
        /// </summary>
        public double Celsius { get => Value; }

        /// <summary>
        /// Gets the temperature value expressed as a unit _Fahrenheit_ (`F°`).
        /// </summary>
        public double Fahrenheit { get => TempConversions.CToF(Value); }

        /// <summary>
        /// Gets the temperature value expressed as a unit _Kelvin_ (`K`).
        /// </summary>
        public double Kelvin { get => TempConversions.CToK(Value); }

        [Pure]
        public double From(UnitType convertTo)
        {
            return TempConversions.Convert(Value, UnitType.Celsius, convertTo);
        }


        ////=============================
        //// FROM convenience conversions

        ///// <summary>
        ///// Creates a new `Temperature` object from a unit value in _Celsius/Centigrade_ (`C°`).
        ///// </summary>
        ///// <param name="value">The temperature value.</param>
        ///// <returns>A new temperature object.</returns>
        //[Pure] public static Temperature FromCelsius(double value) => new Temperature(value, UnitType.Celsius);

        ///// <summary>
        ///// Creates a new `Temperature` object from a unit value in _Fahrenheit_ (`F°`).
        ///// </summary>
        ///// <param name="value">The temperature value.</param>
        ///// <returns>A new temperature object.</returns>

        //[Pure] public static Temperature FromFahrenheit(double value) => new Temperature(value, UnitType.Fahrenheit);
        ///// <summary>
        ///// Creates a new `Temperature` object from a unit value in _Kelvin_ (`K°`).
        ///// </summary>
        ///// <param name="value">The temperature value.</param>
        ///// <returns>A new temperature object.</returns>
        //[Pure] public static Temperature FromKelvin(double value) => new Temperature(value, UnitType.Kelvin);

        //=============================
        // Boilerplate interface stuff.

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Temperature)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Temperature(ushort value) => new Temperature(value);
        //[Pure] public static implicit operator Temperature(short value) => new Temperature(value);
        //[Pure] public static implicit operator Temperature(uint value) => new Temperature(value);
        //[Pure] public static implicit operator Temperature(long value) => new Temperature(value);
        //[Pure] public static implicit operator Temperature(int value) => new Temperature(value);
        //[Pure] public static implicit operator Temperature(float value) => new Temperature(value);
        //[Pure] public static implicit operator Temperature(double value) => new Temperature(value);
        //[Pure] public static implicit operator Temperature(decimal value) => new Temperature((double)value);

        // Comparison
        [Pure] public bool Equals(Temperature other) => Value == other.Value;
        [Pure] public static bool operator ==(Temperature left, Temperature right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Temperature left, Temperature right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Temperature other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Temperature left, Temperature right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Temperature left, Temperature right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Temperature left, Temperature right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Temperature left, Temperature right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Temperature operator +(Temperature lvalue, Temperature rvalue) => new Temperature(lvalue.Value + rvalue.Value);
        [Pure] public static Temperature operator -(Temperature lvalue, Temperature rvalue) => new Temperature(lvalue.Value - rvalue.Value);
        [Pure] public static Temperature operator *(Temperature value, double operand) => new Temperature(value.Value * operand);
        [Pure] public static Temperature operator /(Temperature value, double operand) => new Temperature(value.Value / operand);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Temperature Abs() { return new Temperature(Math.Abs(this.Value)); }

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
