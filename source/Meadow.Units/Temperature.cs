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
    [System.Runtime.InteropServices.StructLayout(LayoutKind.Sequential)]
    public struct Temperature : IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Temperature` object.
        /// </summary>
        /// <param name="value">The temperature value.</param>
        /// <param name="type">_Celsius_ (`C°`), by default.</param>
        public Temperature(double value, UnitType type = UnitType.Celsius)
        {
            Value = value; Unit = type;
        }

        /// <summary>
        /// The temperature expressed as a value.
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

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
        public double Celsius {
            get {
                switch (Unit) {
                    case UnitType.Celsius:
                        return Value;
                    case UnitType.Fahrenheit:
                        return TempConversions.FToC(Value);
                    case UnitType.Kelvin:
                        return TempConversions.KToC(Value);
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        /// <summary>
        /// Gets the temperature value expressed as a unit _Fahrenheit_ (`F°`).
        /// </summary>
        public double Fahrenheit {
            get {
                switch (Unit) {
                    case UnitType.Celsius:
                        return TempConversions.CToF(Value);
                    case UnitType.Fahrenheit:
                        return Value;
                    case UnitType.Kelvin:
                        return TempConversions.CToF(TempConversions.KToC(Value));
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        /// <summary>
        /// Gets the temperature value expressed as a unit _Kelvin_ (`K`).
        /// </summary>
        public double Kelvin {
            get {
                switch (Unit) {
                    case UnitType.Celsius:
                        return TempConversions.CToK(Value);
                    case UnitType.Fahrenheit:
                        return TempConversions.CToK(TempConversions.FToC(Value));
                    case UnitType.Kelvin:
                        return Value;
                    default: throw new Exception("the compiler lies.");
                }
            }
        }

        //=============================
        // FROM convenience conversions

        /// <summary>
        /// Creates a new `Temperature` object from a unit value in _Celsius/Centigrade_ (`C°`).
        /// </summary>
        /// <param name="value">The temperature value.</param>
        /// <returns>A new temperature object.</returns>
        [Pure] public static Temperature FromCelsius(double value) => new Temperature(value, UnitType.Celsius);

        /// <summary>
        /// Creates a new `Temperature` object from a unit value in _Fahrenheit_ (`F°`).
        /// </summary>
        /// <param name="value">The temperature value.</param>
        /// <returns>A new temperature object.</returns>

        [Pure] public static Temperature FromFahrenheit(double value) => new Temperature(value, UnitType.Fahrenheit);
        /// <summary>
        /// Creates a new `Temperature` object from a unit value in _Kelvin_ (`K°`).
        /// </summary>
        /// <param name="value">The temperature value.</param>
        /// <returns>A new temperature object.</returns>
        [Pure] public static Temperature FromKelvin(double value) => new Temperature(value, UnitType.Kelvin);

        //=============================
        // Boilerplate interface stuff.

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Temperature)obj);
        }

        [Pure] public bool Equals(Temperature other) => Value == other.Value;

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        [Pure] public static bool operator ==(Temperature left, Temperature right) => Equals(left, right);
        [Pure] public static bool operator !=(Temperature left, Temperature right) => !Equals(left, right);
        [Pure] public int CompareTo(Temperature other) => Equals(this, other) ? 0 : Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Temperature left, Temperature right) => Comparer<Temperature>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Temperature left, Temperature right) => Comparer<Temperature>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Temperature left, Temperature right) => Comparer<Temperature>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Temperature left, Temperature right) => Comparer<Temperature>.Default.Compare(left, right) >= 0;

        [Pure] public static implicit operator Temperature(int value) => new Temperature(value);

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
            return (other is null) ? -1 : ((IComparable<double>)Value).CompareTo(other.Value);
        }

        [Pure] public bool Equals(double? other) => Value.Equals(other);
        [Pure] public bool Equals(double other) => Value.Equals(other);
        [Pure] public int CompareTo(double other) => Value.CompareTo(other);
        // can't do this.
        //public int CompareTo(double? other) => Value.CompareTo(other);
    }
}
