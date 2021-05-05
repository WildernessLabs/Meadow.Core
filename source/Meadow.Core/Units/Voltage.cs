using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents a value of Electric Potential, or _Voltage_.
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Voltage : IUnitType, IComparable, IFormattable, IConvertible, IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Voltage` object.
        /// </summary>
        /// <param name="value">The Voltage value.</param>
        /// <param name="type">Volts by default.</param>
        public Voltage(double value, UnitType type = UnitType.Volts)
        {
            //always store reference value
            Unit = type;
            _value = VoltageConversions.Convert(value, type, UnitType.Volts);
        }

        /// <summary>
        /// The voltage expressed as a value.
        /// </summary>
        public double Value {
            get => VoltageConversions.Convert(_value, UnitType.Volts, Unit);
            set => _value = VoltageConversions.Convert(value, Unit, UnitType.Volts);
        }

        private double _value;

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public UnitType Unit { get; set; }

        /// <summary>
        /// The type of units available to describe the voltage.
        /// </summary>
        public enum UnitType
        {
            Volts,
            Millivolts,
            Microvolts,
            Kilovolts,
            Megavolts,
            Gigavolts,
            Statvolts //hahhhahahah
        }

        public double Volts => From(UnitType.Volts);
        public double Millivolts => From(UnitType.Millivolts);
        public double Microvolts => From(UnitType.Microvolts);
        public double Kilovolts => From(UnitType.Kilovolts);
        public double Megavolts => From(UnitType.Megavolts);
        public double Gigavolts => From(UnitType.Gigavolts);
        public double Statvolts => From(UnitType.Statvolts);

        [Pure]
        public double From(UnitType convertTo)
        {
            return VoltageConversions.Convert(_value, Voltage.UnitType.Volts, convertTo);
        }

        /// <summary>
        /// Returns the absolute voltage, that is, the voltage without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure]
        public Voltage Abs()
        {
            return new Voltage(Math.Abs(this._value), UnitType.Volts);
        }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Voltage)obj);
        }

        [Pure] public bool Equals(Voltage other) => _value == other._value;

        [Pure] public override int GetHashCode() => _value.GetHashCode();

        [Pure] public static bool operator ==(Voltage left, Voltage right) => Equals(left, right);
        [Pure] public static bool operator !=(Voltage left, Voltage right) => !Equals(left, right);
        [Pure] public int CompareTo(Voltage other) => Equals(this, other) ? 0 : _value.CompareTo(other._value);
        [Pure] public static bool operator <(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Volts, right.Volts) < 0;
        [Pure] public static bool operator >(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Volts, right.Volts) > 0;
        [Pure] public static bool operator <=(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Volts, right.Volts) <= 0;
        [Pure] public static bool operator >=(Voltage left, Voltage right) => Comparer<double>.Default.Compare(left.Volts, right.Volts) >= 0;

        [Pure] public static implicit operator Voltage(double value) => new Voltage(value);

        [Pure] public static Voltage operator +(Voltage lvalue, Voltage rvalue) => new Voltage(lvalue.Volts + rvalue.Volts, UnitType.Volts);
        [Pure] public static Voltage operator -(Voltage lvalue, Voltage rvalue) => new Voltage(lvalue.Volts - rvalue.Volts, UnitType.Volts);
        [Pure] public static Voltage operator /(Voltage lvalue, Voltage rvalue) => new Voltage(lvalue.Volts / rvalue.Volts, UnitType.Volts);
        [Pure] public static Voltage operator *(Voltage lvalue, Voltage rvalue) => new Voltage(lvalue.Volts * rvalue.Volts, UnitType.Volts);

        [Pure] public override string ToString() => _value.ToString();
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => _value.ToString(format, formatProvider);

        // IComparable
        [Pure] public int CompareTo(object obj) => _value.CompareTo(obj);

        [Pure] public TypeCode GetTypeCode() => _value.GetTypeCode();
        [Pure] public bool ToBoolean(IFormatProvider provider) => ((IConvertible)_value).ToBoolean(provider);
        [Pure] public byte ToByte(IFormatProvider provider) => ((IConvertible)_value).ToByte(provider);
        [Pure] public char ToChar(IFormatProvider provider) => ((IConvertible)_value).ToChar(provider);
        // TODO: maybe this shouldn't be here? heh
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