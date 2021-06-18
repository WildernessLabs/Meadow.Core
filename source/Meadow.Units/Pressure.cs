﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents pressure; the force applied perpendicular to the surface of
    /// an object per unit area over which that force is distributed.
    /// </summary>
    [Serializable]
    [ImmutableObject(true)]
    [StructLayout(LayoutKind.Sequential)]
    public struct Pressure : IUnitType, IComparable, IFormattable, IConvertible,
        IEquatable<double>, IComparable<double>
    {
        /// <summary>
        /// Creates a new `Pressure` object.
        /// </summary>
        /// <param name="value">The pressure value.</param>
        /// <param name="type">_Bar_ (`Bar`), by default.</param>
        public Pressure(double value, UnitType type = UnitType.Bar)
        {
            Value = PressureConversions.Convert(value, type, UnitType.Bar);
        }

        public Pressure(Pressure pressure)
        {
            this.Value = pressure.Value;
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
            Bar,            
            Pascal,
            Psi,
            StandardAtmosphere,
            Millibar,
            Hectopascal,
            KiloPascal
        }

        //========================
        // TO property conversions

        /// <summary>
        /// Gets the pressure value expressed as a unit _Bar_ (`Bar`)
        /// </summary>
        public double Bar { get => Value; }
        /// <summary>
        /// Gets the pressure value expressed as a unit _Pascal_ (`Pa`).
        /// </summary>
        public double Pascal { get => From(UnitType.Pascal); }
        /// <summary>
        /// Gets the pressure value expressed as a unit _Pound-force per square inch_ (`Psi`).
        /// </summary>
        public double Psi { get => From(UnitType.Psi); }
        /// <summary>
        /// Gets the pressure value expressed as a unit _Standard Atmosphere_ (`At`).
        /// </summary>
        public double StandardAtmosphere { get => From(UnitType.StandardAtmosphere); }
        /// <summary>
        /// Gets the pressure value expressed as a unit _Bar_ (`Bar`)
        /// </summary>
        public double Millibar { get => From(UnitType.Millibar); }
        /// <summary>
        /// Gets the pressure value expressed as a unit _Bar_ (`Bar`)
        /// </summary>
        public double Hectopascal { get => From(UnitType.Hectopascal); }


        [Pure]
        public double From(UnitType convertTo)
        {
            return PressureConversions.Convert(Value, UnitType.Bar, convertTo);
        }

        //=============================
        // Boilerplate interface stuff.

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Pressure)obj);
        }

        [Pure] public override int GetHashCode() => Value.GetHashCode();

        // implicit conversions
        //[Pure] public static implicit operator Pressure(ushort value) => new Pressure(value);
        //[Pure] public static implicit operator Pressure(short value) => new Pressure(value);
        //[Pure] public static implicit operator Pressure(uint value) => new Pressure(value);
        //[Pure] public static implicit operator Pressure(long value) => new Pressure(value);
        //[Pure] public static implicit operator Pressure(int value) => new Pressure(value);
        //[Pure] public static implicit operator Pressure(float value) => new Pressure(value);
        //[Pure] public static implicit operator Pressure(double value) => new Pressure(value);
        //[Pure] public static implicit operator Pressure(decimal value) => new Pressure((double)value);

        // Comparison
        [Pure] public bool Equals(Pressure other) => Value == other.Value;
        [Pure] public static bool operator ==(Pressure left, Pressure right) => Equals(left.Value, right.Value);
        [Pure] public static bool operator !=(Pressure left, Pressure right) => !Equals(left.Value, right.Value);
        [Pure] public int CompareTo(Pressure other) => Equals(this.Value, other.Value) ? 0 : this.Value.CompareTo(other.Value);
        [Pure] public static bool operator <(Pressure left, Pressure right) => Comparer<double>.Default.Compare(left.Value, right.Value) < 0;
        [Pure] public static bool operator >(Pressure left, Pressure right) => Comparer<double>.Default.Compare(left.Value, right.Value) > 0;
        [Pure] public static bool operator <=(Pressure left, Pressure right) => Comparer<double>.Default.Compare(left.Value, right.Value) <= 0;
        [Pure] public static bool operator >=(Pressure left, Pressure right) => Comparer<double>.Default.Compare(left.Value, right.Value) >= 0;

        // Math
        [Pure] public static Pressure operator +(Pressure lvalue, Pressure rvalue) => new Pressure(lvalue.Value + rvalue.Value);
        [Pure] public static Pressure operator -(Pressure lvalue, Pressure rvalue) => new Pressure(lvalue.Value - rvalue.Value);

        /// <summary>
        /// Returns the absolute length, that is, the length without regards to
        /// negative polarity
        /// </summary>
        /// <returns></returns>
        [Pure] public Pressure Abs() { return new Pressure(Math.Abs(this.Value)); }

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
