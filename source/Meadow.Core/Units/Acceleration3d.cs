using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.Contracts;
using System.Runtime.InteropServices;
using Meadow.Units.Conversions;

namespace Meadow.Units
{
    /// <summary>
    /// Represents Acceleration3d
    /// </summary>
    [Serializable]
    [ImmutableObject(false)]
    [StructLayout(LayoutKind.Sequential)]
    public class Acceleration3d : IUnitType, IFormattable, IComparable, IEquatable<(double ValueX, double ValueY, double ValueZ)>, IComparable<(double, double, double)>
    {
        /// <summary>
        /// Creates a new `Acceleration3d` object.
        /// </summary>
        /// <param name="valueX">The X Acceleration3d value.</param>
        /// <param name="valueY">The Y Acceleration3d value.</param>
        /// <param name="valueZ">The Z Acceleration3d value.</param>
        /// <param name="type"></param>
        public Acceleration3d(double valueX, double valueY, double valueZ,
            Acceleration.UnitType type = Acceleration.UnitType.MetersPerSecondSquared)
        {
            //always store reference value
            Unit = type;
            AccelerationX = new Acceleration(valueX, Unit);
            AccelerationY = new Acceleration(valueY, Unit);
            AccelerationZ = new Acceleration(valueZ, Unit);
        }

        public Acceleration3d()
        {

        }

        public Acceleration3d(Acceleration accelerationX, Acceleration accelerationY, Acceleration accelerationZ)
        {
            AccelerationX = new Acceleration(accelerationX.Value, accelerationX.Unit);
            AccelerationY = new Acceleration(accelerationY.Value, accelerationY.Unit);
            AccelerationZ = new Acceleration(accelerationZ.Value, accelerationZ.Unit);

            Unit = AccelerationX.Unit;
        }

        public Acceleration AccelerationX { get; set; }
        public Acceleration AccelerationY { get; set; }
        public Acceleration AccelerationZ { get; set; }

        /// <summary>
        /// The unit that describes the value.
        /// </summary>
        public Acceleration.UnitType Unit { get; set; }

        [Pure]
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) { return false; }
            if (Equals(this, obj)) { return true; }
            return obj.GetType() == GetType() && Equals((Acceleration3d)obj);
        }

        [Pure]
        public bool Equals(Acceleration3d other) =>
            AccelerationX == other.AccelerationX &&
            AccelerationY == other.AccelerationY &&
            AccelerationZ == other.AccelerationZ;


        [Pure] public override int GetHashCode() => (AccelerationX.GetHashCode() + AccelerationY.GetHashCode() + AccelerationZ.GetHashCode()) / 3;

        [Pure] public static bool operator ==(Acceleration3d left, Acceleration3d right) => Equals(left, right);
        [Pure] public static bool operator !=(Acceleration3d left, Acceleration3d right) => !Equals(left, right);
        //ToDo [Pure] public int CompareTo(Acceleration3d other) => Equals(this, other) ? 0 : AccelerationX.CompareTo(other.AccelerationX);
        [Pure] public static bool operator <(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) < 0;
        [Pure] public static bool operator >(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) > 0;
        [Pure] public static bool operator <=(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) <= 0;
        [Pure] public static bool operator >=(Acceleration3d left, Acceleration3d right) => Comparer<Acceleration3d>.Default.Compare(left, right) >= 0;

        [Pure]
        public static Acceleration3d operator +(Acceleration3d lvalue, Acceleration3d rvalue)
        {
            var x = lvalue.AccelerationX + rvalue.AccelerationX;
            var y = lvalue.AccelerationY + rvalue.AccelerationY;
            var z = lvalue.AccelerationZ + rvalue.AccelerationZ;

            return new Acceleration3d(x, y, z);
        }

        [Pure]
        public static Acceleration3d operator -(Acceleration3d lvalue, Acceleration3d rvalue)
        {
            var x = lvalue.AccelerationX - rvalue.AccelerationX;
            var y = lvalue.AccelerationY - rvalue.AccelerationY;
            var z = lvalue.AccelerationZ - rvalue.AccelerationZ;

            return new Acceleration3d(x, y, z);
        }

        [Pure] public override string ToString() => $"{AccelerationX}, {AccelerationY}, {AccelerationZ}";
        [Pure] public string ToString(string format, IFormatProvider formatProvider) => $"{AccelerationX.ToString(format, formatProvider)}, {AccelerationY.ToString(format, formatProvider)}, {AccelerationZ.ToString(format, formatProvider)}";

        public int CompareTo(object obj)
        {
            throw new NotImplementedException();
        }

        public bool Equals((double ValueX, double ValueY, double ValueZ) other)
        {
            return AccelerationX.Equals(other.ValueX) &&
                AccelerationY.Equals(other.ValueY) &&
                AccelerationZ.Equals(other.ValueZ);
        }

        public int CompareTo((double, double, double) other)
        {
            throw new NotImplementedException();
        }
    }
}